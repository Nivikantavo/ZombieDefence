using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class OnScreenMouse : OnScreenControl, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, InputControl(layout = "Vector2")]
    private string m_ControlPath;

    private bool hasDrag;
    private Vector2 delta;

    protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down");
        delta = eventData.delta;
        hasDrag = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("up");
        hasDrag = false;
    }

    private void LateUpdate()
    {
        if (hasDrag)
        {
            SendValueToControl(new Vector2(0, 0));
            hasDrag = false;
        }
        else
        {
            SendValueToControl(delta);
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrug");
        delta = eventData.delta;
        hasDrag = true;
    }
}
