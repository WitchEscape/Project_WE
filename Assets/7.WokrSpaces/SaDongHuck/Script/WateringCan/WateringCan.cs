using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WateringCan : MonoBehaviour
{
    public ParticleSystem waterParticle; // 물 입자 효과
    private XRGrabInteractable grabInteractable;
    [SerializeField]
    private GameObject paticleObject;

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
            paticleObject?.SetActive(false);
        }
    }

    private void OnTriggerPressed(ActivateEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor)
        {
            Debug.Log("진행시켜!");

            if (waterParticle != null)
            {
                paticleObject?.SetActive(true);
                waterParticle.Play();

            }
        }
    }

    private void OnTriggerReleased(DeactivateEventArgs args)
    {
        // 물 뿌리기 멈춤
        if (args.interactorObject is XRDirectInteractor)
        {
            Debug.Log("멈춰!");
            if (waterParticle != null)
            {
                waterParticle.Stop();
                paticleObject?.SetActive(false);
            }
        }
    }
}
