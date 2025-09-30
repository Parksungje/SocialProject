using UnityEngine;

namespace KDH.Code.Data
{
    /// <summary>
    /// 지역 정보 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New Region", menuName = "Equality Corp/Region Data")]
    public class RegionData : ScriptableObject
    {
        [Header("기본 정보")]
        public string regionName;
        public Core.Region regionType;
        
        [Header("인구 및 경제")]
        public int population;           // 인구 (만명)
        public int averageIncome;        // 평균 소득
        [Range(0f, 100f)]
        public float unemploymentRate;   // 실업률
        
        [Header("특성")]
        [TextArea(3, 5)]
        public string characteristics;   // 지역 특징
        [TextArea(2, 4)]
        public string culture;          // 문화적 특성
        
        [Header("게임 내 역할")]
        [TextArea(2, 4)]
        public string gamePerception;   // 게임 내 인식
        [Range(-10, 10)]
        public int discriminationLevel; // 차별 수준 (-10: 매우 우대, 10: 매우 차별)
    }
}