using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoxPuzzle : MonoBehaviour
{
    [Header("UI 관련 설정")]
    public GameObject uiPrefab; // 생성할 UI 프리팹
    public float distanceFromPlayer = 2f; // 플레이어와 UI 사이 거리
    public Vector3 offset = new Vector3(0, 1.0f, 0); // 위치 오프셋

    [Header("비밀번호 관련 설정")]
    [SerializeField] private string answer = "BROP"; // 비밀번호 정답
    public TextMeshProUGUI[] inputChars; // 입력된 문자들

    [SerializeField] private LockBox lockBox; // LockBox 연결
    public GameObject lockObject;

    private Transform playerCamera; // 플레이어의 카메라 Transform
    private string result; // 입력된 문자열 결과

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

        // LockBox를 자동으로 찾음
        if (lockBox == null)
        {
            lockBox = FindObjectOfType<LockBox>();
            if (lockBox == null)
            {
                Debug.LogError("LockBox 컴포넌트를 가진 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    // UI를 생성하거나 활성화/비활성화
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
            uiPrefab.SetActive(true);
        }
        else if (uiPrefab != null)
        {
            // UI 활성화/비활성화 전환
            uiPrefab.SetActive(!uiPrefab.activeSelf);
        }
    }

    // 비밀번호 입력 확인
    public void AnswerCheck()
    {
        result = string.Empty; // 초기화
        for (int i = 0; i < inputChars.Length; i++)
        {
            result += inputChars[i].text; // 입력된 문자 합치기
        }

        if (answer == result) // 정답 확인
        {
            if (lockBox != null)
            {
                Destroy(lockObject,2f);
                lockBox.UnLockbox(); // 서랍 열기
                Debug.Log("상자가 열렸습니다.");
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
}
