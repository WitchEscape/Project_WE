using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SetTurnType : MonoBehaviour
{
    public ActionBasedSnapTurnProvider snapTurn;
    public ActionBasedContinuousTurnProvider continuousTurn;

    public ActionBasedControllerManager snapTurnManager;
    public ActionBasedControllerManager continuosTurnManager;

    public void SetTypeFromIndex(int index)
    {
        switch (index)
        {
            case 0:
                snapTurn.enabled = true;
                snapTurnManager.enabled = true;
                continuousTurn.enabled = false;
                continuosTurnManager.enabled = false;
                break;
            case 1:
                snapTurn.enabled = false;
                snapTurnManager.enabled= false;
                continuousTurn.enabled = true;
                continuosTurnManager.enabled = true;
                break;
        }
    }
}
