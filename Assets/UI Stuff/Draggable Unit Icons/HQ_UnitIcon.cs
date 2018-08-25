using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HQ_UnitIcon : Draggable {
    public Image icon;
    public Unit  unit;

    public GameObject iconGO;
    public GameObject unitGO;

    public Transform left;
    public Transform mid;
    public Transform right;

    public UnitItemLib itemLib;
    public ClassIcons  iconLib;

    public CurrentlySelected selected;

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
        if (selected == null)
            selected = FindObjectOfType<CurrentlySelected>();
    }

    public override void OnDrag(PointerEventData data) {
        base.OnDrag(data);
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);

        float x = transform.position.x;
        float dist2left  = Mathf.Abs(x);
        float dist2mid   = Mathf.Abs(x - Screen.width / 2f);
        float dist2right = Mathf.Abs(x - Screen.width);

        if (dist2left < dist2mid)
            PlaceLeft();
        else if (dist2right < dist2mid)
            PlaceRight();
        // only one unit can be edited at a time
        else if (mid.transform.childCount == 0)
            PlaceMid();
        else
            PlaceLeft();

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
    }

    public void PlaceLeft() {
        transform.SetParent(left.transform);
        unit.gameObject.SetActive(false);
        SaveWrapper.savef.TakeOffMission(unit.unitID);
        if (selected.unit == gameObject)
            selected.unit = null;
    }

    public void PlaceMid() {
        transform.SetParent(mid.transform);
        unit.gameObject.SetActive(true);
        unit.transform.SetParent(null);   // layout groups will move this after we center it
        unit.transform.position = Vector3.zero;
        SaveWrapper.savef.TakeOffMission(unit.unitID);
        selected.unit = unit.gameObject;
    }
    

    public void PlaceRight() {
        transform.SetParent(right.transform);
        unit.gameObject.SetActive(false);
        SaveWrapper.savef.PutOnMission(unit.unitID);
        if (selected.unit == gameObject)
            selected.unit = null;
    }

    public void Set(CharSheet sheet) {
        // This class might be created and used
        // before it has time to call Awake.
        sheet.FillUnit(ref unit, itemLib);
        icon.sprite = iconLib.Get((int)sheet.unitType);
    }
}
