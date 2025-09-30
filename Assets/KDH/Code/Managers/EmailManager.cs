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
            LoadAllEmails();
            
            // GameManager 이벤트 구독
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
        }
        
        /// <summary>
        /// 모든 이메일 로드
        /// </summary>
        private void LoadAllEmails()
        {
            EmailData[] emails = Resources.LoadAll<EmailData>("Emails");
            allEmails = emails.ToList();
            
            Debug.Log($"Loaded {allEmails.Count} email templates");
        }
        
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.EmailBriefing)
            {
                GenerateTodayEmails();
            }
        }
        
        /// <summary>
        /// 오늘의 이메일 생성
        /// </summary>
        private void GenerateTodayEmails()
        {
            todayEmails.Clear();
            mandatoryEmail = null;
            
            int currentDay = GameManager.Instance.CurrentDay;
            
            // 해당 일차의 이메일들 필터링
            var applicableEmails = allEmails.Where(e => e.IsActiveOnDay(currentDay)).ToList();
            
            foreach (var emailData in applicableEmails)
            {
                var emailInstance = new EmailInstance(emailData);
                todayEmails.Add(emailInstance);
                
                // 필수 이메일 (업무 지침)
                if (emailData.isMandatory)
                {
                    mandatoryEmail = emailInstance;
                }
            }
            
            OnEmailsReceived?.Invoke(todayEmails);
            
            Debug.Log($"Day {currentDay}: {todayEmails.Count} emails received ({(mandatoryEmail != null ? "1 mandatory" : "0 mandatory")})");
        }
        
        /// <summary>
        /// 이메일 읽기
        /// </summary>
        public void ReadEmail(EmailInstance email)
        {
            if (email == null || email.isRead)
                return;
            
            email.isRead = true;
            OnEmailRead?.Invoke(email);
            
            // 필수 이메일인 경우
            if (email.emailData.isMandatory)
            {
                OnMandatoryEmailRead?.Invoke();
            }
            
            Debug.Log($"Email read: {email.emailData.subject}");
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