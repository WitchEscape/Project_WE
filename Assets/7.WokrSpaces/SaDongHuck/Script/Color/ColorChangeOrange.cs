using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeOrange : MonoBehaviour
{
    private Renderer sphareRenderer;

    private void Start()
    {
        sphareRenderer = GetComponent<Renderer>();  
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cube"))
        {
            sphareRenderer.material.color = new Color(1.0f, 0.5f, 0.0f);
        }
    }
}
