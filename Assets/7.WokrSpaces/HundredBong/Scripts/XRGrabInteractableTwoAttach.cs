using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableTwoAttach : XRGrabInteractable
{

    public Transform leftAttachedTransform;
    public Transform rightAttachedTransform;

    public override Transform GetAttachTransform(IXRInteractor interactor)
    {
        //어태치 포인트를 양 손마다 지정하여 어느 손으로 잡던 어태치 포인트가 동일하도록 함
        //사용하려면 한쪽 손 어태치 포인트 설정 후 반대쪽 손은 값을 반대로 해줘야 함
        //Debug.Log("GetAttachTransform");

        Transform i_attachTransform = null;

        if (interactor.transform.CompareTag("Left Hand"))
        {
            //Debug.Log("Left");
            i_attachTransform = leftAttachedTransform;
        }
        if (interactor.transform.CompareTag("Right Hand"))
        {
            // Debug.Log("Right");
            i_attachTransform = rightAttachedTransform;
        }
        return i_attachTransform != null ? i_attachTransform : base.GetAttachTransform(interactor);
    }
}