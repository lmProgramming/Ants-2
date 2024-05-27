using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableUIAddIfHeld : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameInput.StartHoldingUIElement();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameInput.StopHoldingUIElement();
    }
}
