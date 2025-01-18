using UnityEngine;

public class AlphaController : MonoBehaviour
{
    [SerializeField]
    private Material material;
    [SerializeField]
    private float alphaValue;

    private void Awake()
    {
        if (alphaValue > 1 || alphaValue < 0)
        {
            alphaValue = 0.5f;
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
                Destroy(this);
            }
            material.color = newColor;
        }
    }

}
