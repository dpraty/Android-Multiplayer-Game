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
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterController characterController;

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
    }

    protected virtual void Start()
    {

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


}
