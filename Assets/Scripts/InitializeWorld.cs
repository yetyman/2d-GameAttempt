using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitializeWorld : MonoBehaviour
{
    [SerializeField] public List<GameObject> Layers = new List<GameObject>();
    // Start is called before the first frame update
    public Canvas canvas;

    public IEnumerable<TMP_Text> _UICointTracker;
    //lazy loaded
    public IEnumerable<TMP_Text> UICoinTracker { get { 
            _UICointTracker = _UICointTracker ?? UIReferenceObj.Instance?.BoundUIElements["CoinTracker", null]?.Select(x=>x.GetComponent<TMP_Text>());
            return _UICointTracker;
    }}
    
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

        Inventory.AddEvent.AddListener(
            m => {
                if (m.Equals("Coin") && UICoinTracker != null)
                    foreach(var UITxt in UICoinTracker)
                        UITxt.text = inventory.Items["Coin"].ToString();
            });
    }

}
