using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ItemRandomizer : NetworkBehaviour
{

    [SerializeField] private InventoryItem[] items;
    [SerializeField] private float[] chances;
    [SerializeField] private Transform[] poses;
    private bool spawned = false;

    void SpawnItem()
    {
        spawned = true;

        foreach (Transform p in poses)
        {
            float roll = Random.Range(0f, 1f);
            float chance = 0f;
            InventoryItem item = null;

            for (int i = 0; i < chances.Length; i++)
            {
                chance += chances[i];
                if (roll <= chance)
                {
                    if (items[i] != null) item = items[i];
                    i = chances.Length;
                }
            }

            if (item == null) return;
            NetworkObject o = Instantiate(item.worldItemObject, p.position, p.rotation);
            o.Spawn(true);
        }
    }

    void Update()
    {
        if (!IsServer) return;
        if (!TheOvergame.instance.gameStarted) return;

        if (!spawned) SpawnItem();
    }
}
