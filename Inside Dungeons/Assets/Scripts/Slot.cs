using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Slot : MonoBehaviour, IPointerClickHandler
{

    public GameObject item;
    public int Id;
    public string type;
    public string description;
    public Sprite icon;
    public int price;
    public int sum;

    public bool empty;

    public Transform SlotIconGameObject;

    public Button btn;
    public Button btnUse;
    public Button btnSale;
    public Button btnDontUse;

    private void Start()
    {
        btn=GetComponent<Button>();
        SlotIconGameObject = transform.GetChild(0);
        if (transform.Find("use") != null && transform.Find("null") != null)
        {
            btnUse.onClick.AddListener(usar);
            btnSale.onClick.AddListener(vender);
        }
        if (transform.Find("DontUse") != null)
        {
            btnDontUse.onClick.AddListener(noUsar);
        }
    }

    public void UpdateSlot()
    {
        SlotIconGameObject.GetComponent<Image>().sprite = icon;
    }

    public void UseItem()
    {
        item.GetComponent<Item>().ItemUsage();
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!empty)
        {
            if (transform.Find("use") != null && transform.Find("null") != null)
            {
                btnUse.gameObject.SetActive(true);
                btnSale.gameObject.SetActive(true);
                btnDontUse = null;
            }
            if (transform.Find("DontUse") != null)
            {
                btnDontUse.gameObject.SetActive(true);
                btnUse = null;
                btnSale = null;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        btnUse.gameObject.SetActive(false);
    }

    void usar() { }
    void vender() { }
    void noUsar() { }
}
