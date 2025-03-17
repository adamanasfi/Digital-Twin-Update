using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : MonoBehaviour
{
    public GameObject HuskyPrefab, MonitorEchoPrefab, RedChairPrefab, KubokiPrefab, RedRobotPrefab, GreenChairPrefab;
    public GameObject ToolTipPrefab;
    public static Dictionary<string, GameObject> prefabDictionary;
    public static GameObject STODParent;
    public static GameObject toolTipPrefab;
    void Start()
    {
        prefabDictionary = new Dictionary<string, GameObject>
        {
            { "Husky", HuskyPrefab },
            { "MonitorEcho", MonitorEchoPrefab },
            { "RedChair", RedChairPrefab },
            { "Kuboki", KubokiPrefab },
            { "RedRobot", RedRobotPrefab },
            {"GreenChair", GreenChairPrefab }
        };
        STODParent = new GameObject("STODParent");
        toolTipPrefab = ToolTipPrefab;
    }
}
