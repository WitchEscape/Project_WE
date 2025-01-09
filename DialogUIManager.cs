using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogUIManager : MonoBehaviour
{
    public DialogManager dialogManager;
    public Text dialogText;
    
    public void ShowGreeting()
    {
        List<DialogData> greetings = dialogManager.GetDialog("greeting");
        foreach (var dialog in greetings)
        {
            Debug.Log($"{dialog.speaker}: {dialog.message}");
        }
        
        // UI에 시스템 인사말 표시
        string systemGreeting = dialogManager.GetMessage("greeting", "system");
        if (dialogText != null && !string.IsNullOrEmpty(systemGreeting))
        {
            dialogText.text = systemGreeting;
        }
    }

    public void ShowError()
    {
        string errorMessage = dialogManager.GetMessage("error", "system");
        if (dialogText != null && !string.IsNullOrEmpty(errorMessage))
        {
            dialogText.text = errorMessage;
        }
    }
} 