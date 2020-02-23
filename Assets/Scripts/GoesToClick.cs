using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoesToClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        //find clickable areas object and register event
        foreach(var map in Navigatable.NavigableMaps)
        {
            map.PositionSignalled.AddListener(MoveToLocation);
        }
    }
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

        var v = Vector2.MoveTowards(CurrentLocation, TargetLocation, Time.deltaTime * Speed);
        Debug.Log($"Target Location : {TargetLocation}");
        Debug.Log($"Current Position : {CurrentLocation}");
        Debug.Log($"Move To : {v}");
        Body.MovePosition(v);
    }
    public void MoveToLocation(Vector2 pos)
    {
        TargetLocation = pos;
    }
}
