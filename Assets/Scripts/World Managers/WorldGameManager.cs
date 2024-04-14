using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldGameManager : MonoBehaviour
{
    // base world game manager class - singleton

    public static WorldGameManager instance;

    // Scene List - currently only has the world scene
    [Header("Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("Network Join")]
    [SerializeField] bool startGameAsClient;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        StartGameAsClient();
    }

    // start host network
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    // an Async/Corutine thing to load scenes??
    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }
    
    // this function starts the new game called when we press the start game button
    // it starts the host network and loads the game scene
    public void StartNewGame()
    {
        StartNetworkAsHost();
        StartCoroutine(WorldGameManager.instance.LoadWorldScene());
    }

    // to start game as client first shutdown the host network then start a client
    public void StartGameAsClient()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.StartClient();
        }
    }

    // this get world scene index function is for utility - currently used for a clever UI switiching logic
    // to turn off the title screen
    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
