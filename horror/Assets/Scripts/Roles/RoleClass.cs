using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class RoleClass : NetworkBehaviour
{
    public string roleName;
    //public NetworkVariable<FixedString32Bytes> roleName {get; private set;} = new();
    public NetworkVariable<bool> isHuman = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private GameObject text;
    [HideInInspector] public GameObject[] rolePrefabs;

    public void Intro()
    {
        if (!IsOwner) return;
        GameObject canvas = GameObject.Find("Canvas");
        GameObject roleText = Instantiate(text, canvas.transform, false);
        roleText.GetComponent<TextMeshProUGUI>().SetText("You are: " + roleName);
        StartCoroutine(DestroyObject(roleText));
    }

    private IEnumerator DestroyObject(GameObject o)
    {
        yield return new WaitForSeconds(5);

        Destroy(o);
    }
}
