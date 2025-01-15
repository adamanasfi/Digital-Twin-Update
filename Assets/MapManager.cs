using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject map;
    public static GameObject mapParent;

    void Start()
    {
        mapParent = new GameObject("mapParent");
        mapParent.gameObject.SetActive(false);
        foreach (UnityEngine.Transform child in map.transform)
        {
            if (child.gameObject.name.Contains("wall") || child.gameObject.name.Contains("floor"))
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
