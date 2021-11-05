using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool Held;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("SS");
        Held = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Held = false;
    }
}