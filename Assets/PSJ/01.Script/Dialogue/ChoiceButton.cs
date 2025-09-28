using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PSJ._01.Script.Dialogue
{
    [RequireComponent(typeof(Button))]
    public class ChoiceButton : MonoBehaviour
    {
        public TextMeshProUGUI textLabel;
        private Button _button;
        private DialogueChoice _choice;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Setup(DialogueChoice choice)
        {
            _choice = choice;
            if (textLabel != null) textLabel.text = choice.text;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            DialogueManager.Instance.OnChoiceSelected(_choice);
        }
    }
}