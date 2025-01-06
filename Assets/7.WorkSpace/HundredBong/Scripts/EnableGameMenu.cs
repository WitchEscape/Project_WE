using UnityEngine;
using UnityEngine.InputSystem;

public class EnableGameMenu : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2f;
    public GameObject menu;
    public InputActionProperty showButton;

    private void Update()
    {
        //메뉴 버튼을 누르면 플레이어 앞에 메뉴 활성화

        if (showButton.action.WasPressedThisFrame())
        {
            menu.SetActive(!menu.activeSelf);

            menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;


        }
        menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= -1;
    }
}
