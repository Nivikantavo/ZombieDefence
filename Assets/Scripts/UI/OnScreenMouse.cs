using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class OnScreenMouse : OnScreenControl, IDragHandler
{
    [SerializeField, InputControl(layout = "Vector2")]
    private string m_ControlPath;

    private bool hasDrag;
    private Vector2 delta;

    protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }

    private void LateUpdate()
    {
        if (hasDrag)
        {
            SendValueToControl(delta);
            hasDrag = false;
        }
        else
        {
            SendValueToControl(new Vector2(0, 0));
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        delta = eventData.delta;
        hasDrag = true;
    }
}
