using UnityEngine;

[CreateAssetMenu(fileName = "MessageSO", menuName = "SO/MessageSO", order = 0)]
public class MessageSO : ScriptableObject
{
    public string messageTitle;

    public string messageContent;
}
