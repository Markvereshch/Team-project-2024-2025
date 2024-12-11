using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Transport : MonoBehaviour
{
    public GameObject Player {  
        set
        {
            player = value;
            playerInputController = PlayerInputController.Instance;
            playerInputController.GameInput.Gameplay.Interact.performed += OnStartEscorting;
        }
        get
        {
            return player;
        }
    }
    private VehicleHealth vehicleHealth;
    private PlayerInputController playerInputController;
    private AICarMovement aICarMovement;
    private GameObject player;
    private bool isPlayerNear;
    private bool isMoving;

    void Start()
    {
        vehicleHealth = GetComponent<VehicleHealth>();
        vehicleHealth.Invincible = true;

        aICarMovement = GetComponent<AICarMovement>();
        aICarMovement.IsAwaiting = true;
    }

    private void OnStartEscorting(InputAction.CallbackContext context)
    {
        if (isPlayerNear && !isMoving)
        {
            isMoving = true;
            vehicleHealth.Invincible = false;
            aICarMovement.IsAwaiting = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerInputController>() != null)
        {
            isPlayerNear = true;

            if (isMoving)
            {
                aICarMovement.IsAwaiting = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerInputController>() != null) 
        {
            isPlayerNear = false;

            if (isMoving)
            {
                aICarMovement.IsAwaiting = true;
            }
        }
    }

    private void OnDestroy()
    {
        playerInputController.GameInput.Gameplay.Interact.performed -= OnStartEscorting;
    }
}
