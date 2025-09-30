using System.Collections.Generic;
using System.Linq;
using KDH.Code.Core;
using KDH.Code.Data;
using KDH.Code.Managers;
using UnityEngine;

namespace KDH.Code.Document
{
    /// <summary>
    /// 채용 규칙 엔진
    /// 규칙 우선순위에 따라 지원자 평가
    /// </summary>
    public class RuleEngine : MonoBehaviour
    {
        private static RuleEngine instance;
        public static RuleEngine Instance => instance;
        
        [Header("규칙 데이터베이스")]
        [SerializeField] private List<HiringRule> allRules = new List<HiringRule>();
        
        private List<HiringRule> activeRules = new List<HiringRule>();
        
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
            // GameManager의 일차 변경 이벤트 구독
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayChanged += UpdateActiveRules;
            }
            
            LoadAllRules();
        }
        
        /// <summary>
        /// 모든 규칙 로드
        /// </summary>
        private void LoadAllRules()
        {
            // Resources 폴더에서 모든 HiringRule 로드
            HiringRule[] rules = Resources.LoadAll<HiringRule>("Rules");
            allRules = rules.ToList();
            
            Debug.Log($"Loaded {allRules.Count} hiring rules");
            
            // 초기 활성 규칙 설정
            if (GameManager.Instance != null)
            {
                UpdateActiveRules(GameManager.Instance.CurrentDay);
            }
        }
        
        /// <summary>
        /// 현재 일차에 맞는 활성 규칙 업데이트
        /// </summary>
        private void UpdateActiveRules(int currentDay)
        {
            activeRules.Clear();
            
            foreach (var rule in allRules)
            {
                if (rule.IsActiveOnDay(currentDay))
                {
                    activeRules.Add(rule);
                }
            }
            
            // 우선순위 정렬 (높은 것부터)
            activeRules = activeRules.OrderByDescending(r => r.priorityLevel).ToList();
            
            Debug.Log($"Day {currentDay}: {activeRules.Count} active rules");
        }
        
        /// <summary>
        /// 지원자 평가 및 올바른 결정 반환
        /// </summary>
        public RuleEvaluationResult EvaluateApplicant(ApplicantData applicant)
        {
            var result = new RuleEvaluationResult
            {
                applicant = applicant,
                recommendedDecision = Decision.None,
                appliedRules = new List<HiringRule>(),
                isEthical = true
            };
            
            // 우선순위 순서대로 규칙 적용
            foreach (var rule in activeRules)
            {
                if (DoesRuleApply(rule, applicant))
                {
                    result.appliedRules.Add(rule);
                    
                    // 강제 결정이 있는 경우
                    if (rule.enforcedDecision != Decision.None)
                    {
                        result.recommendedDecision = rule.enforcedDecision;
                        result.primaryRule = rule;
                        
                        // 비윤리적 규칙 확인
                        if (!rule.isEthical)
                        {
                            result.isEthical = false;
                        }
                        
                        // 금지 또는 우대 규칙은 즉시 반환
                        if (rule.ruleType == RuleType.Prohibition || 
                            rule.ruleType == RuleType.Priority)
                        {
                            break;
                        }
                    }
                }
            }
            
            // 적용된 규칙이 없으면 기본 평가
            if (result.recommendedDecision == Decision.None)
            {
                result.recommendedDecision = EvaluateBasicCriteria(applicant);
            }
            
            return result;
        }
        
        /// <summary>
        /// 규칙이 지원자에게 적용되는지 확인
        /// </summary>
        private bool DoesRuleApply(HiringRule rule, ApplicantData applicant)
        {
            if (rule.conditions == null || rule.conditions.Length == 0)
                return false;
            
            // 모든 조건이 만족되어야 함 (AND 조건)
            foreach (var condition in rule.conditions)
            {
                if (!condition.Evaluate(applicant))
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 기본 평가 기준 (규칙이 없을 때)
        /// </summary>
        private Decision EvaluateBasicCriteria(ApplicantData applicant)
        {
            int score = 0;
            
            // 학력 점수
            switch (applicant.universityTier)
            {
                case UniversityTier.VNU:
                    score += 40;
                    break;
                case UniversityTier.CIT:
                    score += 35;
                    break;
                case UniversityTier.Mid:
                    score += 25;
                    break;
                case UniversityTier.CCC:
                    score += 15;
                    break;
            }
            
            // 학점 점수
            if (applicant.gpa >= 4.0f)
                score += 30;
            else if (applicant.gpa >= 3.5f)
                score += 20;
            else if (applicant.gpa >= 3.0f)
                score += 10;
            
            // 경력 점수
            if (applicant.experienceMonths >= 36)
                score += 30;
            else if (applicant.experienceMonths >= 24)
                score += 20;
            else if (applicant.experienceMonths >= 12)
                score += 10;
            
            // 추천서 점수
            if (applicant.hasRecommendationLetter)
                score += 10;
            
            // 범죄 기록 감점
            if (applicant.hasCriminalRecord)
                score -= 50;
            
            // 합격 기준: 60점 이상
            return score >= 60 ? Decision.Approve : Decision.Reject;
        }
        
        /// <summary>
        /// 현재 활성 규칙 목록 반환
        /// </summary>
        public List<HiringRule> GetActiveRules()
        {
            return new List<HiringRule>(activeRules);
        }
        
        /// <summary>
        /// 특정 규칙 ID로 규칙 찾기
        /// </summary>
        public HiringRule GetRuleById(string ruleId)
        {
            return allRules.FirstOrDefault(r => r.ruleId == ruleId);
        }
    }
    
    /// <summary>
    /// 규칙 평가 결과
    /// </summary>
    public class RuleEvaluationResult
    {
        public ApplicantData applicant;
        public Decision recommendedDecision;
        public HiringRule primaryRule;              // 주요 적용 규칙
        public List<HiringRule> appliedRules;       // 적용된 모든 규칙
        public bool isEthical;                      // 윤리적인 결정인가?
        
        /// <summary>
        /// 결정 이유 텍스트 생성
        /// </summary>
        public string GetReasonText()
        {
            if (primaryRule != null)
            {
                return primaryRule.ruleDescription;
            }
            
            return recommendedDecision == Decision.Approve 
                ? "기본 평가 기준을 충족합니다." 
                : "기본 평가 기준에 미달합니다.";
        }
    }
}