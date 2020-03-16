using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Inventory;
using static TagList;
using UnityObject = UnityEngine.Object;

public abstract class LookupDrawer<TK, TV> : DictionaryDrawer<TK, List<TV>>
{
    protected SerializableLookup<TK, TV> _Lookup;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        CheckInitialize(property, label);
        if (_Foldout)
        {
            var cnt = 1;
            cnt += _Lookup.Select(x =>
            {
                if (x.Value == null)
                    return 1;
                else if (x.Value.Count == 0)
                    return 1;
                else return x.Value.Count();
            }).Sum();
                
            return cnt * 17f;
        }
        return 17f;
    }

    private new void CheckInitialize(SerializedProperty property, GUIContent label)
    {
        base.CheckInitialize(property, label);
        if(_Lookup == null)
        _Lookup = _Dictionary as SerializableLookup<TK,TV>;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CheckInitialize(property, label);

        position.height = 17f;

        var foldoutRect = position;
        foldoutRect.width -= 2 * kButtonWidth;
        EditorGUI.BeginChangeCheck();
        _Foldout = EditorGUI.Foldout(foldoutRect, _Foldout, label, true);
        if (EditorGUI.EndChangeCheck())
            EditorPrefs.SetBool(label.text, _Foldout);

        var buttonRect = position;
        buttonRect.x = position.width - kButtonWidth + position.x;
        buttonRect.width = kButtonWidth + 2;

        if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButtonRight))
        {
            AddNewItem();
        }

        buttonRect.x -= kButtonWidth;

        if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonLeft))
        {
            ClearDictionary();
        }

        if (!_Foldout)
            return;

        position.y += 17f;

        foreach (var item in _Lookup)
        {
            var key = item.Key;
            var value = item.Value;


            var keyRect = position;
            keyRect.width /= 2;
            keyRect.width -= 4;
            EditorGUI.BeginChangeCheck();
            var newKey = DoField(keyRect, typeof(TK), key);
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    _Lookup.Remove(key);
                    _Lookup.Add(newKey, value);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                break;
            }

            int i = 0;//
            foreach (var val in item.Value ?? new List<TV>())
            {
                var valueRect = position;
                valueRect.x = position.width / 2 + 15;
                valueRect.width = keyRect.width - kButtonWidth;
                EditorGUI.BeginChangeCheck();
                var v = DoField(valueRect, typeof(TV), val);
                if (EditorGUI.EndChangeCheck())
                {
                    _Lookup[key][i] = v;
                    break;
                }

                var removeRect = valueRect;
                removeRect.x = valueRect.xMax + 2;
                removeRect.width = kButtonWidth;
                if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButton))
                {
                    RemoveValue(key, i);
                    break;
                }
                position.y += 17f;
                i++;
            }
            if ((item.Value?.Count()??0) == 0)
            {
                var valueRect = position;
                valueRect.x = position.width / 2 + 15;
                valueRect.width = keyRect.width - kButtonWidth; 
                var removeRect = valueRect;
                removeRect.x = valueRect.xMax + 2;
                removeRect.width = kButtonWidth; 
                if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButton))
                {
                    RemoveItem(key);
                    break;
                }

                position.y += 17f;
            }
            //create + button for more in list
            var btnRect = position;
            btnRect.y -= 17f;
            btnRect.x -= kButtonWidth;
            if (item.Value?.Count() > 1)
                btnRect.x += position.width / 2 + 15 - kButtonWidth;
            btnRect.width = kButtonWidth;
            if (GUI.Button(btnRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
            {
                AddNewValue(key, i);
                break;
            }
        }
    }

    private void AddNewValue(TK key, int i)
    {
        var value = default(TV);
        try
        {
            _Lookup.Add(key, value);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void RemoveValue(TK key, int i)
    {
        _Lookup.RemoveAt(key, i);
    }

}

[CustomPropertyDrawer(typeof(StringGameObjectLookup))]
public class StringGameObjectLookupDrawer : LookupDrawer<string, GameObject> { }