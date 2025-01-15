using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogSettings : MonoBehaviour
{
    [Header("타이핑 속도")]
    [SerializeField] private float typingSpeed = 0.05f;
    public TextMeshProUGUI dialogText;
    private Coroutine CoTyping;

    [Header("체크시 타이핑 효과 적용")]
    public bool isTypingEffect = false;

    private void Start()
    {
        DialogPlayer.Instance.OnDialogStart.AddListener((dialog) =>
        {
            // Dialog Type으로도 분류 가능
            if(isTypingEffect == true)
            {
                dialogText.text = $"{dialog.SpeakerName} : {dialog.Text}";
            }
            else
            {
                if (CoTyping != null)
                {
                    StopCoroutine(CoTyping);
                }
                CoTyping = StartCoroutine(TypeText(dialog.SpeakerName, dialog.Text));
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
}