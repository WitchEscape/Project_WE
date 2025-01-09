using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Puzzles))]
public class PuzzleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Puzzles puzzles = (Puzzles)target;

        puzzles.puzzleType = (PuzzleType)EditorGUILayout.EnumPopup("Puzzle Type", puzzles.puzzleType);
        puzzles.isActivatedObject = EditorGUILayout.Toggle("활성화 여부", puzzles.isActivatedObject);

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

        switch (puzzles.puzzleType)
        {
            case PuzzleType.None:
                EditorGUILayout.LabelField("아무것도 없어요", EditorStyles.boldLabel);
                break;
            case PuzzleType.Slot:
                puzzles.interactorType = (InteractorType)EditorGUILayout.EnumPopup("상호작용 방식", puzzles.interactorType);
                break;
            default:
                break;
        }

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

        if (GUI.changed)
        {
            EditorUtility.SetDirty(puzzles);
        }
    }
}
