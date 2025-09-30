using UnityEngine;

namespace KDH.Code.Data
{
    /// <summary>
    /// 대학 정보 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New University", menuName = "Equality Corp/University Data")]
    public class UniversityData : ScriptableObject
    {
        [Header("기본 정보")]
        public string universityName;
        public string abbreviation;
        public Core.UniversityTier tier;
        
        [Header("상세 정보")]
        [TextArea(2, 4)]
        public string description;
        public int foundedYear;
        public Core.Region location;
        
        [Header("통계")]
        [Range(0f, 100f)]
        public float admissionRate;      // 입학률 (상위 %)
        public int annualTuition;        // 연 등록금
        [Range(0f, 100f)]
        public float employmentRate;     // 취업률
        
        [Header("사회적 인식")]
        [Range(-10, 10)]
        public int socialPrestige;       // 사회적 위신 (-10 ~ 10)
        public string publicPerception;  // 대중 인식
    }
}