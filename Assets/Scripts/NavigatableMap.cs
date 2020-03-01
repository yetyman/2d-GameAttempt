using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using static InputController;

public class NavigatableMap : MonoBehaviour
{
    public static List<NavigatableMap> NavigableMaps = new List<NavigatableMap>();

    public UnityEvent<Vector2> PositionSignalled = new PositionEvent();

    public Vector2 ClickPos => Input.mousePosition;
    // Start is called before the first frame update
    void Start()
    {
        NavigableMaps.Add(this);
        InputController.PositionSignalled.AddListener(MovementInput);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void MovementInput(Vector2 rawPos)
    {
        //clamp pos to a grid tile
        Debug.Log($"clicked {rawPos}");
        //fire event for position
        
        PositionSignalled?.Invoke(rawPos);
    }
}
