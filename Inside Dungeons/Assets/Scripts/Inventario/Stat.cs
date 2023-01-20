using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public int nivel;
    public int damage;
    public bool alive;


    public void NivelUp()
    {
        nivel++;
    }
}
