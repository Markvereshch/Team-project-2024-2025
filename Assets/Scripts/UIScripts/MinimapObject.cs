using UnityEngine;

public class MinimapObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer miniMapSprite;
    [SerializeField] private Color spriteColor;
    [SerializeField] private bool isFreezed = true;
    [SerializeField] private bool displayOnFullMap = true;

    public bool IsFreezed { 
        get { return isFreezed; } 
        set { isFreezed = value; } 
    }

    public bool DisplayOnFullMap
    {
        get { return displayOnFullMap; }
        set { displayOnFullMap = value; }
    }

    private IDamagable damagableParent;
    
    private void Start()
    {
        damagableParent = GetComponentInParent<IDamagable>();
        if (damagableParent != null)
            damagableParent.OnDie += HideOnDeath;
    }

    private void FixedUpdate()
    {
        if (InGameUIManager.Instance != null && IsFreezed && InGameUIManager.Instance.MinimapCamera != null)
        {
            Quaternion cameraRotation = InGameUIManager.Instance.MinimapCamera.transform.rotation;
            transform.rotation = cameraRotation;
        }
    }

    private void HideOnDeath()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (damagableParent != null) 
            damagableParent.OnDie -= HideOnDeath;
    }

    public Color Color
    {
        get { return spriteColor; }
        set 
        { 
            spriteColor = value; 
            miniMapSprite.color = spriteColor;
        }
    }

    public Sprite Sprite 
    { 
        get { return miniMapSprite.sprite; }
        set 
        { 
            miniMapSprite.sprite = value;
            miniMapSprite.size = new Vector2(10, 10);
        }
    }
}
