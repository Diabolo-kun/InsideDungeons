using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

public class Inventario : MonoBehaviour
{
    private bool inventarioActivo;

    public GameObject inventario;

    private int allSlots= 7;
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

    public bool alive=true;
    public Text infoStats;
    public int damage = 1;
    public int nivel = 1;
    public int sumatorio;

    private bool esc=false;
    public GameObject exitmenu;
    public Button btnexit;

    public bool win = false;



    void Start()
    {

        exitmenu.SetActive(false);
        btnexit.onClick.AddListener(Salir);

        inventario.SetActive(false);
        goldcount = 0;
        UpdateGoldTxt();
        slots = new Slot[allSlots];
        slotBody = new Slot[slotsBody];

        slots[0] = slotuno;
        slots[1] = slotdos;
        slots[2] = slottres;
        slots[3] = slotcuatro;
        slots[4] = slotcinco;
        slots[5] = slotseis;
        slots[6] = slotsiete;

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
        ContarStats();

    }
    void Update()
    {
        InventarioActivacion();
    }
    public void ContarStats()
    {
        infoStats.text = "";
        sumatorio = 0;
        if (!slotBody[0].empty)
        {
            sumatorio= sumatorio +slotBody[0].sum;
            infoStats.text = infoStats.text+ "Head= +" + slotBody[0].sum+"\n";
        }
        if (!slotBody[1].empty)
        {
            sumatorio = sumatorio + slotBody[1].sum;
            infoStats.text = infoStats.text+ "Armor= +" + slotBody[1].sum+"\n";
        }
        if (!slotBody[2].empty)
        {
            sumatorio = sumatorio + slotBody[2].sum;
            infoStats.text = infoStats.text+ "Pants= +" + slotBody[2].sum + "\n";
        }
        if (!slotBody[3].empty)
        {
            sumatorio = sumatorio + slotBody[3].sum;
            infoStats.text = infoStats.text + "Boots= +" + slotBody[3].sum + "\n";
        }
        if (!slotBody[4].empty)
        {
            sumatorio = sumatorio + slotBody[4].sum;
            infoStats.text = infoStats.text + "Left arm= +" + slotBody[4].sum + "\n";
        }
        if (!slotBody[5].empty)
        {
            sumatorio = sumatorio + slotBody[5].sum;
            infoStats.text = infoStats.text + "Rigt arm= +" + slotBody[5].sum + "\n";
        }
        if (slotBody[0].empty = false && slotBody[1].empty == false && slotBody[2].empty == false && slotBody[3].empty == false && slotBody[4].empty == false && slotBody[5].empty == false)
        {
            infoStats.text = "Nada equipado \n";
        }
        infoStats.text = "\n"+infoStats.text+"Nivel= "+nivel+" \n";
        damage = nivel + sumatorio+ goldcount/3;
        infoStats.text = infoStats.text + "Damage= "+damage+" \n";

        infoStats.text = infoStats.text + "Gold Points= " + goldcount/3 + " \n";

    }
    public void NivelUp()
    {
        nivel++;
    }
    void Salir()
    {
        if (PhotonNetwork.InRoom)
        {
            Application.Quit(); 
        }
    }
    void InventarioActivacion() {
        
            if (Input.GetKeyDown(KeyCode.E) && esc == false)
            {
                inventarioActivo = !inventarioActivo;
            }
            if (inventarioActivo == true)
            {
                inventario.SetActive(true);
                Cursor.visible = true;
            }
            if (inventarioActivo == false)
            {
                inventario.SetActive(false);
                Cursor.visible = false;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                esc = !esc;
            }
            if (esc == true)
            {
                exitmenu.SetActive(true);

            }
            if (esc == false)
            {
                exitmenu.SetActive(false);
            }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Item")
        {
            
            GameObject itemPickedUp = other.gameObject;
            Item item = itemPickedUp.GetComponent<Item>();
            Debug.Log("Get "+item);
            AddItem( item.Id, item.type, item.description, item.icon, item.price, item.sum);

            Destroy(itemPickedUp);
        }
        if (other.tag=="Enemy")
        {
            GameObject enemy = other.gameObject;
            int hp = enemy.GetComponent<Enemy>().Hp;
            if (damage > hp) { NivelUp(); Destroy(enemy); } else { alive=false; }
        }
    }
    public void EquipItem(Slot slot, int IdIn, string typeIn, string descripIn, Sprite iconIn, int priceIn, int sumIn)
    {
        for (int i = 0; i < slotBody.Length; i++)
        {
            if (slotBody[i].empty && slotBody[i].type == typeIn)
            {
                slotBody[i].EquipItem(slot.Id, slot.type, slot.description, slot.icon, slot.price, slot.sum);
                slot.UnequipItem();
                slotBody[i].GetComponent<Slot>().UpdateSlot();
                RemoveItem(slot);
                ContarStats();
                break;
            }
        }
    }
    public void DesequipItem(Slot slot, int IdIn, string typeIn, string descripIn, Sprite iconIn, int priceIn, int sumIn)
    {

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].empty)
            {
                slots[i].EquipItem(slot.Id, slot.type, slot.description, slot.icon, slot.price, slot.sum);
                slot.UnequipItem();
                slots[i].GetComponent<Slot>().UpdateSlot();
                RemoveItem(slot);
                ContarStats();

                break;
            }
        }
    }
    public void AddItem(int IdIn, string typeIn, string descripIn, Sprite iconIn, int priceIn, int sumIn)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].empty)
            {
                slots[i].EquipItem(IdIn, typeIn,  descripIn,  iconIn,  priceIn,  sumIn);
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
        ContarStats();
    }
    public void UpdateGoldTxt()
    {
        goldtxt.text = goldcount.ToString();
    }

}
