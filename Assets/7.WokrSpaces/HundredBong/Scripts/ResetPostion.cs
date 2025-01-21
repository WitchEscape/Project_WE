using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetPostion : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    [SerializeField, Header("포션 잡았을때 재생할 클립")] private AudioClip grabPotionClip;
    private XRAlyxGrabInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRAlyxGrabInteractable>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void OnEnable()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        interactable.selectEntered.AddListener(PlayGrapPotionClip);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(PlayGrapPotionClip);
    }

    private void PlayGrapPotionClip(SelectEnterEventArgs arg)
    {
        if (grabPotionClip != null)
        { 
            AudioManager.Instance?.PlaySFX(grabPotionClip);
        }
    }
}
