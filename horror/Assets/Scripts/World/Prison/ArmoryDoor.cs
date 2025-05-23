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
        if (s.item.itemId != 6 || !activated) return;
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
}
