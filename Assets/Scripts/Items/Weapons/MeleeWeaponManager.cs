using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponManager : WeaponManager
{
    public MeleeWeaponDamageCollider meleeDamageCollider;

    protected override void Awake()
    {
        base.Awake();

        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
    {
        meleeDamageCollider.characterCausingDamage = characterWieldingWeapon;
        meleeDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeDamageCollider.light_Attack_01_Modifier = weapon.light_Attack_01_Modifier;
    }
}
