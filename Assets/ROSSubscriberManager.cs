using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.VisualScripting;
using Microsoft.MixedReality.Toolkit.UI;


public class ROSSubscriberManager : MonoBehaviour
{
    public GameObject toolTipPrefab;
    ROSConnection ros;
    Dictionary<string, Dictionary<int,RosMessageTypes.CustomedInterfaces.ObjectMsg>> objectLocationsDict;
    Dictionary<string, Dictionary<int, GameObject>> toolTipsDict;
    Dictionary<string, int> tempCountsDict;
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/objectLocations", objectLocationsCallback);
        ros.Subscribe<RosMessageTypes.CustomedInterfaces.TempMsg>("/tempCount", tempCountsCallback);
        objectLocationsDict = new Dictionary<string, Dictionary<int, RosMessageTypes.CustomedInterfaces.ObjectMsg>>();
        toolTipsDict = new Dictionary<string,Dictionary<int, GameObject>>();
        tempCountsDict = new Dictionary<string, int>();
    }

    public void objectLocationsCallback(RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg)
    {
        if (!objectLocationsDict.ContainsKey(objectMsg.name)) // class not in dictionary yet
        {
            objectLocationsDict.Add(objectMsg.name,new Dictionary<int,RosMessageTypes.CustomedInterfaces.ObjectMsg>());
            objectLocationsDict[objectMsg.name][objectMsg.id] = objectMsg;
            AddToolTip(objectMsg.name,objectMsg.id);
        }
        else // class is in dictionary
        {
            if (objectLocationsDict[objectMsg.name].ContainsKey(objectMsg.id)) // object already added
            {
                objectLocationsDict[objectMsg.name][objectMsg.id] = objectMsg;
                AddToolTip(objectMsg.name, objectMsg.id, true);
            }
            else // object not added yet
            {
                objectLocationsDict[objectMsg.name].Add(objectMsg.id, objectMsg);
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
            toolTipsDict[tempMsg.name].Clear();
        }
    }

    private void AddToolTip(string name, int id, bool updating = false) // updating boolean for saving compute
    {
        RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = objectLocationsDict[name][id];
        Vector3 localPosition = new Vector3((float)objectMsg.pose.position.x, (float)objectMsg.pose.position.y, (float)objectMsg.pose.position.z);
        Vector3 worldPosition = ROSPublisherManager.imageTarget.transform.TransformPoint(localPosition);
        GameObject tooltip = Instantiate(toolTipPrefab,worldPosition,Quaternion.identity);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = name + "_" + id.ToString();
        if (!updating)
        {
            if (!toolTipsDict.ContainsKey(name)) toolTipsDict.Add(name, new Dictionary<int, GameObject>());
            toolTipsDict[name].Add(id, tooltip);
            return;
        }
        Destroy(toolTipsDict[name][id]);
        toolTipsDict[name][id] = tooltip;
    }


    



}
