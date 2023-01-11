using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int Id;
    public string type;
    public string description;
    public Sprite icon;
    public int price;
    public int sum;

    [HideInInspector]
    public bool pickedUp;

    [HideInInspector]
    public bool equiped;

    [HideInInspector]
    public GameObject weapon;


    void Update()
    {
        if (equiped)
        {

        }
    }

    public void ItemUsage()
    {
        if (type=="Weapons") {equiped= true; }
    }
}
