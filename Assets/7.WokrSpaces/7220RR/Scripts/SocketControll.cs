using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketControll : MonoBehaviour
{
    [SerializeField]
    private List<XRSocketInteractor> sockets = new List<XRSocketInteractor>();
    [SerializeField]
    private List<XRGrabInteractable> grabs = new List<XRGrabInteractable>();
    [SerializeField]
    private List<Collider> colliders = new List<Collider>();
    [SerializeField]
    private string tagName;
    private Dictionary<Collider, XRGrabInteractable> objectDic = new Dictionary<Collider, XRGrabInteractable>();

    private void Awake()
    {
        for (int i = 0; i < grabs.Count; i++)
        {
            objectDic.Add(colliders[i], grabs[i]);
        }
        foreach (XRSocketInteractor socket in sockets)
        {
            socket.enabled = false;
            socket.selectExited.AddListener((x) => socket.enabled = false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (objectDic.ContainsKey(other) && !objectDic[other].isSelected)
        {
            SocketSet(objectDic[other].gameObject.transform.position);
        }
    }

    private void SocketSet(Vector3 targetPosition)
    {
        foreach (XRSocketInteractor socket in sockets)
        {
            if (!socket.enabled)
            {
                socket.enabled = true;
                Vector3 newPosition = socket.transform.position;
                newPosition.z = targetPosition.z;
                newPosition.y = targetPosition.y;
                socket.transform.position = newPosition;
                break;
            }
        }
    }
}
