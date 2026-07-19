using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable, IDialogueSpeaker
{
    public string ShopID = "shop_merchant_01";
    public string shopKeeperName = "Merchant";

    // unlimited stock
    public List<int> shopItemIDs = new();

    [Header("First greeting")]
    public NPCDialogue greetingDialogue;

    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private bool hasGreeted = false;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if(!hasGreeted && greetingDialogue != null)
        {
            if (isDialogueActive)
            {
                NextLine();
            }
            else
            {
                StartDialogue();
            }
            return;
        }
        OpenCloseShop();
    }

    private void OpenCloseShop()
    {
        if (ShopController.Instance == null)
            return;

        if (ShopController.Instance.shopPanel.activeSelf)
            ShopController.Instance.CloseShop();

        else
            ShopController.Instance.OpenShop(this);
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        dialogueUI.SetNPCInfo(greetingDialogue.npcName, greetingDialogue.npcPortait);
        dialogueUI.ShowDialogue(true);

        dialogueUI.SetActiveNPC(this);

        PauseController.SetPause(true);

        DisplayCurrentLine();
    }

    public void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(greetingDialogue.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;
        }

        dialogueUI.ClearChoices();

        if (greetingDialogue.endDialogueLines.Length > dialogueIndex && greetingDialogue.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        foreach (DialogueChoice dialogueChoice in greetingDialogue.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < greetingDialogue.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }

        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in greetingDialogue.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            SoundEffectManager.PlayVoice(greetingDialogue.voiceSound, greetingDialogue.voicePitch);
            yield return new WaitForSeconds(greetingDialogue.speakSpeed);
        }

        isTyping = false;

        if (greetingDialogue.autoProgressLines.Length > dialogueIndex && greetingDialogue.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(greetingDialogue.autoProgressDelay);
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex));
        }
    }


    void ChooseOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogue(false);
        dialogueUI.SetActiveNPC(null);
        PauseController.SetPause(false);

        hasGreeted = true;
        OpenCloseShop();
    }

    public List<int> GetCurrentStock()
    {
        return shopItemIDs;
    }
}
