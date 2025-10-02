using KDH.Code.Core;
using KDH.Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 일일 성과 보고서 UI
    /// </summary>
    public class UIDailyReportPanel : MonoBehaviour
    {
        [Header("통계 텍스트")]
        [SerializeField] private TextMeshProUGUI processedCountText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI salaryText;
        
        [Header("급여 계산 세부")]
        [SerializeField] private TextMeshProUGUI baseSalaryText;
        [SerializeField] private TextMeshProUGUI bonusText;
        [SerializeField] private TextMeshProUGUI penaltyText;
        [SerializeField] private TextMeshProUGUI totalSalaryText;
        
        [Header("버튼")]
        [SerializeField] private Button continueButton;
        
        private void Start()
        {
            continueButton.onClick.AddListener(OnContinueClicked);
            RegisterEvents();
        }
        
        private void OnDestroy()
        {
            UnregisterEvents();
        }
        
        /// <summary>
        /// 이벤트 등록
        /// </summary>
        private void RegisterEvents()
        {
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
            bool shouldBeActive = newState == GameState.DailyReport;
            gameObject.SetActive(shouldBeActive);
    
            if (shouldBeActive)
            {
                DisplayReport();
            }
    
            Debug.Log($"[UIDailyReportPanel] Active: {shouldBeActive}");
        }
        
        /// <summary>
        /// 보고서 표시
        /// </summary>
        private void DisplayReport()
        {
            if (GameManager.Instance == null)
                return;
            
            // 통계 표시
            int processed = GameManager.Instance.DocumentsProcessedToday;
            float accuracy = GameManager.Instance.TodayAccuracy * 100f;
            
            processedCountText.text = $"{processed}건";
            accuracyText.text = $"{accuracy:F1}%";
            
            // 급여 계산
            DisplaySalaryBreakdown();
        }
        
        /// <summary>
        /// 급여 세부 내역 표시
        /// </summary>
        private void DisplaySalaryBreakdown()
        {
            int baseSalary = GameConstants.BASE_DAILY_SALARY;
            
            int correctCount = (int)(GameManager.Instance.DocumentsProcessedToday * GameManager.Instance.TodayAccuracy);
            int errorCount = GameManager.Instance.DocumentsProcessedToday - correctCount;
            
            int bonus = correctCount * GameConstants.CORRECT_BONUS;
            int penalty = errorCount * GameConstants.ERROR_PENALTY;
            
            int totalSalary = baseSalary + bonus - penalty;
            totalSalary = Mathf.Max(totalSalary, 50000); // 최소 급여
            
            baseSalaryText.text = $"기본급: {baseSalary:N0}원";
            bonusText.text = $"보너스: +{bonus:N0}원";
            penaltyText.text = $"페널티: -{penalty:N0}원";
            totalSalaryText.text = $"총 급여: {totalSalary:N0}원";
            
            salaryText.text = $"{totalSalary:N0}원";
        }
        
        /// <summary>
        /// 계속하기 버튼 클릭
        /// </summary>
        private void OnContinueClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.InnerThoughts);
            }
        }
    }
}