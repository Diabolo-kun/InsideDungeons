using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowController : MonoBehaviour
{
    Rigidbody rb;
    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!PV.IsMine) return;

    }
    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            BasicBehaviour script1 = PV.GetComponent<BasicBehaviour>();
            script1.enabled = false;
            MoveBehaviour script2 = PV.GetComponent<MoveBehaviour>();
            script2.enabled = false;
            AimBehaviourBasic script3 = PV.GetComponent<AimBehaviourBasic>();
            script3.enabled = false;

            //Destroy(rb);
        }
    }
}
