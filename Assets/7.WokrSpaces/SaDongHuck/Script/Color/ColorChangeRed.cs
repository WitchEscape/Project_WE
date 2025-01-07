using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeRed : MonoBehaviour
{
    private Renderer sphareRender;

    private void Start()
    {
        sphareRender = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cube"))
        {
            sphareRender.material.color = Color.red;
        }
    }
}
