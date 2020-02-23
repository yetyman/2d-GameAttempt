using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using static MoveInput;

public class Navigatable : MonoBehaviour
{
    public static List<Navigatable> NavigableMaps = new List<Navigatable>();

    public UnityEvent<Vector2> PositionSignalled = new PositionEvent();

    public Vector2 ClickPos => Input.mousePosition;
    // Start is called before the first frame update
    void Start()
    {
        NavigableMaps.Add(this);
        MoveInput.PositionSignalled.AddListener(MovementInput);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void MovementInput(Vector2 pos)
    {
        Debug.Log($"clicked {pos}");
        //fire event for position
        PositionSignalled?.Invoke(pos);
    }
}
