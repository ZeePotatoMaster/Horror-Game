using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WifiTask : Interactable
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Transform camTransform;
    [SerializeField] private GameObject staticTV;
    [SerializeField] private Material staticMaterial;
    [SerializeField] private Slider s;
    [SerializeField] private Material winMaterial;

    private float camTimer = 0f;
    private Transform playerCamTransform;

    private PlayerBase pb;
    private PlayerInput pi;

    private bool canUse = true;

    private GameObject mainCanvas;

    float correctValue;

    void Start()
    {
        cam.enabled = false;
        correctValue = Random.Range(0f, 1f);
        s.value = Random.Range(0f, 1f);
        OnValueChanged(s.value);
    }
    public override void FinishInteract(GameObject player)
    {
        if (!canUse) return;

        pb = player.GetComponent<PlayerBase>();
        pi = player.GetComponent<PlayerInput>();

        pi.DeactivateInput();

        canUse = false;

        playerCamTransform = pb.playerCamera.transform;

        camTimer = 0.99f;
        pb.playerCamera.enabled = false;
        cam.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        canvas.SetActive(true);

        mainCanvas = GameObject.Find("Canvas");
        mainCanvas.SetActive(false);
    }

    public void OnLeave()
    {
        canUse = true;

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
            camTimer = Mathf.Clamp(camTimer - changeSpeed, 0f, 1f);
            Debug.Log(camTimer);
        }

        if (canUse && cam.enabled && camTimer == 1f)
        {
            pb.playerCamera.enabled = true;
            cam.enabled = false;

            pi.ActivateInput();

            if (Mathf.Abs(s.value - correctValue) < 0.05f) ActivateWifiBoxRpc();
        }

        float randY = Random.Range(0f, 0.2f);
        float randX = Random.Range(0f, 0.2f);

        if (canvas.activeSelf) staticMaterial.mainTextureOffset = new Vector2(randX, randY);
    }

    [Rpc(SendTo.Everyone)]
    void ActivateWifiBoxRpc()
    {
        GetComponent<MeshRenderer>().material = winMaterial;
        canUse = false;
    }

    public void OnValueChanged(float f)
    {
        SpriteRenderer sp = staticTV.GetComponent<SpriteRenderer>();
        Color c = sp.color;
        c.a = Mathf.Abs(f - correctValue);
        staticTV.GetComponent<SpriteRenderer>().color = c;
    }
}
