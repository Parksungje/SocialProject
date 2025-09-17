using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager instance;
        public static DialogueManager Instance => instance;

        public TextMeshProUGUI npcNameVal;
        public TextMeshProUGUI contentsVal;

        public Animator animator = null;

        private Queue<string> contentsQueue;

        public event Action OnDialogueStart;
        public event Action OnDialogueEnd;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            contentsQueue = new Queue<string>();
        }

        public void DialogueStart(Dialogue dialogue)
        {
            OnDialogueStart?.Invoke();
            animator?.SetBool("IsDialogue", true);
            npcNameVal.text = dialogue.npcName;
            contentsQueue.Clear();

            foreach (string contents in dialogue.contents)
            {
                contentsQueue.Enqueue(contents);   
            }

            GetNextContent();
        }

        private void GetNextContent()
        {
            if (contentsQueue.Count == 0)
            {
                DialogueEnd();
                return;
            }
            
            string content = contentsQueue.Dequeue();
            StopAllCoroutines();
            StartCoroutine(ContentsType(content));
        }

        private IEnumerator ContentsType(string contents)
        {
            contentsVal.text = string.Empty;
            yield return new WaitForSeconds(0.25f);
            foreach (char letter in contents.ToCharArray())
            {
                contentsVal.text += letter;
                yield return null;
            }
        }

        private void DialogueEnd()
        {
            animator?.SetBool("IsDialogue", false);
            OnDialogueEnd?.Invoke();
        }
    }
}