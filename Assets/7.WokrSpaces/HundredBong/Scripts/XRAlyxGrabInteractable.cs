using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAlyxGrabInteractable : XRGrabInteractable
{
    //Grab Interactable 대신 이 컴포넌트 넣으면 레이로 선택하고 손을 움직이면 물건이 날아옴
    public float velocityThreshold = 2f;
    public float jumpAngleInDegree = 60f;

    private ParticleSystem dropParticle;

    private XRRayInteractor rayInteractor;
    private Vector3 previousPos;
    private Rigidbody interactableRigidbody;
    private bool canJump = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(attack);
    }

    public void attack(SelectEnterEventArgs arg0)
    {
        print("attack");
        attachTransform = GetAttachTransform(arg0.interactorObject);
    }

    protected override void Awake()
    {
        base.Awake();
        interactableRigidbody = GetComponent<Rigidbody>();
        dropParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (isSelected && firstInteractorSelecting is XRRayInteractor && canJump)
        {
            Vector3 velocity = (rayInteractor.transform.position - previousPos) / Time.deltaTime;
            previousPos = rayInteractor.transform.position;

            if (velocity.magnitude > velocityThreshold)
            {
                Drop();
                interactableRigidbody.velocity = ComputeVelocity();
                canJump = false;

                //par.transform.position = previousPos;
                if (dropParticle != null)
                {
                    dropParticle.Play();
                }
            }
        }
    }

    public Vector3 ComputeVelocity()
    {
        Vector3 diff = rayInteractor.transform.position - transform.position;
        Vector3 diffXZ = new Vector3(diff.x, 0, diff.z);
        float diffXZLength = diffXZ.magnitude;
        float diffYLength = diff.y;

        float angleInRadian = Mathf.Clamp(diff.normalized.y * 90, jumpAngleInDegree, 90) * Mathf.Deg2Rad;

        float jumpSpeed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(diffXZLength, 2)
            / (2 * Mathf.Cos(angleInRadian) * Mathf.Cos(angleInRadian) * (diffXZ.magnitude * Mathf.Tan(angleInRadian) - diffYLength)));

        Vector3 jumpVelocityVector = diffXZ.normalized * Mathf.Cos(angleInRadian) * jumpSpeed + Vector3.up * Mathf.Sin(angleInRadian) * jumpSpeed;

        return jumpVelocityVector;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor)
        {
            trackPosition = false;
            trackRotation = false;
            throwOnDetach = false;

            rayInteractor = (XRRayInteractor)args.interactorObject;
            previousPos = rayInteractor.transform.position;
            canJump = true;
        }
        else
        {
            trackPosition = true;
            trackRotation = true;
            throwOnDetach = true;
        }

        base.OnSelectEntered(args);
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        //hoverParticle.Play();
    }
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        //hoverParticle.Stop();
    }

    #region 핸드 데이터 쓰면서 안쓰이게될듯함

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
            Debug.Log("Left");
            i_attachTransform = leftAttachedTransform;
        }
        if (interactor.transform.CompareTag("Right Hand"))
        {
            Debug.Log("Right");
            i_attachTransform = rightAttachedTransform;
        }
        return i_attachTransform != null ? i_attachTransform : base.GetAttachTransform(interactor);
    }
    #endregion
}

