using System;
using KDH.Code.Core;
using UnityEngine;

namespace KDH.Code.Managers
{
    /// <summary>
    /// 게임 전체 흐름을 관리하는 메인 매니저
    /// Singleton 패턴 적용
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        instance = go.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }
        
        [Header("현재 게임 상태")]
        [SerializeField] private GameState currentState;
        [SerializeField] private int currentDay = 1;
        
        [Header("일일 통계")]
        [SerializeField] private int documentsProcessedToday;
        [SerializeField] private int correctDecisionsToday;
        [SerializeField] private int totalDecisionsToday;
        
        // 이벤트
        public event Action<GameState> OnStateChanged;
        public event Action<int> OnDayChanged;
        public event Action OnDayCompleted;
        
        // 프로퍼티
        public GameState CurrentState => currentState;
        public int CurrentDay => currentDay;
        public int DocumentsProcessedToday => documentsProcessedToday;
        public float TodayAccuracy => totalDecisionsToday > 0 
            ? (float)correctDecisionsToday / totalDecisionsToday 
            : 0f;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            Application.targetFrameRate = GameConstants.TARGET_FPS;
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
        
        private void Start()
        {
            // 게임 씬에서만 초기화
            if (Utils.SceneTransitionManager.Instance != null && 
                Utils.SceneTransitionManager.Instance.IsGameScene())
            {
                InitializeGame();
            }
        }
        
        /// <summary>
        /// 게임 초기화
        /// </summary>
        private void InitializeGame()
        {
            Debug.Log("[GameManager] ===== InitializeGame =====");
            
            // 저장된 게임이 로드되지 않았다면 새 게임 시작
            if (currentDay <= 0)
            {
                currentDay = 1;
            }
            
            ChangeState(GameState.DayStart);
        }
        
        /// <summary>
        /// 게임 상태 변경
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (currentState == newState)
                return;
            
            Debug.Log($"[GameManager] ===== State Change =====");
            Debug.Log($"[GameManager] {currentState} → {newState}");
            
            currentState = newState;
            OnStateChanged?.Invoke(newState);
            
            HandleStateTransition(newState);
        }
        
        /// <summary>
        /// 상태 전환 처리
        /// </summary>
        private void HandleStateTransition(GameState state)
        {
            Debug.Log($"[GameManager] Handling transition: {state}");
            
            switch (state)
            {
                case GameState.DayStart:
                    StartNewDay();
                    break;
                    
                case GameState.EmailBriefing:
                    // EmailManager가 처리
                    break;
                    
                case GameState.DocumentReview:
                    // DocumentManager가 처리
                    break;
                    
                case GameState.DailyReport:
                    GenerateDailyReport();
                    break;
                    
                case GameState.InnerThoughts:
                    // 3일마다만 실행
                    if (ShouldShowInnerThoughts())
                    {
                        // InnerThoughtsManager가 처리
                    }
                    else
                    {
                        ChangeState(GameState.FamilyTalk);
                    }
                    break;
                    
                case GameState.FamilyTalk:
                    // 3일마다만 실행
                    if (ShouldShowFamilyTalk())
                    {
                        // FamilyManager가 처리
                    }
                    else
                    {
                        ChangeState(GameState.DayEnd);
                    }
                    break;
                    
                case GameState.DayEnd:
                    EndDay();
                    break;
                    
                case GameState.FinalChoice:
                    // 16일차 최종 선택
                    break;
                    
                case GameState.Ending:
                    DetermineEnding();
                    break;
            }
        }
        
        /// <summary>
        /// 새로운 하루 시작
        /// </summary>
        private void StartNewDay()
        {
            documentsProcessedToday = 0;
            correctDecisionsToday = 0;
            totalDecisionsToday = 0;
    
            OnDayChanged?.Invoke(currentDay);
    
            Debug.Log($"========================================");
            Debug.Log($"[GameManager] Day {currentDay} started");
            Debug.Log($"[GameManager] Target: {GetTodayTarget()} documents");
            Debug.Log($"========================================");
    
            // ✅ 다른 Manager들이 초기화될 때까지 약간 대기
            StartCoroutine(DelayedEmailBriefing());
        }

        /// <summary>
        /// EmailBriefing으로 전환 (지연)
        /// </summary>
        private System.Collections.IEnumerator DelayedEmailBriefing()
        {
            Debug.Log("[GameManager] Waiting for other managers to initialize...");
    
            // 1프레임 대기
            yield return null;
    
            Debug.Log("[GameManager] All managers ready. Moving to EmailBriefing...");
            ChangeState(GameState.EmailBriefing);
        }
        
        /// <summary>
        /// 서류 처리 결과 기록
        /// </summary>
        public void RecordDocumentDecision(bool wasCorrect)
        {
            documentsProcessedToday++;
            totalDecisionsToday++;
            
            if (wasCorrect)
                correctDecisionsToday++;
            
            Debug.Log($"[GameManager] Decision recorded: {(wasCorrect ? "Correct" : "Wrong")}");
            Debug.Log($"[GameManager] Progress: {documentsProcessedToday}/{GetTodayTarget()}");
            
            // 목표량 달성 확인
            if (documentsProcessedToday >= GetTodayTarget())
            {
                Debug.Log("[GameManager] Target reached! Moving to DailyReport");
                ChangeState(GameState.DailyReport);
            }
        }
        
        /// <summary>
        /// 오늘의 목표 서류량
        /// </summary>
        public int GetTodayTarget()
        {
            if (currentDay >= 1 && currentDay <= GameConstants.TOTAL_DAYS)
                return GameConstants.DAILY_DOCUMENT_TARGETS[currentDay];
            return 0;
        }
        
        /// <summary>
        /// 오늘의 오류율
        /// </summary>
        public float GetTodayErrorRate()
        {
            if (currentDay >= 1 && currentDay <= GameConstants.TOTAL_DAYS)
                return GameConstants.DAILY_ERROR_RATES[currentDay];
            return 0f;
        }
        
        /// <summary>
        /// 일일 보고서 생성
        /// </summary>
        private void GenerateDailyReport()
        {
            Debug.Log("[GameManager] Generating daily report...");
            
            int dailySalary = CalculateDailySalary();
            
            // EconomyManager에 전달
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddDailySalary(dailySalary);
            }
            
            // ParameterManager에 정확도 기반 파라미터 조정
            if (ParameterManager.Instance != null)
            {
                float accuracy = TodayAccuracy;
                if (accuracy >= GameConstants.PERFORMANCE_BONUS_THRESHOLD)
                {
                    ParameterManager.Instance.AdjustLoyalty(2);
                }
            }
        }
        
        /// <summary>
        /// 일일 급여 계산
        /// </summary>
        private int CalculateDailySalary()
        {
            int salary = GameConstants.BASE_DAILY_SALARY;
            
            // 정확한 처리 보너스
            salary += correctDecisionsToday * GameConstants.CORRECT_BONUS;
            
            // 오류 페널티
            int errors = totalDecisionsToday - correctDecisionsToday;
            salary -= errors * GameConstants.ERROR_PENALTY;
            
            // 최소 급여 보장
            salary = Mathf.Max(salary, 50000);
            
            Debug.Log($"[GameManager] Daily salary calculated: {salary:N0}원");
            
            return salary;
        }
        
        /// <summary>
        /// 하루 종료
        /// </summary>
        private void EndDay()
        {
            Debug.Log("[GameManager] Day end");
            
            OnDayCompleted?.Invoke();
            
            // 저장
            SaveManager.Instance?.SaveGame();
            
            // 다음 날로 이동
            if (currentDay < GameConstants.TOTAL_DAYS)
            {
                currentDay++;
                Invoke(nameof(StartNextDay), 2f);
            }
            else
            {
                // 최종일 완료
                Debug.Log("[GameManager] All days completed! Moving to FinalChoice");
                ChangeState(GameState.FinalChoice);
            }
        }
        
        private void StartNextDay()
        {
            ChangeState(GameState.DayStart);
        }
        
        /// <summary>
        /// 속마음을 보여줘야 하는지 확인
        /// </summary>
        private bool ShouldShowInnerThoughts()
        {
            return currentDay % GameConstants.FAMILY_TALK_INTERVAL == 0;
        }
        
        /// <summary>
        /// 가족 대화를 보여줘야 하는지 확인
        /// </summary>
        private bool ShouldShowFamilyTalk()
        {
            return currentDay % GameConstants.FAMILY_TALK_INTERVAL == 0;
        }
        
        /// <summary>
        /// 엔딩 결정
        /// </summary>
        private void DetermineEnding()
        {
            Debug.Log("[GameManager] Determining ending...");
            
            if (ParameterManager.Instance == null)
                return;
            
            EndingType ending = ParameterManager.Instance.DetermineEnding();
            
            Debug.Log($"[GameManager] Ending determined: {ending}");
            
            // EndingManager에 전달
            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.ShowEnding(ending);
            }
        }
        
        /// <summary>
        /// 새 게임 시작
        /// </summary>
        public void StartNewGame()
        {
            Debug.Log("[GameManager] ===== Starting New Game =====");
            
            currentDay = 1;
            
            // 모든 매니저 초기화
            ParameterManager.Instance?.ResetParameters();
            EconomyManager.Instance?.ResetEconomy();
            
            ChangeState(GameState.DayStart);
        }
        
        /// <summary>
        /// 게임 로드
        /// </summary>
        public void LoadGame()
        {
            Debug.Log("[GameManager] Loading game...");
            SaveManager.Instance?.LoadGame();
        }
        
        /// <summary>
        /// 현재 일차 설정 (로드용)
        /// </summary>
        public void SetCurrentDay(int day)
        {
            currentDay = Mathf.Clamp(day, 1, GameConstants.TOTAL_DAYS);
            Debug.Log($"[GameManager] Current day set to: {currentDay}");
        }
    }
}