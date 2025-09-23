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
        private Coroutine typingCoroutine;
        private bool isTyping = false;
        private string currentContent = "";

        public event Action OnDialogueStart;
        public event Action OnDialogueEnd;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            _contentsQueue = new Queue<string>();
            _contentsQueue.Clear();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                {
                    // 타이핑 중이면 즉시 전체 문장 출력
                    if (typingCoroutine != null)
                        StopCoroutine(typingCoroutine);
                    contentsVal.text = currentContent;
                    isTyping = false;
                }
                else
                {
                    // 다음 대사 출력
                    GetNextContent();
                }
            }
        }

        public void DialogueStart(string npcName, string[] contents)
        {
            Debug.Log("DialogueStart");
            OnDialogueStart?.Invoke();
            npcNameVal.text = npcName;

            _contentsQueue.Clear();
            foreach (string content in contents)
            {
                _contentsQueue.Enqueue(content);
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
            
            currentContent = _contentsQueue.Dequeue();
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(ContentsType(currentContent));
        }

        private IEnumerator ContentsType(string contents)
        {
            isTyping = true;
            contentsVal.text = string.Empty;
            yield return new WaitForSeconds(0.25f);
            foreach (char letter in contents)
            {
                contentsVal.text += letter;
                yield return null;
            }
            isTyping = false;
        }

        private void DialogueEnd()
        {
            Debug.Log("DialogueEnd");
            OnDialogueEnd?.Invoke();
        }
    }
}