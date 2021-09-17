using Cinemachine;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Transports.UNET;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject player1CameraPrefab;
    public GameObject player2CameraPrefab;

    public static Character1Controller Player1Component;
    public static Character2Controller Player2Component;
    public static CinemachineVirtualCamera Player1CameraComponent;
    public static CinemachineFreeLook Player2CameraComponent;

    public static bool IsMultiplayer;
    public static bool IsHosting;
    public static string MultiplayerIpAddress;
    //public static int MultiplayerPort;

    [Space]
    public GameObject PauseMenu;
    public static bool IsPaused;

    private GameObject Player1;
    private GameObject Player2;
    private GameObject Player1Camera;
    private GameObject Player2Camera;

    private GameObject _mainCamera;

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

        //player1Prefab = Resources.Load("Prefabs/Players/Player1") as GameObject;
        //player2Prefab = Resources.Load("Prefabs/Players/Player2") as GameObject;
        //player1CameraPrefab = Resources.Load("Prefabs/Cameras/FirstPersonCamera") as GameObject;
        //player2CameraPrefab = Resources.Load("Prefabs/Cameras/ThirdPersonCamera") as GameObject;

        if (!IsMultiplayer)
        {
            Player1 = Instantiate(player1Prefab);
            Player1Camera = Instantiate(player1CameraPrefab);

            SetupPlayer1Components();
            SetupPlayer1();

            Player2 = Instantiate(player2Prefab);
            Player2Camera = Instantiate(player2CameraPrefab);

            SetupPlayer2Components();
            SetupPlayer2();
        }
        else
        {
            if (IsHosting)
            {
                Player1 = Instantiate(player1Prefab, Vector3.zero, Quaternion.identity);

                var localPlayerId = NetworkManager.Singleton.LocalClientId;
                Player1.GetComponent<NetworkObject>().SpawnAsPlayerObject(localPlayerId);
                Player1Camera = Instantiate(player1CameraPrefab);

                SetupPlayer1Components();
                SetupPlayer1();

                NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer2_Server;
            }
            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer2_Client;
            }
        }
        _mainCamera = GameObject.Find("MainCamera");

        PauseMenu = Instantiate(PauseMenu);
        PauseMenu.SetActive(false);
    }

    void Update()
    {
        if (!NetworkManager.Singleton.IsListening && IsMultiplayer)
        {
            SwitchScene("MainMenu");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                PauseMenu.SetActive(false);
                _mainCamera.GetComponent<CinemachineBrain>().enabled = true;
            }
            else
            {
                PauseMenu.SetActive(true);
                _mainCamera.GetComponent<CinemachineBrain>().enabled = false;
            }

            if (!IsMultiplayer)
            {
                Time.timeScale = IsPaused ? 1f : 0f;
            }

            IsPaused = !IsPaused;
        }
    }

    private IEnumerator WaitForPlayer2ToSpawn(ulong cliendId)
    {
        while (!NetworkManager.Singleton.ConnectedClients[cliendId].PlayerObject)
        {
            yield return null;
        }
    }

    private void SpawnPlayer2_Client(ulong cliendId)
    {
        StartCoroutine(SpawnPlayer2Camera(cliendId));
    }

    private void SpawnPlayer2_Server(ulong clientId)
    {
        Player2 = Instantiate(player2Prefab, Vector3.zero, Quaternion.identity);
        Player2.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    private IEnumerator SpawnPlayer2Camera(ulong cliendId)
    {
        yield return WaitForPlayer2ToSpawn(cliendId);

        Player2 = NetworkManager.Singleton.ConnectedClients[cliendId].PlayerObject.gameObject;
        Player2Camera = Instantiate(player2CameraPrefab);

        SetupPlayer2Components();
        SetupPlayer2();
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
    void SetupPlayer1Components()
    {
        Player1Component = Player1.GetComponent<Character1Controller>();

        if (IsHosting || !IsMultiplayer)
        {
            Player1CameraComponent = Player1Camera.GetComponent<CinemachineVirtualCamera>();
        }
    }

    void SetupPlayer2Components()
    {
        Player2Component = Player2.GetComponent<Character2Controller>();

        if (!IsHosting || !IsMultiplayer)
        {
            Player2CameraComponent = Player2Camera.GetComponent<CinemachineFreeLook>();
        }
    }

    void SetupPlayer1()
    {
        Player1CameraComponent.Follow = Player1Component.transform.Find("POV");

        Player1CameraComponent.m_Lens.FieldOfView = PlayerPrefs.GetFloat("FieldOfViewPreference");

        Player1Component.PlayerState = PlayerController.State.Grounded;
    }

    void SetupPlayer2()
    {
        Player2CameraComponent.Follow = Player2Component.transform.Find("POV");
        Player2CameraComponent.LookAt = Player2Component.transform.Find("POV");

        Player2CameraComponent.m_Lens.FieldOfView = PlayerPrefs.GetFloat("FieldOfViewPreference");
        Player2CameraComponent.m_YAxis.m_MaxSpeed =
            Player2CameraComponent.m_XAxis.m_MaxSpeed = PlayerPrefs.GetFloat("SensitivityPreference") * 5;

        if (!IsHosting)
        {
            Player2CameraComponent.enabled = true;
        }

        if (!IsMultiplayer)
        {
            Player2Component.SpawnOveride();
            Player2CameraComponent.enabled = false;
        }
    }
}
