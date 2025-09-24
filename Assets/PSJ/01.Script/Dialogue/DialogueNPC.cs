using System;
using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueNpc : MonoBehaviour
    {
        [SerializeField] private string npcName;
        [TextArea(3, 10)]
        [SerializeField] private string[] contents;

        public bool IsDialogue { get; private set; } = false;

        public string NpcName => npcName;
        public string[] Contents => contents;

        private void Start()
        {
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
        }

        private void OnDialogueEnd()
        {
            IsDialogue = false;
        }
    }
}