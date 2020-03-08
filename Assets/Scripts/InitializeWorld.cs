using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeWorld : MonoBehaviour
{
    [SerializeField] public List<GameObject> Layers = new List<GameObject>();
    // Start is called before the first frame update
    public Canvas canvas;
    public Text canvasText;
    [SerializeField]
    public Inventory inventory;

    void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (var layer in Layers)
            Instantiate(layer, transform);

        Inventory.AddEvent.AddListener(m => { if (m.Equals("Coin")) canvasText.text = inventory.Items["Coin"].ToString(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
