using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanAttackCar : ModalObject {
    public RioterAI ai;
    public Model model;
    public Ctrl ctrl;
    public Unit unit;
    public float tipCarScalingFactor = 0.1f;

    protected int turnsInAttackMode;
    protected bool hasReservation;
    protected int turnsSinceAttacking = 0;
    protected Transform reservation;
    protected FlippableCar nearestCar;
    protected Board board;

    protected void Awake() {
        ai    = GetComponent<RioterAI>();
        unit  = GetComponent<Unit>();

        ctrl  = FindObjectOfType<Ctrl>();
        model = FindObjectOfType<Model>();
        board = FindObjectOfType<Board>();

        Debug.Assert(board != null);

        PlayerTurn += OnPlayerTurn;
    }

    protected void OnPlayerTurn() {
    }

    public float TipCarProb() {
        Debug.Assert(board != null);
        List<FlippableCar> cars = board.cars.Find(unit.Pos(), ai.sightDist);
        if (cars.Count == 0)
            return 0f;
        FlippableCar car = cars.OrderBy(c => c.NumGuests()).Last();
        float monkeySee = car.NumGuests() / Point.ManhattanDistance(car.Pos(), unit.Pos());
        return monkeySee * ai.anger * tipCarScalingFactor;
    }

    public FlippableCar NearestCar() {
        var cars = board.cars.Find(unit.Pos(), ai.sightDist);
        if (cars.Count == 0)
            return null;
        cars.OrderBy(c => Point.ManhattanDistance(c.Pos(), unit.Pos()));
        return cars[0];
    }

    public void SetNoReservation() {
        ai.StopAttackingCar();
        hasReservation = false;
        unit.anim.SetTrigger("stopPush");
    }

    public bool MakeReservation() {
        nearestCar = NearestCar();
        if (nearestCar == null)
            return false;

        reservation = nearestCar.MakeReservation(this);
        if (reservation == null)
            return false;

        hasReservation = true;
        Point rpt = new Point(reservation.position);
        ctrl.EvaluateCmd(new MoveCmd(unit, model, rpt, true));
        return true;
    }

    public void PushCar() {
        nearestCar.GetPushed();
    }

    public bool Attack() {
        if (!hasReservation) {
            return MakeReservation();
        }

        // Face the car.
        Vector3 toLook = nearestCar.transform.position;
        toLook.y = unit.transform.position.y;
        transform.LookAt(toLook);
        unit.anim.SetTrigger("pushCar");

        return true;
    }

    public void OnDestroy() {
        PlayerTurn -= OnPlayerTurn;
    }
}
