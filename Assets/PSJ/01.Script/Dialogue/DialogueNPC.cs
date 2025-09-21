using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueNpc : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue;
        public bool isDialogue = false; 

        private void Start()
        {
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
            isDialogue = true;
            DialogueManager.Instance.DialogueStart(dialogue);
            if (isDialogue)
            {
                Debug.Log("보이루");
            }
        }

        private void Update()
        {
            Debug.Log($"isDialogue 상태: {isDialogue}");
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