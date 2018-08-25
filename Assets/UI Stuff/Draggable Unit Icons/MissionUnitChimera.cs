using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MissionUnitChimera : Draggable {
    public GameObject  iconRepresentation;
    public GameObject  worldRepresentation;
    public CharSheet   charSheet;
    public UnitItemLib itemLib;
    public LayerMask   worldLayers;
    public Party       playerParty;
    public float minDragDist;
    public bool isWorld {
        get {
            return worldRepresentation.activeSelf;
        }

        set {
            if (isWorld != value) {
                iconRepresentation .SetActive(!value);
                worldRepresentation.SetActive( value);
            }
        }
    }

    protected bool wasWorld = false;
    protected Ctrl      ctrl;
    protected Model     model;
    protected Unit      unit;
    protected Vector3   originalPos;
    protected Transform originalParent;

    protected void Awake() {
        originalParent = transform.parent;
        unit = worldRepresentation.GetComponent<Unit>();
        Debug.Assert(unit != null);
        ctrl  = FindObjectOfType<Ctrl>();
        model = FindObjectOfType<Model>();
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
        originalPos = transform.position;
        isWorld = false;
        charSheet.FillUnit(ref unit, itemLib);
    }

    public override void OnDrag(PointerEventData data) {
        base.OnDrag(data);
        Vector3 iconPos = transform.position;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        isWorld = Physics.Raycast(r, out hit, 100, worldLayers);
        if (isWorld) {
            worldRepresentation.transform.position = hit.point;
        }
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnDrag(eventData);

        if (Mathf.Abs(transform.position.y - originalPos.y) <= minDragDist) {
            isWorld = false;
            transform.SetParent(originalParent);
        }

        if (isWorld) {
            unit.transform.SetParent(null);
            unit.transform.position = unit.Pos().Rounded().ToV3();
            foreach (Point pt in unit.GetOccupiedPoints()) {
                model.board.Place(unit, pt);
            }
            playerParty.Add(unit);
            Destroy(gameObject);
        }

        else {
            isWorld = false;
            transform.SetParent(originalParent);
            iconRepresentation.transform.SetParent(originalParent);
            // move unit off screen
            unit.transform.position = new Vector3(-1000, -1000, -1000);
        }

        Rebuild();
    }

    private void Rebuild() {
        RectTransform rect = transform.parent as RectTransform;
        if (rect != null) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    public void Fill(CharSheet sheet) {
        this.charSheet = sheet;
    }
}
