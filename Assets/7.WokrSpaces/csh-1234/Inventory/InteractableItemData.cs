using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableItemData : MonoBehaviour
{
    [Tooltip("Resources 폴더 내의 프리팹 경로")]
    public string prefabPath;
    
    [Tooltip("인벤토리에 넣을 수 있는지 여부")]
    public bool canInventory = true;

    private void OnValidate()
    {
        // 에디터에서 프리팹 경로 자동 설정 (선택사항)
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(prefabPath))
        {
            // 현재 게임오브젝트가 프리팹인 경우
            var prefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefab != null)
            {
                Debug.Log($"프리팹 있음 {prefab}");
                // Resources 폴더 내의 상대 경로 찾기
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(prefab);
                string resourcesPath = "Resources/";
                int index = assetPath.IndexOf(resourcesPath);
                if (index != -1)
                {
                    // .prefab 확장자 제거
                    string path = assetPath.Substring(index + resourcesPath.Length);
                    path = System.IO.Path.ChangeExtension(path, null);
                    prefabPath = path;
                }
            }
        }
        #endif
    }
}