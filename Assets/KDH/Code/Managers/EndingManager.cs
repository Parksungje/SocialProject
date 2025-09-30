using System.Collections.Generic;
using System.Linq;
using KDH.Code.Core;
using KDH.Code.Data;
using UnityEngine;

namespace KDH.Code.Managers
{
    /// <summary>
    /// 엔딩 시스템 관리
    /// </summary>
    public class EndingManager : MonoBehaviour
    {
        private static EndingManager instance;
        public static EndingManager Instance => instance;
        
        [Header("엔딩 데이터베이스")]
        [SerializeField] private List<EndingData> allEndings = new List<EndingData>();
        
        [Header("현재 엔딩")]
        [SerializeField] private EndingData currentEnding;
        
        // 이벤트
        public event System.Action<EndingData> OnEndingShown;
        
        // 프로퍼티
        public EndingData CurrentEnding => currentEnding;
        
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
            LoadAllEndings();
        }
        
        /// <summary>
        /// 모든 엔딩 로드
        /// </summary>
        private void LoadAllEndings()
        {
            EndingData[] endings = Resources.LoadAll<EndingData>("Endings");
            allEndings = endings.ToList();
            
            Debug.Log($"Loaded {allEndings.Count} endings");
        }
        
        /// <summary>
        /// 엔딩 표시
        /// </summary>
        public void ShowEnding(EndingType endingType)
        {
            currentEnding = allEndings.FirstOrDefault(e => e.endingType == endingType);
            
            if (currentEnding != null)
            {
                OnEndingShown?.Invoke(currentEnding);
                Debug.Log($"Showing ending: {currentEnding.endingTitle}");
            }
            else
            {
                Debug.LogError($"Ending not found: {endingType}");
            }
        }
        
        /// <summary>
        /// 엔딩 통계 생성
        /// </summary>
        public EndingStatistics GenerateStatistics()
        {
            var stats = new EndingStatistics();
            
            if (ParameterManager.Instance != null)
            {
                stats.finalLoyalty = ParameterManager.Instance.CorporateLoyalty;
                stats.finalConscience = ParameterManager.Instance.PersonalConscience;
                stats.finalInequalityIndex = ParameterManager.Instance.SocialInequalityIndex;
            }
            
            if (EconomyManager.Instance != null)
            {
                stats.totalEarnings = EconomyManager.Instance.TotalEarnings;
                stats.finalBalance = EconomyManager.Instance.CurrentBalance;
            }
            
            stats.daysWorked = GameManager.Instance.CurrentDay;
            
            return stats;
        }
    }
    
    /// <summary>
    /// 엔딩 통계
    /// </summary>
    [System.Serializable]
    public class EndingStatistics
    {
        public int finalLoyalty;
        public int finalConscience;
        public int finalInequalityIndex;
        public int totalEarnings;
        public int finalBalance;
        public int daysWorked;
        
        public override string ToString()
        {
            return $"근무일수: {daysWorked}일\n" +
                   $"충성도: {finalLoyalty}\n" +
                   $"양심: {finalConscience}\n" +
                   $"불평등 지수: {finalInequalityIndex}\n" +
                   $"총 수입: {totalEarnings:N0}원\n" +
                   $"최종 잔액: {finalBalance:N0}원";
        }
    }
}