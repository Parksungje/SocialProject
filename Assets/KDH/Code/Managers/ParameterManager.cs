using System;
using KDH.Code.Core;
using UnityEngine;

namespace KDH.Code.Managers
{
    /// <summary>
    /// 게임 파라미터(충성도, 양심, 불평등 지수) 관리
    /// </summary>
    public class ParameterManager : MonoBehaviour
    {
        private static ParameterManager instance;
        public static ParameterManager Instance => instance;
        
        [Header("파라미터")]
        [SerializeField] private int corporateLoyalty = 50;
        [SerializeField] private int personalConscience = 50;
        [SerializeField] private int socialInequalityIndex = 0;
        
        [Header("누적 통계")]
        [SerializeField] private int totalDiscriminatoryDecisions;
        [SerializeField] private int totalFairDecisions;
        [SerializeField] private int totalBiasReinforcedDecisions;
        
        // 이벤트
        public event Action<int> OnLoyaltyChanged;
        public event Action<int> OnConscienceChanged;
        public event Action<int> OnInequalityChanged;
        
        // 프로퍼티
        public int CorporateLoyalty => corporateLoyalty;
        public int PersonalConscience => personalConscience;
        public int SocialInequalityIndex => socialInequalityIndex;
        
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
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
        
        /// <summary>
        /// 충성도 조정
        /// </summary>
        public void AdjustLoyalty(int amount)
        {
            int oldValue = corporateLoyalty;
            corporateLoyalty = Mathf.Clamp(corporateLoyalty + amount, 
                GameConstants.PARAM_MIN, GameConstants.PARAM_MAX);
            
            if (oldValue != corporateLoyalty)
            {
                OnLoyaltyChanged?.Invoke(corporateLoyalty);
                Debug.Log($"Loyalty: {oldValue} → {corporateLoyalty} ({amount:+#;-#;0})");
            }
        }
        
        /// <summary>
        /// 양심 조정
        /// </summary>
        public void AdjustConscience(int amount)
        {
            int oldValue = personalConscience;
            personalConscience = Mathf.Clamp(personalConscience + amount, 
                GameConstants.PARAM_MIN, GameConstants.PARAM_MAX);
            
            if (oldValue != personalConscience)
            {
                OnConscienceChanged?.Invoke(personalConscience);
                Debug.Log($"Conscience: {oldValue} → {personalConscience} ({amount:+#;-#;0})");
            }
        }
        
        /// <summary>
        /// 불평등 지수 조정
        /// </summary>
        public void AdjustInequalityIndex(int amount)
        {
            int oldValue = socialInequalityIndex;
            socialInequalityIndex = Mathf.Clamp(socialInequalityIndex + amount, 
                GameConstants.PARAM_MIN, GameConstants.INEQUALITY_INDEX_MAX);
            
            if (oldValue != socialInequalityIndex)
            {
                OnInequalityChanged?.Invoke(socialInequalityIndex);
                Debug.Log($"Inequality Index: {oldValue} → {socialInequalityIndex} ({amount:+#;-#;0})");
            }
        }
        
        /// <summary>
        /// 규칙 준수 시 파라미터 변화 처리
        /// </summary>
        public void OnRuleFollowed(Data.HiringRule rule)
        {
            AdjustLoyalty(rule.loyaltyImpactOnFollow);
            AdjustConscience(rule.conscienceImpactOnFollow);
            
            // 비윤리적 규칙 준수 시 불평등 증가
            if (!rule.isEthical)
            {
                totalDiscriminatoryDecisions++;
                AdjustInequalityIndex(2);
            }
            else
            {
                totalFairDecisions++;
            }
        }
        
        /// <summary>
        /// 규칙 위반 시 파라미터 변화 처리
        /// </summary>
        public void OnRuleViolated(Data.HiringRule rule)
        {
            AdjustLoyalty(rule.loyaltyImpactOnViolate);
            AdjustConscience(rule.conscienceImpactOnViolate);
            
            // 비윤리적 규칙 거부 시 불평등 감소
            if (!rule.isEthical)
            {
                totalFairDecisions++;
                AdjustInequalityIndex(-1);
            }
        }
        
        /// <summary>
        /// 차별적 결정 기록
        /// </summary>
        public void RecordDiscriminatoryDecision()
        {
            totalDiscriminatoryDecisions++;
            AdjustInequalityIndex(2);
        }
        
        /// <summary>
        /// 공정한 결정 기록
        /// </summary>
        public void RecordFairDecision()
        {
            totalFairDecisions++;
            AdjustInequalityIndex(-1);
        }
        
        /// <summary>
        /// 편견 강화 결정 기록
        /// </summary>
        public void RecordBiasReinforcedDecision()
        {
            totalBiasReinforcedDecisions++;
            AdjustInequalityIndex(3);
        }
        
        /// <summary>
        /// 현재 가족 분위기 결정
        /// </summary>
        public FamilyMood GetCurrentFamilyMood()
        {
            // 양쪽 다 낮은 경우 (위기)
            if (corporateLoyalty < GameConstants.ENDING_LOW_THRESHOLD && 
                personalConscience < GameConstants.ENDING_LOW_THRESHOLD)
            {
                return FamilyMood.Crisis;
            }
            
            // 높은 충성도
            if (corporateLoyalty >= GameConstants.ENDING_MID_HIGH_THRESHOLD)
            {
                return FamilyMood.HighLoyalty;
            }
            
            // 높은 양심
            if (personalConscience >= GameConstants.ENDING_MID_HIGH_THRESHOLD)
            {
                return FamilyMood.HighConscience;
            }
            
            // 균형 상태
            return FamilyMood.Balanced;
        }
        
        /// <summary>
        /// 엔딩 결정
        /// </summary>
        public EndingType DetermineEnding()
        {
            // 혁명가 엔딩: 높은 양심 + 시민연대 협력
            bool cooperatedWithCivil = CheckCivilSolidarityCooperation();
            if (personalConscience >= GameConstants.ENDING_HIGH_THRESHOLD && cooperatedWithCivil)
            {
                return EndingType.Revolutionary;
            }
            
            // 충견 엔딩: 높은 충성도, 낮은 양심
            if (corporateLoyalty >= GameConstants.ENDING_HIGH_THRESHOLD && 
                personalConscience <= GameConstants.ENDING_VERY_LOW_THRESHOLD)
            {
                return EndingType.CorporateDog;
            }
            
            // 순교자 엔딩: 낮은 충성도, 높은 양심
            if (corporateLoyalty <= GameConstants.ENDING_VERY_LOW_THRESHOLD && 
                personalConscience >= GameConstants.ENDING_HIGH_THRESHOLD)
            {
                return EndingType.Martyr;
            }
            
            // 파멸 엔딩: 둘 다 낮음
            if (corporateLoyalty < GameConstants.ENDING_LOW_THRESHOLD && 
                personalConscience < GameConstants.ENDING_LOW_THRESHOLD)
            {
                return EndingType.Downfall;
            }
            
            // 균형 엔딩: 둘 다 중간
            return EndingType.Balance;
        }
        
        /// <summary>
        /// 시민연대 협력 여부 확인
        /// </summary>
        private bool CheckCivilSolidarityCooperation()
        {
            // TODO: 시민연대 접촉 이벤트 시스템과 연동
            // 임시로 높은 양심 수치로 판단
            return personalConscience >= GameConstants.ENDING_HIGH_THRESHOLD;
        }
        
        /// <summary>
        /// 파라미터 초기화
        /// </summary>
        public void ResetParameters()
        {
            corporateLoyalty = 50;
            personalConscience = 50;
            socialInequalityIndex = 0;
            
            totalDiscriminatoryDecisions = 0;
            totalFairDecisions = 0;
            totalBiasReinforcedDecisions = 0;
            
            OnLoyaltyChanged?.Invoke(corporateLoyalty);
            OnConscienceChanged?.Invoke(personalConscience);
            OnInequalityChanged?.Invoke(socialInequalityIndex);
        }
        
        /// <summary>
        /// 저장용 데이터 반환
        /// </summary>
        public ParameterSaveData GetSaveData()
        {
            return new ParameterSaveData
            {
                corporateLoyalty = this.corporateLoyalty,
                personalConscience = this.personalConscience,
                socialInequalityIndex = this.socialInequalityIndex,
                totalDiscriminatoryDecisions = this.totalDiscriminatoryDecisions,
                totalFairDecisions = this.totalFairDecisions,
                totalBiasReinforcedDecisions = this.totalBiasReinforcedDecisions
            };
        }
        
        /// <summary>
        /// 저장 데이터 로드
        /// </summary>
        public void LoadSaveData(ParameterSaveData data)
        {
            corporateLoyalty = data.corporateLoyalty;
            personalConscience = data.personalConscience;
            socialInequalityIndex = data.socialInequalityIndex;
            
            totalDiscriminatoryDecisions = data.totalDiscriminatoryDecisions;
            totalFairDecisions = data.totalFairDecisions;
            totalBiasReinforcedDecisions = data.totalBiasReinforcedDecisions;
            
            OnLoyaltyChanged?.Invoke(corporateLoyalty);
            OnConscienceChanged?.Invoke(personalConscience);
            OnInequalityChanged?.Invoke(socialInequalityIndex);
        }
    }
    
    /// <summary>
    /// 파라미터 저장 데이터
    /// </summary>
    [Serializable]
    public class ParameterSaveData
    {
        public int corporateLoyalty;
        public int personalConscience;
        public int socialInequalityIndex;
        public int totalDiscriminatoryDecisions;
        public int totalFairDecisions;
        public int totalBiasReinforcedDecisions;
    }
}