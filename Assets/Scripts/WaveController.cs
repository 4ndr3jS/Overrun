using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class MonsterSpawn
{
    public GameObject prefab;

    [Min(1)]
    [Tooltip("Basicly the higher this value is the more often this monster is chosen")]
    public int spawnWeight = 1;
}

public class WaveController : MonoBehaviour
{
    [Header("Monster prefabs")]
    public MonsterSpawn[] monsters;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private float timeInBetweenSpawns = 0.5f;
    [SerializeField] private float timeInBetweenWaves = 5f;
    [SerializeField] private bool startOnPlay = false;

    [SerializeField] private int enemiesFirstWave = 3;
    [SerializeField] private int extraEnemiesWave = 2;

    [Header("Diff scaling")]
    public AnimationCurve healthMultiByWave = AnimationCurve.Linear(1f, 0.5f, 100f, 12f);
    public AnimationCurve damageMultiByWave = AnimationCurve.Linear(1f, 0.4f, 100f, 10f);
    public AnimationCurve speedMultiByWave = AnimationCurve.Linear(1f, 0.7f, 100f, 2.5f);

    [Header("The current wave")]
    [SerializeField] private int currentWave = 0;

    private readonly List<GameObject> aliveEnem = new List<GameObject>();
    private Coroutine waveRoutine;
    private int totalEnemies;
    private int enemiesLefttoSpawn;

    public int CurrentWave => currentWave;
    public int TotalEnemies => totalEnemies;
    public bool HasStarted => waveRoutine != null;

    public int EnemRemaining
    {
        get
        {
            aliveEnem.RemoveAll(enemy => enemy == null);
            return aliveEnem.Count + enemiesLefttoSpawn;
        }
    }

    private void Start()
    {
        if (startOnPlay)
        {
            StartWave();
        }
            
    }

    public void StartWave()
    {
        if (waveRoutine == null)
        {
            waveRoutine = StartCoroutine(RunWaves());
        }
            
    }

    public void StopWaves()
    {
        if (waveRoutine != null)
             StopCoroutine(waveRoutine);

        waveRoutine = null;
    }

    public void ResetWaves()
    {
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        foreach (GameObject enemy in aliveEnem)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        aliveEnem.Clear();

        currentWave = 0;
        totalEnemies = 0;
        enemiesLefttoSpawn = 0;
    }

    private IEnumerator RunWaves() 
    {
        while (HasMonsterPrefab())
        {
            currentWave++;

            PlayerVitals.Instance?.RecordHighestWave(currentWave);

            int enemCount = enemiesFirstWave + (currentWave - 1) * extraEnemiesWave;

            totalEnemies = enemCount;
            enemiesLefttoSpawn = enemCount;

            for (int i = 0; i < enemCount; i++)
            {
                SpawnEnemy();
                enemiesLefttoSpawn--;

                yield return new WaitForSeconds(timeInBetweenSpawns);
            }

            while(aliveEnem.Count > 0)
            {
                aliveEnem.RemoveAll(enemy => enemy == null);
                yield return null;
            }

            yield return new WaitForSeconds(timeInBetweenWaves);
        }

        waveRoutine = null;
    }

    private void SpawnEnemy()
    {
        GameObject prefab = GetRandomMonsterPrefab();

        if (prefab == null)
            return;

        Vector3 spawnPosition = transform.position;

        if(spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (spawnPoint != null)
                spawnPosition = spawnPoint.position;
        }

        GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
        aliveEnem.Add(enemy);

        ApplyWaveStats(enemy, prefab);
    }

    private void ApplyWaveStats(GameObject enemy, GameObject originalPrefab)
    {
        float healthMutli = healthMultiByWave.Evaluate(currentWave);
        float damageMutli = damageMultiByWave.Evaluate(currentWave);
        float speedMutli = speedMultiByWave.Evaluate(currentWave);

        EnemyHelath prefabHealth = originalPrefab.GetComponent<EnemyHelath>();
        EnemyHelath spawnedHealth = enemy.GetComponent<EnemyHelath>();

        if(prefabHealth != null && spawnedHealth != null)
        {
            float scaleHealh = prefabHealth.maxHealth * healthMutli;
            spawnedHealth.SetMaxHealth(scaleHealh);
        }

        EnemyController prefabController = originalPrefab.GetComponent<EnemyController>();
        EnemyController spawnedController = enemy.GetComponent<EnemyController>();

        if(prefabController != null && spawnedController != null)
        {
            spawnedController.touchDamage = Mathf.Max(1, Mathf.RoundToInt(prefabController.touchDamage * damageMutli));

            spawnedController.moveSpeed = Mathf.Max(0.1f, prefabController.moveSpeed * speedMutli);
        }
    }




    private bool HasMonsterPrefab()
    {
        foreach(MonsterSpawn monster in monsters)
        {
            if (monster != null && monster.prefab != null)
                return true; 
        }

        return false;
    }

    private GameObject GetRandomMonsterPrefab()
    {
        int totalWieght = 0;

        foreach (MonsterSpawn monster in monsters)
        {
            if (monster != null && monster.prefab != null)
                totalWieght += Mathf.Max(1, monster.spawnWeight);
        }

        if (totalWieght == 0)
            return null;

        int rendomValue = Random.Range(0, totalWieght);

        foreach(MonsterSpawn monster in monsters)
        {
            if (monster == null || monster.prefab == null)
                continue;

            rendomValue -= Mathf.Max(1, monster.spawnWeight);

            if (rendomValue < 0)
                return monster.prefab;
        }

        return null;
    }
}