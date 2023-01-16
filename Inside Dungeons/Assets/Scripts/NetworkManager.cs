using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maximoJugadores= 10;
    public static NetworkManager instancia;

    // Referencia al PhotonView asociado a este objeto
    private PhotonView photonView;

    // ID del room master anterior
    private int previousMasterClientId;

    


    public void Awake()
    {
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.ConnectUsingSettings();
        photonView = GetComponent<PhotonView>();
        PhotonNetwork.NetworkingClient.EventReceived += OnMasterClientSwitched;

        Debug.Log("RESULTADO:");
    }

    private void OnMasterClientSwitched(EventData eventData)
    {
        if (eventData.Code != (byte)EventCode.MasterClientSwitched)
        {
            return;
        }

        previousMasterClientId = (int)eventData.CustomData;
        int currentMasterClientId = PhotonNetwork.MasterClient.ActorNumber;

        if (previousMasterClientId != currentMasterClientId)
        {
            //ID más bajo =nuevo room master
            int newMasterClientId = int.MaxValue;
            foreach (PhotonPlayer player in PhotonNetwork.PlayerList)
            {
                if (player.ActorNumber < newMasterClientId)
                {
                    newMasterClientId = player.ActorNumber;
                }
            }
            PhotonPlayer newMasterClient = PhotonNetwork.CurrentRoom.GetPlayer(newMasterClientId);


            // Transferimos el control de la sala al nuevo room master
            PhotonNetwork.SetMasterClient(newMasterClient);
            ControladorMenu controladorMenu= new ControladorMenu();
            controladorMenu.ActualizarLobby();
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("conexionprincipal+");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene=true;
    }

    public void CrearRoom(string nombre)
    {
        RoomOptions optiones = new RoomOptions
        {
            MaxPlayers = (byte)maximoJugadores
        };
        PhotonNetwork.CreateRoom(nombre, optiones);
    }

    public void UnirseRoom(string nombre)
    {
        PhotonNetwork.JoinRoom(nombre);
    }

    [PunRPC]
    public void DejarRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LeaveRoom();
        }
        PhotonNetwork.Disconnect();
    }

    [PunRPC]
    public void CambiarEscena(string escena)
    {
        PhotonNetwork.LoadLevel(escena);
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Menu");
        Destroy(this.gameObject);
        Debug.Log("desconexion");
    }
     
    
}
