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
    RosMessageTypes.CustomInterfaces.WallMsg wall;


    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosMessageTypes.CustomInterfaces.WallMsg>("/Wall");
        wall = new RosMessageTypes.CustomInterfaces.WallMsg();
    }

    private void Update()
    {

    }

    public void PublishWalls()
    {
/*        foreach (UnityEngine.Transform child in map.transform)
        {
            if (child.gameObject.name.Contains("wall") || child.gameObject.name.Contains("floor") || child.gameObject.name.Contains("ceiling"))
            {
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();


                Bounds bounds = meshRenderer.bounds;
                GameObject cube = Instantiate(cubePrefab, bounds.center, Quaternion.identity);
                cube.transform.localScale = bounds.size; // Adjust the cube's size to match the bounds

            }
        }*/
    }

}
