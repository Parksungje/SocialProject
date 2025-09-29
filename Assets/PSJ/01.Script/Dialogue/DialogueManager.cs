using System;
using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        public event Action<DialogueNpc> OnDialogueStart;
        public event Action<DialogueLine> OnLineShown;
        public event Action<DialogueChoice[]> OnChoicesShown;
        public event Action OnDialogueEnd;

        private DialogueNpc _currentNpc;
        private int _currentIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public void DialogueStart(DialogueNpc npc)
        {
            if (npc == null) return;

            _currentNpc = npc;
            _currentIndex = 0;

            OnDialogueStart?.Invoke(npc);
            ShowLine(_currentIndex);
        }

        public void ShowNextLine()
        {
            if (_currentNpc == null) return;

            int next = _currentIndex + 1;
            if (next >= _currentNpc.lines.Length)
            {
                EndDialogue();
                return;
            }

            _currentIndex = next;
            ShowLine(_currentIndex);
        }

        private void ShowLine(int index)
        {
            var line = _currentNpc.GetLine(index);
            if (line == null)
            {
                EndDialogue();
                return;
            }

            OnLineShown?.Invoke(line);

            if (line.choices != null && line.choices.Length > 0)
            {
                OnChoicesShown?.Invoke(line.choices);
            }
            else
            {
                OnChoicesShown?.Invoke(null);
            }
        }

        public void OnChoiceSelected(DialogueChoice choice)
        {
            if (choice == null) return;

            if (choice.lieImpact != 0)
            {
            }

            choice.onSelect?.Invoke();

            if (choice.nextLineIndex == -2)
            {
                EndDialogue();
            }
            else if (choice.nextLineIndex >= 0)
            {
                _currentIndex = choice.nextLineIndex;
                ShowLine(_currentIndex);
            }
            else
            {
                ShowNextLine();
            }
        }

        private void EndDialogue()
        {
            OnDialogueEnd?.Invoke();
            _currentNpc = null;
        }
    }
}
