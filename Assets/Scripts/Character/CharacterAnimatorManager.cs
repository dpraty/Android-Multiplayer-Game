using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    // base class for character animation

    CharacterManager character;

    float vertical;
    float horizontal;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    // function to set animator parameter values
    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
    {
        // we apply some damping to get smoother animations
        character.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        character.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }
}
