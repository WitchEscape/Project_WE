using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialGrab : MonoBehaviour
{
    private XRAlyxGrabInteractable alyx;
    public UnityEvent DropPotion;
    private void Awake()
    {
        alyx = GetComponent<XRAlyxGrabInteractable>();
    }

    private void Start()
    {
        alyx.selectExited.AddListener(GetItem);
    }

    private void GetItem(SelectExitEventArgs arg)
    {
        if (arg.interactorObject is XRDirectInteractor)
        {
            DropPotion?.Invoke();
        }
    }
}
