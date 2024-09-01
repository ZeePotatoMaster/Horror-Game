using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryConcept : MonoBehaviour
{

    public Image slot1;
    public Image slot2;
    public Image slot3;
    public Image slot4;
    public Image slot5;

    public Sprite slotSelected;
    public Sprite defaultSprite;

    public int currentSlot;

    void Start()
    {
        currentSlot = 1;
    }

    void Update()
    {

        updateKeySlot();
        updateScrollSlot();

        updateSlotSelections(currentSlot);
    }

    void updateKeySlot() {

        if (Input.GetKeyDown(KeyCode.Alpha1)){

            currentSlot = 1;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)){

            currentSlot = 2;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)){

            currentSlot = 3;
        } else if (Input.GetKeyDown(KeyCode.Alpha4)){

            currentSlot = 4;
        } else if (Input.GetKeyDown(KeyCode.Alpha5)){

            currentSlot = 5;
        }
    }

    void updateScrollSlot() {

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {

            //upper limit of last slot
            if (currentSlot < 5) {
                currentSlot++;
            }
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f) {

            //lower limit of first slot
            if (currentSlot > 1) {
                currentSlot--;
            }
        }
    }

    void updateSlotSelections(int slot) {

        slot1.sprite = defaultSprite;
        slot2.sprite = defaultSprite;
        slot3.sprite = defaultSprite;
        slot4.sprite = defaultSprite;
        slot5.sprite = defaultSprite;
        
        if (slot == 1) {

            slot1.sprite = slotSelected;
        }

        if (slot == 2) {

            slot2.sprite = slotSelected;
        }

        if (slot == 3) {

            slot3.sprite = slotSelected;
        }

        if (slot == 4) {

            slot4.sprite = slotSelected;
        }

        if (slot == 5) {

            slot5.sprite = slotSelected;
        }
    }
}
