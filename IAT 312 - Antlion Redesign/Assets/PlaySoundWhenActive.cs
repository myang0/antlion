using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundWhenActive : MonoBehaviour {
    private AudioSource audio;
    private bool isPlayed = false;
    // Start is called before the first frame update
    void Start() {
        audio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy && !audio.isPlaying && !isPlayed) {
            audio.Play();
            isPlayed = true;
        }
    }
}
