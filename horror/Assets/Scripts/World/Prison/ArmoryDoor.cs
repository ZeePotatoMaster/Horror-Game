using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArmoryDoor : DoorScript
{
    private int generatorsDone = 0;
    [SerializeField] private int generatorsNeeded;
    [SerializeField] private GameObject completedText;
    private bool activated = false;

    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float explosionShake;
    [SerializeField] private float explosionShakeDuration;
    [SerializeField] private GameObject particles;
    [SerializeField] private LayerMask whatIsPlayer;

    [Rpc(SendTo.Server)]
    public void CompleteGeneratorRpc()
    {
        generatorsDone++;
        if (generatorsDone == generatorsNeeded)
        {
            ActivateDoorRpc();
            TextRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    void ActivateDoorRpc()
    {
        activated = true;
        //anims here
    }

    public override void FinishInteract(GameObject player)
    {
        InventoryManager m = player.GetComponent<InventoryManager>();
        ItemInSlot s = m.GetItemInSlot(m.selectedSlot);
        if (s == null) return;

        if (s.item.itemId == 8)
        {
            m.DestroyItem(m.selectedSlot);
            StartExplosionRpc();
            return;
        }

        else if (s.item.itemId != 6 || !activated) return;
        base.FinishInteract(player);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TextRpc()
    {
        completedText = Instantiate(completedText, GameObject.Find("Canvas").transform);
        Invoke(nameof(DestroyText), 2f);
    }

    void DestroyText()
    {
        Destroy(completedText);
    }

    [Rpc(SendTo.Server)]
    void StartExplosionRpc()
    {
        //animations or whatever
        Invoke(nameof(Explode), 5f);
    }

    private void Explode()
    {
        ParticleRpc();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRange, whatIsPlayer);
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
        p.GetComponent<PlayerHealth>().TryDamageServerRpc(explosionDamage);
        p.GetComponent<PlayerBase>().StartShake(explosionShakeDuration, explosionShake);
    }

    [Rpc(SendTo.Everyone)]
    private void ParticleRpc()
    {
        Instantiate(particles, this.transform.position, this.transform.rotation);
    }
}
