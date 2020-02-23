using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeWorld : MonoBehaviour
{
    [SerializeField] public List<GameObject> Layers = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in TextHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (var layer in Layers)
            Instantiate(layer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
