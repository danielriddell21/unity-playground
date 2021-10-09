using Cinemachine;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject player1CameraPrefab;
    public GameObject player2CameraPrefab;

    [Space]
    public GameObject InterfacePrefab;

    public static bool IsMultiplayer;
    public static bool IsHosting;
    public static string MultiplayerIpAddress;
    //public static int MultiplayerPort;

    public static bool IsPaused;
    public static GameObject PauseMenu;

    public static Character1Controller Player1;
    public static Character2Controller Player2;
    public static CinemachineVirtualCamera Player1Camera;
    public static CinemachineFreeLook Player2Camera;

    void Awake()
    {
        if (IsMultiplayer)
        {
            if (IsHosting)
            {
                var networkTransport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                networkTransport.MaxConnections = 2;
                //unetTransport.ConnectPort

                NetworkManager.Singleton.StartHost();
            }
            else
            {
                if (!string.IsNullOrEmpty(MultiplayerIpAddress))
                {
                    var unetTransport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                    unetTransport.ConnectAddress = MultiplayerIpAddress;
                }

                NetworkManager.Singleton.StartClient();
            }
        }
    }

    void Start()
    {
        Time.timeScale = 1f;

        if (!IsMultiplayer)
        {
            Instantiate(player1Prefab);
            Instantiate(player2Prefab);
        }
        else
        {
            if (IsHosting)
            {
                var player = Instantiate(player1Prefab, Vector3.zero, Quaternion.identity);

                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);

                NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer2_Server;
            }
        }

        Instantiate(InterfacePrefab);

        PauseMenu = Instantiate(Resources.Load("Prefabs/Menus/PauseMenu") as GameObject);
        PauseMenu.SetActive(false);
    }

    void Update()
    {
        if (!NetworkManager.Singleton.IsListening && IsMultiplayer)
        {
            SwitchScene("MainMenu");
        }
    }

    private void SpawnPlayer2_Server(ulong clientId)
    {
        var player = Instantiate(player2Prefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    public static void SwitchScene(string sceneName)
    {
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkSceneManager.SwitchScene(sceneName);
        }
        else
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
