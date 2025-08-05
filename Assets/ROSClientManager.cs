using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.CustomedInterfaces;

public class ROSClientManager : MonoBehaviour
{
    public static ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<RequestSTODRequest, RequestSTODResponse>("request_STOD");
    }

    public static void CallSTODService()
    {
        for (int i = PrefabsManager.STODParent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = PrefabsManager.STODParent.transform.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
        RequestSTODRequest request = new RequestSTODRequest
        {
            agent_name = "hololens"
        };

        ros.SendServiceMessage<RequestSTODResponse>("request_STOD", request, STODResponseCallback);
    }

    static void STODResponseCallback(RequestSTODResponse response)
    {
        if (response.success)
        {
            Debug.Log("STOD request was successful!");
        }
        else
        {
            Debug.Log("STOD request failed.");
        }
    }

}