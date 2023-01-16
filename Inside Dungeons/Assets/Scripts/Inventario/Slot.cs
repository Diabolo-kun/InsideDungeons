using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Slot : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{

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

    public int SyncSlot = -1; // nueva variable para sincronizar el slot en el juego multijugador
    public PhotonView PV;

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
    public void EquipItem(int IdIn, string typeIn, string descripIn,Sprite iconIn, int priceIn, int sumIn)
    {
        Id = IdIn;
        type = typeIn;
        description = descripIn;
        icon = iconIn;
        price = priceIn;
        sum = sumIn;
        empty = false;

        if (PV.IsMine)
        {
            PV.RPC("RPC_EquipItem", RpcTarget.All, IdIn, typeIn, descripIn, iconIn, priceIn, sumIn);
        }
    }
    [PunRPC]
    private void RPC_EquipItem(int IdIn, string typeIn, string descripIn, Sprite iconIn, int priceIn, int sumIn)
    {
        Id = IdIn;
        type = typeIn;
        description = descripIn;
        icon = iconIn;
        price = priceIn;
        sum = sumIn;
        empty = false;
        UpdateSlot();
    }
    public void UnequipItem()
    {
        empty = true;
        Id = 0;
        if (!body)
        {
            type = null;
        }
        description = null;
        SlotIconGameObject.GetComponent<Image>().sprite = Background;
        price = 0;
        sum = 0;
        if (PV.IsMine)
        {
            PV.RPC("RPC_UnequipItem", RpcTarget.All);
        }
    }
    [PunRPC]
    public void RPC_UnequipItem()
    {
        empty = true;
        Id = 0;
        if (!body)
        {
            type = null;
        }
        description = null;
        SlotIconGameObject.GetComponent<Image>().sprite = Background;
        price = 0;
        sum = 0;
    }



    void usar() 
    { 
        Slot slot= GetComponentInParent<Slot>();

        slot.inventario.EquipItem(slot, slot.Id,slot.type, slot.description, slot.icon, slot.price,slot.sum);
        btnUse.gameObject.SetActive(false);
        btnSale.gameObject.SetActive(false);
    }
    void vender() {

        Slot slot = GetComponentInParent<Slot>();
        int price= slot.price;
        slot.inventario.SumGold(price);
        slot.UnequipItem();
        btnUse.gameObject.SetActive(false);
        btnSale.gameObject.SetActive(false);
    }
    void noUsar() 
    {
        Slot slot = GetComponentInParent<Slot>();
        slot.inventario.DesequipItem(slot, slot.Id, slot.type, slot.description, slot.icon, slot.price, slot.sum);
        btnDontUse.gameObject.SetActive(false);
    }
}
