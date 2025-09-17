using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueNpc : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue;
        private bool _isDialogue;

        private void OnMouseDown()
        {
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
            _isDialogue = true;
            DialogueManager.Instance.DialogueStart(dialogue);
        }

        private void StopInteract()
        {
            _isDialogue = false;
        }
        private void OnDialogueEnd()
        {
            StopInteract();
        }

    }
}