using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueNpc : MonoBehaviour
    {
        public string npcName;
        public DialogueLine[] lines;

        public bool isDialogue;
        public string NpcName => npcName;

        public DialogueLine GetLine(int index)
        {
            if (lines == null || lines.Length == 0) return null;
            if (index < 0 || index >= lines.Length) return null;
            return lines[index];
        }

        private void Start()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
            }
        }

        private void OnDestroy()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueEnd -= OnDialogueEnd;
            }
        }

        private void OnDialogueEnd()
        {
            isDialogue = false;
        }

        public void OnClicked()
        {
            DialogueUIController.Instance.ShowDialogue(this);
        }
    }
}