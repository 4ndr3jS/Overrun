using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }

    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    private IDialogueSpeaker activeNPC;
    private int actFrame = -1;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (activeNPC == null || !dialoguePanel.activeSelf)
            return;

        if (Time.frameCount == actFrame)
            return;

        if (choiceContainer.childCount > 0)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            activeNPC.NextLine();
    }

    public void ShowDialogue(bool show)
    {
        dialoguePanel.SetActive(show);
    }

    public void SetActiveNPC(IDialogueSpeaker npc)
    {
        activeNPC = npc;
        actFrame = npc != null ? Time.frameCount : -1;
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);
    }

    public void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
    }
}
