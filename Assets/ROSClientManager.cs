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
        ros.RegisterRosService<RequestCategoryRequest, RequestCategoryResponse>("request_category");
    }

    public static void CallSTODService()
    {
        for (int i = PrefabsManager.stodParent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = PrefabsManager.stodParent.transform.GetChild(i);
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

    public void CallCategoryService(GameObject requestedCategory)
    {
        RequestCategoryRequest request = new RequestCategoryRequest(requestedCategory.name);
        ros.SendServiceMessage<RequestCategoryResponse>("request_category", request, CategoryResponseCallback);
    }

    void CategoryResponseCallback(RequestCategoryResponse response)
    {
        if (response.success)
        {
            Debug.Log("Category request was successful!");
        }
        else
        {
            Debug.Log("Category request failed.");
        }
    }

}