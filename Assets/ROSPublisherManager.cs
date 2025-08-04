using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using TMPro;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;


public class ROSPublisherManager : MonoBehaviour
{
    ROSConnection ros;
    public GameObject vuforiaParent;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosMessageTypes.Geometry.TwistMsg>("/transformation");
        ros.RegisterPublisher<RosMessageTypes.CustomedInterfaces.TempMsg>("/tempResponse");
        ros.RegisterPublisher<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/hololensObject");
        ros.RegisterPublisher<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/humanCorrection");
        //InvokeRepeating("PublishHoloLensTransform", 0, 0.5f);
    }

    public void publishOfflineObject(GameObject tooltip)
    {
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        string text = tooltipText.ToolTipText;
        string[] parts = text.Split('_');
        string className = parts[0];
        int number = int.Parse(parts[1]);
        RosMessageTypes.CustomedInterfaces.TempMsg tempMsg = new RosMessageTypes.CustomedInterfaces.TempMsg(className, number);
        ros.Publish("/tempResponse", tempMsg);
    }

    public void publishHumanCorrectedObject(GameObject tooltip)
    {
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        string text = tooltipText.ToolTipText;
        string[] parts = text.Split('_');
        string className = parts[0];
        int id = int.Parse(parts[1]);
        RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = new RosMessageTypes.CustomedInterfaces.ObjectMsg();
        GameObject parentObject = tooltip.transform.parent.gameObject;
        Vector3 worldPose = parentObject.transform.position;
        Vector3 localPose = OriginManager.imageTarget.transform.InverseTransformPoint(worldPose);
        float y_angle = parentObject.transform.eulerAngles.y - OriginManager.imageTarget.transform.eulerAngles.y;
        objectMsg.name = className;
        objectMsg.id = id;
        objectMsg.pose.position = new RosMessageTypes.Geometry.PointMsg(localPose.x, localPose.z, localPose.y);
        Quaternion rotation = Quaternion.Euler(0, 0, y_angle);
        objectMsg.pose.orientation = new RosMessageTypes.Geometry.QuaternionMsg(rotation.x, rotation.y, rotation.z, rotation.w);
        objectMsg.scale = new RosMessageTypes.Geometry.Vector3Msg(1, 1, 1);
        ros.Publish("/humanCorrection", objectMsg);
    }




    public void PublishActivatedModelTarget()
    {
        foreach (UnityEngine.Transform child in vuforiaParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                PublishAsset(child.gameObject);
                break;
            }
        }
    }



    public RosMessageTypes.CustomedInterfaces.ObjectMsg FillObjectMessage(bool isOnlineObject, string name, Vector3 localPosition, float y_angle)
    {
        RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = new RosMessageTypes.CustomedInterfaces.ObjectMsg();
        if (isOnlineObject) objectMsg.id = int.Parse(TextFieldManager.id.text);
        objectMsg.name = name;
        objectMsg.pose.position = new RosMessageTypes.Geometry.PointMsg(localPosition.x, localPosition.z, localPosition.y);
        Quaternion rotation = Quaternion.Euler(0, 0, y_angle);
        objectMsg.pose.orientation = new RosMessageTypes.Geometry.QuaternionMsg(rotation.x, rotation.y, rotation.z, rotation.w);
        objectMsg.scale = new RosMessageTypes.Geometry.Vector3Msg(1, 1, 1);
        return objectMsg;
    }

    public void PublishAsset(GameObject asset)
    {
        bool isOnlineObject = false;
        Vector3 localPosition = OriginManager.CalculateLocalPosition(asset.transform.position);
        float y_angle = OriginManager.CalculateLocalRotation(asset.transform.eulerAngles.y);
        if (asset.layer == 6)
        {
            isOnlineObject = true;
            if (asset.name == "Kobuki") y_angle += 180;
        }
        RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = FillObjectMessage(isOnlineObject, asset.name.ToString(), localPosition, y_angle);
        ros.Publish("/hololensObject", objectMsg);
    }

    //public void PublishHoloLensTransform()
    //{
    //    if (!detectedImage) return;
    //    Vector3 localPosition = OriginManager.imageTarget.transform.InverseTransformPoint(Camera.main.transform.position);
    //    float x_angle = OriginManager.imageTarget.transform.eulerAngles.x - Camera.main.transform.eulerAngles.x;
    //    float y_angle = OriginManager.imageTarget.transform.eulerAngles.y - Camera.main.transform.eulerAngles.y;
    //    float z_angle = OriginManager.imageTarget.transform.eulerAngles.z - Camera.main.transform.eulerAngles.z;
    //    RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = new RosMessageTypes.CustomedInterfaces.ObjectMsg();
    //    objectMsg.name = "HoloLens";
    //    objectMsg.id = 1;
    //    objectMsg.pose.position = new RosMessageTypes.Geometry.PointMsg(localPosition.x, localPosition.z, localPosition.y);
    //    Quaternion rotation = Quaternion.Euler(-x_angle, -z_angle, y_angle);
    //    objectMsg.pose.orientation = new RosMessageTypes.Geometry.QuaternionMsg(rotation.x, rotation.y, rotation.z, rotation.w);
    //    objectMsg.scale = new RosMessageTypes.Geometry.Vector3Msg(1, 1, 1);
    //    ros.Publish("/hololensObject", objectMsg);
    //}




}


