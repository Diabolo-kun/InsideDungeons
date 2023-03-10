using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ControladorMenu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    [Header("Pantallas")]
    [SerializeField] private GameObject Principal;
    [SerializeField] private GameObject Ajustes;
    [SerializeField] private GameObject Jugar;
    [SerializeField] private GameObject Lobby;


    [Header("Principal")]
    [SerializeField] private Button btnJugar;
    [SerializeField] private Button btnAjustes;
    [SerializeField] private Button btnSalir;

    //[Header("Ajustes")]

    [Header("Jugar")]
    [SerializeField] private Button btnCrear;
    [SerializeField] private Button btnRefresh;
    [SerializeField] private RectTransform ContenedorRoom;
    [SerializeField] private GameObject roomElementoPrefab;


    [Header("Lobby")]
    [SerializeField] private Button btnEmpezar;
    [SerializeField] private Button btnCancelar;
    [SerializeField] private Text txtListaJugadores;
    [SerializeField] private Text txtCodigoSala;

    private List<GameObject> roomElementos = new List<GameObject>();
    private List<RoomInfo> listaRooms = new List<RoomInfo>();

    void Start()
    {
        SetPantalla(Principal);
        btnJugar.interactable = false;
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen  = true;
        }
        InvokeRepeating("ActualizarLobby", 0f, 5f);
    }
    public override void OnConnectedToMaster(){btnJugar.interactable = true;}

    public void SetPantalla(GameObject screen)
    {
        Principal.SetActive(false);
        Ajustes.SetActive(false);
        Jugar.SetActive(false);
        Lobby.SetActive(false);

        screen.SetActive(true);
        if (screen == Jugar) { ActualizarNavegador(); };
    }

    public void OnNombreJugadorCambia(TMP_InputField inpJugadorNombre)
    {
        PhotonNetwork.NickName = inpJugadorNombre.text;
    } 
    public void OnJugarClicked()
    {
       if (!string.IsNullOrWhiteSpace(PhotonNetwork.NickName))
        {
            Debug.Log(PhotonNetwork.NickName);
            SetPantalla(Jugar);
        }
    }
    public void OnAjustesClicked(){SetPantalla(Ajustes);}
    public void OnSalirClicked(){Application.Quit();}
    public void OnCrearClicked(TMP_InputField NombreSala)
    {
        if (!string.IsNullOrWhiteSpace(NombreSala.text)) { 
            NetworkManager.instancia.CrearRoom(NombreSala.text);
            Debug.Log(NombreSala.text);
        }

    }
    public override void OnJoinedRoom()
    {
        SetPantalla(Lobby);
        txtCodigoSala.text = string.Format(PhotonNetwork.CurrentRoom.Name);
        photonView.RPC("ActualizarLobby", RpcTarget.All);

    }

    [PunRPC]
    public void ActualizarLobby()
    {
        btnEmpezar.interactable = PhotonNetwork.IsMasterClient;
        txtListaJugadores.text = "";
        foreach (PhotonPlayer p in PhotonNetwork.PlayerList)
        {
            txtListaJugadores.text += p.NickName + "\n";
        }
        
    }

    private GameObject CrearRoomBoton()
    {
        GameObject obj = Instantiate(roomElementoPrefab, ContenedorRoom.transform);
        roomElementos.Add(obj);
        return obj;
    }

    public void ActualizarNavegador()
    {
        foreach (GameObject b in roomElementos)
        {
            b.SetActive(false);
        }

        for (int x = 0; x < listaRooms.Count; x++)
        {
            if (listaRooms[x].PlayerCount > 0 && listaRooms[x].IsOpen)
            {
                GameObject boton = x >= roomElementos.Count ? CrearRoomBoton() : roomElementos[x];
                boton.SetActive(true);

                boton.transform.Find("NombreRoom").GetComponent<TextMeshProUGUI>().text = listaRooms[x].Name;
                boton.transform.Find("JugadoresRoom").GetComponent<TextMeshProUGUI>().text = listaRooms[x].PlayerCount + "/" + listaRooms[x].MaxPlayers;

                Button b1 = boton.GetComponent<Button>();
                string nombre = listaRooms[x].Name;
                b1.onClick.RemoveAllListeners();
                b1.onClick.AddListener(() => { OnUnirseRoomClicked(nombre); });
            }
        }
    }

    public void OnUnirseRoomClicked(string nombre)
    {
        NetworkManager.instancia.UnirseRoom(nombre);
        ActualizarLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        listaRooms=roomList;
    }

    public void OnRefreshClicked()
    {
        ActualizarNavegador();
    }

    public void OnAtrasClicked()
    {
        SetPantalla(Principal);
    }

    public void OnCancelarClicked()
    {
        NetworkManager.instancia.DejarRoom();
        SetPantalla(Principal);
    }

    public void OnEmpezarClcked()
    {
        if (PhotonNetwork.PlayerList.Length>=2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("Escenario");
        }
    }

}
