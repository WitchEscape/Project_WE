using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketTagInteractor : XRSocketInteractor
{
    //SocketInteractor에서 태그가 일치해야지만 작동하게 함
    //Socket Lock Interactor가 이미 있는데 써야할 필요성이 있을지는 모름
    [Header("상호작용할 타겟 태그")] public string targetTag;

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.transform.CompareTag(targetTag);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.transform.CompareTag(targetTag);
    }
}
