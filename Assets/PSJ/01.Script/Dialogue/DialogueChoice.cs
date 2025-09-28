using System;
using UnityEngine;
using UnityEngine.Events;

namespace PSJ._01.Script.Dialogue
{
    [Serializable]
    public class DialogueChoice
    {
        public string text;
        public int nextLineIndex = -2;
        public int lieImpact = 0;
        public bool isLie = false;
        public UnityEvent onSelect;
    }

    [Serializable]
    public class DialogueLine
    {
        [TextArea(3, 8)]
        public string text;
        public DialogueChoice[] choices;
        public int nextLineIndex = -1;
    }
}