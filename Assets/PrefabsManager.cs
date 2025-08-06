using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : MonoBehaviour
{
    public GameObject HuskyPrefab, MonitorEchoPrefab, RedChairPrefab, KubokiPrefab, RedRobotPrefab, GreenChairPrefab, BMWRobotPrefab;
    public GameObject TempToolTipPrefab, IsHereToolTipPrefab, HumanCorrectionToolTipPrefab, EditObjectsToolTipPrefab;
    public static Dictionary<string, GameObject> prefabDictionary;
    public GameObject VuforiaParent;
    public static GameObject stodParent, isHereParent,vuforiaParent, tempParent;
    public static GameObject tempToolTipPrefab, ishereToolTipPrefab, humanCorrectionToolTipPrefab, editObjectsToolTipPrefab;
    void Start()
    {
        prefabDictionary = new Dictionary<string, GameObject>
        {
            { "Husky", HuskyPrefab },
            { "MonitorEcho", MonitorEchoPrefab },
            { "RedChair", RedChairPrefab },
            { "Kobuki", KubokiPrefab },
            { "RedRobot", RedRobotPrefab },
            {"GreenChair", GreenChairPrefab },
            {"BMWRobot", BMWRobotPrefab }
        };
        stodParent = new GameObject("STODParent");
        isHereParent = new GameObject("IsHereParent");
        tempParent = new GameObject("TempParent");
        tempToolTipPrefab = TempToolTipPrefab;
        ishereToolTipPrefab = IsHereToolTipPrefab;
        humanCorrectionToolTipPrefab = HumanCorrectionToolTipPrefab;
        editObjectsToolTipPrefab = EditObjectsToolTipPrefab;
        vuforiaParent = VuforiaParent;
    }

    public static void SetSTODParentState(bool state)
    {
        stodParent.SetActive(state);
    }

    public static void SetTempParentState(bool state)
    {
        isHereParent.SetActive(state);
    }
}
