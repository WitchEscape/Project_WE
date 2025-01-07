using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WateringCan : MonoBehaviour
{
    public ParticleSystem waterParticle; // 물 입자 효과
    private XRGrabInteractable grabInteractable;

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Grab 이벤트 등록
        grabInteractable.activated.AddListener(OnTriggerPressed);
        grabInteractable.deactivated.AddListener(OnTriggerReleased);

        // 물 효과 초기화
        if (waterParticle != null)
        {
            waterParticle.Stop();
        }
    }

    private void OnTriggerPressed(ActivateEventArgs args)
    {
        // 물 뿌리기 시작
        if (waterParticle != null)
        {
            waterParticle.Play();
        }
    }

    private void OnTriggerReleased(DeactivateEventArgs args)
    {
        // 물 뿌리기 멈춤
        if (waterParticle != null)
        {
            waterParticle.Stop();
        }
    }
}
