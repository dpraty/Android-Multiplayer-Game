using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldGameManager : MonoBehaviour
{
    public static WorldGameManager instance;

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

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }

    public void StartNewGame()
    {
        StartNetworkAsHost();
        StartCoroutine(WorldGameManager.instance.LoadWorldScene());
    }

    public void StartGameAsClient()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.StartClient();
        }
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
