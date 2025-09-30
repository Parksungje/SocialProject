using System;
using KDH.Code.Core;
using UnityEngine;

namespace KDH.Code.Managers
{
    /// <summary>
    /// 경제 시스템 관리 (급여, 지출, 가계부)
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        private static EconomyManager instance;
        public static EconomyManager Instance => instance;
        
        [Header("수입")]
        [SerializeField] private int totalEarnings;
        [SerializeField] private int currentMonthEarnings;
        [SerializeField] private int workDaysThisMonth;
        
        [Header("지출")]
        [SerializeField] private int totalExpenses;
        [SerializeField] private int currentMonthExpenses;
        
        [Header("잔액")]
        [SerializeField] private int currentBalance;
        
        // 이벤트
        public event Action<int> OnSalaryAdded;
        public event Action<int> OnExpensePaid;
        public event Action<int> OnBalanceChanged;
        public event Action<int, int, int> OnMonthlySettlement; // (수입, 지출, 잔액)
        
        // 프로퍼티
        public int TotalEarnings => totalEarnings;
        public int CurrentBalance => currentBalance;
        public int CurrentMonthEarnings => currentMonthEarnings;
        
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
            // GameManager의 일일 완료 이벤트 구독
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayCompleted += CheckMonthlySettlement;
            }
        }
        
        /// <summary>
        /// 일일 급여 추가
        /// </summary>
        public void AddDailySalary(int amount)
        {
            currentMonthEarnings += amount;
            totalEarnings += amount;
            currentBalance += amount;
            workDaysThisMonth++;
            
            OnSalaryAdded?.Invoke(amount);
            OnBalanceChanged?.Invoke(currentBalance);
            
            Debug.Log($"Daily Salary Added: {amount:N0}원 | Balance: {currentBalance:N0}원");
        }
        
        /// <summary>
        /// 지출 처리
        /// </summary>
        public void PayExpense(int amount, string description)
        {
            if (currentBalance < amount)
            {
                Debug.LogWarning($"잔액 부족! 필요: {amount:N0}원, 현재: {currentBalance:N0}원");
                // 대출 또는 빚 시스템 (선택사항)
            }
            
            currentMonthExpenses += amount;
            totalExpenses += amount;
            currentBalance -= amount;
            
            OnExpensePaid?.Invoke(amount);
            OnBalanceChanged?.Invoke(currentBalance);
            
            Debug.Log($"Expense Paid: {amount:N0}원 ({description}) | Balance: {currentBalance:N0}원");
        }
        
        /// <summary>
        /// 월간 정산 확인 (3일마다)
        /// </summary>
        private void CheckMonthlySettlement()
        {
            int currentDay = GameManager.Instance.CurrentDay;
            
            // 3일마다 정산
            if (currentDay % GameConstants.FAMILY_TALK_INTERVAL == 0)
            {
                PerformMonthlySettlement();
            }
        }
        
        /// <summary>
        /// 월간 정산 실행
        /// </summary>
        private void PerformMonthlySettlement()
        {
            // 고정 지출 차감
            int monthlyExpense = GameConstants.TOTAL_MONTHLY_EXPENSE / 
                (GameConstants.TOTAL_DAYS / GameConstants.FAMILY_TALK_INTERVAL);
            
            PayExpense(monthlyExpense, "월세 및 생활비");
            
            // 성과급 확인
            if (ShouldReceivePerformanceBonus())
            {
                int bonus = GameConstants.MONTHLY_PERFORMANCE_BONUS / 
                    (GameConstants.TOTAL_DAYS / GameConstants.FAMILY_TALK_INTERVAL);
                AddDailySalary(bonus);
                Debug.Log($"Performance Bonus: {bonus:N0}원");
            }
            
            OnMonthlySettlement?.Invoke(currentMonthEarnings, monthlyExpense, currentBalance);
            
            // 월간 수입/지출 초기화
            currentMonthEarnings = 0;
            currentMonthExpenses = 0;
            workDaysThisMonth = 0;
        }
        
        /// <summary>
        /// 성과급 수령 조건 확인
        /// </summary>
        private bool ShouldReceivePerformanceBonus()
        {
            // 전체 평균 정확도가 95% 이상인지 확인
            // TODO: 실제 정확도 누적 시스템과 연동
            return false; // 임시
        }
        
        /// <summary>
        /// 잔액 상태 평가
        /// </summary>
        public string GetBalanceStatus()
        {
            if (currentBalance >= 5000000)
                return "여유로움";
            else if (currentBalance >= 2000000)
                return "안정적";
            else if (currentBalance >= 500000)
                return "보통";
            else if (currentBalance >= 0)
                return "빠듯함";
            else
                return "위험";
        }
        
        /// <summary>
        /// 경제 시스템 초기화
        /// </summary>
        public void ResetEconomy()
        {
            totalEarnings = 0;
            totalExpenses = 0;
            currentMonthEarnings = 0;
            currentMonthExpenses = 0;
            currentBalance = 1000000; // 초기 자금
            workDaysThisMonth = 0;
            
            OnBalanceChanged?.Invoke(currentBalance);
            
            Debug.Log("Economy Reset");
        }
        
        /// <summary>
        /// 저장용 데이터 반환
        /// </summary>
        public EconomySaveData GetSaveData()
        {
            return new EconomySaveData
            {
                totalEarnings = this.totalEarnings,
                totalExpenses = this.totalExpenses,
                currentMonthEarnings = this.currentMonthEarnings,
                currentMonthExpenses = this.currentMonthExpenses,
                currentBalance = this.currentBalance,
                workDaysThisMonth = this.workDaysThisMonth
            };
        }
        
        /// <summary>
        /// 저장 데이터 로드
        /// </summary>
        public void LoadSaveData(EconomySaveData data)
        {
            totalEarnings = data.totalEarnings;
            totalExpenses = data.totalExpenses;
            currentMonthEarnings = data.currentMonthEarnings;
            currentMonthExpenses = data.currentMonthExpenses;
            currentBalance = data.currentBalance;
            workDaysThisMonth = data.workDaysThisMonth;
            
            OnBalanceChanged?.Invoke(currentBalance);
        }
    }
    
    /// <summary>
    /// 경제 저장 데이터
    /// </summary>
    [Serializable]
    public class EconomySaveData
    {
        public int totalEarnings;
        public int totalExpenses;
        public int currentMonthEarnings;
        public int currentMonthExpenses;
        public int currentBalance;
        public int workDaysThisMonth;
    }
}