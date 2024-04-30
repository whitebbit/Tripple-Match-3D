using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScaledButton : Button
{
    bool down;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!down)
        {
            transform.localScale -= Vector3.one * 0.1f;
            down = true;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (down)
        {
            transform.localScale += Vector3.one * 0.1f;
            down = false;
        }
    }
}
