using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Base Class for Character
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool canRotate = true;
    public bool canMove = true;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
    }

    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
    }

    protected virtual void Update()
    {
        // If character is owner update network position and rotation values
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        // If character is not owned by us set position and rotation by reading from network values
        else
        {
            Vector3 targetPosition = Vector3.SmoothDamp
                (transform.position,
                 characterNetworkManager.networkPosition.Value,
                 ref characterNetworkManager.networkPositionVelocity,
                 characterNetworkManager.networkPositionSmoothTime);

            //Vector3 targetPosition = characterNetworkManager.networkPosition.Value;

            Quaternion targetRotation = Quaternion.Slerp
                (transform.rotation,
                 characterNetworkManager.networkRotation.Value,
                 characterNetworkManager.networkRotationSmoothTime);

            transform.SetPositionAndRotation(targetPosition, targetRotation);
        }
    }

    public virtual IEnumerator ProcessDeathEvent()
    {
        if (IsOwner)
        {
            Debug.Log(NetworkObjectId);
            
            isDead.Value = true;

            characterAnimatorManager.PlayTargetActionAnimation("Death", true);
            Debug.Log("Played death anim!");
            yield return new WaitForSeconds(5);
        }
    }

    public virtual void ReviveCharacter()
    {

    }

    protected virtual void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoreColliders = new List<Collider>();

        foreach (var collider in damageableCharacterColliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(characterControllerCollider);

        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider);
            }
        }

    }
}
