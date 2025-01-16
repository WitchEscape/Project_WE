using System.Collections.Generic;
using TMPro;
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

    public List<TextMeshProUGUI> dialTexts;
    #endregion
    #region Keypad
    public List<GameObject> _KeypadNum;
    public List<GameObject> _KeypadSub;
    public List<PushButtonTest> numPushButtons;
    public List<PushButtonTest> subPushButtons;
    public List<PressButtonTest> numPressButton;
    public List<PressButtonTest> subPressButton;
    private bool isPull = false;
    public string password;
    private string currentPassword = null;
    private int passwordNum;
    public GameObject keypadMoniter;
    private Material keypadMoniterMaterial;
    private Color keypadMoniterBaseColor;
    public TextMeshPro keypadMoniterText1;
    public TextMeshProUGUI keypadMoniterText2;
    #endregion
    #region ColorButton
    public int colorButtonNum;
    public bool isTrigger;
    public List<PuzzleColor> passwardColors = new List<PuzzleColor>();
    public List<PushButtonTest> pushButtonTests = new List<PushButtonTest>();
    private List<int> currentPasswardColorIndexs = new List<int>();


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
                초기화();
                KeypadInteractableEventSet();
                break;
            case PuzzleType.ColorButton:
                break;
            default:
                Debug.LogError("Puzzels / Awake / PuzzleType is Error");
                break;
        }
    }

    private void PuzzleClear()
    {
        if (isActivatedObject)
            activatedObject.Activate();
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

            if (interactorCount == totalNum)
            {
                PuzzleClear();
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
                    if (dialKnob[index] == null)
                    {
                        Debug.LogError($"Puzzles / DialKnob[{i}] is Null");
                        continue;
                    }
                    if (isDialClamped)
                    {
                        dialKnob[index].angleIncrement = offSetAngle;
                    }
                    dialKnob[index].interactableAxis = rotationAxis;
                    dialKnob[index].interactorAxis = controllerAxis;
                    dialKnob[index].selectExited.AddListener((x) => DialRotationSet(x, dialKnob[index].handle, index));
                    break;
            }
        }
        int temp = 0;
        foreach (Knob knob in dialKnob)
        {
            ++temp;
            knob.handle.localEulerAngles = Vector3.zero;
            print($"knob{temp} : {knob.handle.localEulerAngles}");
        }
    }

    public void DialUIUPButtonEvent(int index)
    {
        Vector3 newHandleRotation = Vector3.zero;
        newHandleRotation[(int)rotationAxis] = offSetAngle + (currentPassward[index] * offSetAngle);

        dialKnob[index].handle.localEulerAngles = newHandleRotation;
        CurrentPasswardSet(index, newHandleRotation[(int)rotationAxis]);
        PasswardCheak();
    }
    public void DialUIDownButtonEvent(int index)
    {
        Vector3 newHandleRotation = Vector3.zero;
        newHandleRotation[(int)rotationAxis] = -offSetAngle + (currentPassward[index] * offSetAngle);

        dialKnob[index].handle.localEulerAngles = newHandleRotation;
        CurrentPasswardSet(index, newHandleRotation[(int)rotationAxis]);
        PasswardCheak();
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

        for (int i = 0; i < dialTexts.Count; i++)
        {
            int index = i;
            dialTexts[index].text = currentPassward[index].ToString();
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
        PuzzleClear();
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
        if (currentPassward[index] < 0)
        {
            currentPassward[index] += 10;
        }
        dialTexts[index].text = currentPassward[index].ToString();
        print(currentPassward[index]);
    }
    #endregion
    #region Keypad

    private void 초기화()
    {
        passwordNum = password.Length;
        keypadMoniterMaterial = keypadMoniter.GetComponent<MeshRenderer>().material;
        keypadMoniterBaseColor = keypadMoniterMaterial.color;
        KeypadMoniterTextChange(null);
    }

    private void KeypadInteractableEventSet()
    {

        switch (interactableType)
        {
            case InteractableType.Push:
                for (int i = 0; i < 10; i++)
                {
                    int index = i;
                    if (numPushButtons.Count < i || numPushButtons[i] == null)
                    {
                        Debug.LogError($"Puzzles / KeypadInteractableEventSet / NumPushButtons{i} is null");
                        continue;
                    }

                    numPushButtons[index].OnPush.AddListener(() => { KeypadNumEvent(index); });
                }
                if (subPushButtons != null && subPushButtons.Count >= 2)
                {
                    subPushButtons[0].OnPush.AddListener(keypadEnterButtonClickEvent);
                    subPushButtons[1].OnPush.AddListener(KeypadClearButtonClickEvent);
                }
                break;
            case InteractableType.Press:
                for (int i = 0; i < 10; i++)
                {
                    int index = i;
                    if (numPressButton.Count < i || numPressButton[i] == null)
                    {
                        Debug.LogError($"Puzzles / KeypadInteractableEventSet / NumPressButton{i} is null");
                        continue;
                    }

                    numPressButton[index].OnPress.AddListener(() => { KeypadNumEvent(index); });
                }
                if (subPressButton != null && subPressButton.Count >= 2)
                {
                    subPressButton[0].OnPress.AddListener(keypadEnterButtonClickEvent);
                    subPressButton[1].OnPress.AddListener(KeypadClearButtonClickEvent);
                }
                break;
            default:
                Debug.LogError("Puzzles / KeypadInteracbleEventSet / InteractableType is Error");
                break;
        }
    }

    private void KeypadNumEvent(int index)
    {
        if (isPull)
        {
            return;
        }

        if (currentPassword == null)
        {
            currentPassword = index.ToString();
        }
        else
        {
            currentPassword += index.ToString();
        }


        if (currentPassword.Length == passwordNum)
        {
            isPull = true;
        }

        KeypadMoniterTextChange(currentPassword);
    }

    private void KeypadMoniterTextChange(string text)
    {
        keypadMoniterText1.text = text;
        keypadMoniterText2.text = text;
    }

    private void KeypadClearButtonClickEvent()
    {
        currentPassword = null;
        isPull = false;
        KeypadMoniterTextChange(currentPassword);
    }


    //NOTE 
    //임시로 메시지만 바꿈
    private void keypadEnterButtonClickEvent()
    {
        if (currentPassword == password)
        {
            print("풀림");
            KeypadMoniterTextChange("Success");
            PuzzleClear();
        }
        else
        {
            KeypadMoniterTextChange("Fail");
            currentPassword = null;
        }
    }

    #endregion
    #region ColorButton

    #endregion
}
