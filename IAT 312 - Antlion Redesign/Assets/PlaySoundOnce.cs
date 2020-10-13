using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnce : MonoBehaviour {
    private AudioSource audio;
    // Start is called before the first frame update
    void Start() {
        audio = gameObject.GetComponent<AudioSource>();
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audio.isPlaying) {
            Destroy(gameObject);
        }
    }
}
