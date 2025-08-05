using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : MonoBehaviour
{
    public GameObject HuskyPrefab, MonitorEchoPrefab, RedChairPrefab, KubokiPrefab, RedRobotPrefab, GreenChairPrefab, BMWRobotPrefab;
    public GameObject TempToolTipPrefab, HumanCorrectionToolTipPrefab, EditObjectsToolTipPrefab;
    public static Dictionary<string, GameObject> prefabDictionary;
    public static GameObject STODParent, TempParent;
    public static GameObject temptoolTipPrefab, humanCorrectionToolTipPrefab, editObjectsToolTipPrefab;
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
        STODParent = new GameObject("STODParent");
        TempParent = new GameObject("TempParent");
        temptoolTipPrefab = TempToolTipPrefab;
        humanCorrectionToolTipPrefab = HumanCorrectionToolTipPrefab;
        editObjectsToolTipPrefab = EditObjectsToolTipPrefab;
    }

    public static void SetSTODParentState(bool state)
    {
        STODParent.SetActive(state);
    }

    public static void SetTempParentState(bool state)
    {
        TempParent.SetActive(state);
    }
}
