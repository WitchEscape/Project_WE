using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class TwoPointsLine : MonoBehaviour
{
    //툴팁 표시 or 말풍선 표시용 라인 렌더러 포지션 설정


    [Header("라인 렌더러 시작점 (오브젝트)")] public Transform pointA;
    [Header("라인 렌더러 시작점 (툴팁)")] public Transform pointB;
    private LineRenderer line;
    private CanvasGroup canvasGroup;
    private Material lineMaterial;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Start()
    {

            if (line != null)
            {
                // LineRenderer Material의 인스턴스 생성 (Instance)
                lineMaterial = line.material;
                lineMaterial = new Material(lineMaterial);
                line.material = lineMaterial;
            }
        

    }

    private void Update()
    { 
        line.positionCount = 2;
        line.SetPosition(0, pointA.position);
        line.SetPosition(1, pointB.position);

        #region 되면 컨펌받으러 가기
        if (line != null && canvasGroup != null)
        {
            Color color = lineMaterial.color;
            color.a = canvasGroup.alpha;
            lineMaterial.color = color;
        }
        #endregion
    }

}

