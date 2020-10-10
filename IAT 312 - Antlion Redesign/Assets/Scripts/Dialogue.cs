using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue {
    [SerializeField] private Sprite antonioSprite;
    [SerializeField] private Sprite antleonSprite;
    [SerializeField] private Sprite antifaSprite;
    private string name;
    private string text;
    private Sprite portrait;

    public Dialogue(string name, string text) {
        this.name = name;
        this.text = text;
        if (string.Equals(name, "Antonio")) {
            portrait = antonioSprite;
        } else if (string.Equals(name, "Antleon")) {
            portrait = antleonSprite;
        } else if (string.Equals(name, "Antifa")) {
            portrait = antifaSprite;
        } else {
            Debug.LogError("Invalid Dialogue speaker name");
        }
    }

    public string GETName() {
        return name;
    }

    public string GETText() {
        return text;
    }

    public Sprite GETPortrait() {
        return portrait;
    }
}
