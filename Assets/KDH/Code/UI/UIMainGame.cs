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
        [SerializeField] private float stampScale = 4f; // 도장 크기 배율

        private void Start()
        {
            InitializeUI();
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void InitializeUI()
        {
            approveButton.onClick.AddListener(() => OnDecisionMade(Decision.Approve));
            rejectButton.onClick.AddListener(() => OnDecisionMade(Decision.Reject));
            rulesButton.onClick.AddListener(OnRulesButtonClicked);

            if (stampImage != null)
                stampImage.gameObject.SetActive(false);
        }

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

        private void OnGameStateChanged(GameState newState)
        {
            Debug.Log($"[UIMainGame] State changed to: {newState}");

            if (newState == GameState.DocumentReview)
            {
                gameObject.SetActive(true);
                documentPanel.SetActive(true);
                EnableDecisionButtons(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void UpdateDayDisplay(int day)
        {
            Debug.Log($"[UIMainGame] >>> UpdateDayDisplay 호출됨 | 전달받은 day = {day} | GameManager.CurrentDay = {GameManager.Instance?.CurrentDay}");

            if (dayText != null)
                dayText.text = $"Day {day}";

            if (progressText != null)
                progressText.text = $"0 / {GameManager.Instance.GetTodayTarget()}";

            if (progressBar != null)
                progressBar.value = 0f;
        }

        private void OnDocumentPresented(Data.ApplicantData applicant)
        {
            if (documentView != null)
                documentView.DisplayDocument(applicant);

            EnableDecisionButtons(true);
            UpdateProgress();
        }

        private void EnableDecisionButtons(bool enable)
        {
            if (approveButton != null)
                approveButton.interactable = enable;
            if (rejectButton != null)
                rejectButton.interactable = enable;
        }

        private void OnDecisionMade(Decision decision)
        {
            Debug.Log($"[UIMainGame] ===== Decision Made: {decision} =====");

            EnableDecisionButtons(false);
            ShowStampEffect(decision);

            if (DocumentManager.Instance != null)
            {
                DocumentManager.Instance.ProcessDocument(decision);
                // ⚠️ NextDay는 DocumentManager 내부에서 처리
            }
        }

        private void ShowStampEffect(Decision decision)
        {
            if (stampImage == null) return;

            Sprite selectedStamp = (decision == Decision.Approve) ? approvedStamp : rejectedStamp;
            if (selectedStamp == null) return;

            stampImage.sprite = selectedStamp;
            stampImage.gameObject.SetActive(true);

            StartCoroutine(StampAnimation());
            PlayStampSound(decision);
            Invoke(nameof(HideStampEffect), stampDisplayTime);
        }

        private void PlayStampSound(Decision decision)
        {
            if (stampSound == null) return;

            if (Utils.AudioManager.Instance != null)
            {
                string soundName = decision == Decision.Approve ? "stamp_approve" : "stamp_reject";
                Utils.AudioManager.Instance.PlaySFX(soundName);
            }
            else
            {
                AudioSource.PlayClipAtPoint(stampSound, Camera.main.transform.position, 1f);
            }
        }

        private System.Collections.IEnumerator StampAnimation()
        {
            float startScale = stampScale * 1.2f;
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

        private void HideStampEffect()
        {
            if (stampImage != null)
            {
                stampImage.gameObject.SetActive(false);
                stampImage.transform.localScale = Vector3.one * stampScale;
            }
        }

        private void OnDocumentProcessed(Decision decision, bool isCorrect)
        {
            UpdateProgress();
            // ⚠️ NextDay 호출 금지
            // DocumentManager에서 오늘 목표 달성 시 NextDay 호출
        }

        private void UpdateProgress()
        {
            if (DocumentManager.Instance == null || GameManager.Instance == null) return;

            int processed = DocumentManager.Instance.ProcessedCount;
            int target = GameManager.Instance.GetTodayTarget();

            if (progressText != null)
                progressText.text = $"{processed} / {target}";

            if (progressBar != null)
                progressBar.value = DocumentManager.Instance.GetProgress();
        }

        private void OnRulesButtonClicked()
        {
            if (rulesPanel != null)
                rulesPanel.ShowRules();
        }
    }
}
