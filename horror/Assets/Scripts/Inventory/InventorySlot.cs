using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    private Image image;
    public Color selectColor, notSelectColor;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public void Select() {
        image.color = selectColor;
    }

    public void Deselect() {
        image.color = notSelectColor;
    }
}
