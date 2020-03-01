using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoesToClick : MonoBehaviour
{
    bool MapFound = false;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            PreviousLocY.Enqueue(CurrentLocation.y);
            PreviousLocX.Enqueue(CurrentLocation.x);
        }

        TargetX = CurrentLocation.x;
        TargetY = CurrentLocation.y;
        Body = GetComponent<Rigidbody2D>();
        //find clickable areas object and register event
        StartCoroutine(LookForMaps());
    }

    private IEnumerator LookForMaps()
    {
        while (!MapFound)
        {
            foreach (var map in Navigatable.NavigableMaps)
            {
                MapFound = true;
                map.PositionSignalled.AddListener(MoveToLocation);
            }
            yield return new WaitForSeconds(1);
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

        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
        //one unit is four pixels. our tilemaps units are 16 units. when we set a new target location clamp to 16s
        //var currentTarget = (TargetLocation.Equals(Vector2.negativeInfinity)) ? CurrentLocation : TargetLocation;
        if (x != 0 || y != 0)
            Debug.Log($"X: {x}\nY: {y}");

        if (x != 0)
            TargetX = CurrentLocation.x + x * (8 + precision);
        else if (TargetX != null)
            TargetX = Mathf.RoundToInt(TargetX.Value / 16) * 16;
        if (y != 0)
            TargetY = CurrentLocation.y + y * (8 + precision);
        else if (TargetY != null)
            TargetY = Mathf.RoundToInt(TargetY.Value / 16) * 16;

        if (TargetX != null || TargetY != null)
        {

            PreviousLocY.Enqueue(CurrentLocation.y);
            PreviousLocX.Enqueue(CurrentLocation.x);

            var targetLocation = new Vector2(TargetX ?? CurrentLocation.x , TargetY ?? CurrentLocation.y);

            var v = Vector2.MoveTowards(CurrentLocation, targetLocation, Time.fixedDeltaTime * Speed);
            //Debug.Log($"Movement Speed : {Time.fixedDeltaTime * Speed}");
            //Debug.Log($"Target Location : {TargetLocation}");
            //Debug.Log($"Current Position : {CurrentLocation}");
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

            Debug.Log(
                $"X Current: {CurrentLocation.x:0000.000} To {TargetX:0000.000}\n" +
                $"Y Current: {CurrentLocation.y:0000.000} To {TargetY:0000.000}");
            //$"{PreviousLocY.Count} frames ago : {PreviousLocY.Peek()}\n" +

            if (CurrentLocation.y == PreviousLocY.Dequeue() && TargetY != null)
                TargetY = null;
            if (CurrentLocation.x == PreviousLocX.Dequeue() && TargetX != null)
                TargetX = null;
        }
    }

    private bool SameTile(float p, float p2)
    {
        return Mathf.RoundToInt(p / 16) == Mathf.RoundToInt(p2 / 16);
    }

    private void ExittingBlock(float x, float y)
    {
    }

    public void MoveToLocation(Vector2 pos)
    {
        Debug.Log("Received movement signal");
        TargetX = pos.x;
        TargetY = pos.y;
    }
}
