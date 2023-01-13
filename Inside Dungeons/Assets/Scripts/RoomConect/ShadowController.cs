using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowController : MonoBehaviour
{
    Rigidbody rb;
    PhotonView PV;

    private NetworkManager net;
    private bool escActive;
    public GameObject exitmenu;
    public Button btnexit;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        exitmenu.SetActive(false);
        btnexit.onClick.AddListener(Salir);
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            BasicBehaviour script1 = PV.GetComponent<BasicBehaviour>();
            script1.enabled = false;
            MoveBehaviour script2 = PV.GetComponent<MoveBehaviour>();
            script2.enabled = false;
            AimBehaviourBasic script3 = PV.GetComponent<AimBehaviourBasic>();
            script3.enabled = false;

        }
    }
    void Update()
    {
        if (!PV.IsMine) return;
        ESCActivacion();
    }
    void ESCActivacion()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escActive = !escActive;
        }
        if (escActive == true)
        {
            exitmenu.SetActive(true);
        }
        if (escActive == false)
        {
            exitmenu.SetActive(false);
        }
    }
    void Salir()
    {
        net.DejarRoom();
    }
    
}
