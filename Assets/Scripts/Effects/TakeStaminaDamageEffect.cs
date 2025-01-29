using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This code let's me create new stamina damage effects by right clicking in the project window
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]

public class TakeStaminaDamageEffect : InstantCharacterEffect
{   
    public float staminaDamage;

    public override void ProcessEffect(CharacterManager character)
    {
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        // take stamina damage only if you are the owner of this character
        if (character.IsOwner)
        {
            character.characterNetworkManager.currentStamina.Value -= staminaDamage;
        }

    }
}
