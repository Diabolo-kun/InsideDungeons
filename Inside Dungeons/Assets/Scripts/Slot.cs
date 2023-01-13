using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Slot : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{

    public Item item;
    public int Id;
    public string type;
    public string description;
    public Sprite icon;
    public int price;
    public int sum;

    public bool empty= true;

    public Transform SlotIconGameObject;
    public Sprite Background;

    public Button btn;
    public Button btnUse;
    public Button btnSale;
    public Button btnDontUse;

    public Inventario inventario;
    public bool body;

    private void Start()
    {
        btn=GetComponent<Button>();
        SlotIconGameObject = transform.GetChild(0);
        if (!body)
        {
            Destroy(btnDontUse);
            btnUse.onClick.AddListener(usar);
            btnSale.onClick.AddListener(vender);
        }
        if (body)
        {
            Destroy(btnUse);
            Destroy(btnSale);
            btnDontUse.onClick.AddListener(noUsar);

        }
    }
    public void UpdateSlot()
    {
        SlotIconGameObject.GetComponent<Image>().sprite = icon;
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!empty)
        {
            if (!body)
            {
                btnUse.gameObject.SetActive(true);
                btnSale.gameObject.SetActive(true);
            }
            if (body)
            {
                btnDontUse.gameObject.SetActive(true);
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!body)
        {
            btnUse.gameObject.SetActive(false);
            btnSale.gameObject.SetActive(false);
        }
        if (body)
        {
            btnDontUse.gameObject.SetActive(false);
        }
    }
    public void EquipItem(Item item)
    {
        this.item = item;
        this.Id= item.Id;
        this.type = item.type;
        this.description = item.description;
        this.icon = item.icon;
        this.price = item.price;
        this.sum = item.sum;
        empty= false;
    }
    public void UnequipItem()
    {
        item= null;
        empty= true;

        Id = 0;
        if (!body)
        {
            type = null;
        }
        description = null;
        icon = Background;
        price = 0;
        sum = 0;
        UpdateSlot();
    }
    

    void usar() 
    { 
        Slot slot= GetComponentInParent<Slot>();
        Item itemequip= slot.item;
        slot.inventario.EquipItem(itemequip, slot);
        btnUse.gameObject.SetActive(false);
        btnSale.gameObject.SetActive(false);
    }
    void vender() {

        Slot slot = GetComponentInParent<Slot>();
        int sum= slot.sum;
        slot.inventario.SumGold(sum);
        slot.inventario.RemoveItem(slot);
        btnUse.gameObject.SetActive(false);
        btnSale.gameObject.SetActive(false);
    }
    void noUsar() 
    {
        Slot slot = GetComponentInParent<Slot>();
        Item item = slot.item;
        slot.inventario.DesequipItem(item, slot);
        btnDontUse.gameObject.SetActive(false);
    }
}
