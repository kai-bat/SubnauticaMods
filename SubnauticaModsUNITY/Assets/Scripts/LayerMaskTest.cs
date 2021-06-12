using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMaskTest : MonoBehaviour
{
    public int layer;
    public LayerMask mask;
    void Start()
    {
        Debug.Log(mask.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
