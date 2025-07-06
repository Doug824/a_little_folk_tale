using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace ALittleFolkTale.Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private GameObject choicesContainer;
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private Image speakerPortrait;

        [Header("Settings")]
        [SerializeField] private float textSpeed = 0.05f;
        [SerializeField] private bool autoAdvance = false;
        [SerializeField] private float autoAdvanceDelay = 2f;

        private Queue<DialogueLine> dialogueQueue;
        private bool isTyping = false;
        private bool canAdvance = false;
        private Coroutine typingCoroutine;
        private DialogueData currentDialogue;
        private int currentLineIndex = 0;

        private static DialogueSystem instance;
        public static DialogueSystem Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<DialogueSystem>();
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            
            dialogueQueue = new Queue<DialogueLine>();
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }

        public void StartDialogue(DialogueData dialogue)
        {
            if (dialogue == null || dialogue.dialogueLines.Length == 0)
                return;

            currentDialogue = dialogue;
            currentLineIndex = 0;
            
            dialoguePanel.SetActive(true);
            Time.timeScale = 0f;
            
            DisplayLine(currentDialogue.dialogueLines[currentLineIndex]);
        }

        private void DisplayLine(DialogueLine line)
        {
            if (speakerNameText != null)
                speakerNameText.text = line.speakerName;
            
            if (speakerPortrait != null && line.speakerPortrait != null)
            {
                speakerPortrait.sprite = line.speakerPortrait;
                speakerPortrait.gameObject.SetActive(true);
            }
            else if (speakerPortrait != null)
            {
                speakerPortrait.gameObject.SetActive(false);
            }

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            
            typingCoroutine = StartCoroutine(TypeSentence(line));
        }

        private IEnumerator TypeSentence(DialogueLine line)
        {
            isTyping = true;
            canAdvance = false;
            dialogueText.text = "";
            
            ClearChoices();

            foreach (char letter in line.text.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSecondsRealtime(textSpeed);
            }

            isTyping = false;
            canAdvance = true;

            if (line.choices != null && line.choices.Length > 0)
            {
                DisplayChoices(line.choices);
            }
            else if (autoAdvance)
            {
                yield return new WaitForSecondsRealtime(autoAdvanceDelay);
                NextLine();
            }
        }

        private void DisplayChoices(DialogueChoice[] choices)
        {
            ClearChoices();
            choicesContainer.SetActive(true);

            for (int i = 0; i < choices.Length; i++)
            {
                DialogueChoice choice = choices[i];
                GameObject choiceButton = Instantiate(choiceButtonPrefab, choicesContainer.transform);
                
                TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = choice.text;

                Button button = choiceButton.GetComponent<Button>();
                int choiceIndex = i;
                button.onClick.AddListener(() => OnChoiceSelected(choice, choiceIndex));
            }
        }

        private void OnChoiceSelected(DialogueChoice choice, int index)
        {
            ClearChoices();
            
            if (choice.nextDialogue != null)
            {
                StartDialogue(choice.nextDialogue);
            }
            else
            {
                NextLine();
            }
        }

        private void ClearChoices()
        {
            if (choicesContainer != null)
            {
                foreach (Transform child in choicesContainer.transform)
                {
                    Destroy(child.gameObject);
                }
                choicesContainer.SetActive(false);
            }
        }

        public void NextLine()
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentDialogue.dialogueLines[currentLineIndex].text;
                isTyping = false;
                canAdvance = true;
                
                if (currentDialogue.dialogueLines[currentLineIndex].choices != null && 
                    currentDialogue.dialogueLines[currentLineIndex].choices.Length > 0)
                {
                    DisplayChoices(currentDialogue.dialogueLines[currentLineIndex].choices);
                }
                
                return;
            }

            if (!canAdvance)
                return;

            currentLineIndex++;
            
            if (currentLineIndex < currentDialogue.dialogueLines.Length)
            {
                DisplayLine(currentDialogue.dialogueLines[currentLineIndex]);
            }
            else
            {
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            dialoguePanel.SetActive(false);
            Time.timeScale = 1f;
            currentDialogue = null;
            currentLineIndex = 0;
        }

        private void Update()
        {
            if (dialoguePanel.activeSelf && Input.GetButtonDown("Submit"))
            {
                NextLine();
            }
        }
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public Sprite speakerPortrait;
        [TextArea(3, 5)]
        public string text;
        public DialogueChoice[] choices;
    }

    [System.Serializable]
    public class DialogueChoice
    {
        public string text;
        public DialogueData nextDialogue;
    }
}