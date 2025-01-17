using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotActive : Activated
{
    public GameObject number;

    public override void Activate()
    {
        number.SetActive(true);
    }
}
