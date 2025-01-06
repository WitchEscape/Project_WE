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
            Debug.Log($"[InventorySlot] 상호작용 불가: isBusy={isBusy}, isDisabling={isDisabling}");
            return;
        }

        // 컨트롤러가 아이템을 들고 있는지 확인
        XRBaseInteractable itemInHand = null;
        if (controller.hasSelection)
        {
            itemInHand = controller.selectTarget;
            
            // InteractableItemData 체크
            var itemData = itemInHand.GetComponent<InteractableItemData>();
            if (!itemData || !itemData.canInventory)
            {
                Debug.Log($"[InventorySlot] 아이템을 인벤토리에 넣을 수 없음: {itemInHand.name}");
                return;
            }
        }

        InteractWithSlot(controller);
    }


    private void InteractWithSlot(XRDirectInteractor controller)
    {
        Debug.Log($"[InventorySlot] InteractWithSlot 시작 - controller: {controller.name}");

        if (animateItemToSlotCoroutine != null)
        {
            StopCoroutine(animateItemToSlotCoroutine);
            Debug.Log("[InventorySlot] 이전 애니메이션 코루틴 중지");
        }

        XRBaseInteractable itemHandIsHolding = null;
        if (controller.hasSelection)
        {
            itemHandIsHolding = controller.selectTarget;
            Debug.Log($"[InventorySlot] 컨트롤러가 들고 있는 아이템: {itemHandIsHolding.name}");
        }

        //Check if item is allowed to be added to inventory
        if (itemHandIsHolding)
        {
            var itemData = itemHandIsHolding.GetComponent<InteractableItemData>();
            if (!itemData || !itemData.canInventory)
            {
                Debug.Log($"[InventorySlot] 아이템을 인벤토리에 넣을 수 없음: {itemHandIsHolding.name}");
                return;
            }
        }

        if (currentSlotItem)
        {
            Debug.Log($"[InventorySlot] 현재 슬롯에 아이템 있음: {currentSlotItem.name}");
            DisableItemInHand(controller);
            GetNewItemFromSlot(controller);
        }
        else
        {
            Debug.Log("[InventorySlot] 빈 슬롯에 아이템 추가 시도");
            DisableItemInHand(controller);
        }

        //Enable Inventory Slot
        currentSlotItem = itemHandIsHolding;
        Debug.Log($"[InventorySlot] 슬롯에 새 아이템 설정: {(currentSlotItem ? currentSlotItem.name : "없음")}");

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
            Debug.Log("[InventorySlot] DisableItemInHand: 컨트롤러가 들고 있는 아이템 없음");
            return;
        }

        Debug.Log($"[InventorySlot] DisableItemInHand: {itemHandIsHolding.name} 비활성화 시작");
        
        // 아이템을 슬롯의 자식으로 설정하기 전에 위치/회전 저장
        Vector3 targetPosition = transform.position;
        Quaternion targetRotation = transform.rotation;
        
        // 컨트롤러에서 아이템 해제
        ReleaseItemFromHand(controller, itemHandIsHolding);
        
        // itemModelHolder 설정
        var itemHolderTransform = itemModelHolder.transform;
        itemHolderTransform.parent = transform;
        itemHolderTransform.localScale = Vector3.one;
        itemHolderTransform.localPosition = Vector3.zero;
        itemHolderTransform.localEulerAngles = Vector3.zero;

        // 아이템을 슬롯의 자식으로 설정하고 위치 조정
        itemHandIsHolding.transform.parent = transform;
        itemHandIsHolding.transform.position = targetPosition;
        itemHandIsHolding.transform.rotation = targetRotation;
        
        StartCoroutine(DisableItem(itemHandIsHolding));
        
        Debug.Log($"[InventorySlot] DisableItemInHand: {itemHandIsHolding.name} 비활성화 완료");
    }

    //Lets physics respond to collider disappearing before disabling object physics update needs to run twice
    private IEnumerator DisableItem(XRBaseInteractable item)
    {
        Debug.Log($"[InventorySlot] DisableItem 시작: {item.name}");
        
        // 아이템의 원래 위치 저장
        Vector3 originalPosition = item.transform.position;
        Quaternion originalRotation = item.transform.rotation;
        
        // 아이템의 Rigidbody 찾기
        var rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true; // 물리 영향 받지 않도록 설정
            Debug.Log("[InventorySlot] Rigidbody를 kinematic으로 설정");
        }

        item.gameObject.SetActive(true);
        yield return null;

        // OnGrabEnableDisable 컴포넌트 처리
        var enableDisable = item.GetComponent<OnGrabEnableDisable>();
        if (enableDisable != null)
        {
            enableDisable.EnableAll();
            Debug.Log("[InventorySlot] OnGrabEnableDisable 컴포넌트 활성화");
        }

        // 아이템을 원래 위치로 설정
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
        
        yield return new WaitForSeconds(Time.fixedDeltaTime * 2);

        // 아이템 비활성화 전 최종 위치 설정
        item.transform.localPosition = Vector3.zero;
        item.gameObject.SetActive(false);
        
        Debug.Log($"[InventorySlot] 아이템 비활성화 완료: {item.name}");

        yield return new WaitForSeconds(Time.fixedDeltaTime);

        SetupNewMeshClone(item);
    }

    private void GetNewItemFromSlot(XRDirectInteractor controller)
    {
        Debug.Log($"[InventorySlot] GetNewItemFromSlot: {currentSlotItem.name} 활성화 시작");
        
        currentSlotItem.gameObject.SetActive(true);
        currentSlotItem.transform.parent = null;

        // Rigidbody의 isKinematic 해제
        var rb = currentSlotItem.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            Debug.Log("[InventorySlot] Rigidbody의 isKinematic 해제됨");
        }

        GrabNewItem(controller, currentSlotItem);
        //grabAudio.Play();
        
        Debug.Log($"[InventorySlot] GetNewItemFromSlot: {currentSlotItem.name} 활성화 완료");
    }

    private void ReleaseItemFromHand(XRBaseInteractor interactor, XRBaseInteractable interactable)
    {
        Debug.Log($"[InventorySlot] ReleaseItemFromHand: {interactable.name} 해제 시도");
        if (interactionManager == null)
        {
            Debug.LogError("[InventorySlot] XR Interaction Manager가 없습니다!");
            return;
        }
        interactionManager.SelectExit(interactor, interactable);
    }

    private void GrabNewItem(XRBaseInteractor interactor, XRBaseInteractable interactable)
    {
        Debug.Log($"[InventorySlot] GrabNewItem: {interactable.name} 잡기 시도");
        if (interactionManager == null)
        {
            Debug.LogError("[InventorySlot] XR Interaction Manager가 없습니다!");
            return;
        }
        interactionManager.SelectEnter(interactor, interactable);
    }


    private void SetupNewMeshClone(XRBaseInteractable itemHandIsHolding)
    {
        Debug.Log($"[InventorySlot] SetupNewMeshClone 시작: {itemHandIsHolding.name}");

        if (itemSlotMeshClone)
        {
            Debug.Log("[InventorySlot] 기존 메시 클론 제거");
            Destroy(itemSlotMeshClone.gameObject);
        }

        // 클론을 itemModelHolder의 자식으로 직접 생성
        Debug.Log($"[InventorySlot] 새 메시 클론 생성 - 부모: {itemModelHolder.name}");
        itemSlotMeshClone = Instantiate(itemHandIsHolding, itemModelHolder.position, itemModelHolder.rotation, itemModelHolder).transform;
        itemSlotMeshClone.gameObject.SetActive(true); // 명시적으로 활성화

        try
        {
            Debug.Log("[InventorySlot] 클론의 컴포넌트 제거 시작");
            DestroyComponentsOnClone(itemSlotMeshClone);
            Debug.Log("[InventorySlot] 클론의 컴포넌트 제거 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySlot] 컴포넌트 제거 중 오류 발생: {e.Message}");
        }

        // 바운드 센터 설정
        Bounds bounds = GetBoundsOfAllMeshes(itemSlotMeshClone.transform);
        if (!boundCenterTransform)
        {
            boundCenterTransform = new GameObject("Bound Center Transform").transform;
            boundCenterTransform.parent = itemModelHolder; // 명시적으로 itemModelHolder의 자식으로 설정
        }

        // 위치와 회전 설정
        boundCenterTransform.position = bounds.center;
        boundCenterTransform.rotation = itemHandIsHolding.transform.rotation;
        
        // 클론을 boundCenterTransform의 자식으로 설정
        itemSlotMeshClone.parent = boundCenterTransform;

        // 시작 트랜스폼 설정
        startingTransformFromHand.SetTransformStruct(
            boundCenterTransform.localPosition,
            boundCenterTransform.localRotation,
            boundCenterTransform.localScale
        );

        // 회전 설정
        boundCenterTransform.localEulerAngles = new Vector3(0, 90, 0);

        // 크기 조절
        inventorySize.enabled = true;
        Vector3 parentSize = inventorySize.bounds.size;
        while (bounds.size.x > parentSize.x || bounds.size.y > parentSize.y || bounds.size.z > parentSize.z)
        {
            bounds = GetBoundsOfAllMeshes(boundCenterTransform.transform);
            boundCenterTransform.transform.localScale *= 0.9f;
        }
        inventorySize.enabled = false;

        goalSizeToFitInSlot = boundCenterTransform.transform.localScale;

        // 애니메이션 시작 전에 계층 구조 확인
        Debug.Log($"[InventorySlot] 클론 부모 확인: {itemSlotMeshClone.parent.name}");
        Debug.Log($"[InventorySlot] boundCenterTransform 부모 확인: {boundCenterTransform.parent.name}");

        animateItemToSlotCoroutine = StartCoroutine(AnimateItemToSlot());
    }

    private void ActivateItemSlotMeshClone() => itemSlotMeshClone.gameObject.SetActive(true);

    private void DestroyComponentsOnClone(Transform clone)
    {
        Debug.Log("[InventorySlot] 클론 컴포넌트 제거 시작");

        try
        {
            // 먼저 IReturnMovedColliders 처리
            var movedColliders = clone.GetComponentsInChildren<IReturnMovedColliders>(true);
            foreach (var t in movedColliders) 
            {
                t.ReturnMovedColliders();
                Debug.Log($"[InventorySlot] IReturnMovedColliders 처리: {t.GetType().Name}");
            }

            // 먼저 InteractableItemData 비활성화
            var itemDataComponents = clone.GetComponentsInChildren<InteractableItemData>(true);
            foreach (var t in itemDataComponents)
            {
                t.enabled = false;
                Debug.Log($"[InventorySlot] InteractableItemData 비활성화: {t.name}");
            }

            // XRGrabInteractable 비활성화
            var grabComponents = clone.GetComponentsInChildren<XRGrabInteractable>(true);
            foreach (var t in grabComponents)
            {
                t.enabled = false;
                Debug.Log($"[InventorySlot] XRGrabInteractable 비활성화: {t.name}");
            }

            // Collider와 Light 제거
            var lights = clone.GetComponentsInChildren<Light>(true);
            foreach (var t in lights) 
            {
                Destroy(t);
                Debug.Log($"[InventorySlot] Light 제거: {t.name}");
            }

            var colliders = clone.GetComponentsInChildren<Collider>(true);
            foreach (var t in colliders)
            {
                Destroy(t);
                Debug.Log($"[InventorySlot] Collider 제거: {t.name}");
            }

            // 나머지 MonoBehaviour 컴포넌트들 제거 (InteractableItemData와 XRGrabInteractable 제외)
            var monoBehaviors = clone.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var t in monoBehaviors)
            {
                if (!(t is InteractableItemData) && !(t is XRGrabInteractable))
                {
                    Destroy(t);
                    Debug.Log($"[InventorySlot] MonoBehaviour 제거: {t.GetType().Name}");
                }
            }

            // Rigidbody는 비활성화만 하고 제거하지 않음
            var rigidBodies = clone.GetComponentsInChildren<Rigidbody>(true);
            foreach (var t in rigidBodies)
            {
                t.isKinematic = true;
                t.useGravity = false;
                Debug.Log($"[InventorySlot] Rigidbody 비활성화: {t.name}");
            }

            Debug.Log("[InventorySlot] 클론 컴포넌트 제거 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySlot] 컴포넌트 제거 중 오류 발생: {e.Message}\n{e.StackTrace}");
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

