using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketTagInteractor : XRSocketInteractor
{
    //SocketInteractor에서 태그가 일치해야지만 작동하게 함
    //Socket Lock Interactor가 이미 있는데 써야할 필요성이 있을지는 모름
    [Header("상호작용할 타겟 태그")] public string[] targetTags;

    private bool isCorrectTag;

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        // Debug.Log($"히히 나도 모르겠다 {interactable.transform.transform.tag}");
        //배열에서 태그를 순회하며 비교
        foreach (string tag in targetTags)
        {
            if (interactable.transform.CompareTag(tag))
            {
                isCorrectTag = true;
                return base.CanHover(interactable) && isCorrectTag;
            }
            else
            {
                isCorrectTag = false;
            }
        }
        return base.CanHover(interactable) && isCorrectTag;//interactable.transform.CompareTag(targetTag);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        //Debug.Log($"히히 나도 모르겠다 {interactable.transform.transform.tag}");


        foreach (string tag in targetTags)
        {
            if (interactable.transform.CompareTag(tag))
            {
                isCorrectTag = true;
                return base.CanSelect(interactable) && isCorrectTag;
            }
            else
            {
                isCorrectTag = false;
            }
        }
        return base.CanSelect(interactable) && isCorrectTag;
    }
}
