using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager instance;
        public static DialogueManager Instance => instance;

        public TextMeshProUGUI npcNameVal;
        public TextMeshProUGUI contentsVal;


        private Queue<string> _contentsQueue;

        public event Action OnDialogueStart;
        public event Action OnDialogueEnd;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            _contentsQueue = new Queue<string>(); // 초기화 추가
            _contentsQueue.Clear();
        }

        public void DialogueStart(Dialogue dialogue)
        {
            Debug.Log("DialogueStart");
            OnDialogueStart?.Invoke();
            npcNameVal.text = dialogue.npcName;

            foreach (string contents in dialogue.contents)
            {
                _contentsQueue.Enqueue(contents);   
            }

            GetNextContent();
        }

        private void GetNextContent()
        {
            if (_contentsQueue.Count == 0)
            {
                DialogueEnd();
                return;
            }
            
            string content = _contentsQueue.Dequeue();
            StopAllCoroutines();
            StartCoroutine(ContentsType(content));
        }

        private IEnumerator ContentsType(string contents)
        {
            contentsVal.text = string.Empty;
            yield return new WaitForSeconds(0.25f);
            foreach (char letter in contents)
            {
                contentsVal.text += letter;
                yield return null;
            }
        }

        private void DialogueEnd()
        {
            Debug.Log("DialogueEnd");
            OnDialogueEnd?.Invoke();
        }
    }
}