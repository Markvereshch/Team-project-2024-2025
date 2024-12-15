using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    [SerializeField] private float retreatWaitTime = 10f;
    private bool isPlayerInside;
    private float currentTime = 0;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (isPlayerInside)
        {
            currentTime += Time.deltaTime;
            if (currentTime > retreatWaitTime) 
            {
                gameManager.HandleEvacuation();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerInputController>())
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerInputController>())
        {
            isPlayerInside = false;
            currentTime = 0;
        }
    }
}
