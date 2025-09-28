using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueUIController : MonoBehaviour
    {
        public static DialogueUIController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            Instance = this;
        }
        
        public void ShowDialogue(DialogueNpc targetNpc)
        {
            if (targetNpc == null) return;
            if (targetNpc.IsDialogue) return;

            targetNpc.IsDialogue = true;
            DialogueManager.Instance.DialogueStart(targetNpc);
        }
    }
}