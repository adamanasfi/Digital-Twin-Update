using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.VisualScripting;
using Microsoft.MixedReality.Toolkit.UI;
using Vuforia;
using Unity.VisualScripting.Antlr3.Runtime.Tree;


public class ROSSubscriberManager : MonoBehaviour
{
    ROSConnection ros;
    public static Dictionary<string, Dictionary<int,RosMessageTypes.CustomedInterfaces.ObjectMsg>> objectLocationsDict;
    public static Dictionary<string, Dictionary<int, GameObject>> toolTipsDict;
    public static Dictionary<string, int> tempCountsDict;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/objectLocations", objectLocationsCallback);
        ros.Subscribe<RosMessageTypes.CustomedInterfaces.TempMsg>("/tempCount", tempCountsCallback);
        ros.Subscribe<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/STOD", STODCallback);
        objectLocationsDict = new Dictionary<string, Dictionary<int, RosMessageTypes.CustomedInterfaces.ObjectMsg>>();
        toolTipsDict = new Dictionary<string,Dictionary<int, GameObject>>();
        tempCountsDict = new Dictionary<string, int>();
    }

    public void objectLocationsCallback(RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg)
    {
        Debug.Log("received ID: " + objectMsg.id);
        if (!objectLocationsDict.ContainsKey(objectMsg.name)) // class not in dictionary yet
        {
            objectLocationsDict.Add(objectMsg.name,new Dictionary<int,RosMessageTypes.CustomedInterfaces.ObjectMsg>());
            objectLocationsDict[objectMsg.name][objectMsg.id] = objectMsg;
        }
        else // class is in dictionary
        {
            if (objectLocationsDict[objectMsg.name].ContainsKey(objectMsg.id)) // object already added
            {
                objectLocationsDict[objectMsg.name][objectMsg.id] = objectMsg;
                
            }
            else // object not added yet
            {
                objectLocationsDict[objectMsg.name].Add(objectMsg.id, objectMsg);
            }
        }
        if (tempCountsDict.ContainsKey(objectMsg.name))
        {
            if (tempCountsDict[objectMsg.name] != 0)
            {
                Debug.Log("Temp Count is: " + tempCountsDict[objectMsg.name]);
                AddToolTip(objectMsg.name, objectMsg.id);
            }
        } 
    }

    public void tempCountsCallback(RosMessageTypes.CustomedInterfaces.TempMsg tempMsg)
    {
        if (!tempCountsDict.ContainsKey(tempMsg.name)) tempCountsDict.Add(tempMsg.name, tempMsg.number); // class doesn't exist in dictionary 
        else tempCountsDict[tempMsg.name] = tempMsg.number;
        if (tempMsg.number == 0)
        {         
            foreach (GameObject tooltip in toolTipsDict[tempMsg.name].Values)
            {
                Destroy(tooltip);
            }
            if (toolTipsDict.ContainsKey(tempMsg.name)) toolTipsDict[tempMsg.name].Clear();
        }
    }

    private void AddToolTip(string name, int id) 
    {
        RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = objectLocationsDict[name][id];
        Vector3 localPosition = new Vector3((float)objectMsg.pose.position.x, (float)objectMsg.pose.position.z + 1, (float)objectMsg.pose.position.y);
        Vector3 worldPosition = ROSPublisherManager.imageTarget.transform.TransformPoint(localPosition);
        GameObject tooltip = Instantiate(PrefabsManager.toolTipPrefab, worldPosition, Quaternion.identity);
        tooltip.SetActive(true);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = name + "_" + id.ToString();
        if (!toolTipsDict.ContainsKey(name)) toolTipsDict.Add(name, new Dictionary<int, GameObject>());
        if (!toolTipsDict[name].ContainsKey(id))
        {
            toolTipsDict[name].Add(id, tooltip);
        }
        else
        {
            Destroy(toolTipsDict[name][id]);
            toolTipsDict[name][id] = tooltip;
        }  
    }


    public void STODCallback(RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg)
    {
        GameObject prefab = PrefabsManager.prefabDictionary[objectMsg.name];
        Vector3 localPose = new Vector3((float)objectMsg.pose.position.x, (float)objectMsg.pose.position.z, (float)objectMsg.pose.position.y);
        Vector3 worldPose = ROSPublisherManager.imageTarget.transform.TransformPoint(localPose);
        Quaternion receivedRotation = new Quaternion(
            (float)objectMsg.pose.orientation.x,
            (float)objectMsg.pose.orientation.y,
            (float)objectMsg.pose.orientation.z,
            (float)objectMsg.pose.orientation.w
        );
        Vector3 eulerRotation = receivedRotation.eulerAngles;
        Quaternion adjustedRotation = Quaternion.Euler(eulerRotation.x, eulerRotation.z, eulerRotation.y);
        GameObject assetCAD = Instantiate(prefab, worldPose, adjustedRotation, PrefabsManager.STODParent.transform);
        GameObject tooltip = Instantiate(PrefabsManager.toolTipPrefab, worldPose + new Vector3(0, 1, 0), Quaternion.identity, assetCAD.transform);
        tooltip.SetActive(true);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = objectMsg.name + "_" + objectMsg.id.ToString();
    }


    



}
