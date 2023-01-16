using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase
{
    public List<Item> items;

    void Start()
    {
        items = new List<Item>();
        //Aqui cargas los items de un archivo json o desde una base de datos
    }

    public Item GetItem(int id)
    {
        return items.Find(item => item.Id == id);
    }
}
