using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCardData", menuName = "Card Data", order = 1)]
public class CardData : ScriptableObject
{
    public string CardName; //카드 이름
    public int CardValue; // 카드 값
}
