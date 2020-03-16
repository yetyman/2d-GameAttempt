
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class SerializableLookup<TKey, TValue> : SerializableDictionary<TKey, List<TValue>>, ISerializationCallbackReceiver
{


    [SerializeField, HideInInspector] TKey[] _Keys2;
    [SerializeField, HideInInspector] TValue[] _Values2;

    public new int Count
    {
        get { return Keys?.SelectMany(x => base[x])?.Where(x=>x!=null)?.Count()??0; }
    }

    public SerializableLookup()
    {
    }

    public new List<KeyValuePair<TKey, TValue>> ToList()
    {
        var ls = new List<KeyValuePair<TKey, TValue>>();
        var enumerator = GetEnumerator();
        while (enumerator.MoveNext())
            if(enumerator.Current.Value != null)
            foreach(var item in enumerator.Current.Value)
                ls.Add(new KeyValuePair<TKey, TValue>(enumerator.Current.Key, item));
        enumerator.Dispose();
        return ls;
    }
    public TValue this[TKey key, int index]
    {
        get
        {
            return base[key][index];
        }
        set
        {
            base[key][index] = value;
        }
    }
    public new List<TValue> this[TKey key]
    {
        set
        {
            if (value == null) value = new List<TValue>();
            base[key] = value;
        }
        get
        {
            return base[key];
        }
    }
    public bool ContainsValue(TValue value)
    {
        return Keys.SelectMany(x => base[x]).Any(x => value.Equals(x));
    }

    public new bool ContainsValue(List<TValue> values)
    {
        return Keys.Select(x => base[x]).Any(x => values.Equals(x));
    }

    public void Add(TKey key, TValue value)
    {
        if (!ContainsKey(key) || base[key] == null)
            base[key] = new List<TValue>() { value };
        else base[key].Add(value);
    }
    public new void Add(TKey key, List<TValue> values)
    {
        if (!ContainsKey(key) || base[key] == null)
            base[key] = values.ToList();
        else base[key].AddRange(values);
    }
    public void RemoveAt(TKey key, int index)
    {
        base[key].RemoveAt(index);
    }

    public void OnBeforeSerialize()
    {
        var ls = ToList();
        _Keys2 = new TKey[ls.Count];
        _Values2 = new TValue[ls.Count];
        for (int i = 0; i < ls.Count; i++)
        {
            _Keys2[i] = ls[i].Key;
            _Values2[i] = ls[i].Value;
        }
    }

    public void OnAfterDeserialize()
    {
        if(_Keys2 != null && _Values2 != null)
            for (int i = 0; i < _Keys2.Length; i++)
                Add(_Keys2[i], _Values2[i]);
            
    }
}