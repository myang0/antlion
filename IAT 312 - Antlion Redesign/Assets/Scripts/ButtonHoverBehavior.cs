using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHoverBehavior : MonoBehaviour {
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite hoveredButton;
    [SerializeField] private Sprite unHoveredButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHoveredButton() {
        buttonImage.sprite = hoveredButton;
    }

    public void SetUnHoveredButton() {
        buttonImage.sprite = unHoveredButton;
    }
}
