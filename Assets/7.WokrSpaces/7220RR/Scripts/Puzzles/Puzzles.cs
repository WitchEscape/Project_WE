using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class Puzzles : MonoBehaviour
{
    public PuzzleType puzzleType;
    public bool isActivatedObject;
    public GameObject ActivatedObject;

    [HideInInspector]
    public Activated activatedObject;
    public UnityEvent ClearEvent;

    public GhostCanvas ghostCanvas;
    public int clearNum;

    public AudioClip clearClip;
    public float clearClipValue;
    public AudioClip subClip;
    public float subClipValue;

    #region Slot
    public InteractorType interactorType;
    public List<GameObject> socketInteractors;
    public List<GameObject> interactors;
    public string interactorName;
    private int interactorCount;
    private int totalNum;
    #endregion
    #region Dial
    public int dialCount;
    public List<GameObject> _dialInteractables;
    [HideInInspector]
    public List<XRBaseInteractable> dialInteractables;
    [HideInInspector]
    public List<Knob> dialKnob;

    public bool isString;
    public InteractableType interactableType;
    public List<int> passward;
    private List<float> currentPassward = new List<float>();
    public Axis rotationAxis;
    public Axis controllerAxis;
    public int offSetAngle = 18;
    public bool isDialClamped;
    public List<TextMeshProUGUI> dialTexts;

    public List<Renderer> dialRenderers;
    public Material changeMaterial;
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
    private List<Material> materials = new List<Material>();
    private Dictionary<string, Color> subColors = new Dictionary<string, Color>();
    #endregion

    //클리어가 되면
    //다이얼 같은 경우
    //=> 다이얼이 서서히 사라지게
    //키패드 같은 경우
    //=> 눌리기는 하지만 실제 상호작용을 하지 않게
    //버튼, 소켓


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
                CurrentPasswardReset();
                PasswardCheak();
                DialInteractablesEventSet();
                break;
            case PuzzleType.Keypad:
                KeypadInitialization();
                KeypadInteractableEventSet();
                break;
            case PuzzleType.ColorButton:
                if (!isTrigger)
                {
                    SubColorSet();
                    ColorButtonMaterialsSet();
                    ColorButtonInteratableEventSet();
                    ColorSet();
                }
                break;
            default:
                Debug.LogError("Puzzels / Awake / PuzzleType is Error");
                break;
        }
    }

    private void PuzzleClear()
    {
        print("풀림");
        if (isActivatedObject)
            activatedObject.Activate();
        ClearEvent.Invoke();
        print("풀림");
        //임시
        //gameObject.SetActive(false);
        if (ghostCanvas != null)
            ghostCanvas.ClearPuzzle(clearNum);

        if (AudioManager.Instance != null && clearClip != null)
        {
            AudioManager.Instance.PlaySFX(clearClip, clearClipValue);
        }

        if (isActivatedObject)
            activatedObject.enabled = false;
    }

    private void SubClipPlay()
    {
        if (AudioManager.Instance != null && subClip != null)
        {
            AudioManager.Instance.PlaySFX(subClip, subClipValue);
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
            int index = i;
            totalNum++;
            if (socketInteractors[i].TryGetComponent<XRSocketInteractor>(out XRSocketInteractor socket))
            {
                socket.selectEntered.AddListener((arg) => SocketInteratorEnterEvent(arg, index));
                socket.selectExited.AddListener((arg) => SocketInteratorExitEventSet(arg, index));
            }
        }
    }

    private void SocketInteratorEnterEvent(SelectEnterEventArgs arg, int index)
    {
        print("Enter Index : " + index);
        if (arg.interactableObject.transform.name == interactors[index].name)
        {
            ++interactorCount;
            print("Total : " + totalNum);
            print("set :" + interactorCount);
            if (interactorCount == totalNum)
            {
                PuzzleClear();
            }
        }

        if (AudioManager.Instance != null && subClip != null)
        {
            AudioManager.Instance.PlaySFX(subClip, subClipValue);
        }
    }

    private void SocketInteratorExitEventSet(SelectExitEventArgs args, int index)
    {
        print("Exit Index : " + index);
        if (args.interactableObject.transform.gameObject.name == interactors[index].name)
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
            //print($"knob{temp} : {knob.handle.localEulerAngles}");
        }

        if (changeMaterial != null)
        {
            ClearEvent.AddListener(DialClearEventSet);
        }
    }

    private void DialClearEventSet()
    {
        for (int i = 0; i < dialCount; i++)
        {
            int index = i;
            dialKnob[index].selectExited.RemoveListener((x) => DialRotationSet(x, dialKnob[index].handle, index));
        }
        _ = StartCoroutine(DialAlphaChange());
    }

    private IEnumerator DialAlphaChange()
    {
        yield return null;
        foreach (Renderer renderer in dialRenderers)
        {
            renderer.material = changeMaterial;
        }

        Color newColor = changeMaterial.color;

        while (newColor.a <= 0.1f)
        {
            newColor.a = Mathf.Lerp(newColor.a, 0f, 0.01f);
            changeMaterial.color = newColor;
            yield return null;
        }

        newColor.a = 0f;
        changeMaterial.color = newColor;

        gameObject.SetActive(false);
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
            if (isString)
            {
                if (currentPassward[index] <= 9)
                {
                    char newChar = (char)('A' + currentPassward[index]);
                    dialTexts[index].text = newChar.ToString();
                }
                else
                {
                    dialTexts[index].text = "R";
                }
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
        PuzzleClear();
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
        if (isString)
        {
            if (currentPassward[index] < 9)
            {
                char newChar = (char)('A' + currentPassward[index]);
                dialTexts[index].text = newChar.ToString();
            }
            else
            {
                dialTexts[index].text = "R";
            }
        }
        print(currentPassward[index]);

        if (AudioManager.Instance != null && subClip != null)
        {
            AudioManager.Instance.PlaySFX(subClip, subClipValue);
        }
    }

    private void DialClearEvent()
    {

    }
    #endregion
    #region Keypad
    private void KeypadInitialization()
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

                ClearEvent.AddListener(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int index = i;
                        numPushButtons[index].OnPush.RemoveListener(() => { KeypadNumEvent(index); });
                        numPushButtons[index].OnPush.AddListener(SubClipPlay);
                    }
                    subPushButtons[0].OnPush.RemoveListener(keypadEnterButtonClickEvent);
                    subPushButtons[0].OnPush.AddListener(SubClipPlay);

                    subPushButtons[1].OnPush.RemoveListener(KeypadClearButtonClickEvent);
                    subPushButtons[1].OnPush.AddListener(SubClipPlay);
                });
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

                ClearEvent.AddListener(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int index = i;
                        numPressButton[index].OnPress.RemoveListener(() => KeypadNumEvent(index));
                        numPressButton[index].OnPress.AddListener(SubClipPlay);
                    }
                    subPressButton[0].OnPress.RemoveListener(keypadEnterButtonClickEvent);
                    subPressButton[0].OnPress.AddListener(SubClipPlay);
                    subPressButton[0].OnPress.RemoveListener(KeypadClearButtonClickEvent);
                    subPressButton[0].OnPress.AddListener(SubClipPlay);
                });
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
        if (AudioManager.Instance != null && subClip != null)
        {
            AudioManager.Instance.PlaySFX(subClip, subClipValue);
        }
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

    private void ColorButtonMaterialsSet()
    {
        if (materials.Count != colorButtonNum)
        {
            while (materials.Count > colorButtonNum) materials.RemoveAt(materials.Count - 1);
            while (materials.Count < colorButtonNum) materials.Add(null);
        }

        Material newMaterial;
        for (int i = 0; i < colorButtonNum; i++)
        {
            newMaterial = pushButtonTests[i].gameObject.GetComponent<Renderer>().material;
            materials[i] = newMaterial;
            currentPasswardColorIndexs.Add(0);
        }
    }

    private void ColorButtonInteratableEventSet()
    {
        for (int i = 0; i < pushButtonTests.Count; i++)
        {
            int index = i;
            pushButtonTests[index].OnPush.AddListener(() => PuzzleColorIndexChange(index));
        }

        ClearEvent.AddListener(() =>
        {
            for (int i = 0; i < pushButtonTests.Count; i++)
            {
                int index = i;
                pushButtonTests[index].OnPush.RemoveListener(() => PuzzleColorIndexChange(index));
                pushButtonTests[index].OnPush.AddListener(SubClipPlay);
            }

        });

    }

    private void PuzzleColorIndexChange(int index)
    {
        if (currentPasswardColorIndexs[index] + 1 >= Enum.GetValues(typeof(PuzzleColor)).Length)
        {
            currentPasswardColorIndexs[index] = 0;
        }
        else
        {
            currentPasswardColorIndexs[index]++;
        }

        ColorChange(index);

        if (subClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(subClip, subClipValue);
        }
    }

    private void ColorSet()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            int index = i;
            ColorChange(index);
        }
    }

    private void ColorChange(int index)
    {
        Color newColor;
        if (ColorUtility.TryParseHtmlString(((PuzzleColor)currentPasswardColorIndexs[index]).ToString(), out newColor))
        {
            materials[index].color = newColor;
        }
        else
        {
            materials[index].color = subColors[((PuzzleColor)currentPasswardColorIndexs[index]).ToString()];
        }
        ColorCheak();
    }

    private void ColorCheak()
    {
        for (int i = 0; i < colorButtonNum; i++)
        {
            if (currentPasswardColorIndexs[i] != (int)passwardColors[i])
            {
                return;
            }
        }

        PuzzleClear();
    }

    private void SubColorSet()
    {
        subColors.Add(PuzzleColor.Pink.ToString(), new Color(1f, 0.4f, 1f));
        subColors.Add(PuzzleColor.Orange.ToString(), new Color(1f, 0.5f, 0f));
        subColors.Add(PuzzleColor.Violet.ToString(), new Color(0.5f, 0f, 1f));
        subColors.Add(PuzzleColor.Skyblue.ToString(), new Color(0.53f, 0.81f, 0.92f));
    }

    public void TriggerEvent()
    {
        if (!isTrigger) return;
        ++passwordNum;

        if (passwordNum == passwardColors.Count)
        {
            isTrigger = false;
            Awake();
        }

    }
    #endregion
}
