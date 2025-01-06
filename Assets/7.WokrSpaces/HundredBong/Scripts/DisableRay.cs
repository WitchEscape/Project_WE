using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class DisableRay : MonoBehaviour
{
    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;

    public XRDirectInteractor leftDirectInteractor;
    public XRDirectInteractor rightDirectInteractor;

    public InputActionProperty leftTriggerAction;
    public InputActionProperty rightTriggerAction;

    private void Update()
    {
        //물건을 잡지 않은 상태에서 트리거 버튼을 조금이라도 눌러야 레이를 활성화 함
        leftRayInteractor.SetActive(leftDirectInteractor.interactablesSelected.Count == 0 && leftTriggerAction.action.ReadValue<float>() > 0.1f);
        rightRayInteractor.SetActive(rightDirectInteractor.interactablesSelected.Count == 0 && rightTriggerAction.action.ReadValue<float>() > 0.1f);
    }
}
