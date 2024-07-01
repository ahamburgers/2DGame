using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Vector3 size = renderer.bounds.size;
        Debug.Log("Size in world space: " + size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
