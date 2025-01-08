using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DisplayMaterial : MonoBehaviour
{
    private Material material;
    public float fadeDuration = 3f;
    private float elapsedTime = 0f;

    private bool isChanging;
    private bool isDone;

    public ParticleSystem par;
    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable()
    {
        material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
    }

    private void Start()
    {
        isChanging = true;
    }

    private void Update()
    {
        if (isChanging)
        {
            elapsedTime = elapsedTime + Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);

            if (elapsedTime >= fadeDuration)
            {
                isChanging = false;
                par.gameObject.SetActive(true);
                //PlayParticle();
            }
        }
    }
    private void PlayParticle()
    {
        if (isDone)
        {

        }
    }
}
