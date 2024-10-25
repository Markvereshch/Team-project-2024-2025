using UnityEngine;
using UnityEngine.Events;

public class CarInitializer : MonoBehaviour
{
    private bool isPlayable;
    private ITargetSeeker targetSeeker;
    private CarControl playerCarControl;
    private AICarControl aiCarControl;

    public UnityEvent<ITargetSeeker> OnTargetSeekerChanged;

    private void Start()
    {
        OnTargetSeekerChanged ??= new UnityEvent<ITargetSeeker>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCarPlayability(!isPlayable);
            isPlayable = !isPlayable;
        }
    }

    public void InitializeCar(bool isPlayerControlled)
    {
        if (isPlayerControlled)
        {
            AddPlayerRelatedComponents();
        }
        else
        {
            AddAIRelatedComponents(); 
        }
        gameObject.AddComponent<ResourceManager>();
    }

    public void SetCarPlayability(bool isPlayerControlled)
    {
        if (isPlayerControlled)
        {
            RemoveAIRelatedComponents();
            AddPlayerRelatedComponents();
        }
        else
        {
            RemovePlayerRelatedComponents();
            AddAIRelatedComponents();
        }
        OnTargetSeekerChanged?.Invoke(targetSeeker);
    }

    private void AddPlayerRelatedComponents()
    {
        targetSeeker = gameObject.AddComponent<PlayerTargetSeeker>();
        playerCarControl = gameObject.AddComponent<CarControl>();
    }

    private void RemovePlayerRelatedComponents()
    {
        if(targetSeeker is PlayerTargetSeeker)
        {
            Destroy(targetSeeker as PlayerTargetSeeker);
            targetSeeker = null;
        }
        if(playerCarControl != null)
        {
            Destroy(playerCarControl);
        }
    }

    private void AddAIRelatedComponents()
    {
        targetSeeker = gameObject.AddComponent<AITargetSeeker>();
        aiCarControl = gameObject.AddComponent<AICarControl>();
    }

    private void RemoveAIRelatedComponents()
    {
        if (aiCarControl != null)
        {
            Destroy(aiCarControl);
        }
        if (targetSeeker is AITargetSeeker)
        {
            Destroy(targetSeeker as AITargetSeeker);
        }
    }
}
