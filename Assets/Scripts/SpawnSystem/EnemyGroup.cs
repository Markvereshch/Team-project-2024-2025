using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField] private float despawnDistance = 70f;
    [SerializeField] private float despawnTimer = 15f;

    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    private int groupSize = 0;
    [SerializeField] private int enemiesNearPlayer = 0;
    private Transform player;
    private Coroutine retreatCoroutine;
    public UnityEvent<EnemyGroup> OnGroupDestroyed = new UnityEvent<EnemyGroup>();

    public void SetPlayer(GameObject player)
    {
        this.player = player.transform;
    }

    public void AddEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            enemy.GetComponent<VehicleHealth>().OnDie += DecreaseGroupSize;
            groupSize++;
            var aiTargetSeeker = enemy.GetComponent<AITargetSeeker>();
            aiTargetSeeker.OnTargetLost.AddListener(DecreaseNearEnemies);
            aiTargetSeeker.OnTargetFound.AddListener(IncreaseNearEnemies);
        }
    }

    private void DecreaseGroupSize()
    {
        groupSize--;
        if (groupSize < 1)
        {
            enemies.Clear();
            OnGroupDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
    }

    private void DecreaseNearEnemies(GameObject target)
    {
        if (target == null || !target.GetComponentInParent<PlayerInputController>())
            return;

        enemiesNearPlayer--;
        if (enemiesNearPlayer < 1)
        {
            float minDistanceToEnemy = float.MaxValue;
            foreach (var enemy in enemies)
            {
                if (enemy != null && !enemy.GetComponent<VehicleHealth>().IsDead)
                {
                    minDistanceToEnemy = Mathf.Min(minDistanceToEnemy, Vector3.Distance(player.position, enemy.transform.position));
                }
            }
            if (player.gameObject.GetComponent<VehicleHealth>().IsDead || minDistanceToEnemy > despawnDistance)
            {
                StartCoroutine(RetreatCoroutine());
            }
        } 
    }

    private void IncreaseNearEnemies(GameObject target)
    {
        if (target == null || !target.GetComponentInParent<PlayerInputController>())
            return;

        enemiesNearPlayer++;
        if (retreatCoroutine != null)
        {
            StopCoroutine(retreatCoroutine);
        }
    }

    public IEnumerator RetreatCoroutine()
    {
        yield return new WaitForSeconds(despawnTimer);
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();
        OnGroupDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}
