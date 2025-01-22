using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE_Ending_Trigger2 : MonoBehaviour
{
    public bool isActivate = false;
    private void Start()
    {
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(TriggerActivate);
    }

    public void TriggerActivate(GameObject go)
    {
        if (!isActivate)
        {
            DialogPlayer.Instance.PlayDialogSequence("ENDING_02");
            isActivate = true;
        }
    }
}
