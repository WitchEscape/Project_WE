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
    public int dailCount;
    public XRBaseInteractable[] dialInteractables;
    public Collider[] dialColliders;

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
            print($"{i + 1} 번 째 오브젝트 이름 변경");
            interactors[i].name = $"{interactorName} {i}";
        }
    }

    private void FindSocket()
    {
        for (int i = 0; i < socketInteractors.Count; i++)
        {
            if (socketInteractors[i] == null) continue;
            print($"{i + 1} 번 째 소켓에 이벤트 할당해요");
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
            print("카운트 높여요");

            ++interactorCount;

            if (interactorCount == totalNum && isActivatedObject)
            {
                print("퍼즐이 다 풀렸어요 \n 오브젝트 활성화 시켜요");
                activatedObject.activate();
            }
        }
    }

    private void SocketInteratorExitEventSet(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.gameObject.name == interactors[currentNum].name)
        {
            print("카운트 낮춰여");
            --interactorCount;
        }
    }
    #endregion
}
