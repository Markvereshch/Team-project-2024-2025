using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour, IVehicleController
{
    public GameInput GameInput { get; private set; }
    private VehicleHealth entityHealth;
    private CarControl carControl;
    private PickUpManager pickUpManager;
    private HeadlightsController headlightsController;
    private PauseMenu pauseMenu;

    private bool isBraking;
    private bool isTurretFreezed;
    private bool isShooting;
    private bool isHeadlightsActive;

    public TurretControl TurretControl { get; set; }
    public IShootable Weapon {  get; set; }
    public ReloadScript ManualReloading { get; set; }

    public static PlayerInputController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            GameInput = new GameInput();
            GameInput.Enable();
            Instance = this;
        }
        else
            Destroy(gameObject);
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
        pauseMenu = FindAnyObjectByType<PauseMenu>();

        GameInput.Gameplay.Menu.performed += pauseMenu.OnPauseInput;
        GameInput.Gameplay.Brake.performed += OnBrakePerformed;
        GameInput.Gameplay.Brake.canceled += OnBrakeCanceled;
        GameInput.Gameplay.FreezeTurret.performed += OnFreezeTurretPerformed;
        GameInput.Gameplay.FreezeTurret.canceled += OnFreezeTurretCancelled;
        GameInput.Gameplay.Fire.performed += OnFireActionPerformed;
        GameInput.Gameplay.Fire.canceled += OnFireActionCanceled;
        GameInput.Gameplay.Reload.performed += OnReloadPerformed;
        GameInput.Gameplay.Reload.canceled += OnReloadCanceled;
        GameInput.Gameplay.ChangeTurret.performed += OnChangeTurretPerformed;
        GameInput.Gameplay.Lights.performed += OnLightsPerformed;

        GameInput.Gameplay.Inventory.performed += OnInventoryOpened;
    }

    private void OnDestroy()
    {
        GameInput.Gameplay.Brake.performed -= OnBrakePerformed;
        GameInput.Gameplay.Brake.canceled -= OnBrakeCanceled;
        GameInput.Gameplay.FreezeTurret.performed -= OnFreezeTurretPerformed;
        GameInput.Gameplay.FreezeTurret.canceled -= OnFreezeTurretCancelled;
        GameInput.Gameplay.Fire.performed -= OnFireActionPerformed;
        GameInput.Gameplay.Fire.canceled -= OnFireActionCanceled;
        GameInput.Gameplay.Reload.performed -= OnReloadPerformed;
        GameInput.Gameplay.Reload.canceled -= OnReloadCanceled;
        GameInput.Gameplay.ChangeTurret.performed -= OnChangeTurretPerformed;
        GameInput.Gameplay.Lights.performed -= OnLightsPerformed;
        GameInput.Gameplay.Menu.performed -= pauseMenu.OnPauseInput;
        GameInput.Gameplay.Inventory.performed -= OnInventoryOpened;
    }

    private void OnLightsPerformed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        isHeadlightsActive = !isHeadlightsActive;
        headlightsController.ToggleAllLights(isHeadlightsActive);
    }

    private void OnChangeTurretPerformed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        pickUpManager?.PickUpTurret();
    }

    private void OnReloadPerformed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused || ManualReloading == null) return;
        ManualReloading.isReloadRequested = true;
    } 

    private void OnReloadCanceled(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused || ManualReloading == null) return;
        ManualReloading.isReloadRequested = false;
    }

    private void OnFreezeTurretPerformed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        isTurretFreezed = true;
    }

    private void OnFreezeTurretCancelled(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        isTurretFreezed = false;
    }

    private void OnBrakePerformed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        isBraking = true;

    }

    private void OnBrakeCanceled(InputAction.CallbackContext context)
    {
        isBraking = false;
    }

    private void OnFireActionPerformed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        isShooting = true;
    }

    private void OnFireActionCanceled(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        isShooting = false;
    }

    private void OnInventoryOpened(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver || PauseMenu.isPauseMenuOpened)
            return;

        InventoryUIManager.Instance.OpenInventory();
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused)
        {
            return;
        }

        carControl.Move(isBraking, GameInput.Gameplay.Movement.ReadValue<Vector2>());

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
