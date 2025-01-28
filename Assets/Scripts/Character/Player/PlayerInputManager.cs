using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerInputManager : MonoBehaviour
{
    // SINGLETON
    public static PlayerInputManager instance;

    public PlayerManager player;

    [Header("Player Inputs")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;
    [SerializeField] bool dodgeInput = false;

    [Header("Touchscreen Controls")]
    public TouchJoystick movementJoystick;
    public TouchButton dodgeButton;

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
        HandleMovementInput();
        HandleDodgeInput();
    }

    // handles movement Input
    private void HandleMovementInput()
    {
        
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }

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

    private void HandleFingerDown(Finger touchedFinger)
    {
        if (movementFinger == null && Vector2.Distance(touchedFinger.screenPosition, movementJoystick.joystickCenterScreenPosition) <= movementJoystick.joystickScreenRadius)
        {
            movementFinger = touchedFinger;
            movementJoystick.knob.anchoredPosition = touchedFinger.screenPosition - movementJoystick.joystickCenterScreenPosition;
            movementInput = movementJoystick.knob.anchoredPosition/movementJoystick.joystickScreenRadius;
        }

       if (Vector2.Distance(touchedFinger.screenPosition, dodgeButton.buttonScreenPosition) <= dodgeButton.buttonRadius && !dodgeButton.buttonPressed)
        {
            dodgeInput = true;
            dodgeButton.PressButton();
        }
    }

    private void HandleFingerMove(Finger movedFinger)
    {
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
