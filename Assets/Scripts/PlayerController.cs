using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static Inventory;

public class PlayerController : MonoBehaviour
{
    bool MapFound = false;
    public Inventory Inventory;
    public bool SnapToGrid = false;
    public int StuckBufferLength = 3;

    public Action MoveAction;
    public Predicate<object> KeepMovingWhile;
    public Func<YieldInstruction> MoveCheckRateInstruction;
    // Start is called before the first frame update


    public InputActionAsset asset;
    InputAction inputAction;
    private Vector2Control MovementControl;
    void Awake()
    {
        inputAction = asset.FindAction("Player/Move");
        // Getting the first binding of the input action using index of 0. If we had more bindings, we would use different indices.
        //MovementControl = (Vector2Control)inputAction.controls[0];
        inputAction.Enable();

        TargetX = CurrentLocation.x;
        TargetY = CurrentLocation.y;
        Body = GetComponent<Rigidbody2D>();
        //find clickable areas object and register event
        StartCoroutine(LookForMaps());

        Inventory = GetComponent<Inventory>();
    }

    private IEnumerator LookForMaps()
    {
        while (!MapFound)
        {
            foreach (var map in NavigatableMap.NavigableMaps)
            {
                MapFound = true;
                map.PositionSignalled.AddListener(MoveToLocation);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public float precision = .001f;
    public Vector2 CurrentLocation => transform.localPosition;
    public float? TargetX;
    public float? TargetY;
    public float Speed;
    Rigidbody2D Body;

    private Queue<float> PreviousLocY = new Queue<float>();
    private Queue<float> PreviousLocX = new Queue<float>();
    // Update is called once per frame
    void FixedUpdate()
    {

        //var x = InputController.XDelta;
        //var y = InputController.YDelta;
        ////one unit is four pixels. our tilemaps units are 16 units. when we set a new target location clamp to 16s
        ////var currentTarget = (TargetLocation.Equals(Vector2.negativeInfinity)) ? CurrentLocation : TargetLocation;
        ////if (x != 0 || y != 0)
        ////Debug.Log($"X: {x}\nY: {y}");

        //if (x != 0)
        //{
        //    PreviousLocX.Clear();
        //    TargetX = CurrentLocation.x + x * 8 + precision;
        //}

        //if (y != 0)
        //{
        //    PreviousLocY.Clear();
        //    TargetY = CurrentLocation.y + y * 8 + precision;
        //}
        //Snap();

        if (TargetX != null || TargetY != null)
        {

            PreviousLocY.Enqueue(CurrentLocation.y);
            PreviousLocX.Enqueue(CurrentLocation.x);

            var targetLocation = new Vector2(TargetX ?? CurrentLocation.x, TargetY ?? CurrentLocation.y);

            var v = Vector2.MoveTowards(CurrentLocation, targetLocation, Time.fixedDeltaTime * Speed);

            //Debug.Log($"Move To : {v}");

            Body.MovePosition(v);

            if ((CurrentLocation - targetLocation).magnitude < precision)
            {
                //Debug.Log($"Setting Target Location: {TargetLocation}");
                TargetX = null;
                TargetY = null;
            }


            //if (Body.position.x == PreviousLocX && TargetX != null)
            //    TargetX = null;

            //Debug.Log(
            //    $"X Current: {CurrentLocation.x:0000.000} To {TargetX:0000.000}\n" +
            //    $"Y Current: {CurrentLocation.y:0000.000} To {TargetY:0000.000}");

            if (PreviousLocY.Count >= StuckBufferLength && CurrentLocation.y == PreviousLocY.Dequeue() && TargetY != null)
            {
                Debug.Log($"Sensing Stuck Y after {StuckBufferLength} frames. {string.Join(", ", PreviousLocY)}");
                TargetY = null;
                PreviousLocY.Clear();

                TargetX = null;
                PreviousLocX.Clear();
            }
            if (PreviousLocX.Count >= StuckBufferLength && CurrentLocation.x == PreviousLocX.Dequeue() && TargetX != null)
            {
                Debug.Log($"Sensing Stuck X after {StuckBufferLength} frames. {string.Join(", ", PreviousLocX)}");
                TargetX = null;
                PreviousLocX.Clear();

                TargetY = null;
                PreviousLocY.Clear();
            }
        }
    }

    Task routine;
    YieldInstruction MovementCheckRate;
    public void MoveRelative(InputAction.CallbackContext context)
    {
        //arrow key movement
        Vector2 movement;

        float x, y;

        KeepMovingWhile = KeepMovingWhile ?? new Predicate<object>(
            (o) => true// inputAction.ReadValue<Vector2>() != Vector2.zero//MovementControl.IsActuated()
        );

        MoveAction = new Action(() =>
        {
            movement = inputAction.ReadValue<Vector2>();
            //Debug.Log(movement);
            x = movement.x;
            y = movement.y; 
            if (x != 0)
            {
                PreviousLocX.Clear();
                TargetX = CurrentLocation.x + x * 8 + precision;//*8 is for grid like movement when snapping. 
            }

            if (y != 0)
            {
                PreviousLocY.Clear();
                TargetY = CurrentLocation.y + y * 8 + precision;
            }

            Snap();

            //Debug.Log($"Movement Speed : {Time.fixedDeltaTime * Speed}");
            //Debug.Log($"Current Position : {CurrentLocation}");
            //Debug.Log($"Target Location : {new Vector2(TargetX ?? CurrentLocation.x, TargetY ?? CurrentLocation.y)}");
        });


        MovementCheckRate = MovementCheckRate ?? new WaitForEndOfFrame();
        MoveCheckRateInstruction = MoveCheckRateInstruction ?? (()=>MovementCheckRate);

        if(!routine?.Running ?? true)
            routine = this.StartSingletonCoroutine(MoveAction, KeepMovingWhile, MoveCheckRateInstruction);
    }
    public void MoveToLocation(Vector2 pos)
    {
        //touch/click movement
        PreviousLocX.Clear();
        PreviousLocY.Clear();
        //Debug.Log("Received movement signal");
        TargetX = pos.x;
        TargetY = pos.y;

        Snap();

        Debug.Log($"Movement Speed : {Time.fixedDeltaTime * Speed}");
        Debug.Log($"Current Position : {CurrentLocation}");
        Debug.Log($"Target Location : {new Vector2(TargetX ?? CurrentLocation.x, TargetY ?? CurrentLocation.y)}");

    }

    private void Snap()
    {

        if (SnapToGrid && TargetX != null)
            TargetX = Mathf.RoundToInt(TargetX.Value / 16) * 16;
        if (SnapToGrid && TargetY != null)
            TargetY = Mathf.RoundToInt(TargetY.Value / 16) * 16;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"Colliding");
        if (collision?.gameObject?.GetComponent<TagList>()?.Attributes?.ContainsKey("Obtainable")??false)
        {
            Destroy(collision.gameObject);
            string type = collision.gameObject.GetComponent<TagList>().Attributes["GameObjectType"];
            Inventory.Add(type);
        }
    }
}
