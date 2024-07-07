using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RoleClass : NetworkBehaviour
{
    [HideInInspector] public string roleName = "Base Role";
    [HideInInspector] public bool isHuman;
    [SerializeField] private GameObject text;

    private void Start()
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
