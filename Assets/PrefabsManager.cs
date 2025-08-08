using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : MonoBehaviour
{
    public GameObject HuskyPrefab, MonitorEchoPrefab, RedChairPrefab, KubokiPrefab, RedRobotPrefab, GreenChairPrefab, BMWRobotPrefab;
    public GameObject TempToolTipPrefab, IsHereToolTipPrefab, HumanCorrectionToolTipPrefab, EditObjectsToolTipPrefab;
    public static Dictionary<string, GameObject> prefabDictionary;
    public GameObject VuforiaParent;
    public static GameObject stodParent, isHereParent, vuforiaParent, tempParent, editParent;
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
        editParent = new GameObject("editParent");
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

    public static GameObject DrawCAD(RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg, Transform parent)
    {
        GameObject prefab = prefabDictionary[objectMsg.name];
        Vector3 localPosition = new Vector3((float)objectMsg.pose.position.x, (float)objectMsg.pose.position.z, (float)objectMsg.pose.position.y);
        Vector3 worldPosition = OriginManager.CalculateWorldPosition(localPosition);
        Quaternion localRotation = new Quaternion(
            (float)objectMsg.pose.orientation.x,
            (float)objectMsg.pose.orientation.y,
            (float)objectMsg.pose.orientation.z,
            (float)objectMsg.pose.orientation.w
        );
        Quaternion worldRotation = OriginManager.CalculateWorldRotation(localRotation);
        GameObject assetCAD = Instantiate(prefab, worldPosition, worldRotation, parent.transform);
        return assetCAD;
    }
}
