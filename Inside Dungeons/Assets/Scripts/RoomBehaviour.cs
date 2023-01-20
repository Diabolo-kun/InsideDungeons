using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.IO;
using UnityEngine.UI;
using System;
using System.Linq;

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
    public bool getItem = false;


    void Start()
    {
        players = PhotonNetwork.PlayerList; //Inicializar la lista de jugadores
        PV= GetComponent<PhotonView>();
        PrintPlayers();
        ShufflePlayers();//Reordenar la lista de jugadores
        currentPlayer = players[currentPlayerIndex];//Asignar el primer jugador en la lista como el jugador actual
        blockedArea.SetActive(true);//Bloquear la zona al iniciar el juego
        InvokeRepeating("NextTurn",0f,30f);
    }
    private void Update()
    {
    }

    //Función para el siguiente turno
    public void NextTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("Teleport", RpcTarget.All);


        currentPlayerIndex++;//Incrementar el indice del jugador actual
        if (currentPlayerIndex == players.Length)//Si el indice del jugador actual es igual a la cantidad de jugadores en la lista, volverlo a 0
        {
            currentPlayerIndex = 0;
        }
        currentPlayer = players[currentPlayerIndex];//Asignar el siguiente jugador en la lista como el jugador actual
        PV.RPC("SyncPlayerList", RpcTarget.All, currentPlayerIndex, players);//Sincronizar la lista de jugadores y el jugador actual a través de la red

        GameObject playerGo = PhotonNetwork.GetPhotonView(currentPlayer.ActorNumber).gameObject;//comparar si el jugador esta vivo para saltar su turno
        currentplayerPV = playerGo.GetComponent<PhotonView>();
        Stat stat = playerGo.GetComponent<Stat>();
        if (stat != null && !stat.alive)
        {
            NextTurn();
            return;
        }
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
            Vector3 newPosition = new Vector3(UnityEngine.Random.Range(10f, 20f), 0f, UnityEngine.Random.Range(20f, 25f));
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
    // Mezcla aleatoriamente la lista de jugadores
    private void ShufflePlayers()
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
    //Función para generar un prefab de manera aleatoria
    [PunRPC]
    public void GeneratePrefab()
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
                Vector3 posicion = new Vector3(15, 0.7f, 2);
                Quaternion rotacion = Quaternion.Euler(0,0,0);
                GameObject randomPrefab = PhotonNetwork.Instantiate(Path.Combine("CanGenerate", prefabs[i].name), posicion, rotacion);
                break;
            }
        } 
    }
    
    public void CloseApp()
    {
        Application.Quit();
    }
}
