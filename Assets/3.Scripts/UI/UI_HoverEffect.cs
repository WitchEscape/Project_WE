using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.Shapes;

public class UI_HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject focus;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (focus != null)
        {
            focus.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (focus != null)
        {
            focus.SetActive(false);
        }
    }
}
