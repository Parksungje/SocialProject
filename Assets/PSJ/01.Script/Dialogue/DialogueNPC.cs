using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueNpc : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue;
        private bool isDialogue = false;

        private void OnMouseDown()
        {
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
            isDialogue = true;
            DialogueManager.Instance.DialogueStart(dialogue);
        }

        public void StopInteract()
        {
            isDialogue = false;
        }
        private void OnDialogueEnd()
        {
            StopInteract();
        }

    }
}