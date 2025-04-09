using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine.UIElements;

public class TextFieldManager : MonoBehaviour
{
    public MRTKUGUIInputField ID;
    public static MRTKUGUIInputField id;

    void Start()
    {
        ID.onEndEdit.AddListener(HandleIDEdit);
        id = ID;
    }

    void OnEnable()
    {
        Vector3 newPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.0f;
        ID.text = "";
        ID.transform.position = newPosition;
        ID.transform.forward = Camera.main.transform.forward;
    }

    void HandleIDEdit(string inputText)
    {

    }

}
