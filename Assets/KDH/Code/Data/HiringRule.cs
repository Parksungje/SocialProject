using System;
using UnityEngine;

namespace KDH.Code.Data
{
    /// <summary>
    /// 채용 규칙 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New Rule", menuName = "Equality Corp/Hiring Rule")]
    public class HiringRule : ScriptableObject
    {
        [Header("규칙 기본 정보")]
        public string ruleId;
        public string ruleName;
        public Core.RuleType ruleType;
        public int applicableFromDay;      // 적용 시작 일차
        public int applicableToDay;        // 적용 종료 일차 (-1이면 끝까지)
        
        [Header("규칙 내용")]
        [TextArea(3, 5)]
        public string ruleDescription;
        public bool isEthical;             // 윤리적인 규칙인가?
        
        [Header("규칙 조건")]
        public RuleCondition[] conditions;
        
        [Header("규칙 결과")]
        public Core.Decision enforcedDecision;  // 강제 결정 (None이면 강제 없음)
        public int priorityLevel;          // 우선순위 (높을수록 먼저 적용)
        
        [Header("파라미터 영향")]
        public int loyaltyImpactOnFollow;      // 규칙 준수 시 충성도 변화
        public int conscienceImpactOnFollow;   // 규칙 준수 시 양심 변화
        public int loyaltyImpactOnViolate;     // 규칙 위반 시 충성도 변화
        public int conscienceImpactOnViolate;  // 규칙 위반 시 양심 변화
        
        /// <summary>
        /// 특정 일차에 이 규칙이 활성화되어 있는지 확인
        /// </summary>
        public bool IsActiveOnDay(int day)
        {
            if (day < applicableFromDay)
                return false;
            
            if (applicableToDay == -1)
                return true;
            
            return day <= applicableToDay;
        }
    }
    
    /// <summary>
    /// 규칙 조건
    /// </summary>
    [Serializable]
    public class RuleCondition
    {
        public enum ConditionField
        {
            Region,
            UniversityTier,
            University,
            Major,
            GPA,
            Experience,
            Age,
            HasCriminalRecord,
            HasRecommendation
        }
        
        public enum ConditionOperator
        {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan,
            GreaterOrEqual,
            LessOrEqual,
            Contains
        }
        
        public ConditionField field;
        public ConditionOperator operatorType;
        public string value;  // 비교할 값 (문자열로 저장 후 파싱)
        
        /// <summary>
        /// 지원자가 이 조건을 만족하는지 확인
        /// </summary>
        public bool Evaluate(ApplicantData applicant)
        {
            switch (field)
            {
                case ConditionField.Region:
                    return EvaluateEnum(applicant.region.ToString(), value);
                    
                case ConditionField.UniversityTier:
                    return EvaluateEnum(applicant.universityTier.ToString(), value);
                    
                case ConditionField.University:
                    return EvaluateString(applicant.universityName, value);
                    
                case ConditionField.Major:
                    return EvaluateString(applicant.major, value);
                    
                case ConditionField.GPA:
                    return EvaluateFloat(applicant.gpa, float.Parse(value));
                    
                case ConditionField.Experience:
                    return EvaluateInt(applicant.experienceMonths, int.Parse(value));
                    
                case ConditionField.Age:
                    return EvaluateInt(applicant.age, int.Parse(value));
                    
                case ConditionField.HasCriminalRecord:
                    return EvaluateBool(applicant.hasCriminalRecord, value);

                case ConditionField.HasRecommendation:
                    return EvaluateBool(applicant.hasRecommendationLetter, value);
                    
                default:
                    return false;
            }
        }
        
        private bool EvaluateBool(bool actual, string expectedStr)
        {
            if (!bool.TryParse(expectedStr, out bool expected))
            {
                Debug.LogWarning($"[RuleCondition] '{expectedStr}' 는 bool 로 변환할 수 없습니다. false 로 처리합니다.");
                return false;
            }
            return actual == expected;
        }
        
        private bool EvaluateEnum(string actual, string expected)
        {
            switch (operatorType)
            {
                case ConditionOperator.Equals:
                    return actual == expected;
                case ConditionOperator.NotEquals:
                    return actual != expected;
                default:
                    return false;
            }
        }
        
        private bool EvaluateString(string actual, string expected)
        {
            switch (operatorType)
            {
                case ConditionOperator.Equals:
                    return actual == expected;
                case ConditionOperator.NotEquals:
                    return actual != expected;
                case ConditionOperator.Contains:
                    return actual.Contains(expected);
                default:
                    return false;
            }
        }
        
        private bool EvaluateFloat(float actual, float expected)
        {
            switch (operatorType)
            {
                case ConditionOperator.Equals:
                    return Mathf.Approximately(actual, expected);
                case ConditionOperator.NotEquals:
                    return !Mathf.Approximately(actual, expected);
                case ConditionOperator.GreaterThan:
                    return actual > expected;
                case ConditionOperator.LessThan:
                    return actual < expected;
                case ConditionOperator.GreaterOrEqual:
                    return actual >= expected;
                case ConditionOperator.LessOrEqual:
                    return actual <= expected;
                default:
                    return false;
            }
        }
        
        private bool EvaluateInt(int actual, int expected)
        {
            switch (operatorType)
            {
                case ConditionOperator.Equals:
                    return actual == expected;
                case ConditionOperator.NotEquals:
                    return actual != expected;
                case ConditionOperator.GreaterThan:
                    return actual > expected;
                case ConditionOperator.LessThan:
                    return actual < expected;
                case ConditionOperator.GreaterOrEqual:
                    return actual >= expected;
                case ConditionOperator.LessOrEqual:
                    return actual <= expected;
                default:
                    return false;
            }
        }
        
        private bool EvaluateBool(bool actual, bool expected)
        {
            return actual == expected;
        }
    }
}