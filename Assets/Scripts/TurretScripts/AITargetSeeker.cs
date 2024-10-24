using UnityEngine;

public class AITargetSeeker : MonoBehaviour, ITargetSeeker
{
    public Vector3 FindTargetPosition()
    {
        return Vector3.back;
    }
}
