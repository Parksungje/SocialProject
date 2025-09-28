using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueNpc : MonoBehaviour
    {
        [Header("NPC Data")]
        public string npcName;
        public DialogueLine[] lines;

        // 외부에서 상태 제어 가능하도록 변경
        [HideInInspector] public bool IsDialogue = false;

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
                DialogueManager.Instance.OnLieDetected += OnLieDetected;
            }
        }

        private void OnDestroy()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueEnd -= OnDialogueEnd;
                DialogueManager.Instance.OnLieDetected -= OnLieDetected;
            }
        }

        private void OnDialogueEnd()
        {
            IsDialogue = false;
        }

        private void OnLieDetected()
        {
            // NPC 반응 넣을 자리 (애니메이션, 색깔변화 등)
            // 예: GetComponent<SpriteRenderer>()?.color = Color.red;
        }
        public void OnClicked()
        {
            DialogueUIController.Instance.ShowDialogue(this);
        }
    }
}