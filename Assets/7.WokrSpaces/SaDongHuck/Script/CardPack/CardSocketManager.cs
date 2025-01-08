using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class CardSocketManager : MonoBehaviour
{
    public XRSocketInteractor[] sockets; // 여러 개의 Socket Interactor
    public TextMeshProUGUI[] cardTexts; // 각각의 카드 값을 표시할 Text UI

    private string[] cardOrderArray; // 카드 순서를 관리하는 배열

    private void Start()
    {
        cardOrderArray = new string[sockets.Length];
        for (int i = 0; i < cardTexts.Length; i++)
        {
            cardTexts[i].text = ""; // 초기화
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            int index = i; // 로컬 변수로 캡처
            sockets[i].selectEntered.AddListener(args => OnCardPlaced(args, index));
            sockets[i].selectExited.AddListener(args => OnCardRemoved(args, index));
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            int index = i; // 로컬 변수로 캡처
            sockets[i].selectEntered.RemoveListener(args => OnCardPlaced(args, index));
            sockets[i].selectExited.RemoveListener(args => OnCardRemoved(args, index));
        }
    }

    private void OnCardPlaced(SelectEnterEventArgs args, int index)
    {
        CardBehavior cardBehavior = args.interactableObject.transform.GetComponent<CardBehavior>();
        if (cardBehavior != null && cardBehavior.cardData != null)
        {
            cardOrderArray[index] = cardBehavior.cardData.CardValue.ToString();
            UpdateDisplay();
        }
    }

    private void OnCardRemoved(SelectExitEventArgs args, int index)
    {
        cardOrderArray[index] = ""; // 해당 슬롯 비우기
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < cardTexts.Length; i++)
        {
            cardTexts[i].text = cardOrderArray[i];
        }
    }

}
