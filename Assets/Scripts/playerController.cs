using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("Player MovementSettings")]
    private CharacterController controller;
    public Vector3 movementVector = Vector3.zero;
    public float playerMagnitude = 0.0f;
    public float playerSpeed = 5.0f;
    private Transform playerTransform;


    [Header("Camera Look Settings")]
    public Camera playerCamera;
    public float mouseXSensitivity = 100.0f;
    public float mouseYSensitivity = 100.0f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;


    [Header("Shoot Settings")]
    public GameObject bulletPrefab;
    public GameObject spawnPoint;
    public GameObject spawnParent;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
        playerTransform = GetComponent<Transform>();
    }

    void Update()
    {
        
        playerMove();
        mouseLook();
        shoot();
    }

    void playerMove()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        movementVector = (camForward * verticalMove + camRight * horizontalMove) * playerSpeed;
        playerMagnitude = movementVector.magnitude;
        if (playerMagnitude > 0)
        {
            controller.SimpleMove(movementVector);
            playerTransform.forward = new Vector3(movementVector.x, 0, movementVector.z).normalized;
        }
    }

    void mouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseXSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseYSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerTransform.transform.localRotation = Quaternion.Euler(0, yRotation, 0);

    }
    
    void shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Object.Instantiate(bulletPrefab, spawnPoint.transform.position, Quaternion.identity, spawnParent.transform);
        }
    }

}
