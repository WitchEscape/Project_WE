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

    public XRBaseInteractable CurrentSlotItem => currentSlotItem;
    public UnityEvent inventorySlotUpdated;

    private XRBaseInteractable currentSlotItem;
    private Transform boundCenterTransform, itemSlotMeshClone;
    private XRInteractionManager interactionManager;
    private InventoryManager inventoryManager;


    private bool isBusy, isDisabling;
    private TransformStruct startingTransformFromHand;
    private Vector3 goalSizeToFitInSlot;

    private void Awake()
    {
        OnValidate();
    }

    public IEnumerator CreateStartingItemAndDisable()
    {
        if (startingItem)
        {
            Debug.Log($"startingItem 설정");
            currentSlotItem = Instantiate(startingItem, transform, true);
            
            // prefabPath 복사
            var sourceItemData = startingItem.GetComponent<InteractableItemData>();
            var targetItemData = currentSlotItem.GetComponent<InteractableItemData>();
            if (sourceItemData != null && targetItemData != null)
            {
                targetItemData.prefabPath = sourceItemData.prefabPath;
            }

            yield return null;

            currentSlotItem.gameObject.SetActive(false);
            currentSlotItem.transform.localPosition = Vector3.zero;
            currentSlotItem.transform.localEulerAngles = Vector3.zero;
            
            // 미리보기 설정
            SetupNewMeshClone(currentSlotItem);
            
            // 슬롯 UI 설정
            slotDisplayWhenContainsItem.gameObject.SetActive(true);
            slotDisplayToAddItem.gameObject.SetActive(false);
            
            // 아이템 상태 설정
            if (boundCenterTransform)
            {
                boundCenterTransform.gameObject.SetActive(true);
                boundCenterTransform.localPosition = Vector3.zero;
                boundCenterTransform.localScale = goalSizeToFitInSlot;
                boundCenterTransform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
            }
        }
        else
        {
            slotDisplayWhenContainsItem.gameObject.SetActive(false);
            slotDisplayToAddItem.gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (!inventoryManager)
            inventoryManager = GetComponentInParent<InventoryManager>();
        if (!interactionManager)
            interactionManager = FindObjectOfType<XRInteractionManager>();
    }

    public void DisableSlot()
    {
        collider.enabled = false;
    }

    public void EnableSlot()
    {
        StopAllCoroutines();
        OnEnable(); 
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
            return;
        }

        XRBaseInteractable itemInHand = null;
        if (controller.hasSelection)
        {
            itemInHand = controller.selectTarget;
            
            var itemData = itemInHand.GetComponent<InteractableItemData>();
            if (!itemData || !itemData.canInventory)
            {
                return;
            }
        }

        InteractWithSlot(controller);
    }

    private void InteractWithSlot(XRDirectInteractor controller)
    {
        if (animateItemToSlotCoroutine != null)
        {
            StopCoroutine(animateItemToSlotCoroutine);
        }

        XRBaseInteractable itemHandIsHolding = null;
        if (controller.hasSelection)
        {
            itemHandIsHolding = controller.selectTarget;
        }

        if (itemHandIsHolding)
        {
            var itemData = itemHandIsHolding.GetComponent<InteractableItemData>();
            if (!itemData || !itemData.canInventory)
            {
                return;
            }
        }

        if (currentSlotItem)
        {
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
        
        Vector3 targetPosition = transform.position;
        Quaternion targetRotation = transform.rotation;

        ReleaseItemFromHand(controller, itemHandIsHolding);
        
        var itemHolderTransform = itemModelHolder.transform;
        itemHolderTransform.parent = transform;
        itemHolderTransform.localScale = Vector3.one;
        itemHolderTransform.localPosition = Vector3.zero;
        itemHolderTransform.localEulerAngles = Vector3.zero;

        itemHandIsHolding.transform.parent = transform;
        itemHandIsHolding.transform.position = targetPosition;
        itemHandIsHolding.transform.rotation = targetRotation;
        
        StartCoroutine(DisableItem(itemHandIsHolding));
        
    }

    //Lets physics respond to collider disappearing before disabling object physics update needs to run twice
    private IEnumerator DisableItem(XRBaseInteractable item)
    {
        Quaternion originalRotation = item.transform.rotation;
        
        var rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true; 
        }

        item.gameObject.SetActive(true);
        yield return null;

        var enableDisable = item.GetComponent<OnGrabEnableDisable>();
        if (enableDisable != null)
        {
            enableDisable.EnableAll();
        }

        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
        
        yield return new WaitForSeconds(Time.fixedDeltaTime * 2);

        item.transform.localPosition = Vector3.zero;
        item.gameObject.SetActive(false);
        

        yield return new WaitForSeconds(Time.fixedDeltaTime);

        SetupNewMeshClone(item);
    }

    private void GetNewItemFromSlot(XRDirectInteractor controller)
    {
        
        currentSlotItem.gameObject.SetActive(true);
        currentSlotItem.transform.parent = null;

        var rb = currentSlotItem.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
        }

        GrabNewItem(controller, currentSlotItem);
        
    }

    private void ReleaseItemFromHand(XRBaseInteractor interactor, XRBaseInteractable interactable)
    {
        if (interactionManager == null)
        {
            return;
        }
        interactionManager.SelectExit(interactor, interactable);
    }

    private void GrabNewItem(XRBaseInteractor interactor, XRBaseInteractable interactable)
    {
        if (interactionManager == null)
        {
            return;
        }
        interactionManager.SelectEnter(interactor, interactable);
    }


    private void SetupNewMeshClone(XRBaseInteractable itemHandIsHolding)
    {
        if (itemSlotMeshClone)
        {
            Destroy(itemSlotMeshClone.gameObject);
        }

        itemSlotMeshClone = Instantiate(itemHandIsHolding, itemModelHolder.position, itemModelHolder.rotation, itemModelHolder).transform;
        itemSlotMeshClone.gameObject.SetActive(true); 
        try
        {
            DestroyComponentsOnClone(itemSlotMeshClone);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{e.Message}");
        }

        Bounds bounds = GetBoundsOfAllMeshes(itemSlotMeshClone.transform);
        if (!boundCenterTransform)
        {
            boundCenterTransform = new GameObject("Bound Center Transform").transform;
            boundCenterTransform.parent = itemModelHolder; 
        }

        boundCenterTransform.position = bounds.center;
        boundCenterTransform.rotation = itemHandIsHolding.transform.rotation;
        
        itemSlotMeshClone.parent = boundCenterTransform;

        startingTransformFromHand.SetTransformStruct(
            boundCenterTransform.localPosition,
            boundCenterTransform.localRotation,
            boundCenterTransform.localScale
        );

        boundCenterTransform.localEulerAngles = new Vector3(0, 90, 0);

        inventorySize.enabled = true;
        Vector3 parentSize = inventorySize.bounds.size;
        while (bounds.size.x > parentSize.x || bounds.size.y > parentSize.y || bounds.size.z > parentSize.z)
        {
            bounds = GetBoundsOfAllMeshes(boundCenterTransform.transform);
            boundCenterTransform.transform.localScale *= 0.9f;
        }
        inventorySize.enabled = false;

        goalSizeToFitInSlot = boundCenterTransform.transform.localScale;

        animateItemToSlotCoroutine = StartCoroutine(AnimateItemToSlot());
    }

    private void ActivateItemSlotMeshClone() => itemSlotMeshClone.gameObject.SetActive(true);

    private void DestroyComponentsOnClone(Transform clone)
    {
        try
        {
            var movedColliders = clone.GetComponentsInChildren<IReturnMovedColliders>(true);
            foreach (var t in movedColliders) 
            {
                t.ReturnMovedColliders();
            }

            var itemDataComponents = clone.GetComponentsInChildren<InteractableItemData>(true);
            foreach (var t in itemDataComponents)
            {
                t.enabled = false;
            }

            var grabComponents = clone.GetComponentsInChildren<XRGrabInteractable>(true);
            foreach (var t in grabComponents)
            {
                t.enabled = false;
            }

            var lights = clone.GetComponentsInChildren<Light>(true);
            foreach (var t in lights) 
            {
                Destroy(t);
            }

            var colliders = clone.GetComponentsInChildren<Collider>(true);
            foreach (var t in colliders)
            {
                Destroy(t);
            }

            var monoBehaviors = clone.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var t in monoBehaviors)
            {
                if (!(t is InteractableItemData) && !(t is XRGrabInteractable))
                {
                    Destroy(t);
                }
            }

            var rigidBodies = clone.GetComponentsInChildren<Rigidbody>(true);
            foreach (var t in rigidBodies)
            {
                t.isKinematic = true;
                t.useGravity = false;
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }

    private void SetNewItemModel()
    {
        if (!currentSlotItem)
            return;

        if (!itemSlotMeshClone)
            SetupNewMeshClone(currentSlotItem);
        else
            animateItemToSlotCoroutine = StartCoroutine(AnimateItemToSlot());

        leftEffect.gameObject.SetActive(false);
        rightEffect.gameObject.SetActive(false);
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

        if (isLeftHand)
        {
            leftEffect.SetActive(true);
        }
        else
        {
            rightEffect.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactor = other.GetComponent<XRDirectInteractor>();
        if (interactor == null) return;

        var controller = interactor.GetComponentInParent<ActionBasedController>();
        if (controller == null) return;

        bool isLeftHand = controller == inventoryManager.leftController;

        if (isLeftHand)
        {
            leftEffect.SetActive(false);
        }
        else
        {
            rightEffect.SetActive(false);
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

