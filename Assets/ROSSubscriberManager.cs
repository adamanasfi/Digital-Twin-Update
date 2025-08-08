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
        ros.Subscribe<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/hololensSTOD", hololensSTODCallback);
        ros.Subscribe<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/categorySTOD", categorySTODCallback);
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
                AddIsHereToolTip(objectMsg.name, objectMsg.id);
            }
        } 
    }

    public void tempCountsCallback(RosMessageTypes.CustomedInterfaces.TempMsg tempMsg)
    {
        int oldCount;
        if (!tempCountsDict.ContainsKey(tempMsg.name)) tempCountsDict.Add(tempMsg.name, tempMsg.number); // class doesn't exist in dictionary 
        else 
        {
            oldCount = tempCountsDict[tempMsg.name];
            tempCountsDict[tempMsg.name] = tempMsg.number;
            if (tempMsg.number < oldCount)
            {
                // delete last added temp tooltip
                string className = tempMsg.name;
                Transform classToolTipsParent = PrefabsManager.tempParent.transform.Find(className);
                if (classToolTipsParent != null) Destroy(classToolTipsParent.transform.GetChild(classToolTipsParent.transform.childCount - 1).gameObject);
            }
        }
        if (tempMsg.number == 0)
        {         
            foreach (GameObject tooltip in toolTipsDict[tempMsg.name].Values)
            {
                Destroy(tooltip);
            }
            if (toolTipsDict.ContainsKey(tempMsg.name)) toolTipsDict[tempMsg.name].Clear();
        }
        else
        {
            if (ROSPublisherManager.shouldAddTemp)
            {
                foreach (Transform child in PrefabsManager.vuforiaParent.transform)
                {
                    if (child.gameObject.activeSelf)
                    {
                        string className = child.gameObject.name;
                        Vector3 worldPosition = child.transform.position;
                        Transform classToolTipsParent = PrefabsManager.tempParent.transform.Find(className);
                        if (classToolTipsParent == null)
                        {
                            GameObject parent = new GameObject(className);
                            parent.transform.SetParent(PrefabsManager.tempParent.transform);
                            classToolTipsParent = parent.transform;
                        }
                        GameObject tempToolTip = Instantiate(
                            PrefabsManager.tempToolTipPrefab,
                            worldPosition + new Vector3(0,1,0),
                            Quaternion.identity,
                            classToolTipsParent
                        );
                        tempToolTip.SetActive(true);
                        ToolTip tooltipText = tempToolTip.GetComponent<ToolTip>();
                        tooltipText.ToolTipText = "Which " + className + "?";
                        break;
                    }
                }
            }
        }
    }

    private void AddIsHereToolTip(string name, int id) 
    {
        RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = objectLocationsDict[name][id];
        Vector3 localPosition = new Vector3((float)objectMsg.pose.position.x, (float)objectMsg.pose.position.z + 1, (float)objectMsg.pose.position.y);
        Vector3 worldPosition = OriginManager.CalculateWorldPosition(localPosition);
        GameObject tooltip = Instantiate(PrefabsManager.ishereToolTipPrefab, worldPosition, Quaternion.identity, PrefabsManager.isHereParent.transform);
        tooltip.SetActive(true);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = "Is " + name + "_" + id.ToString() + " still here?";
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


    public void hololensSTODCallback(RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg)
    {
        GameObject assetCAD = PrefabsManager.DrawCAD(objectMsg, PrefabsManager.stodParent.transform);
        GameObject tooltip = Instantiate(PrefabsManager.humanCorrectionToolTipPrefab, assetCAD.transform.position + new Vector3(0, 1, 0), Quaternion.identity, assetCAD.transform);
        tooltip.SetActive(true);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = "Change " + objectMsg.name + "_" + objectMsg.id.ToString() + " pose?";
    }


    public void categorySTODCallback(RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg)
    {
        GameObject assetCAD = PrefabsManager.DrawCAD(objectMsg, PrefabsManager.editParent.transform);
        GameObject tooltip = Instantiate(PrefabsManager.editObjectsToolTipPrefab, assetCAD.transform.position + new Vector3(0, 1, 0), Quaternion.identity, assetCAD.transform);
        tooltip.SetActive(true);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = "Delete " + objectMsg.name + "_" + objectMsg.id.ToString() + "?";
    }








}
