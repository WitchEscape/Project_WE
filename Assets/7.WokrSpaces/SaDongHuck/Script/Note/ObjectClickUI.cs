using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectClickUI : MonoBehaviour
{
    /*public GameObject uiCanvas; // 호출할 UI Canvas
    private XRGrabInteractable grabInteractable; // XR Grab Interactable

    void Start()
    {
        // XR Grab Interactable 컴포넌트 가져오기
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Select Entered 이벤트에 메서드 연결
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnObjectGrabbed);
            grabInteractable.selectExited.AddListener(OnObjectReleased); // 선택사항
        }
    }

    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        // UI 활성화/비활성화 토글
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(!uiCanvas.activeSelf);
            Debug.Log("UI Toggled: " + uiCanvas.activeSelf);
        }
    }

    private void OnObjectReleased(SelectExitEventArgs args)
    {
        // 선택적으로 UI 비활성화 처리
        Debug.Log("Object Released");
    }

    void OnDestroy()
    {
        // 이벤트 제거
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnObjectGrabbed);
            grabInteractable.selectExited.RemoveListener(OnObjectReleased);
        }
    }*/


    public GameObject uiPrefab; // 생성할 UI 프리팹
    public float distanceFromPlayer = 2f; // 플레이어와 UI 사이 거리
    public Vector3 offset = new Vector3(0, 1.0f, 0); // 위치 오프셋

    private Transform playerCamera; // 플레이어의 카메라 Transform
    private GameObject spawnedUI; // 생성된 UI 인스턴스

    void Start()
    {
        // Main Camera를 자동으로 찾음
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            playerCamera = mainCamera.transform;
        }
        else
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다. Main Camera 태그가 설정되어 있는지 확인하세요.");
        }
    }

    public void SpawnUI()
    {
        if (playerCamera == null)
        {
            Debug.LogError("playerCamera가 설정되지 않았습니다.");
            return;
        }

        if (uiPrefab != null) // UI가 아직 생성되지 않은 경우
        {
            // UI 생성 위치 계산
            Vector3 spawnPosition = playerCamera.position + playerCamera.forward.normalized * distanceFromPlayer + offset;

            // UI 회전 계산 (플레이어를 바라보도록 설정)
            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward, Vector3.up);

            // UI 생성 및 설정
            //spawnedUI = Instantiate(uiPrefab, spawnPosition, spawnRotation);
            uiPrefab.SetActive(true);
        }
        else
        {
            // UI 활성화/비활성화 전환
            spawnedUI.SetActive(!spawnedUI.activeSelf);
        }
    }
}
