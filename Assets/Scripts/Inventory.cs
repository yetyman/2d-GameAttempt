using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [Serializable] public class StringIntDictionary : SerializableDictionary<string, int> { }


    [SerializeField]
    public StringIntDictionary Items = new StringIntDictionary();

    [Serializable]
    public class InventoryAddEvent : UnityEvent<string> { }

    public static InventoryAddEvent AddEvent = new InventoryAddEvent();

    public void Add(string name)
    {
        if (!Items.ContainsKey(name))
            Items.Add(name, 0);
        Items[name]++;

        AddEvent.Invoke(name);
    }
}
