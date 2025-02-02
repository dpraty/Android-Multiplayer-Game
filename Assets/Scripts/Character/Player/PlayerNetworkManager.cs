using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    [Header("Equipment")]
    public NetworkVariable<int> currentMeleeWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentRangedWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void OnCurrentMeleeWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentMeleeWeapon = newWeapon;
        player.playerEquipmentManager.LoadMeleeWeapon();
    }

    public void OnCurrentRangedWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentRangedWeapon = newWeapon;
        player.playerEquipmentManager.LoadRangedWeapon();
    }

    // ITEM ACTIONS

    [ServerRpc]
    public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
    {
        if (IsServer)
        {
            NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
        }
    }

    [ClientRpc]
    private void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformWeaponBasedAction(actionID, weaponID);
        }
    }

    private void PerformWeaponBasedAction(int actionID, int weaponID)
    {
        WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

        if (weaponAction != null)
        {
            weaponAction.AttemptToPerformAction(player, WorldItemDatabase.Instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.LogError("ACTION IS NULL, CANNOT PERFORM");
        }
    }
}
