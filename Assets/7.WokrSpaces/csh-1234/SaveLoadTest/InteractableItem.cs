using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using TMPro;

public class InteractableItem : MonoBehaviour, ISaveableComponent
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private XRSocketInteractor currentSocket;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }

        if (textMeshPro == null)
        {
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        currentSocket = GetComponent<XRSocketTagInteractor>();
        grabInteractable = GetComponent<XRAlyxGrabInteractable>();
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    public void CollectSaveData(Dictionary<string, object> saveData)
    {
        if (currentSocket != null)
        {
            var socketSaveable = currentSocket.GetComponent<SaveableObject>();
            if (socketSaveable != null)
            {
                saveData["socketID"] = socketSaveable.UniqueID;
                saveData["isSocketed"] = "true";
            }
        }
        else
        {
            saveData["isSocketed"] = "false";
        }

        if (textMeshPro != null)
        {
            var textData = new TextData
            {
                content = textMeshPro.text,
                color = textMeshPro.color,
                fontSize = textMeshPro.fontSize
            };
            string json = JsonUtility.ToJson(textData);
            saveData["textData"] = json;
        }
    }

    public void LoadFromSaveData(Dictionary<string, object> saveData)
    {
        try
        {
            if (saveData == null) return;

            if (textMeshPro != null && saveData.TryGetValue("textData", out object textDataObj))
            {
                string json = textDataObj as string;
                if (!string.IsNullOrEmpty(json))
                {
                    var textData = JsonUtility.FromJson<TextData>(json);
                    if (textData != null)
                    {
                        textMeshPro.text = textData.content;
                        textMeshPro.color = textData.color;
                        textMeshPro.fontSize = textData.fontSize;
                    }
                }
            }

            if (saveData.TryGetValue("isSocketed", out object isSocketedObj))
            {
                string isSocketedStr = isSocketedObj as string;
                if (isSocketedStr == "true" && saveData.TryGetValue("socketID", out object socketIDObj))
                {
                    string socketID = socketIDObj as string;
                    if (socketID != null)
                    {
                        var allSockets = FindObjectsOfType<XRSocketInteractor>();
                        foreach (var socket in allSockets)
                        {
                            if (socket == null) continue;

                            var socketSaveable = socket.GetComponent<SaveableObject>();
                            if (socketSaveable != null && socketSaveable.UniqueID == socketID)
                            {
                                StartCoroutine(DelayedSocketConnection(socket));
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private IEnumerator DelayedSocketConnection(XRSocketInteractor socket)
    {
        yield return new WaitForEndOfFrame();
        
        if (grabInteractable != null)
        {
            currentSocket = socket;
            socket.StartManualInteraction(grabInteractable);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor socket)
        {
            currentSocket = socket;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            currentSocket = null;
        }
    }


    /// <summary>
    /// 추가적인 컴포넌트 저장이 필요할 경우 추가해 쓰기
    /// </summary>
    [System.Serializable]
    private class TextData
    {
        public string content = "";
        public Color color = Color.white;
        public float fontSize = 12f;
    }
}