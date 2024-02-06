using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerScript : PlayerScriptProperties
{
    [Header("Respawn")]
    public float respawnWaitTime = 3f;
    public Slider respawnSlider;
    public Text respawnText;
    protected override void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        presentHealth = playerHealth;
        healthBar.GiveFullHealth(playerHealth);
        respawnSlider.gameObject.SetActive(false);
        respawnText.gameObject.SetActive(false);
    }
    public void SwitchJoystickType()
    {
        if (currentJoystickType == NewJoystickType.PlayerJoystick)
        {
            currentJoystickType = NewJoystickType.PlayerSprintJoystick;

        }
        else if (currentJoystickType == NewJoystickType.PlayerSprintJoystick)
        {
            currentJoystickType = NewJoystickType.PlayerJoystick;
        }
    }

    void Update()
    {
        
        if (currentPlayerSpeed > 0)
        {
            //sprintJoystick = null;
        }    
        else
        {
            FixedJoystick sprintJS = GameObject.Find("PlayerSprintJoystick").GetComponent<FixedJoystick>();
            sprintJoystick = sprintJS;
        }

        onSurface = Physics.Raycast(surfaceCheck.position, Vector3.down * 0.1f, surfaceDistance, surfaceMask);

        //Debug.Log("On Surface: " + onSurface);
        //Debug.DrawLine(surfaceCheck.position, surfaceCheck.position + Vector3.down * surfaceDistance, Color.red);

        if (onSurface && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        cC.Move(velocity * Time.deltaTime);

        PlayerMove();
        Jump();
        Sprint();
        //LoadAimCamera();
        UpdatePlayerLookDirection();
        //SwitchCamera();
        CheckYPosition();
        LoadMouseLook();


    }
    public void SwitchCamera()
    {
        if (playerCamera != aimCamera)
        {
            playerCamera = aimCamera;
        }
        else if (playerCamera == aimCamera)
        {
            GameObject tpCameraObject = GameObject.Find("TPCamera");
            if (tpCameraObject != null)
            {
                playerCamera = tpCameraObject.transform;
            }
            else
            {
                Debug.LogError("GameObject with the name 'TPCamera' not found.");
            }
        }
    }
    protected virtual void UpdatePlayerLookDirection()
    {
        Vector3 lookDirection = playerCamera.forward;

        lookDirection.y = 0f;

        Quaternion rotation = Quaternion.LookRotation(lookDirection);

        playerCharacter.rotation = rotation;
    }

    void PlayerMove()
    {
       if(mobileInputs == true)
       {
            if (currentJoystickType == NewJoystickType.PlayerJoystick)
            {
                float horizontal_axis = joystick.Horizontal;
                //Debug.Log("Horizontal Axis: " + horizontal_axis);;
                float vertical_axis = joystick.Vertical;

                Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;
               
                //Debug.Log("Direction " + direction.magnitude);
                if (direction.magnitude >= 0.1f)
                {
                    animator.SetBool("Walk", true);
                    animator.SetBool("Running", false);
                    animator.SetBool("Idle", false);
                    animator.SetTrigger("Jump");
                    animator.SetBool("AimWalk", false);
                    animator.SetBool("IdleAim", false);

                    
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);



                    Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    cC.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);



                    currentPlayerSpeed = playerSpeed;
                }
                else
                {
                    animator.SetBool("Idle", true);
                    animator.SetTrigger("Jump");
                    animator.SetBool("Walk", false);
                    animator.SetBool("Running", false);
                    animator.SetBool("AimWalk", false);
                    currentPlayerSpeed = 0f;
                }

            }
            else if (currentJoystickType == NewJoystickType.PlayerSprintJoystick)
            {
                float horizontal_axis = joystick.Horizontal;
                float vertical_axis = joystick.Vertical;

                Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

                if (direction.magnitude >= 0.1f)
                {
                    animator.SetBool("Running", true);
                    animator.SetBool("Idle", false);
                    // animator.SetBool("Walk", false);
                    animator.SetBool("IdleAim", false);

                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    cC.Move(moveDirection.normalized * playerSprint * Time.deltaTime);
                    currentPlayerSprint = playerSprint;
                }
                else
                {
                    animator.SetBool("Idle", true);
                    animator.SetBool("Running", false);
                    //animator.SetBool("Walk", false);
                    currentPlayerSprint = 0f;
                }
            }    
           
        }   
       else
       {
            float horizontal_axis = Input.GetAxisRaw("Horizontal");
            float vertical_axis = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;
            Debug.Log("Direction Magnitude: " + direction.magnitude);
            if (direction.magnitude >= 0.1f)
            {
                animator.SetBool("Walk", true);
                animator.SetBool("Running", false);
                animator.SetBool("Idle", false);
                animator.SetTrigger("Jump");
                animator.SetBool("AimWalk", false);
                animator.SetBool("IdleAim", false);

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cC.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);
                currentPlayerSpeed = playerSpeed;
            }
            else
            {
                animator.SetBool("Idle", true);
                animator.SetTrigger("Jump");
                animator.SetBool("Walk", false);
                animator.SetBool("Running", false);
                animator.SetBool("AimWalk", false);
                currentPlayerSpeed = 0f;
            }
       }    
    }
    void CheckYPosition()
    {
        if (playerCharacter != null)
        {
            float posY = playerCharacter.position.y;
            //Debug.Log("Y Position: " + posY);
            if (posY < -0.5f)
            {
                LoadSpawn();
                playerCharacter.transform.position = spawn.transform.position;
            }
        }
    }

    void Jump()
    {
        if(mobileInputs == true)
        {
            if (CrossPlatformInputManager.GetButtonDown("Jump") && onSurface)
            {
                animator.SetBool("Walk", false);
                animator.SetTrigger("Jump");
                velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
            }
            else
            {
                animator.ResetTrigger("Jump");
            }
        }    
        else
        {
            if (Input.GetButtonDown("Jump") && onSurface)
            {
                animator.SetBool("Walk", false);
                animator.SetTrigger("Jump");
                velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
            }
            else
            {
                animator.ResetTrigger("Jump");
            }
        }    
    }
    void Sprint()
    {
        if (mobileInputs == true)
        {
            if (currentJoystickType == NewJoystickType.PlayerJoystick)
            {
                float horizontal_axis = sprintJoystick.Horizontal;
                float vertical_axis = sprintJoystick.Vertical;

                Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

                if (direction.magnitude >= 0.1f)
                {
                    animator.SetBool("Running", true);
                    animator.SetBool("Idle", false);
                    // animator.SetBool("Walk", false);
                    animator.SetBool("IdleAim", false);

                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    cC.Move(moveDirection.normalized * playerSprint * Time.deltaTime);
                    currentPlayerSprint = playerSprint;
                }
                else
                {
                    animator.SetBool("Idle", true);
                    //animator.SetBool("Walk", false);
                    currentPlayerSprint = 0f;
                }
            }    
                


        }
        else
        {
            if (Input.GetButton("Sprint") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && onSurface)
            {
                float horizontal_axis = Input.GetAxisRaw("Horizontal");
                float vertical_axis = Input.GetAxisRaw("Vertical");

                Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

                if (direction.magnitude >= 0.1f)
                {
                    animator.SetBool("Running", true);
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("IdleAim", false);

                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    cC.Move(moveDirection.normalized * playerSprint * Time.deltaTime);
                    currentPlayerSprint = playerSprint;
                }
                else
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk", false);
                    currentPlayerSprint = 0f;
                }
            }
        }

        
    }

    public void playerHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;
        healthBar.SetHealth(presentHealth);
        if (presentHealth <= 0.01)
        {
            mouseLook.cameraDistance = 5.5f;
        }
        if (presentHealth <= 0 && !GameManagers.instance.survivalMode)
        {
            //presentHealth = 0f;
            PlayerDie();
        }    
        else if (presentHealth <= 0 && GameManagers.instance.survivalMode)
        {
            playerSpeed = 0f;
            playerSprint = 0f;
            animator.SetBool("Die", true);
            scoreManager.enemyKills += 1;
            Destroy(gameObject);
        }    
    }
    private bool respawnInProgress = false;
    private void PlayerDie()
    {
        Cursor.lockState = CursorLockMode.None;
        if (!respawnInProgress)
        {
            respawnInProgress = true;
            StartCoroutine(Respawn());
        }
    }

   
    IEnumerator Respawn()
    {
        playerSpeed = 0f;
        playerSprint = 0f;
        animator.SetBool("Die", true);
        scoreManager.enemyKills += 1;

        float timer = 0f;
        float respawnTime = respawnWaitTime;

        respawnSlider.gameObject.SetActive(true);
        respawnText.gameObject.SetActive(true);
        respawnSlider.maxValue = respawnWaitTime;

        while (timer < respawnTime)
        {
            timer += Time.deltaTime;
            respawnSlider.value = timer;
            yield return null; 
        }

        respawnSlider.gameObject.SetActive(false);
        respawnText.gameObject.SetActive(false);
        presentHealth = 300f;
        healthBar.SetHealth(presentHealth);
        playerSpeed = 2f;
        playerSprint = 3.5f;
        if (playerCamera == aimCamera)
        {
            mouseLook.cameraDistance = 0.35f;
        }
        else if (playerCamera != aimCamera)
        {
            mouseLook.cameraDistance = 2f;
        }

        animator.SetBool("Idle", true);
        animator.SetBool("Die", false);

        playerCharacter.transform.position = spawn.transform.position;

        respawnInProgress = false;
    }

}
