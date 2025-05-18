using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PropRandomizer : NetworkBehaviour
{

    [SerializeField] private GameObject[] props;
    [SerializeField] private float[] chances;
    private bool loaded = false;


    public void LoadProp()
    {
        loaded = true;

        float roll = Random.Range(0f, 1f);
        float chance = 0f;
        int prop = -1;

        for (int i = 0; i < chances.Length; i++)
        {
            chance += chances[i];
            if (roll <= chance)
            {
                if (props[i] != null) prop = i;
                break;
            }
        }

        if (prop == -1) return;

        if (props[prop].tag == "Painting") Paintings.instance.GetComponent<Paintings>().totalPaintings.Value++;


        if (props[prop].GetComponent<NetworkObject>() == null) MakePropRpc(prop);
        else
        {
            NetworkObject o = Instantiate(props[prop], this.transform).GetComponent<NetworkObject>();
            o.Spawn(true);
            o.TryRemoveParent(true);
        }
    }

    void Update()
    {
        if (!IsServer) return;
        if (TheOvergame.instance.gameStarted && !loaded) LoadProp();
    }

    [Rpc(SendTo.Everyone)]
    void MakePropRpc(int i)
    {
        GameObject o = Instantiate(props[i], this.transform);
    }
}
