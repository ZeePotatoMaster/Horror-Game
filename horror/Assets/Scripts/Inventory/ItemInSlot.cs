using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemInSlot : MonoBehaviour
{
    [HideInInspector] public InventoryItem item;
    private Image image;
    [SerializeField] private Image decayImage;
    [SerializeField] private TMP_Text numberText;
    [HideInInspector] public int number = -1;
    private InventoryManager inventoryManager;
    private int thisSlot;

    private float decayTime = 1f;
    private float currentDecay = 0f;
    private bool isDecaying;
    public bool canDrop = true;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (currentDecay >= decayTime) inventoryManager.DestroyItem(thisSlot);

        if (number != -1) numberText.text = number.ToString();

        if (!isDecaying) return;

        currentDecay += Time.deltaTime;
        decayImage.fillAmount = currentDecay / decayTime;
    }

    public void InitializeItem(InventoryItem i, InventoryManager m, int s)
    {
        item = i;
        image.sprite = i.image;
        inventoryManager = m;
        thisSlot = s;
    }

    public void DestroySelf()
    {
        image.sprite = null;
        Destroy(this.gameObject);
    }

    public void SetDecay(float pDecayTime, float pCurrentDecay, bool decayActive)
    {
        isDecaying = decayActive;
        decayTime = pDecayTime;
        currentDecay = pCurrentDecay;
    }
}
