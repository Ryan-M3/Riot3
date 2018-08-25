using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrestCmd : ICommand {
    Unit suspect;
    Unit officer;
    Model model;
    Point tgt;

    public ArrestCmd(Unit officer, Point tgt, Model model) {
        this.officer = officer;
        this.tgt     = tgt;
        this.model   = model;
    }

    public bool CanDo() {
        Unit tgtUnit = model.board.units.Get(tgt);
        if (tgtUnit == null) {
            View.ShowMsg("You can't arrest that.");
            return false;
        } else {
            this.suspect = tgtUnit;
        }

        if (suspect.tag == "Player") {
            View.ShowMsg("You can't arrest your own units.");
            return false;
        }

        return true;
    }

    protected IEnumerator BlinkOutOfExistence() {
        // magic numbers determined through trial and error
        const int blinks = 4;
        const float blinkDur = 0.35f / (float)blinks;

        MeshSetter mset = suspect.GetComponent<MeshSetter>();

        for (int i = 0; i < blinks; i++) {
            if (mset != null) {
                mset.boots.enabled = !mset.boots.enabled;
                mset.btm  .enabled = !mset.btm  .enabled;
                mset.top  .enabled = !mset.top  .enabled;
                mset.handL.enabled = !mset.handL.enabled;
                mset.handR.enabled = !mset.handR.enabled;
                mset.head .enabled = !mset.head .enabled;
            }
            yield return new WaitForSeconds(blinkDur);
        }
        suspect.gameObject.SetActive(false);
    }

    public void Do() {
        Debug.Log("arresting");
        suspect.anim.SetTrigger("getArrested");
        suspect.party.SetInactive(suspect);
        officer.anim.SetTrigger("arrest");
        model.board.ClearTile(new Point(suspect.transform.position));
        officer.StartCoroutine(BlinkOutOfExistence());
    }

    public void Undo() {
        suspect.party.SetActive(suspect);
        model.board.Place(suspect, new Point(suspect.transform.position));
    }
}
