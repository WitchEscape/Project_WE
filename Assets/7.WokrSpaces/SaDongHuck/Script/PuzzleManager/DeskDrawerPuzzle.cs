using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeskDrawerPuzzle : MonoBehaviour
{
    [Header("비밀번호 관련 설정")]
    [SerializeField] private string answer = "BROP"; // 정답 설정
    private string result; // 사용자가 입력한 결과

    public TextMeshProUGUI[] inputChars; // 사용자가 입력한 문자

    [SerializeField] private LockDeskDrawer lockdeskdrawer; // 서랍 잠금 해제 스크립트

    [SerializeField] private GameObject key; // 정답을 맞췄을 때 활성화할 키 오브젝트

    [Header("UI 관련 설정")]
    public GameObject uiPrefab; // 생성할 UI 프리팹
    public float distanceFromPlayer = 2f; // 플레이어와 UI 사이 거리
    public Vector3 offset = new Vector3(0, 1.0f, 0); // 위치 오프셋

    private Transform playerCamera; // 플레이어의 카메라 Transform

    void Start()
    {
        // Main Camera 찾기
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            playerCamera = mainCamera.transform;
        }
        else
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다.");
        }

        // LockDeskDrawer 찾기
        lockdeskdrawer = FindObjectOfType<LockDeskDrawer>();
        if (lockdeskdrawer == null)
        {
            Debug.LogError("LockDeskDrawer를 찾을 수 없습니다.");
        }
    }

    // 비밀번호 확인
    public void AnswerCheck()
    {
        result = string.Empty; // 입력 결과 초기화
        for (int i = 0; i < inputChars.Length; i++)
        {
            result += inputChars[i].text;
        }

        if (answer == result)
        {
            Debug.Log("서랍이 열렸습니다.");
            lockdeskdrawer?.UnLockDrawer(); // 서랍 잠금 해제

            if (key != null)
            {
                key.SetActive(true); // 키 활성화
            }

            if (uiPrefab != null)
            {
                uiPrefab.SetActive(false); // UI 비활성화
            }
        }
        else
        {
            Debug.Log("비밀번호가 틀렸습니다.");
        }
    }

    // UI 생성 또는 활성화/비활성화
    public void ToggleUI()
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

            // UI 생성
            uiPrefab.SetActive(true);
        }
        else if (uiPrefab != null)
        {
            // UI 활성화/비활성화 전환
            uiPrefab.SetActive(!uiPrefab.activeSelf);
        }
    }
}
