using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerPuzzle : MonoBehaviour
{
    [Header("UI 관련 설정")]
    public GameObject uiPrefab; // 생성할 UI 프리팹
    public float distanceFromPlayer = 2f; // 플레이어와 UI 사이 거리
    public Vector3 offset = new Vector3(0, 1.0f, 0); // 위치 오프셋

    [Header("비밀번호 관련 설정")]
    [SerializeField] private string answer = "BROP"; // 정답 설정
    [SerializeField] private GameObject uipanel; // 닫을 패널
    public TextMeshProUGUI[] inputChars; // 사용자가 입력한 문자 배열

    private Transform playerCamera; // 플레이어의 카메라 Transform
    private string result; // 현재 입력된 문자열

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

        var simpleinteractable = GetComponent<XRSimpleInteractable>();
        {
            if(simpleinteractable != null)
            {
                simpleinteractable.selectEntered.AddListener(SpawnUI);
            }
        }
    }

    // UI를 생성하거나 활성화/비활성화
    private void SpawnUI(SelectEnterEventArgs args)
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
        result = null;
        for (int i = 0; i < inputChars.Length; i++)
        {
            result += inputChars[i].text;
        }

        if (answer == result) // 정답인 경우
        {
            Debug.Log("서랍이 열렸습니다.");
            if (uipanel != null)
            {
                uipanel.SetActive(false);
            }
        }
        else
        {
            Debug.Log("비밀번호가 틀렸습니다.");
        }
    }

    public void CloseUIPanel()
    {
        if (uipanel != null)
        {
            uipanel.SetActive(false);
            Debug.Log("UI 패널이 닫혔습니다.");
        }
        else
        {
            Debug.LogWarning("UI 패널이 설정되지 않았습니다.");
        }
    }
}
