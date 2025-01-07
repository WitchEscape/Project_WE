using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    // Inspector에서 설정할 색상
    [SerializeField]
    private Color targetColor = Color.white; // 기본 색상은 흰색

    private Renderer sphereRenderer;

    private void Start()
    {
        // Renderer 컴포넌트 가져오기
        sphereRenderer = GetComponent<Renderer>();
    }

    private void OnParticleCollision(GameObject other)
    {
        // 파티클 충돌 시 색상 변경
        if (sphereRenderer != null)
        {
            sphereRenderer.material.color = targetColor;
        }
    }
}
