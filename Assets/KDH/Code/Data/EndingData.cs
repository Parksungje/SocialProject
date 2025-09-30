using KDH.Code.Core;
using UnityEngine;

namespace KDH.Code.Data
{
    /// <summary>
    /// 엔딩 데이터 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New Ending", menuName = "Equality Corp/Ending Data")]
    public class EndingData : ScriptableObject
    {
        [Header("엔딩 정보")]
        public EndingType endingType;
        public string endingTitle;
        
        [Header("엔딩 스토리")]
        [TextArea(10, 20)]
        public string endingStory;
        
        [Header("결과 요약")]
        [TextArea(3, 5)]
        public string resultSummary;
        
        [Header("메시지")]
        [TextArea(2, 4)]
        public string coreMessage;
        
        [Header("통계 표시")]
        public bool showStatistics = true;
        
        [Header("비주얼")]
        public Sprite endingImage;
        public Color themeColor = Color.white;
    }
}