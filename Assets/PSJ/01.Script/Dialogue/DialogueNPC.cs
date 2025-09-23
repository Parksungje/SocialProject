using System;
using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueNpc : MonoBehaviour
    {
        [SerializeField] private string npcName;
        [TextArea(3, 10)]
        [SerializeField] private string[] contents;

        public bool isDialogue = false;

        private void Start()
        {
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
            StartDialogue();
        }

        private void Update()
        {
            if (!isDialogue && Input.GetKeyDown(KeyCode.Space))
            {
                StartDialogue();
            }
        }

        private void StartDialogue()
        {
            isDialogue = true;
            DialogueManager.Instance.DialogueStart(npcName, contents);
        }

        private void StopInteract()
        {
            isDialogue = false;
        }

        private void OnDialogueEnd()
        {
            StopInteract();
        }
    }
}