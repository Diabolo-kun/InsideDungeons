using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggers : MonoBehaviour
{
    public RoomBehaviour RB;
    public bool inter;
    // Start is called before the first frame update

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RB.ChestKey();
            }
        }
    }

}
