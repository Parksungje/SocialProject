using System;
using System.Collections.Generic;
using System.Linq;
using KDH.Code.Core;
using KDH.Code.Data;
using UnityEngine;

namespace KDH.Code.Managers
{
    /// <summary>
    /// 이메일 시스템 관리
    /// </summary>
    public class EmailManager : MonoBehaviour
    {
        private static EmailManager instance;
        public static EmailManager Instance => instance;
        
        [Header("이메일 데이터베이스")]
        [SerializeField] private List<EmailData> allEmails = new List<EmailData>();
        
        [Header("현재 이메일")]
        [SerializeField] private List<EmailInstance> todayEmails = new List<EmailInstance>();
        [SerializeField] private EmailInstance mandatoryEmail;
        
        // 이벤트
        public event Action<List<EmailInstance>> OnEmailsReceived;
        public event Action<EmailInstance> OnEmailRead;
        public event Action OnMandatoryEmailRead;
        
        // 프로퍼티
        public List<EmailInstance> TodayEmails => todayEmails;
        public EmailInstance MandatoryEmail => mandatoryEmail;
        public bool HasReadMandatoryEmail => mandatoryEmail != null && mandatoryEmail.isRead;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            Debug.Log("[EmailManager] ===== START =====");
    
            LoadAllEmails();
    
            // GameManager 이벤트 구독
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                Debug.Log("[EmailManager] Subscribed to GameManager events");
        
                // ✅ 현재 상태가 EmailBriefing이면 즉시 실행 (타이밍 이슈 대비)
                if (GameManager.Instance.CurrentState == GameState.EmailBriefing)
                {
                    Debug.Log("[EmailManager] Current state is already EmailBriefing! Generating emails now...");
                    GenerateTodayEmails();
                }
            }
            else
            {
                Debug.LogError("[EmailManager] GameManager not found!");
            }
        }
        
        /// <summary>
        /// 모든 이메일 로드
        /// </summary>
        private void LoadAllEmails()
        {
            Debug.Log("[EmailManager] Loading emails from Resources...");
            
            EmailData[] emails = Resources.LoadAll<EmailData>("Emails");
            allEmails = emails.ToList();
            
            Debug.Log($"[EmailManager] Loaded {allEmails.Count} email templates");
            
            // 디버그: 각 이메일 정보 출력
            foreach (var email in allEmails)
            {
                Debug.Log($"[EmailManager] - {email.emailId}: Day {email.applicableDay}, Mandatory: {email.isMandatory}");
            }
        }
        
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            Debug.Log($"[EmailManager] ===== OnGameStateChanged =====");
            Debug.Log($"[EmailManager] New state: {newState}");
            
            if (newState == GameState.EmailBriefing)
            {
                Debug.Log("[EmailManager] Starting to generate today's emails...");
                GenerateTodayEmails();
            }
        }
        
        /// <summary>
        /// 오늘의 이메일 생성
        /// </summary>
        private void GenerateTodayEmails()
        {
            Debug.Log("[EmailManager] ===== GenerateTodayEmails START =====");
            
            todayEmails.Clear();
            mandatoryEmail = null;
            
            int currentDay = GameManager.Instance.CurrentDay;
            Debug.Log($"[EmailManager] Current day: {currentDay}");
            Debug.Log($"[EmailManager] All emails count: {allEmails.Count}");
            
            // 해당 일차의 이메일들 필터링
            var applicableEmails = allEmails.Where(e => e.IsActiveOnDay(currentDay)).ToList();
            Debug.Log($"[EmailManager] Applicable emails count: {applicableEmails.Count}");
            
            // ✅ 이메일이 없으면 더미 생성
            if (applicableEmails.Count == 0)
            {
                Debug.LogWarning("[EmailManager] No applicable emails found! Creating dummy email...");
                CreateDummyEmail(currentDay);
                Debug.Log("[EmailManager] ===== GenerateTodayEmails END (DUMMY) =====");
                return;
            }
            
            // 이메일 인스턴스 생성
            foreach (var emailData in applicableEmails)
            {
                Debug.Log($"[EmailManager] Adding email: '{emailData.subject}' (Day {emailData.applicableDay}, Mandatory: {emailData.isMandatory})");
                
                var emailInstance = new EmailInstance(emailData);
                todayEmails.Add(emailInstance);
                
                // 필수 이메일 (업무 지침)
                if (emailData.isMandatory)
                {
                    mandatoryEmail = emailInstance;
                    Debug.Log($"[EmailManager] ✓ Mandatory email set: '{emailData.subject}'");
                }
            }
            
            Debug.Log($"[EmailManager] ----------------------------------------");
            Debug.Log($"[EmailManager] Total emails created: {todayEmails.Count}");
            Debug.Log($"[EmailManager] Mandatory email: {(mandatoryEmail != null ? "YES" : "NO")}");
            Debug.Log($"[EmailManager] ----------------------------------------");
            
            // 이벤트 발생
            Debug.Log("[EmailManager] Invoking OnEmailsReceived event...");
            OnEmailsReceived?.Invoke(todayEmails);
            
            Debug.Log($"[EmailManager] Day {currentDay}: {todayEmails.Count} emails received ({(mandatoryEmail != null ? "1 mandatory" : "0 mandatory")})");
            Debug.Log("[EmailManager] ===== GenerateTodayEmails END =====");
        }
        
        /// <summary>
        /// 더미 이메일 생성 (임시)
        /// </summary>
        private void CreateDummyEmail(int day)
        {
            Debug.Log($"[EmailManager] Creating dummy email for Day {day}...");
            
            var dummyData = ScriptableObject.CreateInstance<EmailData>();
            dummyData.emailId = $"dummy_email_day{day}";
            dummyData.emailType = EmailType.WorkGuideline;
            dummyData.applicableDay = -1;
            dummyData.senderName = "인사팀";
            dummyData.senderTitle = "시스템 관리자";
            dummyData.subject = $"{day}일차 업무 지침";
            dummyData.body = $"안녕하십니까.\n\n{day}일차 채용 심사를 시작합니다.\n\n" +
                             $"오늘의 목표: {GameManager.Instance.GetTodayTarget()}건\n\n" +
                             "업무 시작 버튼을 눌러주세요.";
            dummyData.hasAttachment = false;
            dummyData.isMandatory = true;
            
            var dummyEmail = new EmailInstance(dummyData);
            todayEmails.Add(dummyEmail);
            mandatoryEmail = dummyEmail;
            
            Debug.Log("[EmailManager] ✓ Dummy email created successfully");
            
            OnEmailsReceived?.Invoke(todayEmails);
        }
        
        /// <summary>
        /// 이메일 읽기
        /// </summary>
        public void ReadEmail(EmailInstance email)
        {
            if (email == null || email.isRead)
                return;
            
            Debug.Log($"[EmailManager] Reading email: '{email.emailData.subject}'");
            
            email.isRead = true;
            OnEmailRead?.Invoke(email);
            
            // 필수 이메일인 경우
            if (email.emailData.isMandatory)
            {
                Debug.Log("[EmailManager] ✓ Mandatory email read!");
                OnMandatoryEmailRead?.Invoke();
            }
        }
        
        /// <summary>
        /// 읽지 않은 이메일 수
        /// </summary>
        public int GetUnreadCount()
        {
            return todayEmails.Count(e => !e.isRead);
        }
        
        /// <summary>
        /// 특정 타입의 이메일 가져오기
        /// </summary>
        public List<EmailInstance> GetEmailsByType(EmailType type)
        {
            return todayEmails.Where(e => e.emailData.emailType == type).ToList();
        }
    }
}