using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;
    public GameObject gameOptionsDialogBox;
    
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

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        WorldGameManager.instance.relay.OnRelayServerDataReady += HandleRelayServerDataReady;
    }
    private void OnDestroy()
    {
        WorldGameManager.instance.relay.OnRelayServerDataReady -= HandleRelayServerDataReady;
    }

    private void HandleRelayServerDataReady()
    {
        // Show the gameOptionsDialogBox 
        gameOptionsDialogBox.SetActive(true);
    }
}
