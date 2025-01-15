using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectedObjectsManager : MonoBehaviour
{
    public GameObject ImageTarget, ModelTarget;
    public static GameObject imageTarget;
    public static GameObject modelTarget;
    public TextMeshPro text;

    void Start()
    {
        imageTarget = new GameObject("correctAxesImageTarget");
    }

    void Update()
    {
        
    }

    public void CoordinatesWrtImage()
    {
        imageTarget.transform.position = ImageTarget.transform.position;
        imageTarget.transform.up = ImageTarget.transform.forward;
        imageTarget.transform.right = ImageTarget.transform.right;
        imageTarget.transform.forward = -ImageTarget.transform.up;
        Vector3 globalPosition = ModelTarget.transform.position;
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        float y_angle = imageTarget.transform.eulerAngles.y - ModelTarget.transform.eulerAngles.y;
    }

}
