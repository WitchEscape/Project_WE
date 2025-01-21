using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(Puzzles))]
public class PuzzleEditor : Editor
{
    private SerializedObject serializedObject;
    private SerializedProperty serializedProperty;

    private void OnEnable()
    {
        serializedObject = new SerializedObject(target);
        serializedProperty = serializedObject.FindProperty("setList");
    }
    public override void OnInspectorGUI()
    {
        Puzzles puzzles = (Puzzles)target;

        puzzles.puzzleType = (PuzzleType)EditorGUILayout.EnumPopup("Puzzle Type", puzzles.puzzleType);
        puzzles.isActivatedObject = EditorGUILayout.Toggle("활성화 여부", puzzles.isActivatedObject);

        puzzles.puzzleId = EditorGUILayout.TextField("퍼즐 아이디", puzzles.puzzleId);



        if (puzzles.ghostCanvas != null)
            puzzles.clearNum = EditorGUILayout.IntSlider(puzzles.clearNum, 0, 3);

        if (puzzles.isActivatedObject)
        {
            puzzles.ActivatedObject = (GameObject)EditorGUILayout.ObjectField("활성화 시킬 오브젝트", puzzles.ActivatedObject, typeof(GameObject), true);

            if (puzzles.ActivatedObject != null)
            {
                if (puzzles.ActivatedObject.TryGetComponent<Activated>(out Activated act))
                {
                    puzzles.activatedObject = act;
                }
                else
                {
                    puzzles.ActivatedObject = null;
                }
            }
        }

        puzzles.ghostCanvas = (GhostCanvas)EditorGUILayout.ObjectField("유령 친구", puzzles.ghostCanvas, typeof(GhostCanvas), true);

        if (puzzles.puzzleType != PuzzleType.None)
        {
            puzzles.clearClip = (AudioClip)EditorGUILayout.ObjectField("Clear Clip", puzzles.clearClip, typeof(AudioClip), false);
            puzzles.clearClipValue = EditorGUILayout.Slider("Clear Clip Value", puzzles.clearClipValue, 0f, 1f);
            puzzles.subClip = (AudioClip)EditorGUILayout.ObjectField("Sub Clip", puzzles.subClip, typeof(AudioClip), false);
            puzzles.subClipValue = EditorGUILayout.Slider("Sub Clip Value", puzzles.subClipValue, 0f, 1f);
        }

        switch (puzzles.puzzleType)
        {
            case PuzzleType.None:
                EditorGUILayout.LabelField("아무것도 없어요", EditorStyles.boldLabel);
                break;
            case PuzzleType.Slot:
                puzzles.interactorType = (InteractorType)EditorGUILayout.EnumPopup("상호작용 방식", puzzles.interactorType);
                break;
            case PuzzleType.Dial:
                puzzles.changeMaterial = (Material)EditorGUILayout.ObjectField("변경할 마테리얼", puzzles.changeMaterial, typeof(Material), true);

                if (puzzles.dialRenderers == null)
                {
                    puzzles.dialRenderers = new List<Renderer>();
                }
                if (puzzles.dialRenderers.Count != 8)
                {
                    while (puzzles.dialRenderers.Count < 8) puzzles.dialRenderers.Add(new Renderer());
                    while (puzzles.dialRenderers.Count > 8) puzzles.dialRenderers.RemoveAt(puzzles.dialRenderers.Count - 1);
                }

                for (int i = 0; i < 8; i++)
                {
                    puzzles.dialRenderers[i] = (Renderer)EditorGUILayout.ObjectField($"렌더러 {i}", puzzles.dialRenderers[i], typeof(Renderer), true);
                }


                puzzles.isString = EditorGUILayout.Toggle("문자", puzzles.isString);
                puzzles.rotationAxis = (Axis)EditorGUILayout.EnumPopup("오브젝트 회전 방향", puzzles.rotationAxis);
                puzzles.controllerAxis = (Axis)EditorGUILayout.EnumPopup("컨트롤러 회전 방향", puzzles.controllerAxis);
                puzzles.offSetAngle = EditorGUILayout.IntField("제한 각도", puzzles.offSetAngle);
                puzzles.offSetAngle = Mathf.Clamp(puzzles.offSetAngle, 0, 360);

                puzzles.isDialClamped = EditorGUILayout.Toggle("다이얼 회전 각도 제한", puzzles.isDialClamped);

                puzzles.dialCount = EditorGUILayout.IntSlider("다이얼 갯수", puzzles.dialCount, 0, 10);
                puzzles.interactableType = (InteractableType)EditorGUILayout.EnumPopup("Interactable Type", puzzles.interactableType);

                if (puzzles.dialTexts == null)
                {
                    puzzles.dialTexts = new List<TextMeshProUGUI>();
                }
                for (int i = 0; i < puzzles.dialCount; i++)
                {
                    while (puzzles.dialTexts.Count <= i)
                    {
                        puzzles.dialTexts.Add(null);
                    }
                    puzzles.dialTexts[i] = (TextMeshProUGUI)EditorGUILayout.ObjectField($"Text {i}", puzzles.dialTexts[i], typeof(TextMeshProUGUI), true);
                }

                break;
            case PuzzleType.Keypad:
                puzzles.interactableType = (InteractableType)EditorGUILayout.EnumPopup("Interactable Type", puzzles.interactableType);
                puzzles.password = EditorGUILayout.TextField("Password", puzzles.password);
                puzzles.keypadMoniter = (GameObject)EditorGUILayout.ObjectField("모니터", puzzles.keypadMoniter, typeof(GameObject), true);
                puzzles.keypadMoniterText1 = (TextMeshPro)EditorGUILayout.ObjectField("텍스트1", puzzles.keypadMoniterText1, typeof(TextMeshPro), true);
                puzzles.keypadMoniterText2 = (TextMeshProUGUI)EditorGUILayout.ObjectField("텍스트2", puzzles.keypadMoniterText2, typeof(TextMeshProUGUI), true);
                break;
            case PuzzleType.ColorButton:
                puzzles.colorButtonNum = EditorGUILayout.IntSlider("버튼 갯수", puzzles.colorButtonNum, 0, 10);
                puzzles.isTrigger = EditorGUILayout.Toggle("트리거", puzzles.isTrigger);
                EnumListSet(puzzles.passwardColors, puzzles.colorButtonNum, "변경 색상", "비밀번호 색상");
                //SetClass(puzzles.pushButtonTests, puzzles.colorButtonNum, "Test");
                SetList(puzzles.pushButtonTests, puzzles.colorButtonNum);

                for (int i = 0; i < puzzles.colorButtonNum; i++)
                {
                    puzzles.pushButtonTests[i] = (PushButtonTest)EditorGUILayout.ObjectField($"버튼 {i}", puzzles.pushButtonTests[i], typeof(PushButtonTest), true);
                }
                break;
            default:
                break;
        }

        if (puzzles.puzzleType == PuzzleType.Slot)
        {
            if (puzzles.interactorType != InteractorType.None)
            {
                if (puzzles.socketInteractors == null)
                {
                    puzzles.socketInteractors = new List<GameObject>();
                }

                int newSocketSize = EditorGUILayout.IntField("소켓 사이즈", puzzles.socketInteractors.Count);

                if (newSocketSize != puzzles.socketInteractors.Count)
                {
                    while (newSocketSize > puzzles.socketInteractors.Count) puzzles.socketInteractors.Add(null);
                    while (newSocketSize < puzzles.socketInteractors.Count) puzzles.socketInteractors.RemoveAt(puzzles.socketInteractors.Count - 1);
                }

                for (int i = 0; i < puzzles.socketInteractors.Count; i++)
                {
                    puzzles.socketInteractors[i] = (GameObject)EditorGUILayout.ObjectField($"Element {i}", puzzles.socketInteractors[i], typeof(GameObject), true);
                }
                //if (GUILayout.Button("Add Element"))
                //{
                //    puzzles.socketInteractors.Add(null);
                //}
            }

            if (puzzles.interactorType == InteractorType.Multiple)
            {
                if (puzzles.interactors == null)
                {
                    puzzles.interactors = new List<GameObject>();
                }

                int newInteractorSize = EditorGUILayout.IntField("오브젝트 사이즈", puzzles.interactors.Count);

                if (newInteractorSize != puzzles.interactors.Count)
                {
                    while (newInteractorSize > puzzles.interactors.Count) puzzles.interactors.Add(null);
                    while (newInteractorSize < puzzles.interactors.Count) puzzles.interactors.RemoveAt(puzzles.interactors.Count - 1);
                }

                for (int i = 0; i < puzzles.interactors.Count; i++)
                {
                    puzzles.interactors[i] = (GameObject)EditorGUILayout.ObjectField($"Element {i}", puzzles.interactors[i], typeof(GameObject), true);
                }

                puzzles.interactorName = EditorGUILayout.TextField("변경할 이름", puzzles.interactorName);
            }
        }
        else if (puzzles.puzzleType == PuzzleType.Dial)
        {
            #region 따로따로불편러
            //if (puzzles._dialInteractables != null && puzzles._dialInteractables.Count > 0)
            //    EditorGUILayout.LabelField("다이얼 상호작용", EditorStyles.boldLabel);

            //if (puzzles._dialInteractables == null)
            //{
            //    puzzles._dialInteractables = new List<GameObject>();
            //}
            //if (puzzles.dialInteractables == null)
            //{
            //    puzzles.dialInteractables = new List<XRBaseInteractable>();
            //}

            //int newDialInteractablesSize = puzzles.dialCount;

            //if (newDialInteractablesSize != puzzles._dialInteractables.Count)
            //{
            //    while (newDialInteractablesSize > puzzles._dialInteractables.Count) puzzles._dialInteractables.Add(null);
            //    while (newDialInteractablesSize < puzzles._dialInteractables.Count) puzzles._dialInteractables.RemoveAt(puzzles._dialInteractables.Count - 1);
            //}

            //if (newDialInteractablesSize != puzzles.dialInteractables.Count)
            //{
            //    while (newDialInteractablesSize > puzzles.dialInteractables.Count) puzzles.dialInteractables.Add(null);
            //    while (newDialInteractablesSize < puzzles.dialInteractables.Count) puzzles.dialInteractables.RemoveAt(puzzles.dialInteractables.Count - 1);
            //}

            //for (int i = 0; i < newDialInteractablesSize; i++)
            //{
            //    puzzles._dialInteractables[i] = (GameObject)EditorGUILayout.ObjectField($"Element {i}", puzzles._dialInteractables[i], typeof(GameObject), true);

            //    if (puzzles._dialInteractables[i] == null) continue;

            //    if (puzzles._dialInteractables[i].TryGetComponent<XRBaseInteractable>(out XRBaseInteractable xrbaseinteractable))
            //    {
            //        puzzles.dialInteractables[i] = xrbaseinteractable;
            //    }
            //    else
            //    {
            //        puzzles._dialInteractables[i] = null;
            //    }
            //}


            //if (puzzles._dialInteractables != null && puzzles._dialColliders.Count > 0)
            //    EditorGUILayout.LabelField("다이얼 콜라이더", EditorStyles.boldLabel);

            //if (puzzles._dialColliders == null)
            //{
            //    puzzles._dialColliders = new List<GameObject>();
            //}
            //if (puzzles.dialColliders == null)
            //{
            //    puzzles.dialColliders = new List<Collider>();
            //}

            //int newDialCollidersSize = puzzles.dialCount;

            //if (puzzles._dialColliders.Count != newDialCollidersSize)
            //{
            //    while (puzzles._dialColliders.Count > newDialCollidersSize) puzzles._dialColliders.RemoveAt(puzzles._dialColliders.Count - 1);
            //    while (puzzles._dialColliders.Count < newDialCollidersSize) puzzles._dialColliders.Add(null);
            //}
            //if (puzzles.dialColliders.Count != newDialCollidersSize)
            //{
            //    while (puzzles.dialColliders.Count > newDialCollidersSize) puzzles.dialColliders.RemoveAt(puzzles.dialColliders.Count - 1);
            //    while (puzzles.dialColliders.Count < newDialCollidersSize) puzzles.dialColliders.Add(null);
            //}

            //for (int i = 0; i < newDialCollidersSize; i++)
            //{
            //    puzzles._dialColliders[i] = (GameObject)EditorGUILayout.ObjectField($"Element {i}", puzzles._dialColliders[i], typeof(GameObject), true);

            //    if (puzzles._dialColliders[i] != null && puzzles._dialColliders[i].TryGetComponent<Collider>(out Collider collider))
            //    {
            //        puzzles.dialColliders[i] = collider;
            //    }
            //    else
            //    {
            //        puzzles._dialColliders[i] = null;
            //    }
            //}
            #endregion

            #region DialInteractbles
            int newDialInteractablesSize = puzzles.dialCount;

            if (puzzles._dialInteractables == null)
            {
                puzzles._dialInteractables = new List<GameObject>();
            }


            if (newDialInteractablesSize != puzzles._dialInteractables.Count)
            {
                while (newDialInteractablesSize > puzzles._dialInteractables.Count) puzzles._dialInteractables.Add(null);
                while (newDialInteractablesSize < puzzles._dialInteractables.Count) puzzles._dialInteractables.RemoveAt(puzzles._dialInteractables.Count - 1);
            }

            switch (puzzles.interactableType)
            {
                case InteractableType.None:
                    if (puzzles.dialInteractables == null)
                    {
                        puzzles.dialInteractables = new List<XRBaseInteractable>();
                    }
                    if (newDialInteractablesSize != puzzles.dialInteractables.Count)
                    {
                        while (newDialInteractablesSize > puzzles.dialInteractables.Count) puzzles.dialInteractables.Add(null);
                        while (newDialInteractablesSize < puzzles.dialInteractables.Count) puzzles.dialInteractables.RemoveAt(puzzles.dialInteractables.Count - 1);
                    }
                    break;
                case InteractableType.Knob:
                    if (puzzles.dialKnob == null)
                    {
                        puzzles.dialKnob = new List<Knob>();
                    }
                    if (newDialInteractablesSize != puzzles.dialKnob.Count)
                    {
                        while (newDialInteractablesSize > puzzles.dialKnob.Count) puzzles.dialKnob.Add(null);
                        while (newDialInteractablesSize < puzzles.dialKnob.Count) puzzles.dialKnob.RemoveAt(puzzles.dialKnob.Count - 1);
                    }
                    break;
            }




            #endregion
            #region DialColliders
            //if (puzzles._dialColliders == null)
            //{
            //    puzzles._dialColliders = new List<GameObject>();
            //}
            //if (puzzles.dialColliders == null)
            //{
            //    puzzles.dialColliders = new List<Collider>();
            //}

            //int newDialCollidersSize = puzzles.dialCount;

            //if (puzzles._dialColliders.Count != newDialCollidersSize)
            //{
            //    while (puzzles._dialColliders.Count > newDialCollidersSize) puzzles._dialColliders.RemoveAt(puzzles._dialColliders.Count - 1);
            //    while (puzzles._dialColliders.Count < newDialCollidersSize) puzzles._dialColliders.Add(null);
            //}
            //if (puzzles.dialColliders.Count != newDialCollidersSize)
            //{
            //    while (puzzles.dialColliders.Count > newDialCollidersSize) puzzles.dialColliders.RemoveAt(puzzles.dialColliders.Count - 1);
            //    while (puzzles.dialColliders.Count < newDialCollidersSize) puzzles.dialColliders.Add(null);
            //}
            #endregion
            #region password
            int newPasswordSize = puzzles.dialCount;
            if (puzzles.passward == null)
            {
                puzzles.passward = new List<int>();
            }

            if (puzzles.passward.Count != newPasswordSize)
            {
                while (puzzles.passward.Count > newPasswordSize) puzzles.passward.RemoveAt(puzzles.passward.Count - 1);
                while (puzzles.passward.Count < newPasswordSize) puzzles.passward.Add(0);
            }
            #endregion

            EditorGUILayout.LabelField("비밀번호", EditorStyles.boldLabel);
            for (int i = 0; i < newPasswordSize; i++)
            {
                puzzles.passward[i] = EditorGUILayout.IntSlider($"{i + 1}번 비밀번호", puzzles.passward[i], 0, 9);
            }

            EditorGUILayout.LabelField($"\n", EditorStyles.largeLabel);
            for (int i = 0; i < newDialInteractablesSize; i++)
            {
                EditorGUILayout.LabelField($"{i + 1}번 째 다이얼", EditorStyles.centeredGreyMiniLabel);
                //EditorGUILayout.CurveField($"{i}번 째 다이얼", EditorStyles.boldLabel);
                //EditorGUILayout.Toggle(true, EditorStyles.boldLabel);
                puzzles._dialInteractables[i] = (GameObject)EditorGUILayout.ObjectField($"Interactable {i + 1}", puzzles._dialInteractables[i], typeof(GameObject), true);
                //puzzles._dialColliders[i] = (GameObject)EditorGUILayout.ObjectField($"Collider {i + 1}", puzzles._dialColliders[i], typeof(GameObject), true);


                if (puzzles._dialInteractables[i] != null)
                {
                    switch (puzzles.interactableType)
                    {
                        case InteractableType.None:
                            if (puzzles._dialInteractables[i].TryGetComponent<XRBaseInteractable>(out XRBaseInteractable xrbaseinteractable))
                            {
                                puzzles.dialInteractables[i] = xrbaseinteractable;
                            }
                            else
                            {
                                puzzles._dialInteractables[i] = null;
                            }
                            break;
                        case InteractableType.Knob:
                            if (puzzles._dialInteractables[i].TryGetComponent<Knob>(out Knob knob))
                            {
                                puzzles.dialKnob[i] = knob;
                            }
                            else
                            {
                                puzzles._dialInteractables[i] = null;
                            }
                            break;
                        default:
                            EditorGUILayout.LabelField("아무것도 없어요", EditorStyles.boldLabel);
                            break;
                    }
                }
            }


        }
        else if (puzzles.puzzleType == PuzzleType.Keypad)
        {
            if (puzzles.numPushButtons == null) puzzles.numPushButtons = new List<PushButtonTest>();
            if (puzzles.subPushButtons == null) puzzles.subPushButtons = new List<PushButtonTest>();

            if (puzzles.numPressButton == null) puzzles.numPressButton = new List<PressButtonTest>();
            if (puzzles.subPressButton == null) puzzles.subPressButton = new List<PressButtonTest>();

            for (int i = 0; i < 10; i++)
            {
                switch (puzzles.interactableType)
                {
                    case InteractableType.Push:
                        while (puzzles.numPushButtons.Count <= i) puzzles.numPushButtons.Add(null);
                        puzzles.numPushButtons[i] = (PushButtonTest)EditorGUILayout.ObjectField($"Push {i}", puzzles.numPushButtons[i], typeof(PushButtonTest), true);
                        break;
                    case InteractableType.Press:
                        while (puzzles.numPressButton.Count <= i) puzzles.numPressButton.Add(null);
                        puzzles.numPressButton[i] = (PressButtonTest)EditorGUILayout.ObjectField($"Press {i}", puzzles.numPressButton[i], typeof(PressButtonTest), true);
                        break;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                switch (puzzles.interactableType)
                {
                    case InteractableType.Push:
                        while (puzzles.subPushButtons.Count <= i) puzzles.subPushButtons.Add(null);
                        puzzles.subPushButtons[i] = (PushButtonTest)EditorGUILayout.ObjectField($"Sub Push {i}", puzzles.subPushButtons[i], typeof(PushButtonTest), true);
                        break;
                    case InteractableType.Press:
                        while (puzzles.subPressButton.Count <= i) puzzles.subPressButton.Add(null);
                        puzzles.subPressButton[i] = (PressButtonTest)EditorGUILayout.ObjectField($"Sub Press {i}", puzzles.subPressButton[i], typeof(PressButtonTest), true);
                        break;
                }
            }
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(puzzles);
            //serializedObject.Update();
        }
    }

    public void SetClass<T>(List<T> setList, int count = 0, string name = "Element", string header = null) where T : class
    {
        setList ??= new List<T>();

        if (setList.Count != count)
        {
            while (setList.Count > count) setList.RemoveAt(setList.Count - 1);
            while (setList.Count < count) setList.Add(null);
        }

        List<GameObject> temps = new List<GameObject>();

        if (temps.Count != count)
        {
            while (temps.Count > count) temps.RemoveAt(temps.Count - 1);
            while (temps.Count < count) temps.Add(null);
        }


        if (header != null)
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);




        for (int i = 0; i < count; i++)
        {
            temps[i] = (GameObject)EditorGUILayout.ObjectField($"{name} {i}", temps[i], typeof(GameObject), true);
            //setList[i] = (GameObject)EditorGUILayout.ObjectField($"{name} {i}", setList[i] as GameObject, typeof(GameObject), true);

            if (temps[i] != null && temps[i].TryGetComponent<T>(out T tempClass))
            {
                setList[i] = tempClass;
            }
            else
            {
                temps[i] = null;
            }
        }
    }

    public void SetList<T>(List<T> setList, int count = 0) where T : class
    {
        setList ??= new List<T>();

        if (setList.Count != count)
        {
            while (setList.Count > count) setList.RemoveAt(setList.Count - 1);
            while (setList.Count < count) setList.Add(null);
        }
    }


    public void EnumListSet<T>(List<T> enumList, int count = 0, string name = "Element", string header = null) where T : Enum
    {

        if (header != null)
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);

        enumList ??= new List<T>();

        if (enumList.Count != count)
        {
            while (enumList.Count > count) enumList.RemoveAt(enumList.Count - 1); ;
            while (enumList.Count < count) enumList.Add(default);
        }

        for (int i = 0; i < count; i++)
        {
            enumList[i] = (T)EditorGUILayout.EnumPopup($"{name} {i}", enumList[i]);
        }
    }
}
