using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class StringGameObjectLookup : SerializableLookup<string, GameObject>{ }
public class UIReferenceObj : MonoBehaviour
{
    public static UIReferenceObj Instance;
    public StringGameObjectLookup BoundUIElements = new StringGameObjectLookup();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

}
