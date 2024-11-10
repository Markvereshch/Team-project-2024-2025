using UnityEngine;

public class PatrolCarState : ICarState
{
    private AICarControl carControl;
    
    public void EnterState(AICarControl carControl)
    {
        this.carControl = carControl;
        carControl.Seeker.OnTargetLost.AddListener(HandleTargetLoss);
    }

    public void SetInitialTarget(GameObject target)
    {
        carControl.carMovement.SetTarget(target);
    }

    public void ExitState()
    {
        HandleTargetLoss();
    }

    public void UpdateState()
    {
        if (carControl.TurretControl != null)
        {
            carControl.TurretControl.Move();
        }
        if (carControl.Weapon != null && carControl.Seeker.Target != null && carControl.TurretControl.IsTargetInSight(carControl.Seeker.Target))
        {
            carControl.Weapon.Shoot(true);
        }
        if (carControl.Seeker.Target != null)
        {
            carControl.carMovement.SetTarget(carControl.Seeker.Target);
        }
    }

    private void HandleTargetLoss()
    {
        carControl.carMovement.SetTarget(null);
    }
}
