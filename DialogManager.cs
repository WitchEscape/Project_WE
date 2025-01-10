using System.Collections.Generic;
using UnityEngine;

public string GetFormattedMessage(string situation, string speaker, Dictionary<string, string> variables)
{
    string message = GetMessage(situation, speaker);
    if (string.IsNullOrEmpty(message)) return null;
    
    foreach (var variable in variables)
    {
        message = message.Replace($"{{{variable.Key}}}", variable.Value);
    }
    return message;
}

public string GetRandomMessage(string situation)
{
    var situationDialogs = GetDialog(situation);
    if (situationDialogs.Count == 0) return null;
    
    int randomIndex = Random.Range(0, situationDialogs.Count);
    return situationDialogs[randomIndex].message;
} 