using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("Progress bars")]
    [SerializeField] private BarScript healthbar;
    [SerializeField] private BarScript ammobar;
    [Header("Ammo currently in use icon")]
    [SerializeField] private List<Sprite> equippedGunAmmoImages = new List<Sprite>();
    [SerializeField] private Image ammoIcon;
    [Header("Information")]
    [SerializeField] private List<Image> environmentInfoIcons = new List<Image>();
    [SerializeField] private Sprite defaultInteractionIcon;
    [SerializeField] private InformationList informationList;
    [SerializeField] private InformationList objectiveInfoList;
    [Header("Evacuation")]
    [SerializeField] private Image evacuationIcon;
    [SerializeField] private TMP_Text timerText;
    [Header("Colors")]
    [SerializeField] private Color enemyColors = Color.red;
    [SerializeField] private Color allyColors = Color.blue;
    [Header("Radar")]
    [SerializeField] private Camera radarCamera;
    public Color EnemyColor { get { return enemyColors; } }
    public Color AllyColor { get { return allyColors; } }
    public Camera MinimapCamera { get { return radarCamera; } }
    public InformationList InformationList { get { return informationList; } }
    public InformationList ObjectiveInfoList { get { return objectiveInfoList; } }
    public GameObject Player {
        get { return player; }

        set
        {
            player = value;
            var health = player.GetComponent<VehicleHealth>();
            health.OnDamaged += UpdateHealthBar;

            var stats = player.GetComponent<VehicleStats>();
            healthbar.MaxValue = stats.maxHealth;
            healthbar.CurrentValue = stats.maxHealth;
        }
    }
    public static InGameUIManager Instance { get; private set; }

    private Dictionary<IconType, int> iconTypePosition = new Dictionary<IconType, int>();
    private GunBaseScript equippedGun;
    private GameObject player;
    private PickUpManager pickUpManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Cursor.visible = false;
        ammobar.gameObject.SetActive(false);
        pickUpManager = FindAnyObjectByType<PickUpManager>();
        pickUpManager.OnWeaponEquipped += HandleWeaponEquipment;
    }

    public void SetInfoIcon(Sprite sprite, IconType iconType)
    {
        if (sprite == null)
        {
            if (iconTypePosition.TryGetValue(iconType, out var index))
            {
                environmentInfoIcons[index].gameObject.SetActive(false);
                iconTypePosition.Remove(iconType);
            }

            if (iconTypePosition.Count > 0 && environmentInfoIcons[1].gameObject.activeSelf) 
            {
                environmentInfoIcons[0].sprite = environmentInfoIcons[1].sprite;
                environmentInfoIcons[0].gameObject.SetActive(true);
                environmentInfoIcons[1].gameObject.SetActive(false);
                iconTypePosition.Clear();
            }
            return;
        }

        if (iconType == IconType.PickableObject)
        {
            if (!iconTypePosition.ContainsKey(IconType.PickableObject))
            {
                SetIcon(sprite, IconType.PickableObject, GetAvailableSlot());
            }
            else
            {
                environmentInfoIcons[iconTypePosition[IconType.PickableObject]].sprite = sprite;
            }
        }
        else if (iconType == IconType.Interaction)
        {
            if (iconTypePosition.ContainsKey(IconType.PickableObject))
            {
                SetIcon(sprite, IconType.Interaction, GetAvailableSlot());
            }
            else
            {
                SetIcon(sprite, IconType.Interaction, GetAvailableSlot());
            }
        }
    }

    public void SetDefaultInteractionIcon(bool isSet)
    {
        if (isSet)
            SetInfoIcon(defaultInteractionIcon, IconType.Interaction);
        else
            SetInfoIcon(null, IconType.Interaction);
    }

    public void SetEvacuationIconFill(float fillAmount)
    {
        if (fillAmount == 0)
            evacuationIcon.gameObject.SetActive(false);
        else if(fillAmount > 0 && !evacuationIcon.gameObject.activeSelf)
            evacuationIcon.gameObject.SetActive(true);

        evacuationIcon.fillAmount = fillAmount;
    }

    public void SetEvacuationTimer(string text)
    {
        timerText.text = text;
    }

    private void SetIcon(Sprite sprite, IconType iconType, int index)
    {
        if (iconTypePosition.TryGetValue(iconType, out var slot))
        {
            environmentInfoIcons[slot].sprite = sprite;
            environmentInfoIcons[slot].gameObject.SetActive(true);
            iconTypePosition[iconType] = slot;
        }
        else
        {
            environmentInfoIcons[index].sprite = sprite;
            environmentInfoIcons[index].gameObject.SetActive(true);
            iconTypePosition[iconType] = index;
        }
    }

    private int GetAvailableSlot()
    {
        for (int i = 0; i < environmentInfoIcons.Count; i++)
        {
            if (!environmentInfoIcons[i].gameObject.activeSelf || iconTypePosition.Count == 0)
            {
                return i;
            }
        }
        return 0;
    }

    private void HandleWeaponEquipment(GunBaseScript gunScript)
    {
        if (gunScript == null)
        {
            ammobar.gameObject.SetActive(false);
        }
        else
        {
            equippedGun = gunScript;
            ammobar.SetBar(gunScript.currentClip, gunScript.reloadConfig.clipSize);
            gunScript.OnShoot.AddListener(UpdateAmmoBar);
            gunScript.gameObject.GetComponent<IReloadable>().OnReloadEnd += UpdateAmmoBar;
            UpdateAmmoBar();
            SetAmmoIcon(gunScript.weaponConfig.shootableResource);
        }
    }

    private void UpdateAmmoBar()
    {
        ammobar.UpdateBar(equippedGun.currentClip);
    }

    private void UpdateHealthBar(float valueToRemove, GameObject source)
    {
        healthbar.UpdateBar(healthbar.CurrentValue - valueToRemove);
    }

    private void SetAmmoIcon(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.ShotgunAmmo:
                ammoIcon.sprite = equippedGunAmmoImages[0];
                break;
            case ResourceType.MachineGunAmmo:
                ammoIcon.sprite = equippedGunAmmoImages[1];
                break;
            case ResourceType.ExplosiveAmmo:
                ammoIcon.sprite = equippedGunAmmoImages[2];
                break;
        }
    }
}

public enum IconType
{
    PickableObject,
    Interaction,
}
