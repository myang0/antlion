using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarSlotBehavior : MonoBehaviour {
    [SerializeField] private int slotIndex;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Slider slider;
    private GameObject weaponObject;
    private PickupItem weaponScript;
    [SerializeField] private GameObject durabilityBar;
    private Image image;

    [SerializeField] private Image arrowImg;

    // Start is called before the first frame update
    void Start() {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory) {
            if (inventory.DoesSlotHaveWeapon(slotIndex)) {
                durabilityBar.SetActive(true);
                weaponObject = inventory.items[slotIndex];
                weaponScript = weaponObject.GetComponent<PickupItem>();
                image.overrideSprite = weaponObject.GetComponent<SpriteRenderer>().sprite;
                slider.maxValue = weaponScript.baseUses;
                slider.value = weaponScript.remainingUses;
                image.color = new Color(255, 255, 255, 1);

                if (inventory.selectedItemIndex == slotIndex) {
                    arrowImg.enabled = true;
                } else {
                    arrowImg.enabled = false;
                }

            } else {
                image.overrideSprite = null;
                image.color = new Color(255, 255 ,255, 0);
                durabilityBar.SetActive(false);

                arrowImg.enabled = false;
            }
        }    
    }
}
