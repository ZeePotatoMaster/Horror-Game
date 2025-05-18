using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Light : NetworkBehaviour
{

    [SerializeField] private Transform viewmodel, worldmodel;
    private PlayerBase pb;
    private float timer;
    [SerializeField] private Material green;
    [HideInInspector] public NetworkVariable<bool> ready = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private lightminigame lm;
    [SerializeField] private GameObject crosshair;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) {
            viewmodel.gameObject.SetActive(false);
            return;
        }
        pb = this.transform.parent.gameObject.GetComponent<PlayerBase>();
        viewmodel.SetParent(pb.playerCamera.transform, true);

        pb.canSwapWeapons = false;
        pb.canMove = false;

        timer = UnityEngine.Random.Range(90, 200);
        Invoke(nameof(ReadyUp), timer);

        GameObject canvas = GameObject.Find("Canvas");
        GameObject UI = Instantiate(crosshair, Vector3.zero, Quaternion.identity);
        UI.transform.SetParent(canvas.transform, false);

        int invisLayer = LayerMask.NameToLayer("Invisible");
        var children = worldmodel.GetComponentsInChildren<Transform>();
        foreach (var child in children) child.gameObject.layer = invisLayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (pb.attacked) Shoot();
    }

    private void ReadyUp()
    {
        ready.Value = true;
        viewmodel.GetComponent<MeshRenderer>().material = green;
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(pb.playerCamera.transform.position, new Vector3(pb.playerCamera.transform.forward.x, pb.playerCamera.transform.forward.y, pb.playerCamera.transform.forward.z), out hit))
        {
            Debug.Log(hit.transform.name);

            if (hit.transform.tag == "Player")
            {
                NetworkObject p = hit.transform.GetComponent<NetworkObject>();
                lm.OnHit(this.NetworkObject, p);
            }
        }
    }

    public void Reset()
    {
        pb.canSwapWeapons = true;
        pb.canMove = true;
    }

    public void GetMinigameScript(lightminigame mini)
    {
        lm = mini;
    }
}
