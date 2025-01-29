using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    // Title Screen Manager class - singleton

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

    // If the Relay Server Data has been set, display the Game Options Dialog Box
    private void HandleRelayServerDataReady()
    {
        gameOptionsDialogBox.SetActive(true);
    }
}
