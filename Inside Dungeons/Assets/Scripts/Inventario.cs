using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventario : MonoBehaviour
{
    private bool inventarioActivo;

    public GameObject inventario;

    private int allSlots= 7;

    private int enabledSlots;

    private GameObject[] slot;

    public GameObject slotHolder;
    //public GameObject slotHolderExtra;
    public GameObject goldtxt;

    public GameObject SlotHolderBody;
    public GameObject slotCabeza;
    public GameObject slotTorso;
    public GameObject slotPantalon;
    public GameObject slotZapatos;
    public GameObject[] slotsManos;




    void Start()
    {
        inventario.SetActive(false);

        slot = new GameObject[allSlots];
        IsEmty();

    }
    void IsEmty()
    {
        for (int i = 0; i < allSlots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i).gameObject;
            if (slot[i].GetComponent<Slot>().item == null)
            {
                slot[i].GetComponent<Slot>().empty = true;
            }
        }

        slotsManos = new GameObject[2];

        if (slotCabeza.GetComponent<Slot>().item == null)
        {
            slotCabeza.GetComponent<Slot>().empty = true;
        }
        if (slotTorso.GetComponent<Slot>().item == null)
        {
            slotTorso.GetComponent<Slot>().empty = true;
        }
        if (slotPantalon.GetComponent<Slot>().item == null)
        {
            slotPantalon.GetComponent<Slot>().empty = true;
        }
        if (slotZapatos.GetComponent<Slot>().item == null)
        {
            slotZapatos.GetComponent<Slot>().empty = true;
        }
        if (slotsManos[0].GetComponent<Slot>().item == null)
        {
            slotsManos[0].GetComponent<Slot>().empty = true;
        }
        if (slotsManos[1].GetComponent<Slot>().item == null)
        {
            slotsManos[1].GetComponent<Slot>().empty = true;
        }
    }

    void Update()
    {
        InventarioActivacion();
    }

    void InventarioActivacion() {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventarioActivo = !inventarioActivo;
        }
        if (inventarioActivo == true)
        {
            inventario.SetActive(true);
        }
        if (inventarioActivo == false)
        {
            inventario.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Item")
        {
            GameObject itemPickedUp = other.gameObject;
            Item item = itemPickedUp.GetComponent<Item>();
            AddItem(itemPickedUp,item.Id,item.type,item.description,item.icon,item.price,item.sum);
        }
    }
    public void AddItem(GameObject itemObject, int itemId, string itemType, string itemDescription, Sprite itemSprite, int itemPrice, int itemSum) 
    {
        for (int i = 0; i < allSlots; i++)
        {
            if (slot[i].GetComponent<Slot>().empty)
            {
                itemObject.GetComponent<Item>().pickedUp = true;
                slot[i].GetComponent<Slot>().item = itemObject;
                slot[i].GetComponent<Slot>().Id = itemId;
                slot[i].GetComponent<Slot>().type = itemType;
                slot[i].GetComponent<Slot>().description = itemDescription;
                slot[i].GetComponent<Slot>().icon = itemSprite;
                slot[i].GetComponent<Slot>().price = itemPrice;
                slot[i].GetComponent<Slot>().price = itemSum;

                itemObject.transform.parent= slot[i].transform;
                itemObject.SetActive(false);

                slot[i].GetComponent<Slot>().UpdateSlot();

                slot[i].GetComponent<Slot>().empty= false;
            }
            return;
        }
    }

}
