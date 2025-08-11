using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.CustomedInterfaces;
using System.Text.RegularExpressions;
using UnityEditor.VersionControl;


public class ROSClientManager : MonoBehaviour
{
    public static ROSConnection ros;
    public static string requestedCategory;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<RequestSTODRequest, RequestSTODResponse>("request_STOD");
        ros.RegisterRosService<RequestCategoryRequest, RequestCategoryResponse>("request_category");
        ros.RegisterRosService<EditObjectsRequest, EditObjectsResponse>("edit_objects");
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

    public void CallCategoryService(GameObject requestedClass)
    {
        requestedCategory = requestedClass.name;
        RequestCategoryRequest request = new RequestCategoryRequest(requestedCategory);
        ros.SendServiceMessage<RequestCategoryResponse>("request_category", request, CategoryResponseCallback);
    }

    public static void CallDeleteService(GameObject tooltip)
    {
        Match match = PrefabsManager.ExtractObjectFromTooltip(tooltip);
        string className = match.Groups[1].Value;
        int id = int.Parse(match.Groups[2].Value);
        RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = new RosMessageTypes.CustomedInterfaces.ObjectMsg();
        objectMsg.name = className;
        objectMsg.id = id;
        EditObjectsRequest request = new EditObjectsRequest("delete", objectMsg);
        ros.SendServiceMessage<EditObjectsResponse>("edit_objects", request, EditResponseCallBack);
        Destroy(tooltip.transform.parent.gameObject);
    }

    public static void CallAddService()
    {
        foreach (UnityEngine.Transform child in PrefabsManager.vuforiaParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                Vector3 localPosition = OriginManager.CalculateLocalPosition(child.transform.position);
                float y_angle = OriginManager.CalculateLocalRotation(child.transform.eulerAngles.y);
                RosMessageTypes.CustomedInterfaces.ObjectMsg objectMsg = ROSPublisherManager.FillObjectMessage(false, child.name.ToString(), localPosition, y_angle);
                EditObjectsRequest request = new EditObjectsRequest("add", objectMsg);
                ros.SendServiceMessage<EditObjectsResponse>("edit_objects", request, EditResponseCallBack);
                break;
            }
        }
        PrefabsManager.ClearEditCADs();
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

    static void EditResponseCallBack(EditObjectsResponse response)
    {
        if (response.success)
        {
            Debug.Log("Edit request was successful!");
        }
        else
        {
            Debug.Log("Edit request failed.");
        }
    }

}