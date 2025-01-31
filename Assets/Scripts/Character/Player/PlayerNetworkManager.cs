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
}
