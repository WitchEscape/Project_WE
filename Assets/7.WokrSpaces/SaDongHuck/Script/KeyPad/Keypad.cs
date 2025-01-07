using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Keypad : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Ans;

    private string Answer = "1042";

    // 생성할 오브젝트의 프리팹
    public GameObject objectinstant;

    // 오브젝트를 생성할 위치
    public Transform objectPosition;
    public void Number(int number)
    {
        Ans.text += number.ToString();
    }

    public void Excute()
    {
        if (Ans.text == Answer)
        {
            Ans.text = "Correct";
            Instantiate(objectinstant, objectPosition.position, objectPosition.rotation);
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
