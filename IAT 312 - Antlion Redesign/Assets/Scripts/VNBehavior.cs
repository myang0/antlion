using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VNBehavior : MonoBehaviour {
    [SerializeField] private EscMenuBehavior escMenuBehavior;
    [SerializeField] private GameObject backgroundEdge;
    [SerializeField] private Text speakerName;
    [SerializeField] private Text speakerWords;
    private List<List<Dialogue>> currentDialogue = new List<List<Dialogue>>();
    private int lineIndex = 0;
    private int dialogueIndex = 0;
    public enum DialogueChapter {
        Intro = 0,
        Chase = 1,
        Desert = 2,
        EndRunPhase = 3,
        StartFightPhase = 4,
        BossStart = 5,
        BossEnd = 6
    }
    void Start()
    {
        List<Dialogue> introDialogues = new List<Dialogue>();
        List<Dialogue> chaseDialogues = new List<Dialogue>();
        List<Dialogue> desertDialogue = new List<Dialogue>();
        List<Dialogue> endOfRunPhaseDialogue = new List<Dialogue>();
        List<Dialogue> spawnInFightPhaseDialogue = new List<Dialogue>();
        List<Dialogue> bossFightStart = new List<Dialogue>();
        List<Dialogue> bossFightEnd = new List<Dialogue>();
        
        //Note, 2.5 line dialogue limit
        introDialogues.Add(new Dialogue("Antonio",
            "It seems that Antleon has prepared a god forsaken labyrinth for me. However, " +
            "my mandibles will wear through any walls like leaves."
        ));
        introDialogues.Add(new Dialogue("Antonio",
            "Although I suppose getting past the rubble would be another matter. The elder " +
            "ants have told me of how Antleon stores his treasures in the tinted walls."
        ));
        introDialogues.Add(new Dialogue("Antonio",
            "Perhaps I should seek those out and use it against him. That'll show that fool " +
            "to summon the wrath of Antonio, son of the legendary hero Antony."
        ));
        introDialogues.Add(new Dialogue("Antonio",
            "Wait a second, what are these crude cryptic drawings in the ground..? Anywho, " +
            "I must make haste if I am to save Queen Antifa."
        ));
        
        chaseDialogues.Add(new Dialogue("Antleon",
            "It seems the puny Antonio has decided to challenge my might. Very well, I will " +
            "ensure that your arrogance will be the end of you."
        ));
        chaseDialogues.Add(new Dialogue("Antonio",
            "Damn it, Antleon is already here. I need to hurry and escape this labyrinth."
        ));
        
        desertDialogue.Add(new Dialogue("Antonio",
        "Finally, I've managed to give Antleon the slip. He should be off of my trail for now. " +
        "Although frankly I doubt the desert nest of flying Antleon jr's will do me any favors."
        ));
        desertDialogue.Add(new Dialogue("Antonio",
            "I better make good use of any equipment I found during my escape. I'll have to fend " +
            "off his underlings if I am to make my way across these sands."
        ));
        desertDialogue.Add(new Dialogue("Antonio",
            "Although I could explore the desert and attempt to find the rumored Antlion " + 
            "slaying sword." 
        ));
        desertDialogue.Add(new Dialogue("Antonio",
            "The elder ants told me tales of how my father, Antony, had slain the Antlion tyrant " +
            "Anthur with a legendary sword capable of levelling sand dunes."
        ));
        desertDialogue.Add(new Dialogue("Antonio",
            "If I dare seek its power, then I must tear down every wall here until I find it. " +
            "However I am unsure if I should be keeping Queen Antifa waiting."
        ));

        endOfRunPhaseDialogue.Add(new Dialogue("Antonio",
            "There it is! The path to the chambers where Queen Antifa is locked away must be " +
            "down the hall. I must hurry."
        ));
        
        spawnInFightPhaseDialogue.Add(new Dialogue("Antonio",
            "It appears that in order to rescue my Queen, I must first defeat Antleon. Few ants " +
            "have survived a battle against an Antlion, and my father is the only ant to have slain " +
            "one."
        ));
        spawnInFightPhaseDialogue.Add(new Dialogue("Antonio",
            "I can only pray that my strength will remain resolute when my might is tested. " +
            "The elders warned me that when facing off against an Antlion's sand spit, an ant must " +
            "find shelter."
        ));
        spawnInFightPhaseDialogue.Add(new Dialogue("Antonio",
            "However if I cannot seek cover, then I shall simply tear down the sand using my " +
            "mandibles and a hero's resolve. However ideally my weapons should also be able to " + 
            "destroy the sand as well."
        ));
        
        bossFightStart.Add(new Dialogue("Antleon",
            "To have the audacity to kill my children and defile my sands... I, Antleon, son of " + 
            "the late Antlion King Anthur, will end your miserable and pitiful life."
        ));
        bossFightStart.Add(new Dialogue("Antonio",
            "I'm afraid dying isn't quite on my schedule, Antleon. You will pay for your crimes of " + 
            "abducting Queen Antifa. You will meet your end the same way your tyrant father did. By the " + 
            "mandibles of an ant."
        ));
        bossFightStart.Add(new Dialogue("Antleon",
            "GRRRRRRRRRAAAAAAAHHHHHHHHHHHHHHHHHHHHH"
        ));
        
        bossFightEnd.Add(new Dialogue("Antleon",
            "..."
        ));
        bossFightEnd.Add(new Dialogue("Antleon",
            "...."
        ));
        bossFightEnd.Add(new Dialogue("Antleon",
            "....."
        ));
        bossFightEnd.Add(new Dialogue("Antonio",
            "And so he has fallen. Like father, like son. That must be the key for unlocking " + 
            "Queen Antifa's chamber doors! Finally I will be able to reunite with my love."
        ));
        
        currentDialogue.Add(introDialogues);
        currentDialogue.Add(chaseDialogues);
        currentDialogue.Add(desertDialogue);
        currentDialogue.Add(endOfRunPhaseDialogue);
        currentDialogue.Add(spawnInFightPhaseDialogue);
        currentDialogue.Add(bossFightStart);
        currentDialogue.Add(bossFightEnd);
        ResumeGame();
        UpdateVNText();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && 
            !escMenuBehavior.isEscMenuActive) {
            lineIndex++;
            if (lineIndex < currentDialogue[dialogueIndex].Count) {
                UpdateVNText();
            } else if (lineIndex == currentDialogue[dialogueIndex].Count) {
                ResumeGame();
            }
        }
    }

    public void UpdateVN(DialogueChapter chapter) {
        dialogueIndex = (int) chapter;
        lineIndex = 0;
        UpdateVNText();
        ResumeGame();
    }

    private void ResumeGame() {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        backgroundEdge.SetActive(!backgroundEdge.activeInHierarchy);
    }

    private void UpdateVNText() {
        speakerName.text = currentDialogue[dialogueIndex][lineIndex].GETName();
        speakerWords.text = currentDialogue[dialogueIndex][lineIndex].GETText();
    }
}
