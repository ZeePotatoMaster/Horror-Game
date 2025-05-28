using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DownedPlayer : Interactable
{
    private Vector2 lookInput = Vector2.zero;
    private float rotationX = 0;
    public float lookSpeed = 2.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener listener;
    [SerializeField] private float lookXLimit = 90.0f;
    [SerializeField] private NetworkObject playerPrefab;
    private bool attacked = false;

    private float recoverAmount = 0f;
    [SerializeField] private float recoveryNeeded;
    [SerializeField] private GameObject recoverBar;

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attacked = context.action.triggered;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            playerCamera.enabled = false;
            listener.enabled = false;
            return;
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameObject canvas = GameObject.Find("Canvas");
        GameObject recover = canvas.transform.Find("RecoverImage").gameObject;
        if (recover != null) recoverBar = recover;
        else recoverBar = Instantiate(recoverBar, canvas.transform);
    }

    // Update is called once per frame
    void Update()
    {
        rotationX += -lookInput.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);

        recoverBar.GetComponent<Image>().fillAmount = recoverAmount / recoveryNeeded;
        if (attacked) recoverAmount += Time.deltaTime;

        if (recoverAmount >= recoveryNeeded)
        {
            recoverBar.GetComponent<Image>().fillAmount = 0;
            RespawnPlayerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [Rpc(SendTo.Server)]
    void RespawnPlayerRpc(ulong id)
    {
        NetworkObject p = Instantiate(playerPrefab, transform.position, transform.rotation);
        p.SpawnAsPlayerObject(id, false);
        this.NetworkObject.Despawn();
    }

    [Rpc(SendTo.Server)]
    void GotoJailRpc(ulong id)
    {
        Vector3 elevatorSpot = TheOvergame.instance.elevators[id].transform.position;
        transform.position = elevatorSpot + new Vector3(0, 1, 0);
    }

    public override void FinishInteract(GameObject player)
    {
        ulong id = NetworkManager.Singleton.LocalClientId;
        GotoJailRpc(id);
        RespawnPlayerRpc(id);
    }

    public override void OnInteract(GameObject player)
    {
        if (!Prison.instance.GetComponent<Prison>().guards.Contains(player.GetComponent<NetworkObject>().OwnerClientId)) return;
        base.OnInteract(player);
    }
}
