using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue {
    private string name;
    private string text;

    public Dialogue(string name, string text) {
        this.name = name;
        this.text = text;
    }

    public string GETName() {
        return name;
    }

    public string GETText() {
        return text;
    }
}
