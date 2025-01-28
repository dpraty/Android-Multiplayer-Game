using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldGameManager : MonoBehaviour
{
    // base world game manager class - singleton

    public static WorldGameManager instance;
    public TestRelay relay;
    public TitleScreenManager titleScreenManager;

    // Scene List - currently only has the world scene
    [Header("Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            relay = GetComponent<TestRelay>();
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
   
    // start host network

    // an Async/Corutine thing to load scenes??
    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }

    public void StartGame()
    {
        NetworkManager.Singleton.StartHost();
        StartCoroutine(WorldGameManager.instance.LoadWorldScene());
    }

    // to start game as client first shutdown the host network then start a client
    public void JoinGameAsClient(string joinCode)
    {
        relay.JoinRelay(joinCode);
        StartCoroutine(WorldGameManager.instance.LoadWorldScene());
    }

    // this get world scene index function is for utility - currently used for a clever UI switiching logic
    // to turn off the title screen
    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
