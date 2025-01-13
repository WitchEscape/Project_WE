using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Puzzles : MonoBehaviour
{
    public PuzzleType puzzleType;
    public bool isActivatedObject;
    public GameObject ActivatedObject;
    [HideInInspector]
    public Activated activatedObject;

    #region Slot
    public InteractorType interactorType;
    public List<GameObject> socketInteractors;
    public List<GameObject> interactors;
    public string interactorName;
    private int interactorCount;
    private int currentNum;
    private int totalNum;
    #endregion
    #region Dial
    public int dialCount;
    public List<GameObject> _dialInteractables;
    [HideInInspector]
    public List<XRBaseInteractable> dialInteractables;
    [HideInInspector]
    public List<Knob> dialKnob;

    public InteractableType interactableType;
    public List<int> passward;
    private List<float> currentPassward = new List<float>();
    public Axis rotationAxis;
    public Axis controllerAxis;
    public int offSetAngle = 18;
    //임시 풀리면 어떻게 상호작용이 될지
    public bool isDialClear;
    public bool isDialClamped;
    #endregion
    #region Keypad
    public List<GameObject> _KeypadNum;
    #endregion
    private void Awake()
    {
        switch (puzzleType)
        {
            case PuzzleType.None:
                Debug.LogError("None Puzzle Type \n 할당 안할거면 지워주세요.");
                break;
            case PuzzleType.Slot:
                InteractorNameSet();
                FindSocket();
                break;
            case PuzzleType.Dial:
                isDialClear = false;
                CurrentPasswardReset();
                PasswardCheak();
                DialInteractablesEventSet();
                break;
            case PuzzleType.Keypad:
                break;
            default:
                break;
        }
    }

    #region Slot
    private void InteractorNameSet()
    {
        for (int i = 0; interactors.Count > i; i++)
        {
            if (interactors[i] == null) continue;
            interactors[i].name = $"{interactorName} {i}";
        }
    }

    private void FindSocket()
    {
        for (int i = 0; i < socketInteractors.Count; i++)
        {
            if (socketInteractors[i] == null) continue;
            currentNum = i;
            totalNum++;
            if (socketInteractors[i].TryGetComponent<XRSocketInteractor>(out XRSocketInteractor socket))
            {
                socket.selectEntered.AddListener(SocketInteratorEnterEvent);
                socket.selectExited.AddListener(SocketInteratorExitEventSet);
            }
        }
    }

    private void SocketInteratorEnterEvent(SelectEnterEventArgs arg)
    {
        if (arg.interactableObject.transform.name == interactors[currentNum].name)
        {

            ++interactorCount;

            if (interactorCount == totalNum && isActivatedObject)
            {
                activatedObject.activate();
            }
        }
    }

    private void SocketInteratorExitEventSet(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.gameObject.name == interactors[currentNum].name)
        {
            --interactorCount;
        }
    }
    #endregion
    #region Dial
    private void DialInteractablesEventSet()
    {
        for (int i = 0; i < dialCount; i++)
        {
            int index = i;
            switch (interactableType)
            {
                case InteractableType.None:
                    if (dialInteractables[i] == null)
                    {
                        Debug.LogError($"Puzzles / DialInteractables[{i}] is Null");
                        continue;
                    }
                    break;
                case InteractableType.Knob:
                    if (dialKnob[i] == null)
                    {
                        Debug.LogError($"Puzzles / DialKnob[{i}] is Null");
                        continue;
                    }
                    if (isDialClamped)
                    {
                        dialKnob[i].angleIncrement = offSetAngle;
                    }
                    dialKnob[i].interactableAxis = rotationAxis;
                    dialKnob[i].interactorAxis = controllerAxis;
                    dialKnob[i].selectExited.AddListener((x) => DialRotationSet(x, dialKnob[index].handle, index));
                    break;
            }
        }
    }

    private void DialRotationSet(SelectExitEventArgs args, Transform handle, int index)
    {
        Vector3 newHandleRotation = Vector3.zero;
        float angle = 0;
        switch (rotationAxis)
        {
            case Axis.XAxis:
                newHandleRotation.x = NormalizedAngle(handle.eulerAngles.x);
                angle = newHandleRotation.x;
                break;
            case Axis.YAxis:
                newHandleRotation.y = NormalizedAngle(handle.eulerAngles.y);
                angle = newHandleRotation.y;
                break;
            case Axis.ZAxis:
                newHandleRotation.z = NormalizedAngle(handle.eulerAngles.z);
                angle = newHandleRotation.z;
                break;
            default:
                Debug.LogError("Puzzles / RotationAxis is Error");
                break;
        }
        handle.localEulerAngles = newHandleRotation;
        CurrentPasswardSet(index, angle);
        PasswardCheak();
    }

    private float NormalizedAngle(float angle)
    {
        if (angle < 0f) angle += 360;
        angle = ((int)angle) % 360;
        return OffSetAngle(angle);
    }

    private float OffSetAngle(float angle)
    {
        if (offSetAngle <= 0)
        {
            return angle;
        }
        float tempAngle = Mathf.Abs(angle);
        tempAngle = tempAngle % offSetAngle;
        float subAngle = (tempAngle >= offSetAngle / 2) ? offSetAngle - tempAngle : -tempAngle;
        return angle += subAngle;
    }

    private void CurrentPasswardReset()
    {
        if (passward == null)
        {
            Debug.LogError("Puzzles / Passward is Null");
            return;
        }

        for (int i = 0; i < passward.Count; i++)
        {
            if (currentPassward.Count - 1 >= i)
            {
                currentPassward[i] = 0;
            }
            else
            {
                currentPassward.Add(0);
            }
        }
    }

    private void PasswardCheak()
    {
        if (passward == null)
        {
            Debug.LogError("Puzzles / Passward is Null");
            return;
        }

        for (int i = 0; i < passward.Count; i++)
        {
            if (passward.Count >= i && currentPassward.Count >= i)
            {
                if (passward[i] != currentPassward[i])
                {
                    return;
                }
            }
            else
            {
                Debug.LogError("Puzzels / Passward and currentPassward is Not Count");
            }
        }

        print("다이얼 풀림");
        isDialClear = true;
    }

    private void CurrentPasswardSet(int index, float angle)
    {
        if (currentPassward == null || currentPassward.Count <= index)
        {
            Debug.LogError("Puzzles / CurrentPassward is Error");
            return;
        }

        currentPassward[index] = ((angle / 36) >= 10) ? (angle / 36) - 10 : angle / 36;
        print(currentPassward[index]);
    }
    #endregion
    #region Keypad
    private void KeypadInteractableEventSet()
    {
        for (int i = 0; i < 10; i++)
        {

        }
    }
    #endregion
}
