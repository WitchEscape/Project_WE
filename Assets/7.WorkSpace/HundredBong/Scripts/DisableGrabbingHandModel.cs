using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

//물건 잡으면 잡은 손 모델 비활성화
public class DisableGrabbingHandModel : MonoBehaviour
{
    public GameObject leftHandModel;
    public GameObject rightHandModel;

    private XRGrabInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(HideGrabbingHand);
        interactable.selectExited.AddListener(ShowGrabbingHand);
    }


    public void HideGrabbingHand(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("Left Hand"))
        {
            leftHandModel.SetActive(false);
        }
        else if (args.interactorObject.transform.CompareTag("Right Hand"))
        {
            rightHandModel.SetActive(false);
        }

    }

    public void ShowGrabbingHand(SelectExitEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("Left Hand"))
        {
            leftHandModel.SetActive(true);
        }
        else if (args.interactorObject.transform.CompareTag("Right Hand"))
        {
            rightHandModel.SetActive(true);
        }
    }

}

