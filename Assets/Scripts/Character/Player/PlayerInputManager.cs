using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    // SINGLETON
    public static PlayerInputManager instance;

    public PlayerManager player;

    // Player Actions - Used to subscribe to the input event
    PlayerControls playerControls;

    [Header("Player Inputs")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;
    [SerializeField] bool dodgeInput = false;

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

    // singleton check for PlayerInputManager happens on Enable
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // on enable we assign this weird function which sets movementInput to Movement.performed
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
        }

        playerControls.Enable();
    }

    private void Start()
    {
        // Don't destroy when loading into scenes - why is it in start not awake?
        DontDestroyOnLoad(gameObject);

        // Assign OnSceneChange to the activeSceneChange list
        SceneManager.activeSceneChanged += OnSceneChange;
        // Diable Inputs on Start
        instance.enabled = false;
    }

    // collect all inputs on every frame
    private void Update()
    {
        HandleAllInput();
    }

    private void OnDestroy()
    {
        // remove OnSceneChange from activeSceneChanged if this playerInputManager object is destroyed, memory reasons and good practice
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    // makes it so that the active window in parrel sync recieves input
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    // if the new scene is the world scene enable the input manager else disable it
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        else
        {
            instance.enabled = false;
        }
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
}
