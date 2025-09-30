using System.Collections;
using KDH.Code.Core;
using KDH.Code.Data;
using KDH.Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 가족 대화 및 속마음 UI
    /// </summary>
    public class UIFamilyPanel : MonoBehaviour
    {
        [Header("속마음 UI")]
        [SerializeField] private GameObject innerThoughtPanel;
        [SerializeField] private TextMeshProUGUI innerThoughtText;
        
        [Header("가족 대화 UI")]
        [SerializeField] private GameObject familyDialoguePanel;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject spouseBubble;
        [SerializeField] private TextMeshProUGUI spouseText;
        [SerializeField] private GameObject daughterBubble;
        [SerializeField] private TextMeshProUGUI daughterText;
        
        [Header("가계부 UI")]
        [SerializeField] private GameObject budgetPanel;
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI expenseText;
        [SerializeField] private TextMeshProUGUI balanceText;
        
        [Header("스킵 버튼")]
        [SerializeField] private Button skipButton;
        
        private bool isPlaying;
        
        private void Start()
        {
            skipButton.onClick.AddListener(OnSkipClicked);
            RegisterEvents();
        }
        
        private void OnDestroy()
        {
            UnregisterEvents();
        }
        
        /// <summary>
        /// 이벤트 등록
        /// </summary>
        private void RegisterEvents()
        {
            if (FamilyManager.Instance != null)
            {
                FamilyManager.Instance.OnInnerThoughtShown += ShowInnerThought;
                FamilyManager.Instance.OnFamilyDialogueStarted += StartFamilyDialogue;
            }
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
        }
        
        /// <summary>
        /// 이벤트 해제
        /// </summary>
        private void UnregisterEvents()
        {
            if (FamilyManager.Instance != null)
            {
                FamilyManager.Instance.OnInnerThoughtShown -= ShowInnerThought;
                FamilyManager.Instance.OnFamilyDialogueStarted -= StartFamilyDialogue;
            }
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }
        
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            bool isActive = newState == GameState.InnerThoughts || newState == GameState.FamilyTalk;
            gameObject.SetActive(isActive);
            
            if (!isActive)
            {
                StopAllCoroutines();
                isPlaying = false;
            }
        }
        
        /// <summary>
        /// 속마음 표시
        /// </summary>
        private void ShowInnerThought(string thought)
        {
            innerThoughtPanel.SetActive(true);
            familyDialoguePanel.SetActive(false);
            
            innerThoughtText.text = thought;
            
            isPlaying = true;
        }
        
        /// <summary>
        /// 가족 대화 시작
        /// </summary>
        private void StartFamilyDialogue(FamilyDialogueInstance dialogue)
        {
            innerThoughtPanel.SetActive(false);
            familyDialoguePanel.SetActive(true);
            
            StartCoroutine(PlayDialogueSequence(dialogue));
        }
        
        /// <summary>
        /// 대화 시퀀스 재생
        /// </summary>
        private IEnumerator PlayDialogueSequence(FamilyDialogueInstance dialogue)
        {
            isPlaying = true;
            
            // 초기화
            spouseBubble.SetActive(false);
            daughterBubble.SetActive(false);
            budgetPanel.SetActive(false);
            
            // 배경 페이드인
            yield return StartCoroutine(FadeIn(backgroundImage));
            
            // 배우자 대화
            yield return new WaitForSeconds(0.5f);
            spouseText.text = dialogue.spouseDialogue;
            spouseBubble.SetActive(true);
            yield return new WaitForSeconds(GameConstants.FAMILY_DIALOGUE_DISPLAY_TIME);
            
            // 딸 대화
            daughterText.text = dialogue.daughterDialogue;
            daughterBubble.SetActive(true);
            yield return new WaitForSeconds(GameConstants.FAMILY_DIALOGUE_DISPLAY_TIME);
            
            // 가계부 표시
            DisplayBudget();
            budgetPanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            
            isPlaying = false;
        }
        
        /// <summary>
        /// 가계부 표시
        /// </summary>
        private void DisplayBudget()
        {
            if (EconomyManager.Instance == null)
                return;
            
            int income = EconomyManager.Instance.CurrentMonthEarnings;
            int expense = GameConstants.TOTAL_MONTHLY_EXPENSE / 
                (GameConstants.TOTAL_DAYS / GameConstants.FAMILY_TALK_INTERVAL);
            int balance = EconomyManager.Instance.CurrentBalance;
            
            incomeText.text = $"수입: {income:N0}원";
            expenseText.text = $"지출: {expense:N0}원";
            balanceText.text = $"잔액: {balance:N0}원";
            
            // 잔액에 따른 색상 변경
            if (balance < 0)
                balanceText.color = Color.red;
            else if (balance < 500000)
                balanceText.color = Color.yellow;
            else
                balanceText.color = Color.green;
        }
        
        /// <summary>
        /// 페이드인 효과
        /// </summary>
        private IEnumerator FadeIn(Image image)
        {
            float duration = 0.5f;
            float elapsed = 0f;
            Color color = image.color;
            color.a = 0f;
            image.color = color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                image.color = color;
                yield return null;
            }
            
            color.a = 1f;
            image.color = color;
        }
        
        /// <summary>
        /// 스킵 버튼 클릭
        /// </summary>
        private void OnSkipClicked()
        {
            if (isPlaying)
            {
                StopAllCoroutines();
                isPlaying = false;
                
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeState(GameState.DayEnd);
                }
            }
        }
    }
}