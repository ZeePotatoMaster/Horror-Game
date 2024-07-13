using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunItem : WorldItem
{
    [SerializeField] private NetworkVariable<int> currentAmmo = new NetworkVariable<int>(6, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<int> totalAmmo = new NetworkVariable<int>(18, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void SetupItem(NetworkObject item) {
        Gun gun = item.GetComponent<Gun>();
        gun.TotalAmmo = totalAmmo.Value;
        gun.CurrentAmmo = currentAmmo.Value;
    }

    public override void SetupWorldItem(NetworkObject item)
    {
        Gun gun = item.GetComponent<Gun>();
        currentAmmo.Value = gun.CurrentAmmo;
        totalAmmo.Value = gun.TotalAmmo;
        Debug.Log("current: " + currentAmmo.Value + " total: " + totalAmmo.Value);
    }
}
