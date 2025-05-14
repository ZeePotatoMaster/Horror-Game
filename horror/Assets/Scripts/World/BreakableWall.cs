using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BreakableWall : NetworkBehaviour
{
    [SerializeField] private float health;

    private void Death()
    {
        this.GetComponent<NetworkObject>().Despawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(float damage)
    {
        health -= damage;
        if (health <= 0) Death();
    }
}
