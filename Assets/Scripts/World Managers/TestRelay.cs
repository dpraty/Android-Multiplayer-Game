using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TestRelay : MonoBehaviour
{
    // Event to signal that Relay Server Data has been set
    public event System.Action OnRelayServerDataReady;

    private async void Start()
    {
        // Initialize unity before starting
        await UnityServices.InitializeAsync();

        // Sign-in Debug Log Event
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        // Sign-in anonymously
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        CreateRelay();
    }

    public async void CreateRelay()
    {
        try
        {
            // Create allocation for max 3 players
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);
            
            // Display the join code on screen
            PlayerUIManager.instance.joinCode.SetText(joinCode);

            // Create and set relay server data
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            // Invoke the event once Relay Server Data has been set
            OnRelayServerDataReady?.Invoke();

        } 
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to create relay: " + e.Message);
        }
        
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        } 
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay: " + e.Message);
        }
    }
}
