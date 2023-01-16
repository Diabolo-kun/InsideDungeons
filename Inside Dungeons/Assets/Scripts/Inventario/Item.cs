using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int Id;
    public string type;
    public string description;
    public string icon;
    public int price;
    public int sum;

    [HideInInspector]
    public bool pickedUp;

    
}
