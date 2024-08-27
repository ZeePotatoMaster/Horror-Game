using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public abstract class Interactable : NetworkBehaviour
{
    [SerializeField] private bool humansInteract;
    [SerializeField] private bool isInstant;
    [SerializeField] private float interactTime;
    [SerializeField] private bool destroyOnUse;

    public virtual void OnInteract(GameObject player)
    {
        PlayerBase pb = player.GetComponent<PlayerBase>();

        if (player.GetComponent<RoleClass>().isHuman.Value && !humansInteract) {
            pb.canInteract = false;
            return;
        }
        if (isInstant) {
            FinishInteract(player);
            pb.EndInteract();
            return;
        }

        pb.isInteracting = true;

        pb.interactTick += Time.deltaTime;
        pb.lootImage.GetComponent<Image>().fillAmount = pb.interactTick/interactTime;

        if (pb.interactTick >= interactTime)
        {
            FinishInteract(player);
            pb.EndInteract();
            if (destroyOnUse) DestroySelfServerRpc();
        }
        
    }

    public abstract void FinishInteract(GameObject player);

    [ServerRpc(RequireOwnership = false)]
    private void DestroySelfServerRpc()
    {
        this.GetComponent<NetworkObject>().Despawn(true);
    }
}
