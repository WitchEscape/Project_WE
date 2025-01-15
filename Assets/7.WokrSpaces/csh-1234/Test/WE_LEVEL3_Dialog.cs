using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WE_LEVEL3_Dialog : MonoBehaviour
{
    public bool isActivate = false;
    public TextMeshProUGUI dialogText;
    [SerializeField] private float typingSpeed = 0.05f;
    private Coroutine CoTyping;

    private void Start()
    {
        //GetComponent<TriggerZone>().OnEnterEvent.AddListener(TriggerTest);

        DialogPlayer.Instance.OnDialogStart.AddListener((dialog) =>
        {
            if (dialog.DialogType == DialogType.Story)
            {
                dialogText.text = $"{dialog.SpeakerName} : {dialog.Text}";
                //if (CoTyping != null)
                //{
                //    StopCoroutine(CoTyping);
                //}
                //CoTyping = StartCoroutine(TypeText(dialog.SpeakerName, dialog.Text));
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (!isActivate)
            {
                DialogPlayer.Instance.PlayDialogSequence("POSSIONCLASS_01_");
                //isActivate = true;
            }
        }

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
