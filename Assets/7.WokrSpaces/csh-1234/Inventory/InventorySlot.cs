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
            Debug.Log($"[InventorySlot] ?곹샇?묒슜 遺덇?: isBusy={isBusy}, isDisabling={isDisabling}");
            return;
        }

        // 而⑦듃濡ㅻ윭媛 ?꾩씠?쒖쓣 ?ㅺ퀬 ?덈뒗吏 ?뺤씤
        XRBaseInteractable itemInHand = null;
        if (controller.hasSelection)
        {
            itemInHand = controller.selectTarget;
            
            // InteractableItemData 泥댄겕
            var itemData = itemInHand.GetComponent<InteractableItemData>();
            if (!itemData || !itemData.canInventory)
            {
                Debug.Log($"[InventorySlot] ?꾩씠?쒖쓣 ?몃깽?좊━???ｌ쓣 ???놁쓬: {itemInHand.name}");
                return;
            }
        }

        InteractWithSlot(controller);
    }


    private void InteractWithSlot(XRDirectInteractor controller)
    {
        Debug.Log($"[InventorySlot] InteractWithSlot ?쒖옉 - controller: {controller.name}");

        if (animateItemToSlotCoroutine != null)
        {
            StopCoroutine(animateItemToSlotCoroutine);
            Debug.Log("[InventorySlot] ?댁쟾 ?좊땲硫붿씠??肄붾（??以묒?");
        }

        XRBaseInteractable itemHandIsHolding = null;
        if (controller.hasSelection)
        {
            itemHandIsHolding = controller.selectTarget;
            Debug.Log($"[InventorySlot] 而⑦듃濡ㅻ윭媛 ?ㅺ퀬 ?덈뒗 ?꾩씠?? {itemHandIsHolding.name}");
        }

        //Check if item is allowed to be added to inventory
        if (itemHandIsHolding)
        {
            var itemData = itemHandIsHolding.GetComponent<InteractableItemData>();
            if (!itemData || !itemData.canInventory)
            {
                Debug.Log($"[InventorySlot] ?꾩씠?쒖쓣 ?몃깽?좊━???ｌ쓣 ???놁쓬: {itemHandIsHolding.name}");
                return;
            }
        }

        if (currentSlotItem)
        {
            Debug.Log($"[InventorySlot] ?꾩옱 ?щ’???꾩씠???덉쓬: {currentSlotItem.name}");
            DisableItemInHand(controller);
            GetNewItemFromSlot(controller);
        }
        else
        {
            Debug.Log("[InventorySlot] 鍮??щ’???꾩씠??異붽? ?쒕룄");
            DisableItemInHand(controller);
        }

        //Enable Inventory Slot
        currentSlotItem = itemHandIsHolding;
        Debug.Log($"[InventorySlot] ?щ’?????꾩씠???ㅼ젙: {(currentSlotItem ? currentSlotItem.name : "?놁쓬")}");

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
            Debug.Log("[InventorySlot] DisableItemInHand: 而⑦듃濡ㅻ윭媛 ?ㅺ퀬 ?덈뒗 ?꾩씠???놁쓬");
            return;
        }

        Debug.Log($"[InventorySlot] DisableItemInHand: {itemHandIsHolding.name} 鍮꾪솢?깊솕 ?쒖옉");
        
        // ?꾩씠?쒖쓣 ?щ’???먯떇?쇰줈 ?ㅼ젙?섍린 ?꾩뿉 ?꾩튂/?뚯쟾 ???
        Vector3 targetPosition = transform.position;
        Quaternion targetRotation = transform.rotation;
        
        // 而⑦듃濡ㅻ윭?먯꽌 ?꾩씠???댁젣
        ReleaseItemFromHand(controller, itemHandIsHolding);
        
        // itemModelHolder ?ㅼ젙
        var itemHolderTransform = itemModelHolder.transform;
        itemHolderTransform.parent = transform;
        itemHolderTransform.localScale = Vector3.one;
        itemHolderTransform.localPosition = Vector3.zero;
        itemHolderTransform.localEulerAngles = Vector3.zero;

        // ?꾩씠?쒖쓣 ?щ’???먯떇?쇰줈 ?ㅼ젙?섍퀬 ?꾩튂 議곗젙
        itemHandIsHolding.transform.parent = transform;
        itemHandIsHolding.transform.position = targetPosition;
        itemHandIsHolding.transform.rotation = targetRotation;
        
        StartCoroutine(DisableItem(itemHandIsHolding));
        
        Debug.Log($"[InventorySlot] DisableItemInHand: {itemHandIsHolding.name} 鍮꾪솢?깊솕 ?꾨즺");
    }

    //Lets physics respond to collider disappearing before disabling object physics update needs to run twice
    private IEnumerator DisableItem(XRBaseInteractable item)
    {
        Debug.Log($"[InventorySlot] DisableItem ?쒖옉: {item.name}");
        
        // ?꾩씠?쒖쓽 ?먮옒 ?꾩튂 ???        Vector3 originalPosition = item.transform.position;
        Quaternion originalRotation = item.transform.rotation;
        
        // ?꾩씠?쒖쓽 Rigidbody 李얘린
        var rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true; // 臾쇰━ ?곹뼢 諛쏆? ?딅룄濡??ㅼ젙
            Debug.Log("[InventorySlot] Rigidbody瑜?kinematic?쇰줈 ?ㅼ젙");
        }

        item.gameObject.SetActive(true);
        yield return null;

        // OnGrabEnableDisable 而댄룷?뚰듃 泥섎━
        var enableDisable = item.GetComponent<OnGrabEnableDisable>();
        if (enableDisable != null)
        {
            enableDisable.EnableAll();
            //Debug.Log("[InventorySlot] OnGrabEnableDisable 而댄룷?뚰듃 ?쒖꽦??);
        }

        // ?꾩씠?쒖쓣 ?먮옒 ?꾩튂濡??ㅼ젙
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
        
        yield return new WaitForSeconds(Time.fixedDeltaTime * 2);

        // ?꾩씠??鍮꾪솢?깊솕 ??理쒖쥌 ?꾩튂 ?ㅼ젙
        item.transform.localPosition = Vector3.zero;
        item.gameObject.SetActive(false);
        
        Debug.Log($"[InventorySlot] ?꾩씠??鍮꾪솢?깊솕 ?꾨즺: {item.name}");

        yield return new WaitForSeconds(Time.fixedDeltaTime);

        SetupNewMeshClone(item);
    }

    private void GetNewItemFromSlot(XRDirectInteractor controller)
    {
        Debug.Log($"[InventorySlot] GetNewItemFromSlot: {currentSlotItem.name} ?쒖꽦???쒖옉");
        
        currentSlotItem.gameObject.SetActive(true);
        currentSlotItem.transform.parent = null;

        // Rigidbody??isKinematic ?댁젣
        var rb = currentSlotItem.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            //Debug.Log("[InventorySlot] Rigidbody??isKinematic ?댁젣??);
        }

        GrabNewItem(controller, currentSlotItem);
        //grabAudio.Play();
        
        Debug.Log($"[InventorySlot] GetNewItemFromSlot: {currentSlotItem.name} ?쒖꽦???꾨즺");
    }

    private void ReleaseItemFromHand(XRBaseInteractor interactor, XRBaseInteractable interactable)
    {
        Debug.Log($"[InventorySlot] ReleaseItemFromHand: {interactable.name} ?댁젣 ?쒕룄");
        if (interactionManager == null)
        {
            Debug.LogError("[InventorySlot] XR Interaction Manager媛 ?놁뒿?덈떎!");
            return;
        }
        interactionManager.SelectExit(interactor, interactable);
    }

    private void GrabNewItem(XRBaseInteractor interactor, XRBaseInteractable interactable)
    {
        Debug.Log($"[InventorySlot] GrabNewItem: {interactable.name} ?↔린 ?쒕룄");
        if (interactionManager == null)
        {
            Debug.LogError("[InventorySlot] XR Interaction Manager媛 ?놁뒿?덈떎!");
            return;
        }
        interactionManager.SelectEnter(interactor, interactable);
    }


    private void SetupNewMeshClone(XRBaseInteractable itemHandIsHolding)
    {
        Debug.Log($"[InventorySlot] SetupNewMeshClone ?쒖옉: {itemHandIsHolding.name}");

        if (itemSlotMeshClone)
        {
            Debug.Log("[InventorySlot] 湲곗〈 硫붿떆 ?대줎 ?쒓굅");
            Destroy(itemSlotMeshClone.gameObject);
        }

        // ?대줎??itemModelHolder???먯떇?쇰줈 吏곸젒 ?앹꽦
        Debug.Log($"[InventorySlot] ??硫붿떆 ?대줎 ?앹꽦 - 遺紐? {itemModelHolder.name}");
        itemSlotMeshClone = Instantiate(itemHandIsHolding, itemModelHolder.position, itemModelHolder.rotation, itemModelHolder).transform;
        itemSlotMeshClone.gameObject.SetActive(true); // 紐낆떆?곸쑝濡??쒖꽦??
        try
        {
            Debug.Log("[InventorySlot] ?대줎??而댄룷?뚰듃 ?쒓굅 ?쒖옉");
            DestroyComponentsOnClone(itemSlotMeshClone);
            Debug.Log("[InventorySlot] ?대줎??而댄룷?뚰듃 ?쒓굅 ?꾨즺");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySlot] 而댄룷?뚰듃 ?쒓굅 以??ㅻ쪟 諛쒖깮: {e.Message}");
        }

        // 諛붿슫???쇳꽣 ?ㅼ젙
        Bounds bounds = GetBoundsOfAllMeshes(itemSlotMeshClone.transform);
        if (!boundCenterTransform)
        {
            boundCenterTransform = new GameObject("Bound Center Transform").transform;
            boundCenterTransform.parent = itemModelHolder; // 紐낆떆?곸쑝濡?itemModelHolder???먯떇?쇰줈 ?ㅼ젙
        }

        // ?꾩튂? ?뚯쟾 ?ㅼ젙
        boundCenterTransform.position = bounds.center;
        boundCenterTransform.rotation = itemHandIsHolding.transform.rotation;
        
        // ?대줎??boundCenterTransform???먯떇?쇰줈 ?ㅼ젙
        itemSlotMeshClone.parent = boundCenterTransform;

        // ?쒖옉 ?몃옖?ㅽ뤌 ?ㅼ젙
        startingTransformFromHand.SetTransformStruct(
            boundCenterTransform.localPosition,
            boundCenterTransform.localRotation,
            boundCenterTransform.localScale
        );

        // ?뚯쟾 ?ㅼ젙
        boundCenterTransform.localEulerAngles = new Vector3(0, 90, 0);

        // ?ш린 議곗젅
        inventorySize.enabled = true;
        Vector3 parentSize = inventorySize.bounds.size;
        while (bounds.size.x > parentSize.x || bounds.size.y > parentSize.y || bounds.size.z > parentSize.z)
        {
            bounds = GetBoundsOfAllMeshes(boundCenterTransform.transform);
            boundCenterTransform.transform.localScale *= 0.9f;
        }
        inventorySize.enabled = false;

        goalSizeToFitInSlot = boundCenterTransform.transform.localScale;

        // ?좊땲硫붿씠???쒖옉 ?꾩뿉 怨꾩링 援ъ“ ?뺤씤
        Debug.Log($"[InventorySlot] ?대줎 遺紐??뺤씤: {itemSlotMeshClone.parent.name}");
        Debug.Log($"[InventorySlot] boundCenterTransform 遺紐??뺤씤: {boundCenterTransform.parent.name}");

        animateItemToSlotCoroutine = StartCoroutine(AnimateItemToSlot());
    }

    private void ActivateItemSlotMeshClone() => itemSlotMeshClone.gameObject.SetActive(true);

    private void DestroyComponentsOnClone(Transform clone)
    {
        Debug.Log("[InventorySlot] ?대줎 而댄룷?뚰듃 ?쒓굅 ?쒖옉");

        try
        {
            // 癒쇱? IReturnMovedColliders 泥섎━
            var movedColliders = clone.GetComponentsInChildren<IReturnMovedColliders>(true);
            foreach (var t in movedColliders) 
            {
                t.ReturnMovedColliders();
                Debug.Log($"[InventorySlot] IReturnMovedColliders 泥섎━: {t.GetType().Name}");
            }

            // 癒쇱? InteractableItemData 鍮꾪솢?깊솕
            var itemDataComponents = clone.GetComponentsInChildren<InteractableItemData>(true);
            foreach (var t in itemDataComponents)
            {
                t.enabled = false;
                Debug.Log($"[InventorySlot] InteractableItemData 鍮꾪솢?깊솕: {t.name}");
            }

            // XRGrabInteractable 鍮꾪솢?깊솕
            var grabComponents = clone.GetComponentsInChildren<XRGrabInteractable>(true);
            foreach (var t in grabComponents)
            {
                t.enabled = false;
                Debug.Log($"[InventorySlot] XRGrabInteractable 鍮꾪솢?깊솕: {t.name}");
            }

            // Collider? Light ?쒓굅
            var lights = clone.GetComponentsInChildren<Light>(true);
            foreach (var t in lights) 
            {
                Destroy(t);
                Debug.Log($"[InventorySlot] Light ?쒓굅: {t.name}");
            }

            var colliders = clone.GetComponentsInChildren<Collider>(true);
            foreach (var t in colliders)
            {
                Destroy(t);
                Debug.Log($"[InventorySlot] Collider ?쒓굅: {t.name}");
            }

            // ?섎㉧吏 MonoBehaviour 而댄룷?뚰듃???쒓굅 (InteractableItemData? XRGrabInteractable ?쒖쇅)
            var monoBehaviors = clone.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var t in monoBehaviors)
            {
                if (!(t is InteractableItemData) && !(t is XRGrabInteractable))
                {
                    Destroy(t);
                    Debug.Log($"[InventorySlot] MonoBehaviour ?쒓굅: {t.GetType().Name}");
                }
            }

            // Rigidbody??鍮꾪솢?깊솕留??섍퀬 ?쒓굅?섏? ?딆쓬
            var rigidBodies = clone.GetComponentsInChildren<Rigidbody>(true);
            foreach (var t in rigidBodies)
            {
                t.isKinematic = true;
                t.useGravity = false;
                Debug.Log($"[InventorySlot] Rigidbody 鍮꾪솢?깊솕: {t.name}");
            }

            Debug.Log("[InventorySlot] ?대줎 而댄룷?뚰듃 ?쒓굅 ?꾨즺");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySlot] 而댄룷?뚰듃 ?쒓굅 以??ㅻ쪟 諛쒖깮: {e.Message}\n{e.StackTrace}");
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
        var controller = other.GetComponent<ActionBasedController>();
        if (controller)
        {
            slotDisplayToAddItem.GetComponent<Animator>().SetBool(onHoverAnimatorHash, true);
            slotDisplayWhenContainsItem.GetComponent<Animator>().SetBool(onHoverAnimatorHash, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var controller = other.GetComponent<ActionBasedController>();
        if (controller)
        {
            slotDisplayToAddItem.GetComponent<Animator>().SetBool(onHoverAnimatorHash, false);
            slotDisplayWhenContainsItem.GetComponent<Animator>().SetBool(onHoverAnimatorHash, false);
        }
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

