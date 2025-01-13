using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleDial : MonoBehaviour
{
    [SerializeField] // 인스펙터 창에서 설정 가능
    private string answer = "BROP"; // 초기값 설정

    private string result;

    [SerializeField] // 인스펙터 창에서 LockBox 연결 가능
    private LockBox lockBox;

    public TextMeshProUGUI[] inputChars;

    private void Start()
    {
        // LockBox 스크립트를 가진 오브젝트를 찾습니다.
        lockBox = FindObjectOfType<LockBox>();
        if (lockBox == null)
        {
            Debug.LogError("LockBox 컴포넌트를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    public void AnswerCheck()
    {
        result = null;
        for (int i = 0; i < inputChars.Length; i++)
        {
            result += inputChars[i].text;
        }
        if (answer == result)
        {
            lockBox.UnLockbox();
            Debug.Log("서랍이 열렸습니다.");
        }
        else
            return;
    }
}
