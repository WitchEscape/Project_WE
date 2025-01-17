using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleDelay : MonoBehaviour
{

    private XRAlyxGrabInteractable grabInteractable;
    private void Start()
    {
        grabInteractable = GetComponent<XRAlyxGrabInteractable>();
        StartCoroutine(CoDelayEnable());
    }
    IEnumerator CoDelayEnable()
    {
        grabInteractable.enabled = false;
        yield return new WaitForSeconds(0.5f);
        grabInteractable.enabled = true;
    }
}
