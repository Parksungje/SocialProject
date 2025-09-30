using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI content;
    private MessageSO message;

    private void Awake()
    {
        _title.text = message.messageTitle;
        content.text = message.messageContent;
    }
}
