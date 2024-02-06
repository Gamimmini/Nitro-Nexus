using UnityEngine;

public class PlayerScriptProperties : GamiManagers
{
    [Header("Player Health Things")]
    protected float playerHealth = 300f;
    public float presentHealth;
    public HealthBar healthBar;

    [Header("Player Movement")]
    public float playerSpeed = 1.9f;
    public float currentPlayerSpeed = 0f;
    public float playerSprint = 3f;
    public float currentPlayerSprint = 0f;

    [Header("Player Movement")]
    public Transform playerCamera;
    public Transform aimCamera;
    public MouseLook mouseLook;

    [Header("Player Animator and Gravity")]
    public CharacterController cC;
    public float gravity = -9.81f;
    public Animator animator;

    [Header("Player Jumping & Velocity")]
    public float jumpRange = 1f;
    public float turnCalmTime = 0.1f;
    protected float turnCalmVelocity;
    protected Vector3 velocity;
    public Transform surfaceCheck;
    protected bool onSurface;
    protected float surfaceDistance = 0.1f;
    public LayerMask surfaceMask;

    [Header("Joystick Handle")]
    public bool mobileInputs;
    public FixedJoystick joystick;
    public FixedJoystick sprintJoystick;
    public NewJoystickType currentJoystickType;

    [Header("Resoawn")]
    public GameObject spawnPointsForPlayer;
    public Transform spawn;
    public Transform playerCharacter;

   
    public ScoreManager scoreManager;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        LoadJoystick();
        LoadSprintJoystick();
        LoadAimCamera();
        LoadSpawnPointsForPlayer();
        LoadSpawn();
        LoadPlayerCharacter();
        LoadMouseLook();
    }
    protected virtual void LoadJoystick()
    {
        GameObject joystickObject = GameObject.Find("PlayerJoystick");

        if (joystickObject != null)
        {
            joystick = joystickObject.GetComponent<FixedJoystick>();

            if (joystick == null)
            {
                Debug.LogError("FixedJoystick component not found on the object: " + joystickObject.name);
            }
        }
        else
        {
            Debug.LogError("GameObject with the name 'PlayerJoystick' not found.");
        }

        if (joystick != null)
        {
            currentJoystickType = NewJoystickType.PlayerJoystick;
        }
    }

    protected virtual void LoadSprintJoystick()
    {
        GameObject sprintJoystickObject = GameObject.Find("PlayerSprintJoystick");

        if (sprintJoystickObject != null)
        {
            sprintJoystick = sprintJoystickObject.GetComponent<FixedJoystick>();

            if (sprintJoystick == null)
            {
                Debug.LogError("FixedJoystick component not found on the object: " + sprintJoystickObject.name);
            }
        }
        else
        {
            Debug.LogError("GameObject with the name 'PlayerSprintJoystick' not found.");
        }
    }

    protected virtual void LoadAimCamera()
    {
        GameObject aimCameraObject = GameObject.Find("AIMCamera");

        if (aimCameraObject != null)
        {
            aimCamera = aimCameraObject.transform;
        }
        else
        {
            //Debug.LogWarning("GameObject with the name 'AIMCamera' not found. Setting aimCamera to null.");
            aimCamera = null;
        }
    }
    protected virtual void LoadMouseLook()
    {
        if (playerCamera != null)
        {
            mouseLook = playerCamera.GetComponent<MouseLook>();
        }
        else
        {
            Debug.LogError("PlayerCamera not assigned. Cannot load MouseLook.");
        }
    }
    protected virtual void LoadSpawnPointsForPlayer()
    {
        if (this.spawnPointsForPlayer != null) return;

        this.spawnPointsForPlayer = GameObject.Find("SpawnPointsForPlayer");

        if (this.spawnPointsForPlayer == null)
        {
            Debug.LogError("SpawnPointsForPlayer object not found in the hierarchy.");
        }
    }
    protected virtual void LoadSpawn()
    {

        if (this.spawnPointsForPlayer != null)
        {
            Transform[] spawnPointChildren = spawnPointsForPlayer.GetComponentsInChildren<Transform>();

            if (spawnPointChildren.Length > 1)
            {
                this.spawn = spawnPointChildren[Random.Range(1, spawnPointChildren.Length)];
            }
            else
            {
                Debug.LogError("No spawn points found under SpawnPointsForPlayer.");
            }
        }
        else
        {
            Debug.LogError("SpawnPointsForPlayer not found. Cannot load spawn point.");
        }
    }
    protected virtual void LoadPlayerCharacter()
    {
        if (this.playerCharacter != null) return;

        this.playerCharacter = transform;

        if (this.playerCharacter == null)
        {
            Debug.LogError("Transform component not found on the object.");
        }
    }
}
