using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueUIController : MonoBehaviour
    {
        public void ShowDialogue(DialogueNpc targetNpc)
        {
            if (targetNpc == null) return;

            if (!targetNpc.IsDialogue)
            {
                targetNpc.GetType()
                    .GetProperty("IsDialogue")
                    ?.SetValue(targetNpc, true, null);
                DialogueManager.Instance.DialogueStart(
                    targetNpc.NpcName,
                    targetNpc.Contents
                );
            }
        }
    }
}