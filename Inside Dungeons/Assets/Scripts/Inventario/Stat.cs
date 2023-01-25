using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public PhotonView PV;
    public int nivel=0;
    public int damage=0;
    public bool alive=true;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (GetComponent<PhotonView>() != null) PV = GetComponent<PhotonView>();
        nivel = 1;
        damage= 1;
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (PV.IsMine && PV != null && PhotonNetwork.IsConnected) {
            Debug.Log(PV);
            PV.RPC("SyncStats", RpcTarget.All, nivel, damage, alive); 
        }
    }
    [PunRPC]
    public void SyncStats(int nivel,int damage, bool alive)
    {
        this.nivel = nivel;
        this.damage =damage;
        this.alive = alive;
    }
    public void NivelUp()
    {
        nivel++;
    }
}
