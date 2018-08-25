using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    protected RectTransform dragBox;

    public virtual void OnBeginDrag(PointerEventData eventData) {
        Cursor.visible = false;
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;
        dragBox = transform as RectTransform;
        SetDraggedPosition(eventData);
    }

    public virtual void OnDrag(PointerEventData data) {
        SetDraggedPosition(data);
    }

    private void SetDraggedPosition(PointerEventData data) {
        if (data.pointerEnter != null)
            if (data.pointerEnter.transform as RectTransform != null)
                dragBox = data.pointerEnter.transform as RectTransform;

        Vector3 mousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragBox, data.position, data.pressEventCamera, out mousePos)) {
            transform.position = mousePos;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData) {
        Cursor.visible = true;
    }

    static public T FindInParents<T>(GameObject go) where T : Component {
        if (go == null)
            return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null) {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}
