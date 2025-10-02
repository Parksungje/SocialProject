using System;
using System.Collections.Generic;
using KDH.Code.Core;
using KDH.Code.Data;
using KDH.Code.Document;
using UnityEngine;
using ErrorType = KDH.Code.Core.ErrorType;

namespace KDH.Code.Managers
{
    /// <summary>
    /// 서류 심사 프로세스 관리
    /// </summary>
    public class DocumentManager : MonoBehaviour
    {
        private static DocumentManager instance;
        public static DocumentManager Instance => instance;
        
        [Header("현재 서류")]
        [SerializeField] private ApplicantData currentApplicant;
        [SerializeField] private Queue<ApplicantData> documentQueue = new Queue<ApplicantData>();
        
        [Header("심사 통계")]
        [SerializeField] private int processedCount;
        [SerializeField] private int correctCount;
        
        // 이벤트
        public event Action<ApplicantData> OnDocumentPresented;
        public event Action<Decision, bool> OnDocumentProcessed; // (결정, 정답여부)
        public event Action OnAllDocumentsCompleted;
        
        // 프로퍼티
        public ApplicantData CurrentApplicant => currentApplicant;
        public int ProcessedCount => processedCount;
        public int RemainingCount => documentQueue.Count;
        
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
            // GameManager 이벤트 구독
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
        }
        
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.DocumentReview)
            {
                Invoke(nameof(StartDocumentReview), 0.5f);
            }
        }

        /// <summary>
        /// 서류 심사 시작
        /// </summary>
        private void StartDocumentReview()
        {
            Debug.Log("[DocumentManager] Starting document review");
    
            processedCount = 0;
            correctCount = 0;
            documentQueue.Clear();
    
            // 오늘의 목표량만큼 서류 생성
            int targetCount = GameManager.Instance.GetTodayTarget();
            GenerateDocuments(targetCount);
    
            // 첫 서류 제시
            PresentNextDocument();
        }
        
        /// <summary>
        /// 서류 생성
        /// </summary>
        private void GenerateDocuments(int count)
        {
            int currentDay = GameManager.Instance.CurrentDay;
            
            for (int i = 0; i < count; i++)
            {
                ApplicantData applicant = Document.DocumentGenerator.Instance.GenerateApplicant(currentDay);
                documentQueue.Enqueue(applicant);
            }
            
            Debug.Log($"Generated {count} documents for Day {currentDay}");
        }
        
        /// <summary>
        /// 다음 서류 제시
        /// </summary>
        private void PresentNextDocument()
        {
            if (documentQueue.Count > 0)
            {
                currentApplicant = documentQueue.Dequeue();
                OnDocumentPresented?.Invoke(currentApplicant);
                
                Debug.Log($"Presenting document: {currentApplicant.fullName}");
            }
            else
            {
                // 모든 서류 완료
                currentApplicant = null;
                OnAllDocumentsCompleted?.Invoke();
                
                // GameManager에 하루 완료 알림
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeState(GameState.DailyReport);
                }
            }
        }
        
        /// <summary>
        /// 서류 처리 (플레이어 결정)
        /// </summary>
        public void ProcessDocument(Decision playerDecision)
        {
            if (currentApplicant == null)
            {
                Debug.LogWarning("No current applicant to process!");
                return;
            }
            
            // 정답 확인
            bool isCorrect = playerDecision == currentApplicant.correctDecision;
            
            processedCount++;
            if (isCorrect)
                correctCount++;
            
            // 파라미터 조정
            AdjustParametersBasedOnDecision(playerDecision, isCorrect);
            
            // 이벤트 발생
            OnDocumentProcessed?.Invoke(playerDecision, isCorrect);
            
            // GameManager에 기록
            GameManager.Instance.RecordDocumentDecision(isCorrect);
            
            Debug.Log($"Processed: {currentApplicant.fullName} - Player: {playerDecision}, Correct: {currentApplicant.correctDecision}, Result: {(isCorrect ? "✓" : "✗")}");
            
            // 다음 서류로
            Invoke(nameof(PresentNextDocument), GameConstants.FEEDBACK_DISPLAY_TIME);
        }
        
        /// <summary>
        /// 결정에 따른 파라미터 조정
        /// </summary>
        private void AdjustParametersBasedOnDecision(Decision playerDecision, bool isCorrect)
        {
            if (ParameterManager.Instance == null)
                return;
            
            // 규칙 엔진으로 평가
            var evaluationResult = RuleEngine.Instance.EvaluateApplicant(currentApplicant);
            
            if (evaluationResult.primaryRule != null)
            {
                // 규칙이 적용된 경우
                if (isCorrect)
                {
                    // 규칙을 따름
                    ParameterManager.Instance.OnRuleFollowed(evaluationResult.primaryRule);
                }
                else
                {
                    // 규칙을 위반함
                    ParameterManager.Instance.OnRuleViolated(evaluationResult.primaryRule);
                }
            }
            else
            {
                // 일반 결정
                if (isCorrect)
                {
                    ParameterManager.Instance.AdjustLoyalty(1);
                    ParameterManager.Instance.RecordFairDecision();
                }
                else
                {
                    ParameterManager.Instance.AdjustLoyalty(-2);
                }
            }
        }
        
        /// <summary>
        /// 현재 서류에 오류가 있는지 확인
        /// </summary>
        public bool HasError()
        {
            return currentApplicant != null && currentApplicant.errorType != ErrorType.None;
        }
        
        /// <summary>
        /// 오류 위치 반환
        /// </summary>
        public string GetErrorLocation()
        {
            if (currentApplicant != null && currentApplicant.errorType != ErrorType.None)
            {
                return currentApplicant.errorLocation;
            }
            return string.Empty;
        }
        
        /// <summary>
        /// 진행률 반환 (0~1)
        /// </summary>
        public float GetProgress()
        {
            int target = GameManager.Instance.GetTodayTarget();
            return target > 0 ? (float)processedCount / target : 0f;
        }
        
        /// <summary>
        /// 현재 정확도 반환
        /// </summary>
        public float GetCurrentAccuracy()
        {
            return processedCount > 0 ? (float)correctCount / processedCount : 0f;
        }
    }
}