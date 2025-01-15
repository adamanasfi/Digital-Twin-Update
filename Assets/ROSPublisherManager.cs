using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class ROSPublisherManager : MonoBehaviour
{
    ROSConnection ros;
    float timeElapsed = 0.0f;
    int publishMessageFrequency = 1;
    public GameObject ImageTarget, ModelTarget;
    public static GameObject imageTarget;
    public static GameObject modelTarget;
    RosMessageTypes.CustomInterfaces.WallMsg wall;
    RosMessageTypes.CustomInterfaces.WallMsg husky;

    void Start()
    {
        imageTarget = new GameObject("correctAxesImageTarget");
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosMessageTypes.CustomInterfaces.WallMsg>("/hololensWall");
        ros.RegisterPublisher<RosMessageTypes.CustomInterfaces.WallMsg>("/husky");
        wall = new RosMessageTypes.CustomInterfaces.WallMsg();
        husky = new RosMessageTypes.CustomInterfaces.WallMsg();
    }


    public void PublishHusky()
    {
        imageTarget.transform.position = ImageTarget.transform.position;
        imageTarget.transform.up = ImageTarget.transform.forward;
        imageTarget.transform.right = ImageTarget.transform.right;
        imageTarget.transform.forward = -ImageTarget.transform.up;
        Vector3 globalPosition = ModelTarget.transform.position;
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        float y_angle = imageTarget.transform.eulerAngles.y - ModelTarget.transform.eulerAngles.y;
        husky.name = "husky";
        husky.position = new RosMessageTypes.Geometry.Vector3Msg(localPosition.x, localPosition.z, localPosition.z);
        husky.rotation = new RosMessageTypes.Geometry.Vector3Msg(0, 0, y_angle);
        husky.scale = new RosMessageTypes.Geometry.Vector3Msg(1, 1, 1);
        ros.Publish("/husky", husky);
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
            yield return new WaitForSeconds(1.0f);
        }
    }




}



