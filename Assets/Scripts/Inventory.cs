using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Serializable] public class StringIntDictionary : SerializableDictionary<string, int> { }


    [SerializeField]
    public StringIntDictionary Items = new StringIntDictionary();
}
