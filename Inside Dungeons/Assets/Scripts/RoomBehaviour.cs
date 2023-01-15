using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.IO;
using UnityEngine.UI;

public class RoomBehaviour : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    public Inventario inv;

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

    void Start()
    {
        inv= GetComponent<Inventario>();
        players = PhotonNetwork.PlayerList; //Inicializar la lista de jugadores
        
        PrintPlayers();
        ShufflePlayers();//Reordenar la lista de jugadores
        currentPlayer = players[currentPlayerIndex];//Asignar el primer jugador en la lista como el jugador actual
        blockedArea.SetActive(true);//Bloquear la zona al iniciar el juego
        InvokeRepeating("NextTurn", 0f, 90f);
    }
    private void Awake()
    {
        PV= GetComponent<PhotonView>();
    }
    private void Update()
    {
        CheckWinCondition();
    }
    //Función para el siguiente turno
    public void NextTurn()
    {
        blockedArea.SetActive(true);//Desbloquear la zona para el jugador actual
        currentPlayerIndex++;//Incrementar el indice del jugador actual
        if (currentPlayerIndex == players.Length)//Si el indice del jugador actual es igual a la cantidad de jugadores en la lista, volverlo a 0
        {
            currentPlayerIndex = 0;
        }
        currentPlayer = players[currentPlayerIndex];//Asignar el siguiente jugador en la lista como el jugador actual
        PV.RPC("SyncPlayerList", RpcTarget.All, currentPlayerIndex);//Sincronizar la lista de jugadores y el jugador actual a través de la red

        GameObject playerGo = PhotonNetwork.GetPhotonView(currentPlayer.ActorNumber).gameObject;//comparar si el jugador esta vivo para saltar su turno
        Inventario inv = playerGo.GetComponent<Inventario>();
        if (inv != null && !inv.alive)
        {
            NextTurn();
            return;
        }

        PV.RPC("UnblockArea", currentPlayer);//Bloquear la zona para el siguiente jugador

        cartelTurno.text="Turno de :" + currentPlayer.NickName;
        hasInteract= false;
       
    }
    [PunRPC]
    public void UnblockArea()
    {
        blockedArea.SetActive(false);
    }
    [PunRPC]
    private void SyncPlayerList(int currentPlayer)
    {
        this.currentPlayerIndex = currentPlayer;
        this.currentPlayer = players[currentPlayer];
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
            Invoke("GeneratePrefab", 2f);
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
    public void CheckWinCondition()
    {
        if (players == null) return;
        int alivePlayers = 0;
        int winnerPlayer = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i]==null) continue;

            GameObject playerGo = PhotonNetwork.GetPhotonView(players[i].ActorNumber).gameObject;
            if (playerGo == null) continue;
            Inventario inv = playerGo.GetComponent<Inventario>();
            if (inv == null) continue;
            if (inv.alive)
            {
                alivePlayers++;
                winnerPlayer = i;
            }
            if (inv.damage >= 15)
            {
                winnerPlayer = i;
                Win(winnerPlayer);
                return;
            }
            
            
        }

        if (alivePlayers == 1)
        {
            Win(winnerPlayer);
        }
    }

    public void Win(int winnerPlayer)
    {
        PhotonPlayer winnerplayer = players[winnerPlayer];
        winnerNameText.text = "Ganador: " + winnerplayer.NickName;
        winPanel.SetActive(true);
        inv.win= true;
    }
    public void CloseApp()
    {
        Application.Quit();
    }
}
