using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerInputManager : MonoBehaviour
{
    // Player Input Manager Class Singleton
    public static PlayerInputManager instance;

    public PlayerManager player;

    [Header("Player Inputs")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;
    [SerializeField] bool lightAttackInput = false;
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool switchMeleeWeaponInput = false;
    [SerializeField] bool lockOnInput = false;

    [Header("Touchscreen Controls")]
    public TouchJoystick movementJoystick;
    public TouchButton lightAttackButton;
    public TouchSquareButton dodgeButton;
    public TouchSquareButton switchMeleeWeaponButton;
    public TouchSquareButton lockOnButton;

    [Header("Lock On")]
    [SerializeField] private float lockOnRadius = 20;
    [SerializeField] private float minimumViewableAngle = -70;
    [SerializeField] private float maximumViewableAngle = 70;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    private CharacterManager nearestLockOnTarget;

    // Finger variable to track joystick input
    private Finger movementFinger;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Subscribe and un-subscribe to Finger Down, Finger Move and Finger Up events on Enable and Disable
    private void OnEnable()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.TouchSimulation.Enable();
        Touch.onFingerDown += HandleFingerDown;
        Touch.onFingerMove += HandleFingerMove;
        Touch.onFingerUp += HandleFingerUp;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= HandleFingerDown;
        Touch.onFingerMove -= HandleFingerMove;
        Touch.onFingerUp -= HandleFingerUp;
        UnityEngine.InputSystem.EnhancedTouch.TouchSimulation.Disable();
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Disable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    // Activate Touch Controls when we load into the world scene, de-activate it when loading out of world scene
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            instance.enabled = false;
        }
    }

    private void Start()
    {
        // Don't destroy when loading into scenes - why is it in start not awake?
        DontDestroyOnLoad(gameObject);

        // Subscribe to On Scene Change and then disable the insance and deactivate the Game Object
        SceneManager.activeSceneChanged += OnSceneChange;
        gameObject.SetActive(false);
        instance.enabled = false;

    }

    // collect all inputs on every frame
    private void Update()
    {
        HandleAllInput();
    }

    private void HandleAllInput()
    {
        HandleLockOnInput();
        HandleMovementInput();
        HandleDodgeInput();
        HangleLightAttackInput();
        HandleSwitchMeleeWeaponInput();
    }

    // handles movement Input
    private void HandleMovementInput()
    {
        
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount != 0)
            moveAmount = 1;

        if (player == null)
            return;

        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
    }

    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;

            player.playerLocomotionManager.AttemptToPerformDodge();
        }
    }

    private void HandleSwitchMeleeWeaponInput()
    {
        if (switchMeleeWeaponInput)
        {
            switchMeleeWeaponInput = false;

            player.playerEquipmentManager.SwitchMeleeWeapon();
        }
    }

    private void HangleLightAttackInput()
    {
        if (lightAttackInput)
        {
            lightAttackInput = false;

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentMeleeWeapon.lightAttack, player.playerInventoryManager.currentMeleeWeapon);
        }
            
    }

    private void HandleLockOnInput()
    {
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null)
                return;

            if (player.playerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;
            }
        }

        if (lockOnInput && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;

            ClearLockOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;
            return;
        }

        if (lockOnInput && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;

            HandleLocatingLockOnTargets();
            
            if (nearestLockOnTarget != null)
            {
                player.playerCombatManager.SetTarget(nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }

            return;
        }
    }

    private void HandleLocatingLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity;
        //float shortestDistanceOfLeftTarget = -Mathf.Infinity;
        //float shortestDistanceOfRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

            if (lockOnTarget != null)
            {
                Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float targetAngle = Vector3.Angle(lockOnTargetsDirection, player.transform.forward);

                if (lockOnTarget.isDead.Value)
                    continue;

                if (lockOnTarget.transform.root == player.transform.root)
                    continue;

                if (targetAngle >= minimumViewableAngle && targetAngle <= maximumViewableAngle)
                {
                    RaycastHit hit;

                    if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position, 
                        lockOnTarget.characterCombatManager.lockOnTransform.position, 
                        out hit, WorldUtilityManager.Instance.GetEnviroLayers()))
                    {
                        continue;
                    }
                    else
                    {
                        availableTargets.Add(lockOnTarget);
                    }
                }
            }
        }

        for (int k = 0; k < availableTargets.Count; k++)
        {
            if (availableTargets[k] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);
                Vector3 lockOnTargetsDirection = availableTargets[k].transform.position - player.transform.position;

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }
            }
            else
            {
                ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
            }
        }
    }

    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        availableTargets.Clear();
    }

    // Handle Touch Inputs
    private void HandleFingerDown(Finger touchedFinger)
    {
        // Calculate joystick input based on the position of the touched finger
        if (movementFinger == null && Vector2.Distance(touchedFinger.screenPosition, movementJoystick.joystickCenterScreenPosition) <= movementJoystick.joystickScreenRadius)
        {
            // set up the movement finger when the joystick is touched
            movementFinger = touchedFinger;

            movementJoystick.knob.anchoredPosition = touchedFinger.screenPosition - movementJoystick.joystickCenterScreenPosition;
            movementInput = movementJoystick.knob.anchoredPosition/movementJoystick.joystickScreenRadius;

            return;
        }

        // Presses the button based on the position of the touched finger
        if (Vector2.Distance(touchedFinger.screenPosition, lightAttackButton.buttonScreenPosition) <= lightAttackButton.buttonRadius && !lightAttackButton.buttonPressed)
        {
            lightAttackInput = true;
            lightAttackButton.PressButton();

            return;
        }

        if (touchedFinger.screenPosition.x >= dodgeButton.cornerPoint_1.x && touchedFinger.screenPosition.x <= dodgeButton.cornerPoint_2.x && touchedFinger.screenPosition.y >= dodgeButton.cornerPoint_1.y && touchedFinger.screenPosition.y <= dodgeButton.cornerPoint_2.y)
        {
            if (!dodgeButton.buttonPressed)
            {
                dodgeInput = true;
                dodgeButton.PressButton();
            }

            return;
        }

        if (touchedFinger.screenPosition.x >= switchMeleeWeaponButton.cornerPoint_1.x && touchedFinger.screenPosition.x <= switchMeleeWeaponButton.cornerPoint_2.x && touchedFinger.screenPosition.y >= switchMeleeWeaponButton.cornerPoint_1.y && touchedFinger.screenPosition.y <= switchMeleeWeaponButton.cornerPoint_2.y)
        {
            if (!switchMeleeWeaponButton.buttonPressed)
            {
                switchMeleeWeaponInput = true;
                switchMeleeWeaponButton.PressButton();
            }

            return;
        }

        if (touchedFinger.screenPosition.x >= lockOnButton.cornerPoint_1.x && touchedFinger.screenPosition.x <= lockOnButton.cornerPoint_2.x && touchedFinger.screenPosition.y >= lockOnButton.cornerPoint_1.y && touchedFinger.screenPosition.y <= lockOnButton.cornerPoint_2.y)
        {
            if (!lockOnButton.buttonPressed)
            {
                lockOnInput = true;
                lockOnButton.PressButton();
            }

            return;
        }
    }

    private void HandleFingerMove(Finger movedFinger)
    {
        // calculate the joystick input using the movement finger
        if (movedFinger == movementFinger)
        {

            Touch currentTouch = movedFinger.currentTouch;
            Vector2 targetKnobPosition = movementFinger.screenPosition - movementJoystick.joystickCenterScreenPosition;

            if (targetKnobPosition.magnitude > movementJoystick.joystickScreenRadius)
            {
                movementJoystick.knob.anchoredPosition = targetKnobPosition.normalized * movementJoystick.joystickScreenRadius;
            }
            else
            {
                movementJoystick.knob.anchoredPosition = targetKnobPosition;
            }

            movementInput = movementJoystick.knob.anchoredPosition / movementJoystick.joystickScreenRadius;
        }

    }

    private void HandleFingerUp(Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            movementFinger = null;
            movementJoystick.knob.anchoredPosition = Vector2.zero;
            movementInput = movementJoystick.knob.anchoredPosition / movementJoystick.joystickScreenRadius;
        }
    }

}
