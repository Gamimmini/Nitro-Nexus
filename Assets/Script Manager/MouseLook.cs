using UnityEngine;

public class MouseLook : GamiManagers
{
    [Header("Min & Max Camera View")]
    private const float YMin = -25f;
    private const float YMax = 40f;

    [Header("Camera View")]
    public Transform lookAt;
    //public Transform player;


    [Header("camera Position")]
    public float cameraDistance = 10f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    public float cameraSensitivity = 100f;


    [Header("Floating Joystick")]
    public FloatingJoystick floatingJoystick;
    protected override void Start()
    {
        cameraSensitivity = GraphicsOptions.Instance.cameraSensitivity;
    }    
  
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadLookAt();
        this.LoadFloatingJoystickt();
    }
    protected virtual void LoadLookAt()
    {
        if (this.lookAt != null) return;

        GameObject playerObject = GameObject.Find("Player");

        if (playerObject != null)
        {
            Transform lookPointTransform = playerObject.transform.Find("LookPoint");

            if (lookPointTransform != null)
            {
                this.lookAt = lookPointTransform;
            }
            else
            {
                Debug.LogError("LookPoint not found as a child of Player.");
            }
        }
        else
        {
            Debug.LogError("Player not found in the hierarchy.");
        }
    }
    protected virtual void LoadFloatingJoystickt()
    {
        if (this.floatingJoystick != null) return;

        GameObject joystickObject = GameObject.Find("PlayerUI");

        if (joystickObject != null)
        {
            FloatingJoystick joystickComponent = joystickObject.transform.Find("Floating Joystick")?.GetComponent<FloatingJoystick>();

            if (joystickComponent != null)
            {
                this.floatingJoystick = joystickComponent;
            }
            else
            {
                Debug.LogError("Floating Joystick component not found as a child of PlayerUI.");
            }
        }
        else
        {
            Debug.LogError("PlayerUI not found in the hierarchy.");
        }
    }

    private void LateUpdate()
    {
        Vector2 input = new Vector2(floatingJoystick.Horizontal, floatingJoystick.Vertical);

        currentX += input.x * cameraSensitivity * Time.deltaTime;
        currentY -= input.y * cameraSensitivity * Time.deltaTime;

        currentY = Mathf.Clamp(currentY, YMin, YMax);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        transform.rotation = rotation;

        Vector3 direction = new Vector3(0, 0, -cameraDistance);

        transform.position = lookAt.position + rotation * direction;
    }








    /*
     *  currentX += floatingJoystick.Horizontal * cameraSensitivity * Time.deltaTime;
        currentY -= floatingJoystick.Vertical * cameraSensitivity * Time.deltaTime; // Use '-' to invert vertical movement

        // Clamp the vertical rotation to a specified range
        currentY = Mathf.Clamp(currentY, YMin, YMax);

        // Calculate the rotation for the camera
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Set the rotation of the camera
        transform.rotation = rotation;

        // Calculate the direction for the camera
        Vector3 direction = new Vector3(0, 0, -cameraDistance);

        // Set the position of the camera
        transform.position = lookAt.position + rotation * direction;
    */


}
