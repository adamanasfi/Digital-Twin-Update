using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OriginManager : MonoBehaviour
{

    public GameObject ImageTarget;
    public static GameObject imageTarget;
    public static bool detectedImage;

    void Start()
    {
        detectedImage = false;
        imageTarget = new GameObject("correctedImageTarget");
    }

    public void SaveImageTarget()
    {
        imageTarget.transform.position = ImageTarget.transform.position;
        imageTarget.transform.up = ImageTarget.transform.forward;
        imageTarget.transform.right = ImageTarget.transform.right;
        imageTarget.transform.forward = -ImageTarget.transform.up;
        detectedImage = true;
    }

    public static Vector3 CalculateLocalPosition(Vector3 globalPosition)
    {
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        return localPosition;
    }

    public static float CalculateLocalRotation(float globalAngle)
    {
        float y_angle = globalAngle - OriginManager.imageTarget.transform.eulerAngles.y;
        return -y_angle; 
    }

    public static Vector3 CalculateWorldPosition(Vector3 localPosition)
    {
        Vector3 worldPosition = OriginManager.imageTarget.transform.TransformPoint(localPosition);
        return worldPosition;
    }

    public static Quaternion CalculateWorldRotation(Quaternion localRotation)
    {
        Vector3 eulerRotation = localRotation.eulerAngles;
        Quaternion adjustedRotation = Quaternion.Euler(eulerRotation.x, -eulerRotation.z, eulerRotation.y);
        Quaternion worldRotation = OriginManager.imageTarget.transform.rotation * adjustedRotation;
        return worldRotation;
    }




}
