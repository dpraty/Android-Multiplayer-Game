using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldGameManager : MonoBehaviour
{
    // World Game Manager class - singleton

    public static WorldGameManager instance;
    public TestRelay relay;

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

    // Coroutine to Load the world scene
    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }

    // Start the host and load scene, called from Start Game button
    public void StartGame()
    {
        NetworkManager.Singleton.StartHost();
        StartCoroutine(WorldGameManager.instance.LoadWorldScene());
    }

    // Start game as client and load scene, called from join code text input field using On End Edit 
    public void JoinGameAsClient(string joinCode)
    {
        relay.JoinRelay(joinCode);
        StartCoroutine(WorldGameManager.instance.LoadWorldScene());
    }

    // returns the world scene index
    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
