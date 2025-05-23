using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Crafter : Interactable
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Transform camTransform;
    [SerializeField] private Transform itemSpawnTransform;
    private float camTimer = 0f;
    private Transform playerCamTransform;

    private PlayerBase pb;
    private PlayerInput pi;

    private bool canUse = true;

    private GameObject mainCanvas;

    [SerializeField] private InventoryItem[] items;
    [SerializeField] private int[] scrapNeeded;
    private int selectedItem = 0;
    private NetworkVariable<int> scrap = new NetworkVariable<int>(0);

    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text scrapText;

    void Start()
    {
        cam.enabled = false;
        ChangeSelectedItem(0);
        canvas.SetActive(false);
    }
    public override void FinishInteract(GameObject player)
    {
        if (!canUse) return;

        pb = player.GetComponent<PlayerBase>();
        pi = player.GetComponent<PlayerInput>();

        pi.DeactivateInput();

        SetUseRpc(false);

        playerCamTransform = pb.playerCamera.transform;

        camTimer = 0.99f;
        pb.playerCamera.enabled = false;
        cam.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        canvas.SetActive(true);
        ChangeSelectedItem(0);

        mainCanvas = GameObject.Find("Canvas");
        mainCanvas.SetActive(false);
    }

    [Rpc(SendTo.Everyone)]
    void SetUseRpc(bool set)
    {
        canUse = set;
    }

    public void OnLeave()
    {
        SetUseRpc(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canvas.SetActive(false);

        camTimer = 0.01f;
        mainCanvas.SetActive(true);
    }

    void Update()
    {
        if (camTimer < 1 && camTimer > 0)
        {
            float changeSpeed = canUse ? -Time.deltaTime : Time.deltaTime;
            cam.transform.SetPositionAndRotation(Vector3.Lerp(camTransform.position, playerCamTransform.position, camTimer), Quaternion.Lerp(camTransform.rotation, playerCamTransform.rotation, camTimer));
            camTimer = Mathf.Clamp(camTimer - changeSpeed * 3, 0f, 1f);
            Debug.Log(camTimer);
        }

        if (canUse && cam.enabled && camTimer == 1f)
        {
            pb.playerCamera.enabled = true;
            cam.enabled = false;

            pi.ActivateInput();
        }

    }

    public void ChangeSelectedItem(int change)
    {
        if (selectedItem + change >= items.Length || selectedItem + change < 0) return;

        selectedItem += change;
        itemImage.sprite = items[selectedItem].image;
        scrapText.text = "Scrap needed: " + scrapNeeded[selectedItem] + " scrap: " + scrap.Value;
    }

    public void Craft()
    {
        if (scrap.Value < scrapNeeded[selectedItem]) return;

        CreateItemRpc(scrapNeeded[selectedItem], items[selectedItem].itemId);
        OnLeave();
    }

    [Rpc(SendTo.Server)]
    void CreateItemRpc(int s, int itemId)
    {
        scrap.Value -= s;
        NetworkObject i = Instantiate(ItemHolder.instance.inventoryItems[itemId].worldItemObject, itemSpawnTransform.position, itemSpawnTransform.rotation);
        i.Spawn(true);
    }

    [Rpc(SendTo.Server)]
    public void AddScrapRpc(int amount)
    {
        scrap.Value += amount;
    }
}
