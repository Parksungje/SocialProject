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
        
        [Header("도장 효과")]
        [SerializeField] private Image stampImage;           // 도장 이미지
        [SerializeField] private Sprite approvedStamp;       // 승인 도장 스프라이트
        [SerializeField] private Sprite rejectedStamp;       // 거부 도장 스프라이트
        [SerializeField] private AudioClip stampSound;       // 도장 사운드
        [SerializeField] private float stampDisplayTime = 1.5f; // 도장 표시 시간
        
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
            documentPanel.SetActive(false);
            
            // 도장 이미지 초기화
            if (stampImage != null)
            {
                stampImage.gameObject.SetActive(false);
            }
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
            
            // 도장 효과 표시
            ShowStampEffect(decision);
            
            // DocumentManager에 전달
            if (DocumentManager.Instance != null)
            {
                DocumentManager.Instance.ProcessDocument(decision);
            }
        }
        
        /// <summary>
        /// 도장 효과 표시
        /// </summary>
        // UIMainGame.cs에 추가

        private void ShowStampEffect(Decision decision)
        {
            if (stampImage == null) return;
    
            // 도장 이미지 설정
            if (decision == Decision.Approve)
            {
                stampImage.sprite = approvedStamp;
            }
            else
            {
                stampImage.sprite = rejectedStamp;
            }
    
            // 도장 표시 + 애니메이션
            stampImage.gameObject.SetActive(true);
            StartCoroutine(StampAnimation());
    
            // 도장 사운드 재생
            PlayStampSound();
    
            // 일정 시간 후 숨김
            Invoke(nameof(HideStampEffect), stampDisplayTime);
        }

        /// <summary>
        /// 도장 애니메이션 (스케일 효과)
        /// </summary>
        private System.Collections.IEnumerator StampAnimation()
        {
            // 크기 1.5배로 시작
            stampImage.transform.localScale = Vector3.one * 1.5f;
    
            float duration = 0.2f;
            float elapsed = 0f;
    
            // 1.5배 → 1.0배로 부드럽게
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(1.5f, 1.0f, elapsed / duration);
                stampImage.transform.localScale = Vector3.one * scale;
                yield return null;
            }
    
            stampImage.transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// 도장 효과 숨김
        /// </summary>
        private void HideStampEffect()
        {
            if (stampImage != null)
            {
                stampImage.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 도장 사운드 재생
        /// </summary>
        private void PlayStampSound()
        {
            if (stampSound == null) return;
            
            // AudioManager가 있으면 사용
            if (Utils.AudioManager.Instance != null)
            {
                Utils.AudioManager.Instance.PlaySFX("stamp_approve"); // 또는 "stamp_reject"
            }
            else
            {
                // AudioManager가 없으면 직접 재생
                AudioSource.PlayClipAtPoint(stampSound, Camera.main.transform.position, 1f);
            }
        }
        
        /// <summary>
        /// 서류 처리 결과 표시
        /// </summary>
        private void OnDocumentProcessed(Decision decision, bool isCorrect)
        {
            // 진행률 업데이트
            UpdateProgress();
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