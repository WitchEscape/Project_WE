using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogTest : MonoBehaviour
{
    public TextMeshProUGUI dialogText;

    private Coroutine CoTyping;

    [Header("체크시 한글자씩 나옵니다!")]
    public bool isAnim = false;

    [Header("체크시 한글자씩 나올때의 출력 속도입니다!")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("입력하신 DialogId에 따라 대사가 출력됩니다.")]

    [Header("대사1")]
    public string DialogId1;
    [Header("대사2")]
    public string DialogId2;
    [Header("대사3")]
    public string DialogId3;
    [Header("대사4")]
    public string DialogId4;
    [Header("대사5")]
    public string DialogId5;
    [Header("대사6")]
    public string DialogId6;
    [Header("대사7")]
    public string DialogId7;
    [Header("대사8")]
    public string DialogId8;
    [Header("대사9")]
    public string DialogId9;
    [Header("대사10")]
    public string DialogId10;


    [Header("버튼입니다")]
    public Button Dialog1;
    public Button Dialog2;
    public Button Dialog3;
    public Button Dialog4;
    public Button Dialog5;
    public Button Dialog6;
    public Button Dialog7;
    public Button Dialog8;
    public Button Dialog9;
    public Button Dialog10;
    void Start()
    {
        Dialog1.onClick.AddListener(StartDialog1);
        Dialog2.onClick.AddListener(StartDialog2);
        Dialog3.onClick.AddListener(StartDialog3);
        Dialog4.onClick.AddListener(StartDialog4);
        Dialog5.onClick.AddListener(StartDialog5);
        Dialog6.onClick.AddListener(StartDialog6);
        Dialog7.onClick.AddListener(StartDialog7);
        Dialog8.onClick.AddListener(StartDialog8);
        Dialog9.onClick.AddListener(StartDialog9);
        Dialog10.onClick.AddListener(StartDialog10);


        DialogPlayer.Instance.OnDialogStart.AddListener((dialog) =>
        {
            if (dialog.DialogType == DialogType.Story)
            {
                if (isAnim == true)
                {
                    if (CoTyping != null)
                    {
                        StopCoroutine(CoTyping);
                    }
                    CoTyping = StartCoroutine(TypeText(dialog.SpeakerName, dialog.Text));
                }
                else
                {
                    dialogText.text = $"{dialog.SpeakerName} : {dialog.Text}";
                }

            }
            else
            {
                if (isAnim == true)
                {
                    if (CoTyping != null)
                    {
                        StopCoroutine(CoTyping);
                    }
                    CoTyping = StartCoroutine(TypeText(dialog.SpeakerName, dialog.Text));
                }
                else
                {
                    dialogText.text = $"{dialog.SpeakerName} : {dialog.Text}";
                }
            }
        });

        DialogPlayer.Instance.OnDialogEnd.AddListener(() =>
        {
            if (CoTyping != null)
            {
                StopCoroutine(CoTyping);
                CoTyping = null;
            }
            dialogText.text = "";
        });
    }

    private IEnumerator TypeText(string speaker, string text)
    {
        string fullText = $"{speaker} : {text}";
        dialogText.text = "";

        string currentText = "";
        string currentTag = "";
        bool isTag = false;

        foreach (char letter in fullText)
        {
            if (letter == '<')
            {
                isTag = true;
                currentTag += letter;
                continue;
            }

            if (isTag)
            {
                currentTag += letter;
                if (letter == '>')
                {
                    isTag = false;
                    currentText += currentTag;
                    currentTag = "";
                }
                continue;
            }

            currentText += letter;
            dialogText.text = currentText;

            if (!isTag)
            {
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }

    /// <summary>
    /// 타이핑 즉시완료
    /// </summary>
    public void CompleteTyping()
    {
        if (CoTyping != null)
        {
            StopCoroutine(CoTyping);
            CoTyping = null;

            var currentDialog = DialogPlayer.Instance.GetCurrentDialog();
            if (currentDialog != null)
            {
                dialogText.text = $"{currentDialog.SpeakerName} : {currentDialog.Text}";
            }
        }
    }

    public void StartDialog1()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId1);
    }

    public void StartDialog2()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId2);
    }

    public void StartDialog3()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId3);
    }
    public void StartDialog4()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId4);
    }
    public void StartDialog5()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId5);
    }
    public void StartDialog6()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId6);
    }
    public void StartDialog7()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId7);
    }
    public void StartDialog8()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId8);
    }
    public void StartDialog9()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId9);
    }
    public void StartDialog10()
    {
        DialogPlayer.Instance.PlayDialogSequence(DialogId10);
    }


}
