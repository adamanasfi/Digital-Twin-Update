using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject map;
    GameObject mapParent;
    void Start()
    {
        mapParent = new GameObject("mapParent");
        foreach (UnityEngine.Transform child in map.transform)
        {
            if (child.gameObject.name.Contains("ceiling"))
            {
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                Bounds bounds = meshRenderer.bounds;
                Debug.Log($"Child Name: {child.name}");
                Debug.Log($"World Center: {bounds.center}");
                Debug.Log($"World Size: {bounds.size}");
                GameObject cube = Instantiate(cubePrefab, bounds.center, Quaternion.identity, mapParent.transform);
                cube.transform.localScale = bounds.size;
            }
        }
    }

}
