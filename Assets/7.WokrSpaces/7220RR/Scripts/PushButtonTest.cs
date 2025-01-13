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

    public bool isToggle;
    private bool isPush = false;
    private bool isPop = false;

    public UnityEvent OnPush;
    public UnityEvent OnPop;
    public ValueChangedEvent onValueChange;

    private Vector3 baseButtonPosition;
    private List<XRBaseInteractor> interactors;

    private void Start()
    {
        if (button != null)
        {
            baseButtonPosition = button.localPosition;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(StartHover);
        hoverExited.AddListener(EndHover);
        OnPush.AddListener(IsBoolReset);
        OnPop.AddListener(IsBoolReset);
    }

    protected override void OnDisable()
    {
        hoverEntered.RemoveListener(StartHover);
        hoverExited.RemoveListener(EndHover);
        OnPop.RemoveListener(IsBoolReset);
        OnPush.RemoveListener(IsBoolReset);
        base.OnDisable();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (interactors.Count > 0)
            {
                UpdatePress();
            }
        }
    }

    private void UpdatePress()
    {
        float clampedHeight = -pushDistance;

        foreach (XRBaseInteractor interactor in interactors)
        {
            Transform interactorTransform = interactor.GetAttachTransform(this);
            Vector3 localOffset = transform.InverseTransformVector(interactorTransform.position - baseButtonPosition);

            switch (interactorAxis)
            {
                case Axis.XAxis:
                    if (Mathf.Abs(localOffset.y) < buttonSize && Mathf.Abs(localOffset.z) < buttonSize)
                    {
                        if (localOffset.x < button.localPosition.x)
                        {
                            clampedHeight = Mathf.Min(clampedHeight, localOffset.x - button.localPosition.x);
                        }
                    }
                    break;
                case Axis.YAxis:
                    if (Mathf.Abs(localOffset.x) < buttonSize && Mathf.Abs(localOffset.z) < buttonSize)
                    {
                        if (localOffset.y < button.localPosition.y)
                        {
                            clampedHeight = Mathf.Min(clampedHeight, localOffset.y - button.localPosition.y);
                        }
                    }
                    break;
                case Axis.ZAxis:
                    if (Mathf.Abs(localOffset.x) < buttonSize && Mathf.Abs(localOffset.y) < buttonSize)
                    {
                        if (localOffset.y < button.localPosition.y)
                        {
                            clampedHeight = Mathf.Min(clampedHeight, localOffset.z - button.localPosition.z);
                        }
                    }
                    break;
                default:
                    Debug.LogError("PushButtonTest / UpdatePress / InteractorAxis is Error");
                    break;
            }

        }


        switch (pushAxis)
        {
            case Axis.XAxis:
                clampedHeight = Mathf.Max(clampedHeight, baseButtonPosition.x);
                break;
            case Axis.YAxis:
                clampedHeight = Mathf.Max(clampedHeight, baseButtonPosition.y);
                break;
            case Axis.ZAxis:
                clampedHeight = Mathf.Max(clampedHeight, baseButtonPosition.z);
                break;
            default:
                Debug.LogError("PusgButtonTest / UdatePress / PushAxis is Error");
                break;
        }

        SetButtonHeight(clampedHeight);
    }

    private void EndHover(HoverExitEventArgs arg)
    {
        if (arg.interactorObject.transform.TryGetComponent<XRBaseInteractor>(out XRBaseInteractor interactor))
        {
            if (interactors.Contains(interactor))
            {
                interactors.Remove(interactor);
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
        if (arg.interactorObject.transform.TryGetComponent<XRBaseInteractor>(out XRBaseInteractor interactor))
        {
            if (interactors.Contains(interactor))
            {
                Debug.LogWarning("Find XRBaseInteractor is Not Reset");
            }
            else
            {
                interactors.Add(interactor);
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
                newPosition.x += height;
                if (newPosition.x == baseButtonPosition.x - pushDistance)
                {
                    isPush = true;
                }
                if (newPosition.x == baseButtonPosition.x)
                {
                    isPop = true;
                }
                break;
            case Axis.YAxis:
                newPosition.y += height;
                if (newPosition.y == baseButtonPosition.y - pushDistance)
                {
                    isPush = true;
                }
                if (newPosition.y == baseButtonPosition.y)
                {
                    isPop = true;
                }
                break;
            case Axis.ZAxis:
                newPosition.z += height;
                if (newPosition.z == baseButtonPosition.z - pushDistance)
                {
                    isPush = true;
                }
                if (newPosition.z == baseButtonPosition.z)
                {
                    isPop = true;
                }
                break;
            default:
                Debug.LogError("PushButtonTest / SetButtonHeight / pushAxis is Error"); ;
                break;
        }

        if (isPush)
        {
            OnPush.Invoke();
        }
        else if (isPop)
        {
            OnPop.Invoke();
        }

        button.localPosition = newPosition;
    }

    private void IsBoolReset()
    {
        isPush = false;
        isPop = false;
    }
}
