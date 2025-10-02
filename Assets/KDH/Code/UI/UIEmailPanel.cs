using System.Collections;
using System.Collections.Generic;
using KDH.Code.Core;
using KDH.Code.Data;
using KDH.Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 이메일 브리핑 UI 패널
    /// </summary>
    public class UIEmailPanel : MonoBehaviour
    {
        [Header("이메일 목록")]
        [SerializeField] private Transform emailListContainer;
        [SerializeField] private GameObject emailItemPrefab;
        
        [Header("이메일 내용")]
        [SerializeField] private GameObject emailContentPanel;
        [SerializeField] private TextMeshProUGUI senderText;
        [SerializeField] private TextMeshProUGUI subjectText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private GameObject attachmentIndicator;
        
        [Header("버튼")]
        [SerializeField] private Button startWorkButton;
        
        private List<UIEmailItem> emailItems = new List<UIEmailItem>();
        private EmailInstance currentEmail;
        
        private void Start()
        {
            Debug.Log("[UIEmailPanel] ===== START =====");
            Debug.Log($"[UIEmailPanel] Initial active state: {gameObject.activeSelf}");
    
            InitializeUI();
            RegisterEvents();
            
            if (GameManager.Instance != null)
            {
                GameState currentState = GameManager.Instance.CurrentState;
                Debug.Log($"[UIEmailPanel] Current game state: {currentState}");
        
                // EmailBriefing 상태면 즉시 활성화
                if (currentState == GameState.EmailBriefing)
                {
                    Debug.Log("[UIEmailPanel] State is EmailBriefing! Activating panel now...");
                    gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("[UIEmailPanel] GameManager not found!");
            }
    
            Debug.Log("[UIEmailPanel] Initialization complete");
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
            Debug.Log("[UIEmailPanel] Initializing UI...");
            
            if (startWorkButton != null)
            {
                startWorkButton.onClick.AddListener(OnStartWorkClicked);
                startWorkButton.interactable = false; // 초기에는 비활성화
                Debug.Log("[UIEmailPanel] Start work button initialized (disabled)");
            }
            else
            {
                Debug.LogError("[UIEmailPanel] StartWorkButton is NULL!");
            }
            
            if (emailContentPanel != null)
            {
                emailContentPanel.SetActive(false);
                Debug.Log("[UIEmailPanel] Email content panel hidden");
            }
        }
        
        /// <summary>
        /// 이벤트 등록
        /// </summary>
        private void RegisterEvents()
        {
            Debug.Log("[UIEmailPanel] Registering events...");
            
            if (EmailManager.Instance != null)
            {
                EmailManager.Instance.OnEmailsReceived += OnEmailsReceived;
                EmailManager.Instance.OnMandatoryEmailRead += OnMandatoryEmailRead;
                Debug.Log("[UIEmailPanel] ✓ EmailManager events registered");
            }
            else
            {
                Debug.LogError("[UIEmailPanel] EmailManager not found!");
            }
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                Debug.Log("[UIEmailPanel] ✓ GameManager events registered");
            }
            else
            {
                Debug.LogError("[UIEmailPanel] GameManager not found!");
            }
        }
        
        /// <summary>
        /// 이벤트 해제
        /// </summary>
        private void UnregisterEvents()
        {
            Debug.Log("[UIEmailPanel] Unregistering events...");
            
            if (EmailManager.Instance != null)
            {
                EmailManager.Instance.OnEmailsReceived -= OnEmailsReceived;
                EmailManager.Instance.OnMandatoryEmailRead -= OnMandatoryEmailRead;
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
            Debug.Log($"[UIEmailPanel] ===== OnGameStateChanged =====");
            Debug.Log($"[UIEmailPanel] New state: {newState}");
            
            bool shouldBeActive = newState == GameState.EmailBriefing;
            
            Debug.Log($"[UIEmailPanel] Should be active: {shouldBeActive}");
            gameObject.SetActive(shouldBeActive);
            
            Debug.Log($"[UIEmailPanel] Panel {(shouldBeActive ? "activated" : "deactivated")}");
        }
        
        /// <summary>
        /// 이메일 수신 처리
        /// </summary>
        private void OnEmailsReceived(List<EmailInstance> emails)
        {
            Debug.Log($"[UIEmailPanel] ===== OnEmailsReceived =====");
            Debug.Log($"[UIEmailPanel] Emails count: {emails.Count}");
            
            ClearEmailList();
            
            // 이메일이 없어도 진행 가능하도록
            if (emails.Count == 0)
            {
                Debug.LogWarning("[UIEmailPanel] No emails received! Enabling start button anyway...");
                if (startWorkButton != null)
                {
                    startWorkButton.interactable = true;
                }
                if (emailContentPanel != null)
                {
                    emailContentPanel.SetActive(false);
                }
                return;
            }
            
            // 이메일 아이템 생성
            foreach (var email in emails)
            {
                Debug.Log($"[UIEmailPanel] Creating item for: '{email.emailData.subject}'");
                CreateEmailItem(email);
            }
            
            // 필수 이메일 자동 열기
            var mandatoryEmail = EmailManager.Instance.MandatoryEmail;
            if (mandatoryEmail != null)
            {
                Debug.Log($"[UIEmailPanel] ✓ Displaying mandatory email: '{mandatoryEmail.emailData.subject}'");
                DisplayEmail(mandatoryEmail);
                EmailManager.Instance.ReadEmail(mandatoryEmail);
            }
            else
            {
                // 필수 이메일이 없으면 바로 활성화
                Debug.LogWarning("[UIEmailPanel] No mandatory email! Enabling start button...");
                if (startWorkButton != null)
                {
                    startWorkButton.interactable = true;
                }
            }
            
            Debug.Log("[UIEmailPanel] ===== OnEmailsReceived END =====");
        }
        
        /// <summary>
        /// 이메일 목록 초기화
        /// </summary>
        private void ClearEmailList()
        {
            Debug.Log($"[UIEmailPanel] Clearing {emailItems.Count} email items...");
            
            foreach (var item in emailItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            emailItems.Clear();
        }
        
        /// <summary>
        /// 이메일 아이템 생성
        /// </summary>
        private void CreateEmailItem(EmailInstance email)
        {
            if (emailItemPrefab == null)
            {
                Debug.LogError("[UIEmailPanel] EmailItemPrefab is NULL!");
                return;
            }
            
            if (emailListContainer == null)
            {
                Debug.LogError("[UIEmailPanel] EmailListContainer is NULL!");
                return;
            }
            
            GameObject itemObj = Instantiate(emailItemPrefab, emailListContainer);
            UIEmailItem item = itemObj.GetComponent<UIEmailItem>();
            
            if (item != null)
            {
                item.Setup(email, () => OnEmailItemClicked(email));
                emailItems.Add(item);
                Debug.Log($"[UIEmailPanel] ✓ Email item created: '{email.emailData.subject}'");
            }
            else
            {
                Debug.LogError("[UIEmailPanel] UIEmailItem component not found on prefab!");
            }
        }
        
        /// <summary>
        /// 이메일 아이템 클릭 처리
        /// </summary>
        private void OnEmailItemClicked(EmailInstance email)
        {
            Debug.Log($"[UIEmailPanel] Email item clicked: '{email.emailData.subject}'");
            DisplayEmail(email);
            EmailManager.Instance.ReadEmail(email);
        }
        
        /// <summary>
        /// 이메일 내용 표시
        /// </summary>
        private void DisplayEmail(EmailInstance email)
        {
            Debug.Log($"[UIEmailPanel] Displaying email: '{email.emailData.subject}'");
            
            currentEmail = email;
            
            if (emailContentPanel != null)
            {
                emailContentPanel.SetActive(true);
            }
            
            if (senderText != null)
            {
                senderText.text = $"{email.emailData.senderName} ({email.emailData.senderTitle})";
            }
            
            if (subjectText != null)
            {
                subjectText.text = email.emailData.subject;
            }
            
            if (bodyText != null)
            {
                bodyText.text = email.emailData.body;
            }
            
            if (attachmentIndicator != null)
            {
                attachmentIndicator.SetActive(email.emailData.hasAttachment);
            }
        }
        
        /// <summary>
        /// 필수 이메일 읽음 처리
        /// </summary>
        private void OnMandatoryEmailRead()
        {
            Debug.Log("[UIEmailPanel] ✓ Mandatory email read! Enabling start work button...");
            
            if (startWorkButton != null)
            {
                startWorkButton.interactable = true;
            }
        }
        
        /// <summary>
        /// 업무 시작 버튼 클릭
        /// </summary>
        private void OnStartWorkClicked()
        {
            Debug.Log("[UIEmailPanel] ===== START WORK CLICKED =====");
            
            // 버튼 비활성화
            if (startWorkButton != null)
            {
                startWorkButton.interactable = false;
            }
            
            // 부드러운 전환
            StartCoroutine(TransitionToDocumentReview());
        }
        
        /// <summary>
        /// 서류 심사로 전환 (코루틴)
        /// </summary>
        private IEnumerator TransitionToDocumentReview()
        {
            Debug.Log("[UIEmailPanel] Transition starting...");
            
            // 짧은 대기
            yield return new WaitForSeconds(0.3f);
            
            // 패널 비활성화
            gameObject.SetActive(false);
            Debug.Log("[UIEmailPanel] Panel hidden");
            
            // 상태 전환
            if (GameManager.Instance != null)
            {
                Debug.Log("[UIEmailPanel] Changing state to DocumentReview");
                GameManager.Instance.ChangeState(GameState.DocumentReview);
            }
            else
            {
                Debug.LogError("[UIEmailPanel] GameManager not found!");
            }
        }
    }
}