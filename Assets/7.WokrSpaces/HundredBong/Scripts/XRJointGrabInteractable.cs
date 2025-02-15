using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRJointGrabInteractable : XRGrabInteractable
{
    public float maxDistance = 0.5f;

    private void Update()
    {
        if (isSelected && firstInteractorSelecting is XRDirectInteractor)
        {
            float currentDistance = Vector3.Distance(firstInteractorSelecting.transform.position, transform.position);

            //Debug.Log($"Current Distance : {currentDistance}");

            if (currentDistance > maxDistance)
            {
                //Drop();
                //Debug.Log("Drop 실행");

                //상호작용 강제 종료
                interactionManager.SelectExit(firstInteractorSelecting, this);
            }
        }
    }
}

