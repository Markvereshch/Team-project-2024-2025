using UnityEngine;

public class PatrolCarState : ICarState
{
    private AICarControl carControl;
    
    public void EnterState(AICarControl carControl)
    {
        this.carControl = carControl;
        carControl.Seeker.OnTargetLost.AddListener(HandleTargetLoss);
        if (carControl.Weapon != null)
        {
            carControl.Weapon.Shoot(true);
        }
    }

    public void ExitState()
    {
        HandleTargetLoss();
    }

    public void UpdateState()
    {
        if (carControl.TurretControl != null && carControl.Seeker.Target != null)
        {
            carControl.TurretControl.Move();
        }
        if (carControl.Weapon != null && carControl.Seeker.Target != null && carControl.TurretControl.IsTargetInSight(carControl.Seeker.Target))
        {
            carControl.Weapon.Shoot(true);
        }
        if (carControl.Seeker.Target != null)
        {
            carControl.CarMovement.SetTarget(carControl.Seeker.Target);
        }
    }

    private void HandleTargetLoss()
    {
        carControl.CarMovement.SetTarget(null);
    }
}
