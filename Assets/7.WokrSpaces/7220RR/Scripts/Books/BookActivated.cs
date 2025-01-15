using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BookActivated : Activated
{
    public Rigidbody rigid;
    [HideInInspector]
    public bool isActive;
    public bool isTrigger;
    public BookAnimationControll bookAnimationControll;
    public Collider myCollider;

    protected override void Awake()
    {
        ActivateUI(false);

        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        ObjectOnOff<GameObject>(objects, false);
        ObjectOnOff<XRBaseInteractable>(interactables, false);

        if (rigid == null)
            rigid = GetComponent<Rigidbody>();
        if (bookAnimationControll == null)
            bookAnimationControll = GetComponent<BookAnimationControll>();
        if (myCollider == null)
            myCollider = GetComponent<Collider>();
        if (isTrigger)
            OnOffActive();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener((x) =>
            {
                myCollider.isTrigger = true;
            });
            interactable.selectExited.AddListener((x) =>
            {
                myCollider.isTrigger = false;
            });
        }
    }

    public override void ActivateUI()
    {
        if (bookAnimationControll == null || bookAnimationControll.animator == null)
        {
            Debug.LogError("BookActivated / BookAnimationControll is Null");
        }

        if (bookAnimationControll.animator.GetBool("IsOpen"))
        {
            base.ActivateUI();
        }
    }

    public override void Activate()
    {
        if (isTrigger)
        {
            isActive = true;
            OnOffActive();
        }
    }

    private void OnOffActive()
    {
        interactable.enabled = isActive;
        rigid.isKinematic = !isActive;
    }
}
