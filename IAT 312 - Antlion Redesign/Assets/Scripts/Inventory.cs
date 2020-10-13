using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    public int numItemsCarried = 0;

    public bool[] isFull;
    public GameObject[] items;

    public int selectedItemIndex;
    public GameObject selectedItem;

    public Camera cam;

    private Vector3 mousePos;

    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6
    };

    void Update() {
        if (Time.timeScale != 0) {
            if (Input.mouseScrollDelta.y > 0) {
                if (numItemsCarried > 0) nextItem();
            } else if (Input.mouseScrollDelta.y < 0) {
                if (numItemsCarried > 0) prevItem();
            }

            GameObject player = GameObject.FindWithTag("Player");
            if (Input.GetMouseButtonDown(0) && player && selectedItem != null) {
                // if (Input.GetMouseButtonDown(0) & isFightScene() && player && selectedItem != null) {
                useCurrentItem();
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                DropSelectedItem();
            }

            for(int i = 0 ; i < keyCodes.Length; i ++ ){
                if (Input.GetKeyDown(keyCodes[i])) {
                    SwitchToItemWithIndex(i);
                    i = keyCodes.Length;
                }
            }
        }
    }

    public bool DoesSlotHaveWeapon(int index) {
        if (index <= 5) return items[index] != null;
        
        Debug.LogError("Invalid Inventory Index");
        return false;
    }

    void prevItem() {
        selectedItemIndex -= 1;
        selectedItemIndex = (selectedItemIndex < 0) ? numItemsCarried - 1 : selectedItemIndex;

        selectedItem = items[selectedItemIndex];
    }

    void nextItem() {
        selectedItemIndex = (selectedItemIndex + 1) % numItemsCarried;

        selectedItem = items[selectedItemIndex];
    }

    void SwitchToItemWithIndex(int index) {
        if (numItemsCarried > 0 && index < numItemsCarried) {
            selectedItemIndex = index;
            selectedItem = items[selectedItemIndex];
        }
    }

    void useCurrentItem() {
        // if (selectedItem == null) return;

        PickupItem itemComponent = selectedItem.GetComponent<PickupItem>();
        itemComponent.Use();

        if (itemComponent.remainingUses <= 0) removeSelectedItem();
    }

    void DropSelectedItem() {
        PickupItem droppedItem = selectedItem.GetComponent<PickupItem>();
        GameObject player = GameObject.FindWithTag("Player");

        Vector3 newItemPos = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            0
        );

        droppedItem.transform.position = newItemPos;

        droppedItem.hitbox.enabled = true;
        droppedItem.sprite.enabled = true;
        droppedItem.isDropped = true;

        removeSelectedItem();
    }

    void removeSelectedItem() {
        // TODO: remove hardcoding of array lengths
        bool[] isFullCopy = new bool[6];
        GameObject[] itemsCopy = new GameObject[6];

        int sweep = 0;
        for (int i = 0; i < numItemsCarried; i++) {
            if (i == selectedItemIndex) {

            } else {
                isFullCopy[sweep] = isFull[i];
                itemsCopy[sweep] = items[i];

                sweep++;
            }
        }

        numItemsCarried--;

        isFull = isFullCopy;
        items = itemsCopy;

        selectedItem = (numItemsCarried > 0) ? items[0] : null;
        selectedItemIndex = 0;
    }
}
