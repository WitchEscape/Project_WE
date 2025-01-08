using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Optional Starting item")]
    private XRBaseInteractable startingItem = null;

    [SerializeField]
    [Tooltip("Display used when holding slot is holding an item")]
    private GameObject slotDisplayWhenContainsItem = null;

    [SerializeField]
    [Tooltip("Display used when slot is empty and can add an item")]
    private GameObject slotDisplayToAddItem = null;
        
    [SerializeField]
    [Tooltip("Transform to hold the viewing model of the current Inventory Slot Item.")]
    private Transform itemModelHolder = null;

    [SerializeField]
    [Tooltip("Transform of back image that rotates during animations, used to attach ItemModelHolder to after positioning model")]
    private Transform backImagesThatRotate = null;

    [SerializeField]
    [Tooltip("Item will be scaled down to size to fit inside this box collider")]
    private BoxCollider inventorySize = null;

    [SerializeField] private GameObject leftEffect;
    [SerializeField] private GameObject rightEffect;

    [SerializeField] private new Collider collider = null;
    [SerializeField] private AudioSource grabAudio = null, releaseAudio = null;

    public XRBaseInteractable CurrentSlotItem => currentSlotItem;
    public UnityEvent inventorySlotUpdated;

    private XRBaseInteractable currentSlotItem;
    private Transform boundCenterTransform, itemSlotMeshClone;
    private XRInteractionManager interactionManager;
    private InventoryManager inventoryManager;

    //Animation
    private int disableAnimatorHash, enableAnimatorHash, onHoverAnimatorHash, resetAnimatorHash;

    private bool isBusy, isDisabling;
    private Animator addItemAnimator, hasItemAnimator;
    private TransformStruct startingTransformFromHand;
    private Vector3 goalSizeToFitInSlot;
    private const float AnimationDisableLength = .5f, AnimationLengthItemToSlot = .15f;

    private void Awake()
    {
        OnValidate();

        disableAnimatorHash = Animator.StringToHash("Disable");
        enableAnimatorHash = Animator.StringToHash("Enable");
        onHoverAnimatorHash = Animator.StringToHash("OnHover");
        resetAnimatorHash = Animator.StringToHash("Reset");
    }

    public IEnumerator CreateStartingItemAndDisable()
    {
        //Called from PlayerInventory, to give a frame for the start methods to be called on currentSlotItem
        if (startingItem)
        {
            currentSlotItem = Instantiate(startingItem, transform, true);
            yield return null;
            currentSlotItem.gameObject.SetActive(false);
            currentSlotItem.transform.localPosition = Vector3.zero;
            currentSlotItem.transform.localEulerAngles = Vector3.zero;
            startingTransformFromHand.SetTransformStruct(
                Vector3.zero, Quaternion.Euler(new Vector3(0, 90, 0)), startingTransformFromHand.scale * .1f);
            SetupNewMeshClone(currentSlotItem);
        }

        gameObject.SetActive(false);
    }


    private void OnValidate()
    {
        if (!inventoryManager)
            inventoryManager = GetComponentInParent<InventoryManager>();
        if (!interactionManager)
            interactionManager = FindObjectOfType<XRInteractionManager>();
        if (!addItemAnimator)
            addItemAnimator = slotDisplayToAddItem.GetComponent<Animator>();
        if (!hasItemAnimator)
            hasItemAnimator = slotDisplayWhenContainsItem.GetComponent<Animator>();
    }

    public void DisableSlot()
    {
        //Disable hand from adding item when animating to disable slot
        collider.enabled = false;
        if (!isDisabling)
            StartCoroutine(DisableAfterAnimation(AnimationDisableLength));
    }

    public void EnableSlot()
    {
        StopAllCoroutines();
        OnEnable();
    }

    private void ResetAnimationState(Animator anim, bool setToStartingAnimState)
    {
        //anim.ResetTrigger(enableAnimatorHash);
        //anim.ResetTrigger(disableAnimatorHash);
        //anim.SetBool(onHoverAnimatorHash, false);
        if (setToStartingAnimState)
            hasItemAnimator.SetTrigger(resetAnimatorHash);
    }


    private void OnEnable()
    {
        isBusy = false;
        isDisabling = false;
        startingTransformFromHand.SetTransformStruct(
            Vector3.zero, Quaternion.Euler(new Vector3(0, 90, 0)), startingTransformFromHand.scale * .1f);

        if (currentSlotItem)
        {
            if (boundCenterTransform)
                boundCenterTransform.gameObject.SetActive(false);
            SetNewItemModel();
        }

        inventorySlotUpdated.Invoke();
    }

    private void OnDisable() => CancelInvoke(nameof(SetNewItemModel));

    public void TryInteractWithSlot(XRDirectInteractor controller)
    {
        if (isBusy || isDisabling) 
        {
            Debug.Log($"[InventorySlot] ????嚥?????????怨쀫뎐??: isBusy={isBusy}, isDisabling={isDisabling}");
            return;
        }

        // ????????룻꼥?????⑤９苡???ル봿?? ?????밸븶??뫢??癲ル슢???믩쨨?????怨좊????????뀀땽?轅붽틓??? ??꿔꺂??틝?????
        XRBaseInteractable itemInHand = null;
        if (controller.hasSelection)
        {
            itemInHand = controller.selectTarget;
            
            // InteractableItemData ?轅붽틓??????⒟?
            var itemData = itemInHand.GetComponent<InteractableItemData>();
            if (!itemData || !itemData.canInventory)
            {
                Debug.Log($"[InventorySlot] ?????밸븶??뫢??癲ル슢???믩쨨???꿔꺂???沃???????る?????傭?끆?????????????쇨덧?? {itemInHand.name}");
                return;
            }
        }

        InteractWithSlot(controller);
    }


    private void InteractWithSlot(XRDirectInteractor controller)
    {
        Debug.Log($"[InventorySlot] InteractWithSlot ??癲ル슢??節녿쨨?- controller: {controller.name}");

        if (animateItemToSlotCoroutine != null)
        {
            StopCoroutine(animateItemToSlotCoroutine);
            Debug.Log("[InventorySlot] ?????쇨덫???????ル뭸????룸퉲?????щ탵???????썹땟??雍???μ떝?띄몭??袁㏉떋?");
        }

        XRBaseInteractable itemHandIsHolding = null;
        if (controller.hasSelection)
        {
            itemHandIsHolding = controller.selectTarget;
            Debug.Log($"[InventorySlot] ????????룻꼥?????⑤９苡???ル봿?? ????怨좊????????뀀땽 ?????밸븶??뫢?? {itemHandIsHolding.name}");
        }

        //Check if item is allowed to be added to inventory
        if (itemHandIsHolding)
        {
            var itemData = itemHandIsHolding.GetComponent<InteractableItemData>();
            if (!itemData || !itemData.canInventory)
            {
                Debug.Log($"[InventorySlot] ?????밸븶??뫢??癲ル슢???믩쨨???꿔꺂???沃???????る?????傭?끆?????????????쇨덧?? {itemHandIsHolding.name}");
                return;
            }
        }

        if (currentSlotItem)
        {
            Debug.Log($"[InventorySlot] ?????밸븶????????????밸븶??뫢?????濚밸Ŧ援?? {currentSlotItem.name}");
            DisableItemInHand(controller);
            GetNewItemFromSlot(controller);
        }
        else
        {
            DisableItemInHand(controller);
        }

        //Enable Inventory Slot
        currentSlotItem = itemHandIsHolding;

        StartCoroutine(AnimateIcon());
        SetNewItemModel();
        inventorySlotUpdated.Invoke();
    }

    private bool CheckIfCanAddItemToSlot(XRBaseInteractable itemHandIsHolding)
    {
        // Itemda helper = itemHandIsHolding.GetComponent<InventoryItemHelper>();
        // return helper.canInventory;
        return true;
    }

    private IEnumerator AnimateIcon()
    {
        isBusy = true;
        if (currentSlotItem)
        {
            slotDisplayWhenContainsItem.gameObject.SetActive(true);
            slotDisplayToAddItem.gameObject.SetActive(false);
        }
        else
        {
            if (boundCenterTransform) Destroy(boundCenterTransform.gameObject);
            slotDisplayToAddItem.gameObject.SetActive(true);
            slotDisplayWhenContainsItem.gameObject.SetActive(false);
        }

        collider.enabled = true;
        isBusy = false;
        yield break;
    }


    private IEnumerator DisableAfterAnimation(float seconds)
    {
        isDisabling = true;
        
        if (boundCenterTransform)
        {
            boundCenterTransform.localScale = Vector3.zero;
        }

        isDisabling = false;
        gameObject.SetActive(false);
        yield break;
    }

    private void DisableItemInHand(XRDirectInteractor controller)
    {
        var itemHandIsHolding = controller.selectTarget;
        if (!itemHandIsHolding)
        {
            return;
        }

        
        // ?????밸븶??뫢??癲ル슢???믩쨨?????????癲???????????濚밸Ŧ??????熬곣뫂爰??????밸븶???????밸븶???????????
        Vector3 targetPosition = transform.position;
        Quaternion targetRotation = transform.rotation;
        
        // ????????룻꼥?????⑤９苡??????????밸븶??뫢???????쇨덫??
        ReleaseItemFromHand(controller, itemHandIsHolding);
        
        // itemModelHolder ???濚밸Ŧ???
        var itemHolderTransform = itemModelHolder.transform;
        itemHolderTransform.parent = transform;
        itemHolderTransform.localScale = Vector3.one;
        itemHolderTransform.localPosition = Vector3.zero;
        itemHolderTransform.localEulerAngles = Vector3.zero;

        // ?????밸븶??뫢??癲ル슢???믩쨨?????????癲???????????濚밸Ŧ??????롪퍓肉???????밸븶?????곗뒭????
        itemHandIsHolding.transform.parent = transform;
        itemHandIsHolding.transform.position = targetPosition;
        itemHandIsHolding.transform.rotation = targetRotation;
        
        StartCoroutine(DisableItem(itemHandIsHolding));
        
    }

    //Lets physics respond to collider disappearing before disabling object physics update needs to run twice
    private IEnumerator DisableItem(XRBaseInteractable item)
    {
        Debug.Log($"[InventorySlot] DisableItem ??癲ル슢??節녿쨨? {item.name}");
        
        // ?????밸븶??뫢??癲ル슢???먮뙀???????????밸븶??????        Vector3 originalPosition = item.transform.position;
        Quaternion originalRotation = item.transform.rotation;
        
        // ?????밸븶??뫢??癲ル슢???먮뙀?Rigidbody ?轅붽틓?????븐넂??ル젗?
        var rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true; // ????ル늅???????釉뚯뺏????ш끽維뽳쭩?? ??????몃뜪?????濚밸Ŧ???
        }

        item.gameObject.SetActive(true);
        yield return null;

        // OnGrabEnableDisable ??????????살퓢???轅붽틓??影?뽧걤??
        var enableDisable = item.GetComponent<OnGrabEnableDisable>();
        if (enableDisable != null)
        {
            enableDisable.EnableAll();
            //Debug.Log("[InventorySlot] OnGrabEnableDisable ??????????살퓢????癲????);
        }

        // ?????밸븶??뫢??癲ル슢???믩쨨???????????밸븶????????濚밸Ŧ???
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
        
        yield return new WaitForSeconds(Time.fixedDeltaTime * 2);

        // ?????밸븶??뫢????????μ떜媛?걫??????轅붽틓????彛??쒓랜???????밸븶?????濚밸Ŧ???
        item.transform.localPosition = Vector3.zero;
        item.gameObject.SetActive(false);
        
        Debug.Log($"[InventorySlot] ?????밸븶??뫢????????μ떜媛?걫????????밸븶?? {item.name}");

        yield return new WaitForSeconds(Time.fixedDeltaTime);

        SetupNewMeshClone(item);
    }

    private void GetNewItemFromSlot(XRDirectInteractor controller)
    {
        
        currentSlotItem.gameObject.SetActive(true);
        currentSlotItem.transform.parent = null;

        // Rigidbody??isKinematic ?????쇨덫??
        var rb = currentSlotItem.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            //Debug.Log("[InventorySlot] Rigidbody??isKinematic ?????쇨덫???);
        }

        GrabNewItem(controller, currentSlotItem);
        //grabAudio.Play();
        
    }

    private void ReleaseItemFromHand(XRBaseInteractor interactor, XRBaseInteractable interactable)
    {
        if (interactionManager == null)
        {
            Debug.LogError("[InventorySlot] XR Interaction Manager???ル봿?? ?????욱룏???????낆젵!");
            return;
        }
        interactionManager.SelectExit(interactor, interactable);
    }

    private void GrabNewItem(XRBaseInteractor interactor, XRBaseInteractable interactable)
    {
        if (interactionManager == null)
        {
            Debug.LogError("[InventorySlot] XR Interaction Manager???ル봿?? ?????욱룏???????낆젵!");
            return;
        }
        interactionManager.SelectEnter(interactor, interactable);
    }


    private void SetupNewMeshClone(XRBaseInteractable itemHandIsHolding)
    {
        Debug.Log($"[InventorySlot] SetupNewMeshClone ??癲ル슢??節녿쨨? {itemHandIsHolding.name}");

        if (itemSlotMeshClone)
        {
            Destroy(itemSlotMeshClone.gameObject);
        }

        // ???嚥??itemModelHolder?????癲?????????轅붽틓????????袁⑸즴???
        Debug.Log($"[InventorySlot] ???轅붽틓????????嚥????袁⑸즴???- ????뉖??? {itemModelHolder.name}");
        itemSlotMeshClone = Instantiate(itemHandIsHolding, itemModelHolder.position, itemModelHolder.rotation, itemModelHolder).transform;
        itemSlotMeshClone.gameObject.SetActive(true); // ?轅붽틓??筌뚮챶夷??????⑤뜪癲ル슢?뤺キ????癲????
        try
        {
            DestroyComponentsOnClone(itemSlotMeshClone);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySlot] ??????????살퓢?????怨뚰뇠???떐????????⑤챷逾???ш끽維뽳쭩?좊쐪筌먲퐢??? {e.Message}");
        }

        // ??ш끽維뽳쭩?????????モ닪?????濚밸Ŧ???
        Bounds bounds = GetBoundsOfAllMeshes(itemSlotMeshClone.transform);
        if (!boundCenterTransform)
        {
            boundCenterTransform = new GameObject("Bound Center Transform").transform;
            boundCenterTransform.parent = itemModelHolder; // ?轅붽틓??筌뚮챶夷??????⑤뜪癲ル슢?뤺キ??itemModelHolder?????癲???????????濚밸Ŧ???
        }

        // ?????밸븶??? ????????濚밸Ŧ???
        boundCenterTransform.position = bounds.center;
        boundCenterTransform.rotation = itemHandIsHolding.transform.rotation;
        
        // ???嚥??boundCenterTransform?????癲???????????濚밸Ŧ???
        itemSlotMeshClone.parent = boundCenterTransform;

        // ??癲ル슢??節녿쨨???꿔꺂?????????貫猷????濚밸Ŧ???
        startingTransformFromHand.SetTransformStruct(
            boundCenterTransform.localPosition,
            boundCenterTransform.localRotation,
            boundCenterTransform.localScale
        );

        // ????????濚밸Ŧ???
        boundCenterTransform.localEulerAngles = new Vector3(0, 90, 0);

        // ???????곗뒭????
        inventorySize.enabled = true;
        Vector3 parentSize = inventorySize.bounds.size;
        while (bounds.size.x > parentSize.x || bounds.size.y > parentSize.y || bounds.size.z > parentSize.z)
        {
            bounds = GetBoundsOfAllMeshes(boundCenterTransform.transform);
            boundCenterTransform.transform.localScale *= 0.9f;
        }
        inventorySize.enabled = false;

        goalSizeToFitInSlot = boundCenterTransform.transform.localScale;

        // ?????ル뭸????룸퉲?????щ탵?????癲ル슢??節녿쨨??????밸븶????壤굿???잏솾?????繹????꿔꺂??틝?????
        Debug.Log($"[InventorySlot] ???嚥?????뉖?????꿔꺂??틝????? {itemSlotMeshClone.parent.name}");
        Debug.Log($"[InventorySlot] boundCenterTransform ????뉖?????꿔꺂??틝????? {boundCenterTransform.parent.name}");

        animateItemToSlotCoroutine = StartCoroutine(AnimateItemToSlot());
    }

    private void ActivateItemSlotMeshClone() => itemSlotMeshClone.gameObject.SetActive(true);

    private void DestroyComponentsOnClone(Transform clone)
    {

        try
        {
            // ?雅?퍔瑗ⓩ뤃?? IReturnMovedColliders ?轅붽틓??影?뽧걤??
            var movedColliders = clone.GetComponentsInChildren<IReturnMovedColliders>(true);
            foreach (var t in movedColliders) 
            {
                t.ReturnMovedColliders();
                Debug.Log($"[InventorySlot] IReturnMovedColliders ?轅붽틓??影?뽧걤?? {t.GetType().Name}");
            }

            // ?雅?퍔瑗ⓩ뤃?? InteractableItemData ??????μ떜媛?걫???
            var itemDataComponents = clone.GetComponentsInChildren<InteractableItemData>(true);
            foreach (var t in itemDataComponents)
            {
                t.enabled = false;
                Debug.Log($"[InventorySlot] InteractableItemData ??????μ떜媛?걫??? {t.name}");
            }

            // XRGrabInteractable ??????μ떜媛?걫???
            var grabComponents = clone.GetComponentsInChildren<XRGrabInteractable>(true);
            foreach (var t in grabComponents)
            {
                t.enabled = false;
                Debug.Log($"[InventorySlot] XRGrabInteractable ??????μ떜媛?걫??? {t.name}");
            }

            // Collider?? Light ???怨뚰뇠???떐?
            var lights = clone.GetComponentsInChildren<Light>(true);
            foreach (var t in lights) 
            {
                Destroy(t);
                Debug.Log($"[InventorySlot] Light ???怨뚰뇠???떐? {t.name}");
            }

            var colliders = clone.GetComponentsInChildren<Collider>(true);
            foreach (var t in colliders)
            {
                Destroy(t);
                Debug.Log($"[InventorySlot] Collider ???怨뚰뇠???떐? {t.name}");
            }

            // ?????猷붽틛????? MonoBehaviour ??????????살퓢??????怨뚰뇠???떐?(InteractableItemData?? XRGrabInteractable ??癲ル슢????
            var monoBehaviors = clone.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var t in monoBehaviors)
            {
                if (!(t is InteractableItemData) && !(t is XRGrabInteractable))
                {
                    Destroy(t);
                    Debug.Log($"[InventorySlot] MonoBehaviour ???怨뚰뇠???떐? {t.GetType().Name}");
                }
            }

            // Rigidbody????????μ떜媛?걫????怨뺣만?????롪퍓肉?????怨뚰뇠???떐??? ?????ㅿ폎??
            var rigidBodies = clone.GetComponentsInChildren<Rigidbody>(true);
            foreach (var t in rigidBodies)
            {
                t.isKinematic = true;
                t.useGravity = false;
                Debug.Log($"[InventorySlot] Rigidbody ??????μ떜媛?걫??? {t.name}");
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySlot] ??????????살퓢?????怨뚰뇠???떐????????⑤챷逾???ш끽維뽳쭩?좊쐪筌먲퐢??? {e.Message}\n{e.StackTrace}");
        }
    }

    private void SetNewItemModel()
    {
        if (!currentSlotItem)
            return;

        //Create a clone of the new item
        if (!itemSlotMeshClone)
            SetupNewMeshClone(currentSlotItem);
        else
            animateItemToSlotCoroutine = StartCoroutine(AnimateItemToSlot());
    }

    private Coroutine animateItemToSlotCoroutine;

    private IEnumerator AnimateItemToSlot()
    {
        boundCenterTransform.localPosition = Vector3.zero;
        boundCenterTransform.localScale = goalSizeToFitInSlot;
        boundCenterTransform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        boundCenterTransform.gameObject.SetActive(true);
        yield break;
    }

    private Bounds GetBoundsOfAllMeshes(Transform item)
    {
        Bounds bounds = new Bounds();
        Renderer[] rends = itemSlotMeshClone.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in rends)
        {
            if (rend.GetComponent<ParticleSystem>()) continue;

            if (bounds.extents == Vector3.zero)
                bounds = rend.bounds;

            bounds.Encapsulate(rend.bounds);
        }

        return bounds;
    }

    private void OnDrawGizmos()
    {
        if (!itemSlotMeshClone) return;
        Bounds tempBounds = GetBoundsOfAllMeshes(itemSlotMeshClone);
        Gizmos.DrawWireCube(tempBounds.center, tempBounds.size);
        Gizmos.DrawSphere(tempBounds.center, .01f);
    }

    public InventorySlot(TransformStruct startingTransformFromHand)
    {
        this.startingTransformFromHand = startingTransformFromHand;
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactor = other.GetComponent<XRDirectInteractor>();
        if (interactor == null) return;

        var controller = interactor.GetComponentInParent<ActionBasedController>();
        if (controller == null) return;

        bool isLeftHand = controller == inventoryManager.leftController;

        Debug.Log($"[InventorySlot] OnTriggerEnter - Controller: {controller.name}, IsLeft: {isLeftHand}");

        if (isLeftHand)
        {
            leftEffect.SetActive(true);
            //Debug.Log("[InventorySlot] ???リ옇?ユ뤃???壤굿??뗫툞 ??嶺????);
        }
        else
        {
            rightEffect.SetActive(true);
            //Debug.Log("[InventorySlot] ?????봔饔낃퀣????壤굿??뗫툞 ??嶺????);
        }

        if (slotDisplayToAddItem != null)
            slotDisplayToAddItem.GetComponent<Animator>()?.SetBool(onHoverAnimatorHash, true);
        if (slotDisplayWhenContainsItem != null)
            slotDisplayWhenContainsItem.GetComponent<Animator>()?.SetBool(onHoverAnimatorHash, true);
    }

    private void OnTriggerExit(Collider other)
    {
        var interactor = other.GetComponent<XRDirectInteractor>();
        if (interactor == null) return;

        var controller = interactor.GetComponentInParent<ActionBasedController>();
        if (controller == null) return;

        bool isLeftHand = controller == inventoryManager.leftController;

        Debug.Log($"[InventorySlot] OnTriggerExit - Controller: {controller.name}, IsLeft: {isLeftHand}");

        if (isLeftHand)
        {
            leftEffect.SetActive(false);
        }
        else
        {
            rightEffect.SetActive(false);
        }

        if (slotDisplayToAddItem != null)
            slotDisplayToAddItem.GetComponent<Animator>()?.SetBool(onHoverAnimatorHash, false);
        if (slotDisplayWhenContainsItem != null)
            slotDisplayWhenContainsItem.GetComponent<Animator>()?.SetBool(onHoverAnimatorHash, false);
    }
}


public static class BoundsExtension
{
    public static Bounds GrowBounds(this Bounds a, Bounds b)
    {
        Vector3 max = Vector3.Max(a.max, b.max);
        Vector3 min = Vector3.Min(a.min, b.min);

        a = new Bounds((max + min) * 0.5f, max - min);
        return a;
    }
}

