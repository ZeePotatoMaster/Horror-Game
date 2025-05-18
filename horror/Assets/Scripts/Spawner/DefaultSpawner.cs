using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class DefaultSpawner : NetworkBehaviour
{
    private bool isSpawning = false;
    [SerializeField] private NetworkObject spawnObject;
    [SerializeField] private float avgSpawnTime;
    [SerializeField] private float spawnRandomTime;
    [SerializeField] private bool spawnOnce;
    [SerializeField] private float spawnChance;

    private bool firstSpawn = false;

    void Update()
    {
        if (!IsServer) return;
        if (!TheOvergame.instance.gameStarted) return;

        if (!firstSpawn) {
            StartSpawn();
            firstSpawn = true;
        }
        if (!isSpawning && !spawnOnce) StartSpawn();
            else if (!isSpawning && spawnOnce) Destroy(this.gameObject);
    }

    void StartSpawn()
    {
        isSpawning = true;
        if (!ShouldSpawn()) {
            Invoke(nameof(ResetSpawn), GetSpawnTime());
            return;
        }

        Invoke(nameof(Spawn), GetSpawnTime());
    }

    bool ShouldSpawn()
    {
        float chance = Random.Range(0f, 1f);
        if (chance <= spawnChance) return true;
        else return false;
    }

    float GetSpawnTime()
    {
        float rand = Random.Range(-spawnRandomTime, spawnRandomTime);
        return avgSpawnTime + rand;
    }

    void Spawn()
    {
        NetworkObject o = Instantiate(spawnObject, this.transform.position, this.transform.rotation);
        o.Spawn(true);
        isSpawning = false;
    }

    void ResetSpawn()
    {
        isSpawning = false;
        if (spawnOnce) Destroy(this.gameObject);
    }
}
