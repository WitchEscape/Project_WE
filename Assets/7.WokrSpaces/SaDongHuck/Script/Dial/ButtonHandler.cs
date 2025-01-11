using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{

    public GameObject uiPrefab; // 생성할 UI 프리팹
    public Transform playerCamera; // 플레이어의 카메라 Transform
    public float distanceFromPlayer = 2f; // 플레이어와 UI 사이 거리
    public Vector3 offset = new Vector3(0, 1.0f, 0); // 위치 오프셋

    private GameObject spawnedUI; // 생성된 UI 인스턴스

    public void SpawnUI()
    {
        if (spawnedUI == null) // UI가 이미 생성되어 있지 않은 경우
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
