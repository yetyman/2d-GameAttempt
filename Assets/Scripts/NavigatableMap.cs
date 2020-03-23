using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class NavigatableMap : MonoBehaviour
{
    public static List<NavigatableMap> NavigableMaps = new List<NavigatableMap>();

    public class PositionEvent : UnityEvent<Vector2> { }
    public UnityEvent<Vector2> PositionSignalled = new PositionEvent();

    public Vector2 ClickPos => Input.mousePosition;
    // Start is called before the first frame update
    void Start()
    {
        NavigableMaps.Add(this);
        //InputController.LeftMouseClick.AddListener(MovementInput);
        //InputController.LeftMouseMove.AddListener(MovementInput);
    }
    public void Click(InputAction.CallbackContext context){
        try
        {
            Debug.Log("Mouse click comes in as "+context.ReadValue<Vector2>());
        }catch(Exception ex)
        {
            ;
        }
        MovementInput(Mouse.current.position.ReadValue());
    }
    void MovementInput(Vector2 rawPos)
    {
        //clamp pos to a grid tile
        Debug.Log($"clicked {rawPos}");
        //fire event for position

        var pos = rawPos;//Camera.main.ScreenToWorldPoint(rawPos);
        pos = transform.InverseTransformPoint(pos);

        //need to convert 

        PositionSignalled?.Invoke(pos);
    }
}
