using System.Collections;
using TMPro;
using UnityEngine;

public class DialogTest : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    [SerializeField] private float typingSpeed = 0.05f;
    private Coroutine CoTyping;

    [Header("체크시 한글자씩 나옵니다!")]
    public bool isAnim = false;

    void Start()
    {
        DialogPlayer.Instance.OnDialogStart.AddListener((dialog) =>
        {
            if(dialog.DialogType == DialogType.Story)
            {
                if(isAnim == true)
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

    public void StartDialog()
    {
        DialogPlayer.Instance.PlayDialogSequence("LOBBY_01_");
    }

    public void StartDialog2()
    {
        DialogPlayer.Instance.PlayDialogSequence("LOBBY_02_");
    }

    public void StartDialog3()
    {
        DialogPlayer.Instance.PlayDialogSequence("LOBBY_03_");
    }

    void Update()
    {

    }
}
