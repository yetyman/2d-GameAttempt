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

    public float precision = .01f;
    public Vector2 CurrentLocation => transform.localPosition;
    public Vector2 TargetLocation;
    public float Speed;
    Rigidbody2D Body;
    // Update is called once per frame
    void FixedUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
        {
            //one unit is four pixels. our tilemaps units are 16 units. when we set a new target location clamp to 16s
            //var currentTarget = (TargetLocation.Equals(Vector2.negativeInfinity)) ? CurrentLocation : TargetLocation;
            var currentTarget =  CurrentLocation;

            TargetLocation = currentTarget + new Vector2(x * 16, y * 16);
            TargetLocation.x = Mathf.RoundToInt(TargetLocation.x/16)*16;
            TargetLocation.y = Mathf.RoundToInt(TargetLocation.y/16)*16;

        }
        if (!TargetLocation.Equals(Vector2.negativeInfinity))
        {
            var v = Vector2.MoveTowards(CurrentLocation, TargetLocation, Time.fixedDeltaTime * Speed);
            Debug.Log($"Movement Speed : {Time.fixedDeltaTime * Speed}");
            Debug.Log($"Target Location : {TargetLocation}");
            Debug.Log($"Current Position : {CurrentLocation}");
            Debug.Log($"Move To : {v}");

            Body.MovePosition(v);

            if ((CurrentLocation - TargetLocation).sqrMagnitude < precision)
            {
                Debug.Log($"Setting Target Location: {TargetLocation}");
                TargetLocation = Vector2.negativeInfinity;
            }
        }
    }
    public void MoveToLocation(Vector2 pos)
    {
        Debug.Log("Received movement signal");
        TargetLocation = pos;
    }
}
