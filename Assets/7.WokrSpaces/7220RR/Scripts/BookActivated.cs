using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BookActivated : Activated
{
    public XRGrabInteractable grab;
    public Rigidbody rigid;
    public bool isActive;

    private void Awake()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();
        if (rigid == null)
            rigid = GetComponent<Rigidbody>();
        OnOffActive();
    }

    public override void activate()
    {
        isActive = true;
        OnOffActive();
    }

    private void OnOffActive()
    {
        grab.enabled = isActive;
        rigid.isKinematic = !isActive;
    }
}
