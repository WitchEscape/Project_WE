using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Keypad : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI Ans;

    private string Answer = "1042";

    [SerializeField] private LockCabinet lockcabinet;

    [SerializeField] private GameObject WateringCan;

    private void Start()
    {
        lockcabinet = FindObjectOfType<LockCabinet>();
        if (lockcabinet == null )
        {
            print("LockCabinet을 찾을 수 없습니다");
        }

        WateringCan.SetActive(false);
    }
    public void Number(int number)
    {
        Ans.text += number.ToString();
    }

    public void Excute()
    {
        if (Ans.text == Answer)
        {
            Ans.text = "Correct";
            lockcabinet.UnLockDrawer();
            WateringCan.SetActive(true);
        }
        else
        {
            Ans.text = "Invalid";
        }

    }
    public void Clear()
    {
        Ans.text = "";
    }
}
