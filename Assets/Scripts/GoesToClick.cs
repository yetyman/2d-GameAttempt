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
    void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
            TargetLocation = CurrentLocation + new Vector2(x,y).normalized;

        if (!TargetLocation.Equals(Vector2.negativeInfinity))
        {
            var v = Vector2.MoveTowards(CurrentLocation, TargetLocation, Time.deltaTime * Speed);
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
