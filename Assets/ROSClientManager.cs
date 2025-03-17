using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.CustomedInterfaces;

public class ROSClientManager : MonoBehaviour
{
    ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<RequestSTODRequest, RequestSTODResponse>("request_STOD");
    }

    public void CallSTODService()
    {
        RequestSTODRequest request = new RequestSTODRequest
        {
            agent_name = "hololens"
        };

        ros.SendServiceMessage<RequestSTODResponse>("request_STOD", request, STODResponseCallback);
    }

    void STODResponseCallback(RequestSTODResponse response)
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