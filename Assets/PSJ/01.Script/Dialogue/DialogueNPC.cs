using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueNpc : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue;
        public bool _isDialogue;

        private void OnMouseDown()
        {
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
            _isDialogue = true;
            DialogueManager.Instance.DialogueStart(dialogue);
            if (_isDialogue)
            {
                Debug.Log("보이루");
            }
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