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
            startWorkButton.onClick.AddListener(OnStartWorkClicked);
            startWorkButton.interactable = false;
            emailContentPanel.SetActive(false);
        }
        
        /// <summary>
        /// 이벤트 등록
        /// </summary>
        private void RegisterEvents()
        {
            if (EmailManager.Instance != null)
            {
                EmailManager.Instance.OnEmailsReceived += OnEmailsReceived;
                EmailManager.Instance.OnMandatoryEmailRead += OnMandatoryEmailRead;
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
            gameObject.SetActive(newState == GameState.EmailBriefing);
        }
        
        /// <summary>
        /// 이메일 수신 처리
        /// </summary>
        private void OnEmailsReceived(List<EmailInstance> emails)
        {
            ClearEmailList();
    
            foreach (var email in emails)
            {
                CreateEmailItem(email);
            }
    
            // 필수 이메일 자동 열기
            var mandatoryEmail = EmailManager.Instance.MandatoryEmail;
            if (mandatoryEmail != null)
            {
                DisplayEmail(mandatoryEmail);
                EmailManager.Instance.ReadEmail(mandatoryEmail);
            }
            else
            {
                // ✅ 필수 이메일이 없으면 바로 활성화
                startWorkButton.interactable = true;
            }
        }
        
        /// <summary>
        /// 이메일 목록 초기화
        /// </summary>
        private void ClearEmailList()
        {
            foreach (var item in emailItems)
            {
                Destroy(item.gameObject);
            }
            emailItems.Clear();
        }
        
        /// <summary>
        /// 이메일 아이템 생성
        /// </summary>
        private void CreateEmailItem(EmailInstance email)
        {
            GameObject itemObj = Instantiate(emailItemPrefab, emailListContainer);
            UIEmailItem item = itemObj.GetComponent<UIEmailItem>();
            
            if (item != null)
            {
                item.Setup(email, () => OnEmailItemClicked(email));
                emailItems.Add(item);
            }
        }
        
        /// <summary>
        /// 이메일 아이템 클릭 처리
        /// </summary>
        private void OnEmailItemClicked(EmailInstance email)
        {
            DisplayEmail(email);
            EmailManager.Instance.ReadEmail(email);
        }
        
        /// <summary>
        /// 이메일 내용 표시
        /// </summary>
        private void DisplayEmail(EmailInstance email)
        {
            currentEmail = email;
            emailContentPanel.SetActive(true);
            
            senderText.text = $"{email.emailData.senderName} ({email.emailData.senderTitle})";
            subjectText.text = email.emailData.subject;
            bodyText.text = email.emailData.body;
            
            attachmentIndicator.SetActive(email.emailData.hasAttachment);
        }
        
        /// <summary>
        /// 필수 이메일 읽음 처리
        /// </summary>
        private void OnMandatoryEmailRead()
        {
            startWorkButton.interactable = true;
        }
        
        /// <summary>
        /// 업무 시작 버튼 클릭
        /// </summary>
        private void OnStartWorkClicked()
        {
            startWorkButton.interactable = false;
            
            StartCoroutine(TransitionToDocumentReview());
        }

        /// <summary>
        /// 서류 심사로 전환 (코루틴)
        /// </summary>
        private System.Collections.IEnumerator TransitionToDocumentReview()
        {
            // 짧은 대기
            yield return new WaitForSeconds(0.3f);
    
            // 패널 비활성화
            gameObject.SetActive(false);
    
            // 상태 전환
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.DocumentReview);
            }
        }
    }
}