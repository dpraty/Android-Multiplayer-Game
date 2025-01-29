using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    // Temporary Set-up to test Instant Effects
    [Header("Debug Delete Later")]
    [SerializeField] InstantCharacterEffect effectToTest;
    [SerializeField] bool processEffect = false;


    private void Update()
    {
        if (processEffect)
        {
            processEffect = false;

            // Temp use of Scriptable Object for Testing
            InstantCharacterEffect effect = Instantiate(effectToTest);

            ProcessInstantEffects(effect);
        }
    }
}
