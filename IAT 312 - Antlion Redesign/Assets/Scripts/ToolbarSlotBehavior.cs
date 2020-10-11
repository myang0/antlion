using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarSlotBehavior : MonoBehaviour {
    [SerializeField] private int slotIndex;
    [SerializeField] private Inventory inventory;
    private Image image;

    // Start is called before the first frame update
    void Start() {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory) {
            if (inventory.DoesSlotHaveWeapon(slotIndex)) {
                image.overrideSprite = inventory.items[slotIndex].GetComponent<SpriteRenderer>().sprite;
                image.color = new Color(255, 255, 255, 1);
            } else {
                image.overrideSprite = null;
                image.color = new Color(255, 255 ,255, 0);
            }
        }    
    }
}
