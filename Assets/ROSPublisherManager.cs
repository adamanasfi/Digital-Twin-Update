using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using TMPro;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class ROSPublisherManager : MonoBehaviour
{
    ROSConnection ros;
    float timeElapsed = 0.0f;
    int publishMessageFrequency = 1;
    public GameObject ImageTarget;
    public static GameObject imageTarget;
    public GameObject vuforiaParent;
    RosMessageTypes.CustomedInterfaces.WallMsg wall;
    RosMessageTypes.CustomedInterfaces.WallMsg VRLAsset;
    RosMessageTypes.Geometry.TwistMsg transformation;

    void Start()
    {
        imageTarget = new GameObject("correctAxesImageTarget");
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosMessageTypes.CustomedInterfaces.WallMsg>("/hololensWall");
        ros.RegisterPublisher<RosMessageTypes.CustomedInterfaces.WallMsg>("/assets");
        ros.RegisterPublisher<RosMessageTypes.Geometry.TwistMsg>("/transformation");
        wall = new RosMessageTypes.CustomedInterfaces.WallMsg();
        VRLAsset = new RosMessageTypes.CustomedInterfaces.WallMsg();
        transformation = new RosMessageTypes.Geometry.TwistMsg();
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
        float y_angle = imageTarget.transform.eulerAngles.y - robot.transform.eulerAngles.y;
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
        imageTarget.transform.up = ImageTarget.transform.forward;
        imageTarget.transform.right = ImageTarget.transform.right;
        imageTarget.transform.forward = -ImageTarget.transform.up;
    }


    public void PublishAsset(GameObject asset)
    {
        Vector3 globalPosition = asset.transform.position;
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        float y_angle = asset.transform.eulerAngles.y - imageTarget.transform.eulerAngles.y;
        VRLAsset.name = asset.name.ToString();
        VRLAsset.position = new RosMessageTypes.Geometry.Vector3Msg(localPosition.x, localPosition.z, localPosition.y);
        VRLAsset.rotation = new RosMessageTypes.Geometry.Vector3Msg(0, 0, -y_angle);
        VRLAsset.scale = new RosMessageTypes.Geometry.Vector3Msg(1, 1, 1);
        ros.Publish("/assets", VRLAsset);
    }

    public void PublishWalls()
    {
        StartCoroutine(PublishWallsCoroutine());
    }

    private IEnumerator PublishWallsCoroutine()
    {
        int count = 0;
        foreach (UnityEngine.Transform child in MapManager.mapParent.transform)
        {
            wall.name = $"wall{count}";
            wall.position = new RosMessageTypes.Geometry.Vector3Msg(child.transform.position.x, child.transform.position.z, child.transform.position.y);
            wall.rotation = new RosMessageTypes.Geometry.Vector3Msg(child.transform.eulerAngles.x, child.transform.eulerAngles.z, child.transform.eulerAngles.y);
            wall.scale = new RosMessageTypes.Geometry.Vector3Msg(child.transform.localScale.x, child.transform.localScale.z, child.transform.localScale.y);
            ros.Publish("/hololensWall", wall);
            count++;
            yield return new WaitForSeconds(0.2f);
        }
    }




}



