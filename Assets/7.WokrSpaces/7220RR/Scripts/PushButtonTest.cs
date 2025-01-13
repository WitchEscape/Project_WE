using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class PushButtonTest : XRBaseInteractable
{
    public Transform button;
    public Axis pushAxis;
    public Axis interactorAxis;
    public float pushDistance;
    public float buttonSize;

    private bool isPush = true;
    private bool isPop = false;

    private bool isHover = false;
    private float currentHeight;
    public float duration = 0f;

    public UnityEvent OnPush;
    public UnityEvent OnPop;

    private Vector3 baseButtonPosition;
    private List<XRDirectInteractor> interactors;

    private void Start()
    {
        if (button != null)
        {
            baseButtonPosition = button.localPosition;
        }

        if (interactors == null)
        {
            interactors = new List<XRDirectInteractor>();
        }

        if (duration <= 0)
        {
            duration = 1f;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(StartHover);
        hoverExited.AddListener(EndHover);
        OnPush.AddListener(IsBoolReset);
        OnPop.AddListener(IsBoolReset);

        OnPush.AddListener(IsPushEventTest);
        OnPop.AddListener(IsPopEventTest);

        _ = StartCoroutine(ButtonPositionReset());
    }

    protected override void OnDisable()
    {
        StopCoroutine(ButtonPositionReset());

        hoverEntered.RemoveListener(StartHover);
        hoverExited.RemoveListener(EndHover);
        OnPop.RemoveListener(IsBoolReset);
        OnPush.RemoveListener(IsBoolReset);

        OnPush.RemoveListener(IsPushEventTest);
        OnPop.RemoveListener(IsPopEventTest);
        base.OnDisable();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (interactors.Count > 0 && interactors != null)
            {
                UpdatePress();
            }
        }
    }

    //private void UpdatePress()
    //{
    //    float clampedHeight = -pushDistance;

    //    float subLocalOffset = float.MaxValue;

    //    foreach (XRDirectInteractor interactor in interactors)
    //    {
    //        Transform interactorTransform = interactor.GetAttachTransform(this);
    //        Vector3 localOffset = transform.InverseTransformVector(interactorTransform.position - baseButtonPosition);

    //        //switch (interactorAxis)
    //        //{
    //        //    case Axis.XAxis:
    //        //        if (Mathf.Abs(localOffset.y) < buttonSize && Mathf.Abs(localOffset.z) < buttonSize)
    //        //        {
    //        //            if (localOffset.x < button.localPosition.x)
    //        //            {
    //        //                clampedHeight = Mathf.Max(clampedHeight, localOffset.x - button.localPosition.x);
    //        //            }
    //        //        }
    //        //        break;
    //        //    case Axis.YAxis:
    //        //        if (Mathf.Abs(localOffset.x) < buttonSize && Mathf.Abs(localOffset.z) < buttonSize)
    //        //        {
    //        //            if (localOffset.y < button.localPosition.y)
    //        //            {
    //        //                //clampedHeight = Mathf.Min(clampedHeight, localOffset.y - button.localPosition.y);
    //        //                clampedHeight = Mathf.Max(clampedHeight, localOffset.y - button.localPosition.y);
    //        //            }
    //        //        }
    //        //        break;
    //        //    case Axis.ZAxis:
    //        //        if (Mathf.Abs(localOffset.x) < buttonSize && Mathf.Abs(localOffset.y) < buttonSize)
    //        //        {
    //        //            if (localOffset.z < button.localPosition.z)
    //        //            {
    //        //                clampedHeight = Mathf.Max(clampedHeight, localOffset.z - button.localPosition.z);
    //        //            }
    //        //        }
    //        //        break;
    //        //    default:
    //        //        Debug.LogError("PushButtonTest / UpdatePress / InteractorAxis is Error");
    //        //        break;
    //        //}
    //        float tempLoaclOffset = Vector3.Distance(baseButtonPosition, localOffset);
    //        if (subLocalOffset > tempLoaclOffset)
    //        {
    //            subLocalOffset = tempLoaclOffset;
    //            clampedHeight = Mathf.Clamp(tempLoaclOffset, baseButtonPosition.x - pushDistance, baseButtonPosition.x);
    //        }
    //    }

    //    //switch (pushAxis)
    //    //{
    //    //    case Axis.XAxis:
    //    //        clampedHeight = Mathf.Min(clampedHeight, baseButtonPosition.x);
    //    //        break;
    //    //    case Axis.YAxis:
    //    //        //clampedHeight = Mathf.Max(clampedHeight, baseButtonPosition.y);
    //    //        clampedHeight = Mathf.Min(clampedHeight, baseButtonPosition.y);
    //    //        break;
    //    //    case Axis.ZAxis:
    //    //        clampedHeight = Mathf.Min(clampedHeight, baseButtonPosition.z);
    //    //        break;
    //    //    default:
    //    //        Debug.LogError("PusgButtonTest / UdatePress / PushAxis is Error");
    //    //        break;
    //    //}
    //    SetButtonHeight(clampedHeight);
    //}
    private void UpdatePress()
    {
        float targetHeight = 0f;
        foreach (XRDirectInteractor interactor in interactors)
        {
            Transform interactortransform = interactor.GetAttachTransform(this);
            Vector3 localOffset = transform.InverseTransformVector(interactortransform.position);

            float distance = 0f;
            switch (interactorAxis)
            {
                case Axis.XAxis:
                    distance = (localOffset.x - (baseButtonPosition.x + buttonSize / 2));
                    break;
                case Axis.YAxis:
                    distance = (localOffset.y - (baseButtonPosition.y + buttonSize / 2));
                    break;
                case Axis.ZAxis:
                    distance = (localOffset.z - (baseButtonPosition.z + buttonSize / 2));
                    break;
                default:
                    Debug.LogError("PushButtonTest / UpdatePress / InteractorAxis is Error");
                    break;
            }

            targetHeight = Mathf.Clamp01(distance);

        }

        SetButtonHeight(targetHeight);
    }

    private void EndHover(HoverExitEventArgs arg)
    {
        if (arg.interactorObject.transform.TryGetComponent<XRDirectInteractor>(out XRDirectInteractor interactor))
        {
            if (interactors.Contains(interactor))
            {
                interactors.Remove(interactor);
                isHover = false;
            }
            else
            {
                Debug.LogWarning("Find XRBaseInteractor is Not Set");
            }
        }
        else
        {
            Debug.LogWarning("Not Find XRBaseInteractor");
        }
    }

    private void StartHover(HoverEnterEventArgs arg)
    {
        if (arg.interactorObject.transform.TryGetComponent<XRDirectInteractor>(out XRDirectInteractor interactor))
        {
            if (interactors.Contains(interactor))
            {
                Debug.LogWarning("Find XRBaseInteractor is Not Reset");
            }
            else
            {
                interactors.Add(interactor);
                isHover = true;
            }
        }
        else
        {
            Debug.LogWarning("Not Find XRBaseInteractor");
        }
    }

    private void SetButtonHeight(float height)
    {
        if (button == null)
        {
            Debug.LogError("PushButtonTest / SetButtonHeight / Button is null");
            return;
        }

        Vector3 newPosition = button.localPosition;
        switch (pushAxis)
        {
            case Axis.XAxis:
                newPosition.x = Mathf.Lerp(baseButtonPosition.x - pushDistance, baseButtonPosition.x, height);
                if (newPosition.x == baseButtonPosition.x - pushDistance)
                {
                    isPop = true;
                    if (isPush) OnPush.Invoke();
                }
                if (newPosition.x == baseButtonPosition.x)
                {
                    isPush = true;
                    if (isPop) OnPop.Invoke();
                }
                break;
            case Axis.YAxis:
                newPosition.y = Mathf.Lerp(baseButtonPosition.y - pushDistance, baseButtonPosition.y, height);
                if (newPosition.y == baseButtonPosition.y - pushDistance)
                {
                    isPop = true;
                    if (isPush) OnPush.Invoke();
                }
                if (newPosition.y == baseButtonPosition.y)
                {
                    isPush = true;
                    if (isPop) OnPop.Invoke();
                }
                break;
            case Axis.ZAxis:
                newPosition.z = Mathf.Lerp(baseButtonPosition.z - pushDistance, baseButtonPosition.z, height);
                if (newPosition.z == baseButtonPosition.z - pushDistance)
                {
                    isPop = true;
                    if (isPush) OnPush.Invoke();
                }
                if (newPosition.z == baseButtonPosition.z)
                {
                    isPush = true;
                    if (isPop) OnPop.Invoke();
                }
                break;
            default:
                Debug.LogError("PushButtonTest / SetButtonHeight / pushAxis is Error"); ;
                break;
        }

        button.localPosition = newPosition;
        currentHeight = height;
    }

    private void IsBoolReset()
    {
        isPush = false;
        isPop = false;
    }

    private void IsPushEventTest()
    {
        print("눌림");
    }

    private void IsPopEventTest()
    {
        print("때짐");
    }

    private IEnumerator ButtonPositionReset()
    {
        float subTime = 0f;
        while (true)
        {
            if (isHover)
            {
                subTime = 0f;
                yield return null;
                continue;
            }
            while (Vector3.Distance(baseButtonPosition, button.localPosition) > 0.1f)
            {
                button.localPosition = Vector3.Lerp(button.localPosition, baseButtonPosition, subTime / duration);
                subTime += Time.deltaTime;
                yield return null;
            }

            SetButtonHeight(1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 newButtonPoint = Vector3.zero;
        if (button != null)
        {
            newButtonPoint = button.localPosition;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(newButtonPoint, new Vector3(buttonSize, buttonSize, buttonSize));
    }
}
