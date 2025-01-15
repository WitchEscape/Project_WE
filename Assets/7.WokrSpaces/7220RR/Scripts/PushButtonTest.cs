using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class PushButtonTest : XRBaseInteractable
{
    public Transform button;
    public Axis pushAxis;
    public float pushDistance;
    public float accuracy;
    public float duration = 0f;

    public UnityEvent OnPush;
    public UnityEvent OnPop;

    private bool isPush = true;
    private Vector3 baseButtonPosition;
    private Vector3 pushButtonPosition;
    private List<XRDirectInteractor> interactors;
    private Coroutine currentCoroutine = null;

    private void Start()
    {
        if (button != null)
        {
            baseButtonPosition = button.localPosition;
            pushButtonPosition = button.localPosition;
            pushButtonPosition[(int)pushAxis] = pushButtonPosition[(int)pushAxis] - pushDistance;
        }

        if (interactors == null)
        {
            interactors = new List<XRDirectInteractor>();
        }

        if (duration <= 0)
        {
            duration = 1f;
        }

        if (accuracy <= 0)
        {
            accuracy = 1f;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(StartHover);
        hoverExited.AddListener(EndHover);

        OnPush.AddListener(IsPushEventTest);
        OnPop.AddListener(IsPopEventTest);
    }

    protected override void OnDisable()
    {
        hoverEntered.RemoveListener(StartHover);
        hoverExited.RemoveListener(EndHover);

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
            else if (currentCoroutine == null && !Mathf.Approximately(Vector3.Distance(button.localPosition, baseButtonPosition), 0))
            {
                currentCoroutine = StartCoroutine(ButtonPositionReset());
            }
        }
    }

    private void UpdatePress()
    {
        float targetHeight = 0f;
        float distance = 0f;
        foreach (XRDirectInteractor interactor in interactors)
        {
            Transform interactortransform = interactor.GetAttachTransform(this);
            Vector3 localOffset = transform.InverseTransformVector(interactortransform.position);
            float temp = (accuracy * localOffset[(int)pushAxis]) - (baseButtonPosition[(int)pushAxis] * accuracy);
            if (distance <= temp || distance == 0f)
            {
                distance = temp;
            }
            if (distance > (0.02f * accuracy))
            {
                targetHeight = Mathf.Clamp01(distance / (Mathf.Abs(pushDistance)) * accuracy);
            }
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
                if (currentCoroutine == null && !Mathf.Approximately(Vector3.Distance(button.localPosition, baseButtonPosition), 0f))
                {
                    currentCoroutine = StartCoroutine(ButtonPositionReset());
                }
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
            }
        }
        else
        {
            Debug.LogWarning("Not Find XRBaseInteractor");
        }

    }

    private void SetButtonHeight(float height)
    {
        print(height);
        if (button == null)
        {
            Debug.LogError("PushButtonTest / SetButtonHeight / Button is null");
            return;
        }

        Vector3 newPosition = button.localPosition;

        newPosition[(int)pushAxis] = Mathf.Lerp(baseButtonPosition[(int)pushAxis], pushButtonPosition[(int)pushAxis], height);
        if (Mathf.Approximately(Vector3.Distance(pushButtonPosition, newPosition), 0f) && isPush)
        {
            if (isPush)
            {
                print("IsPush");
                isPush = false;
                OnPush.Invoke();
            }
            newPosition = pushButtonPosition;
        }
        else if (Mathf.Approximately(Vector3.Distance(baseButtonPosition, newPosition), 0f) && !isPush)
        {
            print("IsPop");
            isPush = true;
            OnPop.Invoke();
            newPosition = baseButtonPosition;
        }

        button.localPosition = newPosition;
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
        yield return new WaitForSeconds(0.1f);
        float subTime = 0f;
        while (!Mathf.Approximately(Vector3.Distance(baseButtonPosition, button.localPosition), 0f) && interactors.Count <= 0)
        {
            button.localPosition = Vector3.Lerp(button.localPosition, baseButtonPosition, subTime / duration);
            subTime += Time.deltaTime;
            yield return null;
        }
        currentCoroutine = null;
        SetButtonHeight(0f);
    }
}
