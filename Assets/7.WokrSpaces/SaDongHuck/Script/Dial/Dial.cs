using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class Dial : MonoBehaviour
{
    string answer;
    string result;

    public TextMeshProUGUI[] inputChars;

    [SerializeField]
    private LockDeskDrawer lockdeskdrawer;

    private void Start()
    {
        lockdeskdrawer = FindObjectOfType<LockDeskDrawer>();
        if(lockdeskdrawer == null )
        {
            print("locKbox를 찾을 수 없습니다");
        }
    }
    void Awake()
    {
        answer = "BROP";
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
            //Debug.Log("서랍이 열렸습니다.");
            lockdeskdrawer.UnLockDrawer();
            
        }
        else
            return;
    }

    

   

}
