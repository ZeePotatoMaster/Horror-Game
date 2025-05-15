using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PropRandomizer : NetworkBehaviour
{

    [SerializeField] private GameObject[] props;
    [SerializeField] private float[] chances;


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        float roll = Random.Range(0f, 1f);
        float chance = 0f;
        int prop = -1;

        for (int i = 0; i < chances.Length; i++)
        {
            chance += chances[i];
            if (roll <= chance) {
                if (props[i] != null) prop = i;
                break;
            } 
        }

        if (prop == -1) return;

        if (props[prop].tag == "Painting") Paintings.instance.totalPaintings.Value++;

        MakePropRpc(prop);
    }

    [Rpc(SendTo.Everyone)]
    void MakePropRpc(int i)
    {
        GameObject o = Instantiate(props[i], this.transform);
        if (o.GetComponent<NetworkObject>() != null && IsServer) o.GetComponent<NetworkObject>().Spawn(true);
    }
}
