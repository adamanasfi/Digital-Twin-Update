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
    float timeElapsed = 0.0f;
    int publishMessageFrequency = 1;
    public GameObject ImageTarget;
    public static GameObject imageTarget;
    public bool detectedImage;
    public GameObject vuforiaParent;
    RosMessageTypes.Geometry.TwistMsg transformation;

    void Start()
    {
        imageTarget = new GameObject("correctAxesImageTarget");
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosMessageTypes.Geometry.TwistMsg>("/transformation");
        ros.RegisterPublisher<RosMessageTypes.CustomedInterfaces.TempMsg>("/tempResponse");
        ros.RegisterPublisher<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/hololensObject");
        ros.RegisterPublisher<RosMessageTypes.CustomedInterfaces.ObjectMsg>("/humanCorrection");
        transformation = new RosMessageTypes.Geometry.TwistMsg();
        detectedImage = false;
        //InvokeRepeating("PublishHoloLensTransform", 0, 0.5f);
    }

    public void publishOfflineObject(GameObject tooltip)
    {
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        string text = tooltipText.ToolTipText;
        string[] parts = text.Split('_');
        string className = parts[0];
        int number = int.Parse(parts[1]);
        RosMessageTypes.CustomedInterfaces.TempMsg tempMsg = new RosMessageTypes.CustomedInterfaces.TempMsg(className,number);
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
        Vector3 localPose = imageTarget.transform.InverseTransformPoint(worldPose);
        float y_angle = parentObject.transform.eulerAngles.y - imageTarget.transform.eulerAngles.y;
        objectMsg.name = className;
        objectMsg.id = id;
        objectMsg.pose.position = new RosMessageTypes.Geometry.PointMsg(localPose.x, localPose.z, localPose.y);
        Quaternion rotation = Quaternion.Euler(0, 0, y_angle);
        objectMsg.pose.orientation = new RosMessageTypes.Geometry.QuaternionMsg(rotation.x, rotation.y, rotation.z, rotation.w);
        objectMsg.scale = new RosMessageTypes.Geometry.Vector3Msg(1, 1, 1);
        ros.Publish("/humanCorrection",objectMsg);
    }

    private void CreateDebugCube(Vector3 position, Color color)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = Vector3.one * 0.2f; // Set size to make it more visible
        cube.GetComponent<Renderer>().material.color = color;
    }

    public void PublishTransform(GameObject robot)
    {
        Vector3 globalPosition = robot.transform.position;
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        CreateDebugCube(robot.transform.position + robot.transform.forward * 1.0f, Color.blue);  // Forward (Z)
        CreateDebugCube(robot.transform.position + robot.transform.up * 1.0f, Color.green);      // Up (Y)
        CreateDebugCube(robot.transform.position + robot.transform.right * 1.0f, Color.red);     // Right (X)
        float y_angle = robot.transform.eulerAngles.y - imageTarget.transform.eulerAngles.y;
        transformation.linear.x = localPosition.x;
        transformation.linear.y = localPosition.z;
        transformation.linear.z = localPosition.y;
        transformation.angular.x = 0;
        transformation.angular.y = 0;
        transformation.angular.z = y_angle;
        ros.Publish("/transformation", transformation);
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

    public void SaveImageTarget()
    {
        imageTarget.transform.position = ImageTarget.transform.position;
        //imageTarget.transform.up = ImageTarget.transform.forward;
        //imageTarget.transform.right = ImageTarget.transform.right;
        //imageTarget.transform.forward = -ImageTarget.transform.up;
        imageTarget.transform.rotation = ImageTarget.transform.rotation;
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.forward * 1.0f, Color.blue);  // Forward (Z)
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.up * 1.0f, Color.green);      // Up (Y)
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.right * 1.0f, Color.red);     // Right (X)
        detectedImage = true;
    }


    public void PublishAsset(GameObject asset)
    {
        Vector3 globalPosition = asset.transform.position;
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        float y_angle = asset.transform.eulerAngles.y - imageTarget.transform.eulerAngles.y;
        RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = new RosMessageTypes.CustomedInterfaces.ObjectMsg();
        if (asset.name == "Husky" || asset.name == "Kobuki")
        {
            objectMsg.id = int.Parse(TextFieldManager.id.text);
            if (asset.name == "Kobuki") y_angle += 180;
            CreateDebugCube(asset.transform.position + asset.transform.forward * 1.0f, Color.blue);  // Forward (Z)
            CreateDebugCube(asset.transform.position + asset.transform.up * 1.0f, Color.green);      // Up (Y)
            CreateDebugCube(asset.transform.position + asset.transform.right * 1.0f, Color.red);     // Right (X)
        }
        objectMsg.name = asset.name.ToString();
        objectMsg.pose.position = new RosMessageTypes.Geometry.PointMsg(localPosition.x, localPosition.z, localPosition.y);
        Quaternion rotation = Quaternion.Euler(0, 0, -y_angle);
        objectMsg.pose.orientation = new RosMessageTypes.Geometry.QuaternionMsg(rotation.x,rotation.y,rotation.z,rotation.w);
        objectMsg.scale = new RosMessageTypes.Geometry.Vector3Msg(1, 1, 1);
        ros.Publish("/hololensObject", objectMsg);
    }

    //public void PublishHoloLensTransform()
    //{
    //    if (!detectedImage) return;
    //    Vector3 localPosition = imageTarget.transform.InverseTransformPoint(Camera.main.transform.position);
    //    float x_angle = imageTarget.transform.eulerAngles.x - Camera.main.transform.eulerAngles.x;
    //    float y_angle = imageTarget.transform.eulerAngles.y - Camera.main.transform.eulerAngles.y;
    //    float z_angle = imageTarget.transform.eulerAngles.z - Camera.main.transform.eulerAngles.z;
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



