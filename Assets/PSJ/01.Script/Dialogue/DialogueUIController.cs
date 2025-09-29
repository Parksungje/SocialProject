using System.Collections;
using TMPro;
using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueUIController : MonoBehaviour
    {
        public static DialogueUIController Instance { get; private set; }

        public TextMeshProUGUI npcNameVal;
        public TextMeshProUGUI contentsVal;
        public Transform choicesParent;
        public GameObject choiceButtonPrefab;

        public float charDelay = 0.02f;

        private Coroutine _typingCoroutine;
        private bool _isTyping;
        private bool _isChoiceActive;
        private string _fullText;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            Instance = this;
        }

        private void OnEnable()
        {
            DialogueManager.Instance.OnDialogueStart += HandleStart;
            DialogueManager.Instance.OnLineShown += HandleLine;
            DialogueManager.Instance.OnChoicesShown += HandleChoices;
            DialogueManager.Instance.OnDialogueEnd += HandleEnd;
        }

        private void OnDisable()
        {
            if (DialogueManager.Instance == null) return;
            DialogueManager.Instance.OnDialogueStart -= HandleStart;
            DialogueManager.Instance.OnLineShown -= HandleLine;
            DialogueManager.Instance.OnChoicesShown -= HandleChoices;
            DialogueManager.Instance.OnDialogueEnd -= HandleEnd;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_isTyping)
                {
                    StopTyping();
                }
                else if (!_isChoiceActive)
                {
                    DialogueManager.Instance.ShowNextLine();    
                }
            }
        }

        private void HandleStart(DialogueNpc npc)
        {
            npcNameVal.text = npc.NpcName;
            contentsVal.text = string.Empty;
            ClearChoices();
            _isChoiceActive = false;
        }

        private void HandleLine(DialogueLine line)
        {
            ClearChoices();
            _fullText = line.text;
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeText(_fullText));
        }

        private IEnumerator TypeText(string text)
        {
            _isTyping = true;
            contentsVal.text = "";
            foreach (char c in text)
            {
                contentsVal.text += c;
                yield return new WaitForSeconds(charDelay);
            }
            _isTyping = false;
        }

        private void StopTyping()
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            contentsVal.text = _fullText;
            _isTyping = false;
        }

        private void HandleChoices(DialogueChoice[] choices)
        {
            ClearChoices();

            if (choices == null || choices.Length == 0 || choiceButtonPrefab == null || choicesParent == null)
            {
                _isChoiceActive = false;
                return;
            }

            _isChoiceActive = true;

            foreach (var choice in choices)
            {
                var go = Instantiate(choiceButtonPrefab, choicesParent);
                var btn = go.GetComponent<ChoiceButton>();
                if (btn != null)
                {
                    btn.Setup(choice);
                    btn.OnClickedCallback = () =>
                    {
                        _isChoiceActive = false;
                    };
                }
            }
        }

        private void HandleEnd()
        {
            npcNameVal.text = "";
            contentsVal.text = "";
            ClearChoices();
            _isChoiceActive = false;
        }

        private void ClearChoices()
        {
            if (choicesParent == null) return;
            for (int i = choicesParent.childCount - 1; i >= 0; i--)
                Destroy(choicesParent.GetChild(i).gameObject);
        }

        public void ShowDialogue(DialogueNpc targetNpc)
        {
            if (targetNpc == null || targetNpc.isDialogue) return;

            targetNpc.isDialogue = true;
            DialogueManager.Instance.DialogueStart(targetNpc);
        }
    }
}
