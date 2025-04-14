using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class allowedNumber : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int ID;
    ItemData Object;
    [SerializeField]
    private ItemDataBaseSO structuresData;
    [SerializeField]
    private TextMeshProUGUI text;
    void Start()
    {
        Object = structuresData.GetItemWithID(ID);
        text.SetText(structuresData.GetItemWithID(ID).allowedNumber.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(Object.allowedNumber.ToString());
    }
   
}
