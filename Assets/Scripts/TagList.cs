using UnityEngine;
using System.Collections.Generic;
using System;

public class TagList : MonoBehaviour
{
    [Serializable] public class StringStringDictionary : SerializableDictionary<string, string> { }


    [SerializeField]
    public StringStringDictionary Attributes = new StringStringDictionary();
    
}