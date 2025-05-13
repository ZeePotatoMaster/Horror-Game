using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    [SerializeField] private float explodeTimer = 0.8f;
    [SerializeField] private GameObject particles;
    [SerializeField] private float bombRange;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private float bombDamage;

    [SerializeField] private float shakeAmount;
    [SerializeField] private float shakeDuration;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Invoke(nameof(Explode), explodeTimer);
    }

    private void Explode()
    {
        ParticleRpc();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, bombRange, whatIsPlayer);
        foreach (var hitCollider in hitColliders)
        {
            ExplodeRpc(RpcTarget.Single(hitCollider.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
            Debug.Log(hitCollider);
        }

        this.GetComponent<NetworkObject>().Despawn(true);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ExplodeRpc(RpcParams rpcParams = default)
    {
        NetworkObject p = NetworkManager.LocalClient.PlayerObject;
        p.GetComponent<PlayerHealth>().TryDamageServerRpc(bombDamage);
        p.GetComponent<PlayerBase>().StartShake(shakeDuration, shakeAmount);
    }

    [Rpc(SendTo.Everyone)]
    private void ParticleRpc()
    {
        Instantiate(particles, this.transform.position, this.transform.rotation);
    }
}
