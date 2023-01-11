using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class RoomBehaviour : MonoBehaviour
{
    PhotonView photonView;

    List<Player> players;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        players = PhotonNetwork.CurrentRoom.Players.Values.ToList();

        PrintPlayers();

        ShufflePlayers();

        PrintPlayers();


    }

    private void PrintPlayers()
    {
        Debug.Log("Lista de jugadores:");
        foreach (Player player in players)
        {
            Debug.Log(player.NickName);
        }
    }

    // Mezcla aleatoriamente la lista de jugadores
    private void ShufflePlayers()
    {
        System.Random rng = new System.Random();
        int n = players.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Player value = players[k];
            players[k] = players[n];
            players[n] = value;
        }
    }

}
