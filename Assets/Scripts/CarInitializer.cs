using UnityEngine;
using UnityEngine.Events;

public class CarInitializer : MonoBehaviour
{
    public bool isPlayable;
    private ITargetSeeker targetSeeker;
    private PlayerInputController inputController;
    private CarControl playerCarControl;
    private AICarControl aiCarControl;

    public UnityEvent<ITargetSeeker> OnTargetSeekerChanged;

    private void Start()
    {
        OnTargetSeekerChanged ??= new UnityEvent<ITargetSeeker>();
        SetCarPlayability(isPlayable);
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
        isPlayable = !isPlayable;
        OnTargetSeekerChanged?.Invoke(targetSeeker);
    }

    private void AddPlayerRelatedComponents()
    {
        targetSeeker = gameObject.AddComponent<PlayerTargetSeeker>();
        playerCarControl = gameObject.AddComponent<CarControl>();
        inputController = gameObject.AddComponent<PlayerInputController>();
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
        if(inputController != null)
        {
            Destroy(inputController);
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
