using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shuffle;

public struct Reservation {
    public CanAttackCar guest;
    public Transform seat;
    public bool rightSide;

    public Reservation(CanAttackCar guest, Transform seat, bool rightSide) {
        this.guest     = guest;
        this.seat      = seat;
        this.rightSide = rightSide;
    }
}

public class FlippableCar : ModalObject {
    public int peopleRequiredToTipCar = 1;
    public GameObject deathFx;
    public Transform leftSideTipsTo;
    public Transform rightSideTipsTo;
    public bool flipNextTurn;

    public List<Transform> seatsL = new List<Transform>();
    public List<Transform> seatsR = new List<Transform>();

    protected Dictionary<CanAttackCar, Reservation> reservations = new Dictionary<CanAttackCar, Reservation>();
    protected int guestsOnLeft;
    protected int guestsOnRight;
    protected Animator anim;
    protected Point originalPos;
    
    protected Board board;

     protected void Awake() {
        anim = GetComponent<Animator>();

        seatsL.Shuffle();
        seatsR.Shuffle();
        board = FindObjectOfType<Board>();

        // we need to remember the position we start on
        // because animations may move our center of mass
        // over into another tile temporarily.
        originalPos = new Point(transform.position);
        board.cars.Add(originalPos, this);

        EnemyTurn += OnEnemyTurn;
    }

    public int NumGuests() {
        return guestsOnLeft + guestsOnRight;
    }

    public void GetPushed() {
        anim.SetTrigger("tip");
    }

    public void SpawnDeathFx() {
        board.cars.Del(originalPos);
        var dfx = Instantiate(deathFx);
        dfx.transform.position = transform.position;
        //if (guestsOnLeft > guestsOnRight)
        //    dfx.transform.position = leftSideTipsTo.position;
        //else
        //    dfx.transform.position = rightSideTipsTo.position;

        foreach (CanAttackCar guest in reservations.Keys) {
            guest.SetNoReservation();
        }


        gameObject.SetActive(false);
    }

    protected void OnEnemyTurn() {
        // We won't flip over the car until we're sure everyone has shown up to
        // push the car.
        if (flipNextTurn)
            anim.SetTrigger("flipOver");

        else if (NumGuests() >= peopleRequiredToTipCar) {
            flipNextTurn = true;
        }
    }

    public Point Pos() {
        return new Point(transform.position);
    }

    /// <summary>
    /// Imagine flipping a car as being analgous to getting a reservation at a restaurant.
    /// A restaurant can only seat so many people and there may be more demand than space.
    /// So someone who wants to eat at the restaurant (or join your friends to flip over a
    /// car) needs to call up and ask for a reservation. The restaurant will then tell you
    /// which seats to go to (typically once you get there, but it doesn't really matter).
    /// Unlike at a real restaurant, you have to explicitly cancel a reservation - perhaps
    /// I should have called it "ask for check," but you're not actually getting anything
    /// back, you're just giving up your seat.
    /// </summary>
    public Transform MakeReservation(CanAttackCar guest) {
        bool sideL;
        // Determine which side is being tipped.
        if (guestsOnLeft == guestsOnRight)
            sideL = Random.Range(0, 1) > 0.5f;
        else
            sideL = guestsOnLeft > guestsOnRight;
        anim.SetBool("rightSide", !sideL);

        // If we don't have any spots left, return null;
        if ((sideL && seatsL.Count == 0) || seatsR.Count == 0)
            return null;

        // Transfer to taken spots and return.
        // This isn't complicated enough to refactor into a new function yet.
        if (sideL) {
            var seat = seatsL[0];
            seatsL.Remove(seat);
            reservations.Add(guest, new Reservation(guest, seat, false));
            guestsOnLeft++;
        }

        else {
            var seat = seatsR[0];
            seatsR.Remove(seat);
            reservations.Add(guest, new Reservation(guest, seat, true));
            guestsOnRight++;
        }

        return reservations[guest].seat;
    }

    public void CancelReservation(CanAttackCar guest) {
        Reservation reservation = reservations[guest];

        if (!reservation.rightSide) {
            guestsOnLeft--;
            seatsL.Add(reservation.seat);
            reservations.Remove(guest);
        }

        else {
            guestsOnRight--;
            seatsR.Add(reservation.seat);
            reservations.Remove(guest);
        }

        guest.SetNoReservation();
    }
}
