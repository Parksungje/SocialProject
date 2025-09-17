using System;
using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    [Serializable]
    public class Dialogue : MonoBehaviour
    {
        public string npcName;

        [TextArea(3, 10)] 
        public string[] contents;
    }
}
