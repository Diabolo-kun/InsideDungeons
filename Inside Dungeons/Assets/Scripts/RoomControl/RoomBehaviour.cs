using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.IO;
using UnityEngine.UI;
using System;

public class RoomBehaviour : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    private PhotonView currentplayerPV;

    public PhotonPlayer[] players; //Lista de jugadores
    private int currentPlayerIndex = 0; //Indice del jugador actual
    public PhotonPlayer currentPlayer; //Jugador actual
    public int currentPlayerViewID;
    
    public GameObject blockedArea; //Zona bloqueada

    public GameObject[] prefabs; //Array de prefabs
    public int[] prefabSpawnChance; //Porcentaje de salida de cada prefab

    public Text cartelTurno;

    public bool hasInteract = false;

    public Button closeAppButton;
    public GameObject winPanel;
    public Text winnerNameText;

    public bool isBlocked;


    void Start()
    {
        players = PhotonNetwork.PlayerList; //Inicializar la lista de jugadores
        PV= GetComponent<PhotonView>();
        PrintPlayers();
        ShufflePlayers();//Reordenar la lista de jugadores
        currentPlayer = players[currentPlayerIndex];//Asignar el primer jugador en la lista como el jugador actual
        blockedArea.SetActive(true);//Bloquear la zona al iniciar el juego
        //InvokeRepeating("NextTurn",0f,30f);
        Next();
        InvokeRepeating("CheckPlayerConnection", 0f, 10f);
    }
    public void CheckPlayerConnection()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PhotonPlayer[] connectedPlayers = PhotonNetwork.PlayerList;
        bool playerDisconnected = true;
        for (int i = 0; i < connectedPlayers.Length; i++)
        {
            if (connectedPlayers[i] == currentPlayer)
            {
                playerDisconnected = false;
                break;
            }
        }
        if (playerDisconnected && currentPlayer.ActorNumber == currentPlayer.ActorNumber)
        {
            LlamadeNext();
        }
    }
    [PunRPC]
    public void Next()
    {
        Debug.Log("NextTurn:");
        Invoke("LlamadeNext", 5f);
    }
    [PunRPC]
    public void LlamadeNext()
    {

        PV.RPC("NextTurn", RpcTarget.All);
    }

    //Función para el siguiente turno
    [PunRPC]
    public void NextTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        CheckWin();
        PV.RPC("Teleport", RpcTarget.All);
        currentPlayerIndex++;//Incrementar el indice del jugador actual
        if (currentPlayerIndex >= players.Length)//Si el indice del jugador actual es igual a la cantidad de jugadores en la lista, volverlo a 0
        {
            currentPlayerIndex = 0;
        }
        currentPlayer = players[currentPlayerIndex];//Asignar el siguiente jugador en la lista como el jugador actual
        
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerObjects.Length; i++)
        {
            PhotonView pv = playerObjects[i].GetComponent<PhotonView>();
            if (pv.Owner.ActorNumber == currentPlayer.ActorNumber)
            {
                //encontrado el objeto correcto
                Stat stat = playerObjects[i].GetComponent<Stat>();
                Debug.Log(playerObjects[i] + " // "+stat.nivel + " // "+stat.damage + " // "+stat.alive);
                if (stat != null && !stat.alive)
                {
                    PV.RPC("NextTurn", RpcTarget.All);
                    return;
                }
            }
        }
        
        PV.RPC("SyncPlayerList", RpcTarget.All, currentPlayerIndex, players);//Sincronizar la lista de jugadores y el jugador actual a través de la red
        PV.RPC("BlockedArea", RpcTarget.All, currentPlayer.ActorNumber);//Bloquear la zona para el siguiente jugador

        PV.RPC("UpdateTurnText", RpcTarget.All, currentPlayer.NickName);
    }
    [PunRPC]
    public void BlockedArea(int player)
    {
        
        if (PhotonNetwork.LocalPlayer.ActorNumber == player)
        {
            blockedArea.SetActive(false);
        }
        else
        {
            blockedArea.SetActive(true);
        }

    }
    [PunRPC]
    void UpdateTurnText(string playerName)
    {
        cartelTurno.text = "Turno de :" + playerName;
        hasInteract = false;
    }
    [PunRPC]
    private void Teleport()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playerObjects)
        {
            Vector3 newPosition = new Vector3(UnityEngine.Random.Range(-7, 7), 5.1f, UnityEngine.Random.Range(19, 21));
            player.transform.position = newPosition;
        }

    }
    [PunRPC]
    private void SyncPlayerList(int currentPlayerIndex, PhotonPlayer[] players)
    {
        this.currentPlayerIndex = currentPlayerIndex;
        this.players = players;
        this.currentPlayer = players[currentPlayerIndex];
    }
    private void PrintPlayers()
    {
        Debug.Log("Lista de jugadores:");
        for (int i = 0; i < players.Length; i++)
        {
            PhotonPlayer player = players[i];
            Debug.Log(player);
        }
    }
    private void ShufflePlayers() //Funcion para mezclar la lista de jugadores
    {
        for (int i = 0; i < players.Length; i++)
        {
            PhotonPlayer temp = players[i];
            int randomIndex = UnityEngine.Random.Range(i, players.Length);
            players[i] = players[randomIndex];
            players[randomIndex] = temp;
        }
    }
    public void ChestKey()
    {
        if (!hasInteract)
        {
            Debug.Log("Interact");
            hasInteract = true;
            Invoke("GeneratePrefab", 1f);
        }
    }
    [PunRPC]
    public void GeneratePrefab() //Función para generar un prefab de manera aleatoria
    {
        int totalChance = 0;
        for (int i = 0; i < prefabSpawnChance.Length; i++)
        {
            totalChance += prefabSpawnChance[i];
        }
        int randomChance = UnityEngine.Random.Range(0, totalChance);
        int currentChance = 0;
        
        for (int i = 0; i < prefabSpawnChance.Length; i++)
        {
            currentChance += prefabSpawnChance[i];
            if (randomChance < currentChance)
            {
                if (prefabs[i].tag =="Enemy")
                {
                    PV.RPC("Teleport", RpcTarget.All);
                }
                Vector3 posicion = new Vector3(0, 0.7f, 12);
                Quaternion rotacion = Quaternion.Euler(0,0,0);
                GameObject randomPrefab = PhotonNetwork.Instantiate(Path.Combine("CanGenerate", prefabs[i].name), posicion, rotacion);
                break;
            }
        } 
    }
    [PunRPC]
    public void ReciveOrderCheckWin()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        CheckWin();
    }

    public void CheckWin()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        int alivePlayers = 0;
        PhotonView winner = null;
        foreach (GameObject player in playerList)
        {
            Stat stat = player.GetComponent<Stat>();
            if (stat != null)
            {
                if (stat.damage >= 15) 
                {
                    winner = player.GetComponent<PhotonView>();
                    PV.RPC("Win", RpcTarget.All, winner.ViewID);
                    return;
                }
                if (stat.alive)
                {
                    alivePlayers++;
                    winner = player.GetComponent<PhotonView>();
                }
            }
            else
            {
                Debug.Log("No stat found");
            }
        }
        if (PhotonNetwork.PlayerList.Length == 1 || alivePlayers == 1)
        {
            PV.RPC("Win", RpcTarget.All, winner.ViewID);
        }
    }
    [PunRPC]
    public void Win(int winnerPV)
    {
        PhotonView winner = PhotonView.Find(winnerPV);
        if (winner != null)
        {
            Debug.Log(winner.Owner.NickName + " wins");
            winnerNameText.text = "Ha ganado: "+winner.Owner.NickName;
            winPanel.SetActive(true);
            winner.GetComponent<Inventario>().CallWin();
        }
        else
        {
            Debug.LogError("No hay jugador con ese Photonview");
        }
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
