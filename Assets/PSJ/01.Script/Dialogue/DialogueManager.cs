using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PSJ._01.Script.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager instance;
        public static DialogueManager Instance => instance;

        public TextMeshProUGUI npcNameVal;
        public TextMeshProUGUI contentsVal;
        public Transform choicesParent;
        public GameObject choiceButtonPrefab;

        public Image lieMeterFill;
        [Range(0f,1f)] public float lieDetectionThreshold = 0.7f;
        private float lieMeter = 0f;

        public float charDelay = 0.02f;

        private DialogueNpc currentNpc;
        private int currentIndex = 0;
        private Coroutine typingCoroutine;
        private bool isTyping = false;
        private bool choicesActive = false;
        private string currentFullText = "";

        public event Action OnDialogueStart;
        public event Action OnDialogueEnd;
        public event Action OnLieDetected;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);

            if (choicesParent == null && choiceButtonPrefab != null)
                Debug.LogWarning("ChoicesParent not set in DialogueManager.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                {
                    // 즉시 전체 표시
                    if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                    contentsVal.text = currentFullText;
                    isTyping = false;
                }
                else
                {
                    if (choicesActive) return;
                    ShowNextLine();
                }
            }
        }

        public void DialogueStart(DialogueNpc npc)
        {
            if (npc == null) return;
            currentNpc = npc;
            currentIndex = 0;
            lieMeter = 0f;
            RefreshLieUI();
            npcNameVal.text = npc.NpcName;
            OnDialogueStart?.Invoke();

            ClearChoices();
            ShowLine(currentIndex);
        }

        private void ShowNextLine()
        {
            if (currentNpc == null) return;
            int next = currentIndex + 1;
            if (next >= currentNpc.lines.Length)
            {
                DialogueEnd();
                return;
            }
            currentIndex = next;
            ShowLine(currentIndex);
        }

        private void ShowLine(int index)
        {
            ClearChoices();
            var line = currentNpc.GetLine(index);
            if (line == null)
            {
                DialogueEnd();
                return;
            }

            currentFullText = line.text ?? "";
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(currentFullText, () =>
            {
                if (line.choices != null && line.choices.Length > 0)
                {
                    ShowChoices(line.choices);
                }
                else
                {
                    if (line.nextLineIndex == -2)
                    {
                        DialogueEnd();
                    }
                    else
                    {
                        // 다음은 키입력(스페이스) 혹은 자동으로 넘어가게 할 수도 있음
                    }
                }
            }));
        }

        private IEnumerator TypeText(string textToType, Action onFinished)
        {
            isTyping = true;
            contentsVal.text = string.Empty;
            yield return new WaitForSeconds(0.05f);

            for (int i = 0; i < textToType.Length; i++)
            {
                contentsVal.text += textToType[i];
                yield return new WaitForSeconds(charDelay);
            }

            isTyping = false;
            onFinished?.Invoke();
        }

        private void ShowChoices(DialogueChoice[] choices)
        {
            if (choiceButtonPrefab == null || choicesParent == null)
            {
                Debug.LogWarning("Choice prefab or parent missing.");
                return;
            }

            choicesActive = true;
            for (int i = 0; i < choices.Length; i++)
            {
                var go = Instantiate(choiceButtonPrefab, choicesParent);
                var cb = go.GetComponent<ChoiceButton>();
                if (cb != null)
                {
                    cb.Setup(choices[i]);
                }
                else
                {
                    var tmp = go.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null) tmp.text = choices[i].text;
                    var btn = go.GetComponent<Button>();
                    int idx = i;
                    if (btn != null) btn.onClick.AddListener(() => OnChoiceSelected(choices[idx]));
                }
            }
        }

        private void ClearChoices()
        {
            choicesActive = false;
            if (choicesParent == null) return;
            for (int i = choicesParent.childCount - 1; i >= 0; i--)
            {
                Destroy(choicesParent.GetChild(i).gameObject);
            }
        }

        public void OnChoiceSelected(DialogueChoice choice)
        {
            if (choice == null) return;

            if (choice.lieImpact != 0)
            {
                lieMeter += choice.lieImpact * 0.01f;
                lieMeter = Mathf.Clamp01(lieMeter);
                RefreshLieUI();

                if (lieMeter >= lieDetectionThreshold)
                {
                    OnLieDetected?.Invoke();
                }
            }

            choice.onSelect?.Invoke();

            ClearChoices();
            choicesActive = false;

            if (choice.nextLineIndex == -2)
            {
                DialogueEnd();
                return;
            }
            else if (choice.nextLineIndex >= 0)
            {
                currentIndex = choice.nextLineIndex;
                ShowLine(currentIndex);
                return;
            }
            else
            {
                ShowNextLine();
                return;
            }
        }

        private void RefreshLieUI()
        {
            if (lieMeterFill != null)
            {
                lieMeterFill.fillAmount = lieMeter;
            }
        }

        private void DialogueEnd()
        {
            ClearChoices();
            currentNpc?.GetType();
            OnDialogueEnd?.Invoke();
            currentNpc = null;
            isTyping = false;
            choicesActive = false;
            currentFullText = "";
        }
    }
}
