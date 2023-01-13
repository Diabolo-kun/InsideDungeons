using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    private bool inventarioActivo;

    public GameObject inventario;

    private int allSlots= 7;

    private int enabledSlots;

    private Slot[] slots;
    public GameObject slotHolder;

    private int slotsBody = 6;
    private Slot[] slotBody;
    public GameObject slotHolderBody;


    public int goldcount;
    public Text goldtxt;

    public Slot slotCabeza;
    public Slot slotTorso;
    public Slot slotPantalon;
    public Slot slotZapatos;
    public Slot slotManoIz;
    public Slot slotManoDe;

    public Slot slotuno;
    public Slot slotdos;
    public Slot slottres;
    public Slot slotcuatro;
    public Slot slotcinco;
    public Slot slotseis;
    public Slot slotsiete;




    void Start()
    {
        inventario.SetActive(false);
        goldcount = 0;
        UpdateGoldTxt();
        slots = new Slot[allSlots];
        slotBody = new Slot[slotsBody];
        startInventory();

    }
    void startInventory()
    {
        slots[0]= slotuno;
        slots[1]= slotdos;
        slots[2]= slottres;
        slots[3]= slotcuatro;
        slots[4]= slotcinco;
        slots[5]= slotseis;
        slots[6]= slotsiete;

        slotBody[0] = slotCabeza;
        slotBody[1] = slotTorso;
        slotBody[2] = slotPantalon;
        slotBody[3] = slotZapatos;
        slotBody[4] = slotManoIz;
        slotBody[5] = slotManoDe;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].empty = true;
            
}
        for (int i = 0; i < slotBody.Length; i++)
        {
            slotBody[i].empty = true;
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
            AddItem(item);
            Destroy(itemPickedUp);
        }
    }

    public void EquipItem(Item item, Slot slot)
    {
        for (int i = 0; i < slotBody.Length; i++)
        {
            if (slotBody[i].empty && slotBody[i].type == item.type)
            {
                slotBody[i].EquipItem(item);
                slot.UnequipItem();
                slotBody[i].GetComponent<Slot>().UpdateSlot();
                RemoveItem(slot);
                break;
            }
        }
    }
    public void DesequipItem(Item item, Slot slot)
    {

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].empty)
            {
                slots[i].EquipItem(item);
                slot.UnequipItem();
                slots[i].GetComponent<Slot>().UpdateSlot();
                RemoveItem(slot);
                break;
            }
        }
    }
    public void AddItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].empty)
            {
                item.GetComponent<Item>().pickedUp = true;
                slots[i].EquipItem(item);
                slots[i].GetComponent<Slot>().UpdateSlot();
                break;
            }
        }
    }

    public void RemoveItem(Slot slot)
    {
        slot.UnequipItem();
    }

    public void SumGold(int sum)
    {
        goldcount = goldcount + sum;
        UpdateGoldTxt();
    }
    public void UpdateGoldTxt()
    {
        goldtxt.text = goldcount.ToString();
    }

}
