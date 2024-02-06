using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SwitchCamera : GamiManagers
{
    [Header("Camera to Assign")]
    public GameObject AimCam;
    public GameObject AimCanvas;
    public GameObject ThirdPersonCam;
    public GameObject ThirdPersonCanvas;
    public PlayerScript player;

    [Header("Camera Animator")]
    public Animator animator;

    [Header("Camera Aim")]
    public bool isAiming = false;
    private bool isSprinting;


  
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadAimCam();
        this.LoadAimCanvas();
        this.LoadThirdPersonCam();
        this.LoadThirdPersonCanvas();
        this.LoadPlayer();
        this.LoadAnimator();
    }
    protected virtual void LoadAimCam()
    {
        if (this.AimCam != null) return;

        this.AimCam = GameObject.Find("AIMCamera");

        if (this.AimCam == null)
        {
            Debug.LogError("AimCam object not found in the hierarchy.");
        }
    }
    protected virtual void LoadAimCanvas()
    {
        if (this.AimCanvas != null) return;

        this.AimCanvas = GameObject.Find("AC");

        if (this.AimCanvas == null)
        {
            Debug.LogError("AC object not found in the hierarchy.");
        }
    }
    protected virtual void LoadThirdPersonCam()
    {
        if (this.ThirdPersonCam != null) return;

        this.ThirdPersonCam = GameObject.Find("TPCamera");

        if (this.ThirdPersonCam == null)
        {
            Debug.LogError("TPCamera object not found in the hierarchy.");
        }
    }
    protected virtual void LoadThirdPersonCanvas()
    {
        if (this.ThirdPersonCanvas != null) return;

        this.ThirdPersonCanvas = GameObject.Find("TPC");

        if (this.ThirdPersonCanvas == null)
        {
            Debug.LogError("TPCanvas object not found in the hierarchy.");
        }
    }

    protected virtual void LoadPlayer()
    {
        if (this.player != null) return;

        this.player = GetComponent<PlayerScript>();

        if (this.player == null)
        {
            Debug.LogError("PlayerScript component not found on the object.");
        }
    }
    protected virtual void LoadAnimator()
    {
        if (this.animator != null) return;

        this.animator = GetComponent<Animator>();

        if (this.animator == null)
        {
            Debug.LogError("Animator component not found on the object.");
        }
    }
    protected virtual void EnableThirdPersonCamera()
    {
        Camera thirdPersonCamera = ThirdPersonCam.GetComponent<Camera>();

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.enabled = true;
        }
        else
        {
            Debug.LogError("Camera component not found on ThirdPersonCam object.");
        }
        AudioListener audioListener = ThirdPersonCam.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = true;
        }
        else
        {
            Debug.LogError("AudioListener component not found on ThirdPersonCam object.");
        }
    }
    protected virtual void DisableThirdPersonCamera()
    {
        Camera thirdPersonCamera = ThirdPersonCam.GetComponent<Camera>();

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.enabled = false;
        }
        else
        {
            Debug.LogError("Camera component not found on ThirdPersonCam object.");
        }
        AudioListener audioListener = ThirdPersonCam.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = false;
        }
        else
        {
            Debug.LogError("AudioListener component not found on ThirdPersonCam object.");
        }
    }
    protected virtual void EnableAimCamera()
    {
        Camera aimCamera = AimCam.GetComponent<Camera>();

        if (aimCamera != null)
        {
            aimCamera.enabled = true;
        }
        else
        {
            Debug.LogError("Camera component not found on AimCam object.");
        }
        AudioListener audioListener = AimCam.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = true;
        }
        else
        {
            Debug.LogError("AudioListener component not found on AimCam object.");
        }
    }
    protected virtual void DisableAimCamera()
    {
        Camera aimCamera = AimCam.GetComponent<Camera>();

        if (aimCamera != null)
        {
            aimCamera.enabled = false;
        }
        else
        {
            Debug.LogError("Camera component not found on AimCam object.");
        }
        AudioListener audioListener = AimCam.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = false;
        }
        else
        {
            Debug.LogError("AudioListener component not found on AimCam object.");
        }
    }
    private void Update()
    {
        if(player.mobileInputs == true)
        {
            if (CrossPlatformInputManager.GetButtonDown("Aim") )
            {
                isAiming = !isAiming;
                
            }
            float horizontalMove = isSprinting ? player.sprintJoystick.Horizontal : player.joystick.Horizontal;
            float verticalMove = isSprinting ? player.sprintJoystick.Vertical : player.joystick.Vertical;
            bool isMoving = Mathf.Abs(horizontalMove) > 0.1f || Mathf.Abs(verticalMove) > 0.1f;
            if (isAiming)
            {
                //Debug.Log("Is Player Moving: " + isMoving);
                if (isMoving)
                {

                    animator.SetBool("Idle", false);
                    animator.SetBool("IdleAim", false);
                    animator.SetBool("AimWalk", true);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Running", false);
                }
                else
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("IdleAim", true);
                    animator.SetBool("AimWalk", false);
                    animator.SetBool("Walk", false);
                }
                DisableThirdPersonCamera();
                ThirdPersonCanvas.SetActive(false);
                EnableAimCamera();
                AimCanvas.SetActive(true);
            }
            else
            {
                animator.SetBool("Idle", true);
                animator.SetBool("IdleAim", false);
                animator.SetBool("AimWalk", false);

                EnableThirdPersonCamera();
                ThirdPersonCanvas.SetActive(true);
                DisableAimCamera();
                AimCanvas.SetActive(false);
            }
        }   
        else
        {
            if (Input.GetButton("Fire2") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                animator.SetBool("Idle", false);
                animator.SetBool("IdleAim", true);
                animator.SetBool("AimWalk", true);
                animator.SetBool("Walk", true);

                ThirdPersonCam.SetActive(false);
                ThirdPersonCanvas.SetActive(false);
                AimCam.SetActive(true);
                AimCanvas.SetActive(true);
            }
            else if (Input.GetButton("Fire2"))
            {
                animator.SetBool("Idle", false);
                animator.SetBool("IdleAim", true);
                animator.SetBool("AimWalk", false);
                animator.SetBool("Walk", false);

                ThirdPersonCam.SetActive(false);
                ThirdPersonCanvas.SetActive(false);
                AimCam.SetActive(true);
                AimCanvas.SetActive(true);
            }
            else
            {
                animator.SetBool("Idle", true);
                animator.SetBool("IdleAim", false);
                animator.SetBool("AimWalk", false);

                ThirdPersonCam.SetActive(true);
                ThirdPersonCanvas.SetActive(true);
                AimCam.SetActive(false);
                AimCanvas.SetActive(false);
            }
        }    
    }
}
