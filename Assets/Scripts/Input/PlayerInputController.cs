using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour, IVehicleController
{
    private GameInput gameInput;
    private VehicleHealth entityHealth;
    private CarControl carControl;
    private PickUpManager pickUpManager;
    private HeadlightsController headlightsController;

    private bool isBraking;
    private bool isTurretFreezed;
    private bool isShooting;
    private bool isHeadlightsActive;

    public TurretControl TurretControl { get; set; }
    public IShootable Weapon {  get; set; }
    public ReloadScript ManualReloading { get; set; }

    private void Awake()
    {
        gameInput = new GameInput();
        gameInput.Enable();
    }

    private void Start()
    {
        entityHealth = GetComponent<VehicleHealth>();
        carControl = GetComponent<CarControl>();
        TurretControl = GetComponentInChildren<TurretControl>();
        Weapon = GetComponentInChildren<IShootable>();
        ManualReloading = GetComponentInChildren<ReloadScript>();
        pickUpManager = FindAnyObjectByType<PickUpManager>();
        headlightsController = GetComponentInChildren<HeadlightsController>();
    }

    private void OnEnable()
    {
        gameInput.Gameplay.Brake.performed += OnBrakePerformed;
        gameInput.Gameplay.Brake.canceled += OnBrakeCanceled;
        gameInput.Gameplay.FreezeTurret.performed += OnFreezeTurretPerformed;
        gameInput.Gameplay.FreezeTurret.canceled += OnFreezeTurretCancelled;
        gameInput.Gameplay.Fire.performed += OnFireActionPerformed;
        gameInput.Gameplay.Fire.canceled += OnFireActionCanceled;
        gameInput.Gameplay.Reload.performed += OnReloadPerformed;
        gameInput.Gameplay.Reload.canceled += OnReloadCanceled;
        gameInput.Gameplay.ChangeTurret.performed += OnChangeTurretPerformed;
        gameInput.Gameplay.Lights.performed += OnLightsPerformed;
    }

    private void OnLightsPerformed(InputAction.CallbackContext context)
    {
        if(!entityHealth.IsDead)
        {
            isHeadlightsActive = !isHeadlightsActive;
            headlightsController.ToggleAllLights(isHeadlightsActive);
        }
    }

    private void OnChangeTurretPerformed(InputAction.CallbackContext context)
    {
        if(!entityHealth.IsDead)
        {
            pickUpManager?.PickUpTurret();
        }
    }

    private void OnDisable()
    {
        gameInput.Gameplay.Brake.performed -= OnBrakePerformed;
        gameInput.Gameplay.Brake.canceled -= OnBrakeCanceled;
        gameInput.Gameplay.FreezeTurret.performed -= OnFreezeTurretPerformed;
        gameInput.Gameplay.FreezeTurret.canceled -= OnFreezeTurretCancelled;
        gameInput.Gameplay.Fire.performed -= OnFireActionPerformed;
        gameInput.Gameplay.Fire.canceled -= OnFireActionCanceled;
        gameInput.Gameplay.Reload.performed -= OnReloadPerformed;
        gameInput.Gameplay.Reload.canceled -= OnReloadCanceled;
        gameInput.Gameplay.ChangeTurret.performed -= OnChangeTurretPerformed;
        gameInput.Gameplay.Lights.performed -= OnLightsPerformed;
    }

    private void OnReloadPerformed(InputAction.CallbackContext context) => ManualReloading.isReloadRequested = true;

    private void OnReloadCanceled(InputAction.CallbackContext context) => ManualReloading.isReloadRequested = false;

    private void OnFreezeTurretPerformed(InputAction.CallbackContext context) => isTurretFreezed = true;

    private void OnFreezeTurretCancelled(InputAction.CallbackContext context) => isTurretFreezed = false;

    private void OnBrakePerformed(InputAction.CallbackContext context) => isBraking = true;

    private void OnBrakeCanceled(InputAction.CallbackContext context) => isBraking = false;

    private void OnFireActionPerformed(InputAction.CallbackContext context) => isShooting = true;

    private void OnFireActionCanceled(InputAction.CallbackContext context) => isShooting = false;

    private void LateUpdate()
    {
        if (entityHealth.IsDead)
        {
            return;
        }

        carControl.Move(isBraking, gameInput.Gameplay.Movement.ReadValue<Vector2>());

        if (TurretControl != null && !isTurretFreezed)
        {
            TurretControl.Move();
        }

        if (Weapon != null)
        {
            Weapon.Shoot(isShooting);
        }
    }
}
