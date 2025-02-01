using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] UI_StatBar staminaBar;
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] Image meleeWeaponLogo;
    public void SetNewHealthValue(int oldValue, int newValue)
    {
        healthBar.SetStat(newValue);
    }

    public void SetMaxHealthValue(int maxValue)
    {
        healthBar.SetMaxStat(maxValue);
    }

    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxStaminaValue(int maxValue)
    {
        staminaBar.SetMaxStat(maxValue);
    }

    public void SwitchWeaponLogo(Sprite weaponLogo)
    {
        meleeWeaponLogo.sprite = weaponLogo;
    }
}
