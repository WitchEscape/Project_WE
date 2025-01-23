using System.Collections.Generic;
using UnityEngine;

public class AlphaController : MonoBehaviour
{
    private Material material;
    [SerializeField]
    private Material baseMaterial;
    [SerializeField]
    private Renderer re;
    [SerializeField]
    private float alphaValue;
    [SerializeField]
    private Puzzles puzzles;
    List<Material> materials = new List<Material>();

    [SerializeField] private ParticleSystem completeParticle;

    private void Awake()
    {
        if (alphaValue > 1 || alphaValue < 0)
        {
            alphaValue = 0.5f;
        }
        if (baseMaterial != null && re != null)
        {
            material = new Material(baseMaterial);
            materials.Add(re.material);
            materials.Add(material);
            re.SetMaterials(materials);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        print("파티클 닿음");
        if (other.CompareTag("Water"))
        {
            print("알파 값 조절되고 있음.");
            Color newColor = material.color;
            newColor.a = Mathf.Lerp(newColor.a, 0f, alphaValue);
            if (newColor.a <= 0.05f)
            {
                newColor.a = 0f;
                material.color = newColor;
                if (puzzles != null)
                    puzzles.TriggerEvent();
                completeParticle?.Play();
                Destroy(this);
            }
            material.color = newColor;
        }
    }
}
