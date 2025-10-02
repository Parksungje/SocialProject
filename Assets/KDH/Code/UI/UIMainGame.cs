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
        
        [Header("규칙 패널")]
        [SerializeField] private UIRulesPanel rulesPanel;
        
        [Header("서류 표시 영역")]
        [SerializeField] private GameObject documentPanel;
        [SerializeField] private UIDocumentView documentView;
        
        [Header("결정 버튼")]
        [SerializeField] private Button approveButton;
        [SerializeField] private Button rejectButton;
        
        [Header("도장 효과")]
        [SerializeField] private Image stampImage;
        [SerializeField] private Sprite approvedStamp;
        [SerializeField] private Sprite rejectedStamp;
        [SerializeField] private AudioClip stampSound;
        [SerializeField] private float stampDisplayTime = 1.5f;
        [SerializeField] private float stampScale = 4f; // ✅ 도장 크기 배율 (기본 4배)
        
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
            approveButton.onClick.AddListener(() => OnDecisionMade(Decision.Approve));
            rejectButton.onClick.AddListener(() => OnDecisionMade(Decision.Reject));
            rulesButton.onClick.AddListener(OnRulesButtonClicked);
            
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
            Debug.Log($"[UIMainGame] State changed to: {newState}");
            
            if (newState == GameState.DocumentReview)
            {
                gameObject.SetActive(true);
                documentPanel.SetActive(true);
                EnableDecisionButtons(false);
                Debug.Log("[UIMainGame] Panel activated");
            }
            else
            {
                gameObject.SetActive(false);
                Debug.Log("[UIMainGame] Panel deactivated");
            }
        }
        
        /// <summary>
        /// 일차 표시 업데이트
        /// </summary>
        private void UpdateDayDisplay(int day)
        {
            if (dayText != null)
                dayText.text = $"Day {day}";
        }
        
        /// <summary>
        /// 서류 제시 처리
        /// </summary>
        private void OnDocumentPresented(Data.ApplicantData applicant)
        {
            Debug.Log($"[UIMainGame] Document presented: {applicant.fullName}");
            
            if (documentView != null)
            {
                documentView.DisplayDocument(applicant);
            }
            
            EnableDecisionButtons(true);
            UpdateProgress();
        }
        
        /// <summary>
        /// 결정 버튼 활성화/비활성화
        /// </summary>
        private void EnableDecisionButtons(bool enable)
        {
            if (approveButton != null)
                approveButton.interactable = enable;
            if (rejectButton != null)
                rejectButton.interactable = enable;
        }
        
        /// <summary>
        /// 결정 처리
        /// </summary>
        private void OnDecisionMade(Decision decision)
        {
            Debug.Log($"[UIMainGame] ===== Decision Made: {decision} =====");
            
            EnableDecisionButtons(false);
            ShowStampEffect(decision);
            
            if (DocumentManager.Instance != null)
            {
                DocumentManager.Instance.ProcessDocument(decision);
            }
        }
        
        /// <summary>
        /// 도장 효과 표시
        /// </summary>
        private void ShowStampEffect(Decision decision)
        {
            if (stampImage == null)
            {
                Debug.LogError("[UIMainGame] StampImage is NULL!");
                return;
            }
            
            // ✅ 명확한 디버그
            Debug.Log($"[UIMainGame] ========================================");
            Debug.Log($"[UIMainGame] ShowStampEffect called with: {decision}");
            Debug.Log($"[UIMainGame] ApprovedStamp is null? {approvedStamp == null}");
            Debug.Log($"[UIMainGame] RejectedStamp is null? {rejectedStamp == null}");
            
            if (approvedStamp != null)
                Debug.Log($"[UIMainGame] ApprovedStamp name: {approvedStamp.name}");
            if (rejectedStamp != null)
                Debug.Log($"[UIMainGame] RejectedStamp name: {rejectedStamp.name}");
            
            // ✅ 명확하게 분기
            Sprite selectedStamp = null;
            
            if (decision == Decision.Approve)
            {
                selectedStamp = approvedStamp;
                Debug.Log("[UIMainGame] → Using APPROVED stamp");
            }
            else if (decision == Decision.Reject)
            {
                selectedStamp = rejectedStamp;
                Debug.Log("[UIMainGame] → Using REJECTED stamp");
            }
            else
            {
                Debug.LogError($"[UIMainGame] Unknown decision: {decision}");
                return;
            }
            
            if (selectedStamp == null)
            {
                Debug.LogError($"[UIMainGame] Selected stamp is NULL for decision: {decision}");
                return;
            }
            
            Debug.Log($"[UIMainGame] Final selected stamp: {selectedStamp.name}");
            Debug.Log($"[UIMainGame] Assigning stamp to Image...");
            
            stampImage.sprite = selectedStamp;
            stampImage.gameObject.SetActive(true);
            
            Debug.Log($"[UIMainGame] Stamp Image active: {stampImage.gameObject.activeSelf}");
            Debug.Log($"[UIMainGame] Stamp Image sprite: {stampImage.sprite.name}");
            Debug.Log($"[UIMainGame] ========================================");
            
            StartCoroutine(StampAnimation());
            PlayStampSound(decision);
            Invoke(nameof(HideStampEffect), stampDisplayTime);
        }
    
        /// <summary>
        /// 도장 사운드 재생
        /// </summary>
        private void PlayStampSound(Decision decision)
        {
            if (stampSound == null)
            {
                Debug.LogWarning("[UIMainGame] StampSound is NULL!");
                return;
            }
            
            Debug.Log($"[UIMainGame] Playing stamp sound for {decision}");
            
            if (Utils.AudioManager.Instance != null)
            {
                // ✅ Decision에 따라 다른 사운드 재생
                string soundName = decision == Decision.Approve ? "stamp_approve" : "stamp_reject";
                Utils.AudioManager.Instance.PlaySFX(soundName);
            }
            else
            {
                // AudioManager가 없으면 직접 재생
                AudioSource.PlayClipAtPoint(stampSound, Camera.main.transform.position, 1f);
                Debug.Log("[UIMainGame] Playing stamp sound with PlayClipAtPoint");
            }
        }
        
        /// <summary>
        /// 도장 애니메이션
        /// </summary>
        private System.Collections.IEnumerator StampAnimation()
        {
            // ✅ 시작 크기: stampScale * 1.2배 (예: 4 * 1.2 = 4.8배)
            float startScale = stampScale * 1.2f;
            // ✅ 종료 크기: stampScale (예: 4배)
            float endScale = stampScale;
            
            stampImage.transform.localScale = Vector3.one * startScale;
            
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float currentScale = Mathf.Lerp(startScale, endScale, elapsed / duration);
                stampImage.transform.localScale = Vector3.one * currentScale;
                yield return null;
            }
            
            stampImage.transform.localScale = Vector3.one * endScale;
        }
        
        /// <summary>
        /// 도장 효과 숨김
        /// </summary>
        private void HideStampEffect()
        {
            if (stampImage != null)
            {
                stampImage.gameObject.SetActive(false);
                // ✅ 스케일 초기화 (다음 도장을 위해)
                stampImage.transform.localScale = Vector3.one * stampScale;
            }
        }
        
        /// <summary>
        /// 도장 사운드 재생
        /// </summary>
        private void PlayStampSound()
        {
            if (stampSound == null) return;
            
            if (Utils.AudioManager.Instance != null)
            {
                Utils.AudioManager.Instance.PlaySFX("stamp_sound");
            }
            else
            {
                AudioSource.PlayClipAtPoint(stampSound, Camera.main.transform.position, 1f);
            }
        }
        
        /// <summary>
        /// 서류 처리 결과 표시
        /// </summary>
        private void OnDocumentProcessed(Decision decision, bool isCorrect)
        {
            UpdateProgress();
        }
        
        /// <summary>
        /// 진행률 업데이트
        /// </summary>
        private void UpdateProgress()
        {
            if (DocumentManager.Instance == null || GameManager.Instance == null)
                return;
            
            int processed = DocumentManager.Instance.ProcessedCount;
            int target = GameManager.Instance.GetTodayTarget();
            
            if (progressText != null)
                progressText.text = $"{processed} / {target}";
            
            if (progressBar != null)
                progressBar.value = DocumentManager.Instance.GetProgress();
        }

        /// <summary>
        /// 규칙 버튼 클릭
        /// </summary>
        private void OnRulesButtonClicked()
        {
            Debug.Log("[UIMainGame] Rules button clicked");
    
            if (rulesPanel != null)
            {
                rulesPanel.ShowRules();
            }
            else
            {
                Debug.LogError("[UIMainGame] RulesPanel is NULL!");
            }
        }
    }
}