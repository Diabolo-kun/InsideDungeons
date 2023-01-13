using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV=GetComponent<PhotonView>();
    }

    void Start()
    {        
            if (PV.IsMine)
            {
                InstantiateCharacter();
            }
    }

    void InstantiateCharacter()
    {
        Vector3 randomPosition = new Vector3(Random.Range(10, 20), 0, Random.Range(20, 25));
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        GameObject character = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefab", "shadow"), randomPosition, randomRotation);
    }
}
