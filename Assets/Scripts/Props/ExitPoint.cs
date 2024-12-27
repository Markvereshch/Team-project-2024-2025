using UnityEngine;
using UnityEngine.UI;

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
            InGameUIManager.Instance.SetEvacuationIconFill(currentTime / retreatWaitTime);
            InGameUIManager.Instance.SetEvacuationTimer(string.Format("Evacuation in {0:0.0} sec.", (retreatWaitTime - currentTime)));
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
            InGameUIManager.Instance.SetEvacuationIconFill(0);
            InGameUIManager.Instance.SetEvacuationTimer("");
            isPlayerInside = false;
            currentTime = 0;
        }
    }
}
