using KDH.Code.Core;
using KDH.Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 메인 게임 화면 UI 관리
    /// </summary>
    public class UIMainGame : MonoBehaviour
    {
        [Header("상단 UI")]
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Button rulesButton;
        
        [Header("서류 표시 영역")]
        [SerializeField] private GameObject documentPanel;
        [SerializeField] private UIDocumentView documentView;
        
        [Header("결정 버튼")]
        [SerializeField] private Button approveButton;
        [SerializeField] private Button rejectButton;
        
        [Header("피드백 UI")]
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Image feedbackIcon;
        [SerializeField] private Sprite correctIcon;
        [SerializeField] private Sprite incorrectIcon;
        
        private void Start()
        {
            InitializeUI();
            RegisterEvents();
        }
        
        private void OnDestroy()
        {
            UnregisterEvents();
        }
        
        /// <summary>
        /// UI 초기화
        /// </summary>
        private void InitializeUI()
        {
            // 버튼 이벤트 연결
            approveButton.onClick.AddListener(() => OnDecisionMade(Decision.Approve));
            rejectButton.onClick.AddListener(() => OnDecisionMade(Decision.Reject));
            rulesButton.onClick.AddListener(OnRulesButtonClicked);
            
            // 초기 상태
            feedbackPanel.SetActive(false);
            documentPanel.SetActive(false);
        }
        
        /// <summary>
        /// 이벤트 등록
        /// </summary>
        private void RegisterEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayChanged += UpdateDayDisplay;
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
            
            if (DocumentManager.Instance != null)
            {
                DocumentManager.Instance.OnDocumentPresented += OnDocumentPresented;
                DocumentManager.Instance.OnDocumentProcessed += OnDocumentProcessed;
            }
        }
        
        /// <summary>
        /// 이벤트 해제
        /// </summary>
        private void UnregisterEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayChanged -= UpdateDayDisplay;
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
            
            if (DocumentManager.Instance != null)
            {
                DocumentManager.Instance.OnDocumentPresented -= OnDocumentPresented;
                DocumentManager.Instance.OnDocumentProcessed -= OnDocumentProcessed;
            }
        }
        
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.DocumentReview:
                    documentPanel.SetActive(true);
                    EnableDecisionButtons(false); // 서류가 제시될 때까지 비활성화
                    break;
                    
                case GameState.DailyReport:
                case GameState.InnerThoughts:
                case GameState.FamilyTalk:
                    documentPanel.SetActive(false);
                    break;
            }
        }
        
        /// <summary>
        /// 일차 표시 업데이트
        /// </summary>
        private void UpdateDayDisplay(int day)
        {
            dayText.text = $"Day {day}";
        }
        
        /// <summary>
        /// 서류 제시 처리
        /// </summary>
        private void OnDocumentPresented(Data.ApplicantData applicant)
        {
            // 서류 뷰에 표시
            if (documentView != null)
            {
                documentView.DisplayDocument(applicant);
            }
            
            // 결정 버튼 활성화
            EnableDecisionButtons(true);
            
            // 진행률 업데이트
            UpdateProgress();
        }
        
        /// <summary>
        /// 결정 버튼 활성화/비활성화
        /// </summary>
        private void EnableDecisionButtons(bool enable)
        {
            approveButton.interactable = enable;
            rejectButton.interactable = enable;
        }
        
        /// <summary>
        /// 결정 처리
        /// </summary>
        private void OnDecisionMade(Decision decision)
        {
            // 버튼 비활성화
            EnableDecisionButtons(false);
            
            // DocumentManager에 전달
            if (DocumentManager.Instance != null)
            {
                DocumentManager.Instance.ProcessDocument(decision);
            }
        }
        
        /// <summary>
        /// 서류 처리 결과 표시
        /// </summary>
        private void OnDocumentProcessed(Decision decision, bool isCorrect)
        {
            // 피드백 표시
            ShowFeedback(isCorrect);
            
            // 진행률 업데이트
            UpdateProgress();
        }
        
        /// <summary>
        /// 피드백 표시
        /// </summary>
        private void ShowFeedback(bool isCorrect)
        {
            feedbackPanel.SetActive(true);
            
            if (isCorrect)
            {
                feedbackText.text = "정확한 판단입니다";
                feedbackText.color = Color.green;
                feedbackIcon.sprite = correctIcon;
            }
            else
            {
                feedbackText.text = "잘못된 판단입니다";
                feedbackText.color = Color.red;
                feedbackIcon.sprite = incorrectIcon;
            }
            
            // 자동으로 숨김
            Invoke(nameof(HideFeedback), GameConstants.FEEDBACK_DISPLAY_TIME);
        }
        
        private void HideFeedback()
        {
            feedbackPanel.SetActive(false);
        }
        
        /// <summary>
        /// 진행률 업데이트
        /// </summary>
        private void UpdateProgress()
        {
            if (DocumentManager.Instance == null)
                return;
            
            int processed = DocumentManager.Instance.ProcessedCount;
            int target = GameManager.Instance.GetTodayTarget();
            
            progressText.text = $"{processed} / {target}";
            progressBar.value = DocumentManager.Instance.GetProgress();
        }
        
        /// <summary>
        /// 규칙 버튼 클릭
        /// </summary>
        private void OnRulesButtonClicked()
        {
            // UIRulesPanel 표시 (별도 구현)
            Debug.Log("Rules button clicked");
        }
    }
}