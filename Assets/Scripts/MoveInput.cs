using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveInput : MonoBehaviour
{
    public class PositionEvent : UnityEvent<Vector2> { }
    public static UnityEvent<Vector2> PositionSignalled = new PositionEvent();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool PreviousDownState;
    Vector2 PreviousPos;
    // Update is called once per frame
    void Update()
    {
        Vector2 v = Vector2.zero;
        //get mouse or touch.
        //Touch touch = Input.GetTouch(0);

        //var v = touch.position;
        ////Update the Text on the screen depending on current position of the touch each frame
        //if (v != Vector2.zero){
        //    Debug.Log("Touch Position : " + v);
        //}
        //else 
        if (Input.GetMouseButtonDown(0)){
            v = Input.mousePosition;
            Debug.Log("Mouse Position : " + v);
        }
        else PreviousDownState = false;

        if(v!= Vector2.zero)
        {
            if (!PreviousDownState || PreviousPos != v)
                MovementInput(v);
            PreviousDownState = true;
        }

        PreviousPos = v;
    }

    void MovementInput(Vector2 pos)
    {
        Debug.Log($"clicked {pos}");
        //fire event for position
        PositionSignalled?.Invoke(pos);
    }
}
