using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyClickUI : MonoBehaviour
{
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

        if (spawnedUI == null) // UI가 아직 생성되지 않은 경우
        {
            // UI 생성 위치 계산
            Vector3 spawnPosition = playerCamera.position + playerCamera.forward.normalized * distanceFromPlayer + offset;

            // UI 회전 계산 (플레이어를 바라보도록 설정)
            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward, Vector3.up);

            // UI 생성 및 설정
            spawnedUI = Instantiate(uiPrefab, spawnPosition, spawnRotation);
            spawnedUI.SetActive(true);
        }
        else
        {
            // UI 활성화/비활성화 전환
            spawnedUI.SetActive(!spawnedUI.activeSelf);
        }
    }
}
