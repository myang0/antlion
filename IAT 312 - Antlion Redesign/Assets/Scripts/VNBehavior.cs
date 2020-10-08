using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VNBehavior : MonoBehaviour {
    [SerializeField] private EscMenuBehavior escMenuBehavior;
    [SerializeField] private GameObject backgroundEdge;
    [SerializeField] private Text speakerName;
    [SerializeField] private Text speakerWords;
    private List<Dialogue> dialogues = new List<Dialogue>();
    private int dialogueIndex = 0;
    void Start()
    {
        //Note, 2.5 line dialogue limit
        dialogues.Add(new Dialogue("Antony",
            "God I better fucking haul ass before that Antlion son of a bitch actually" +
            " eats my ass. I should be able to find some weapons in the tinted walls."
        ));
        dialogues.Add(new Dialogue("Antony",
            "But my priority is to find out where that fucker locked away Antalia." +
            " What kind of sick fuck kidnaps someone's girlfriend instead of just duking it out?"
        ));
        dialogues.Add(new Dialogue("Antony",
            "Hold on a second, what are these strange symbols carved into the ground?" +
            " Oh well, probably just some crackhead's chicken scratchings."
        ));

        ResumeGame();
        UpdateVNText();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && 
            !escMenuBehavior.isEscMenuActive) {
            dialogueIndex++;
            if (dialogueIndex < dialogues.Count) {
                UpdateVNText();
            } else if (dialogueIndex == dialogues.Count) {
                ResumeGame();
            }
        }
    }

    private void ResumeGame() {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        backgroundEdge.SetActive(!backgroundEdge.activeInHierarchy);
    }

    public void UpdateVNText() {
        speakerName.text = dialogues[dialogueIndex].GETName();
        speakerWords.text = dialogues[dialogueIndex].GETText();
    }
}
