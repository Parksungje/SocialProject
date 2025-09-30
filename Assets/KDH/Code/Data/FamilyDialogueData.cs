using System;
using KDH.Code.Core;
using UnityEngine;

namespace KDH.Code.Data
{
    /// <summary>
    /// 가족 대화 데이터 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New Family Dialogue", menuName = "Equality Corp/Family Dialogue")]
    public class FamilyDialogueData : ScriptableObject
    {
        [Header("조건")]
        public FamilyMood requiredMood;
        public int applicableDay;          // 적용 일차 (3, 6, 9, 12, 15)
        
        [Header("속마음")]
        [TextArea(3, 5)]
        public string innerThought;
        
        [Header("배우자 대화")]
        [TextArea(2, 4)]
        public string spouseDialogue;
        
        [Header("딸 대화")]
        [TextArea(2, 4)]
        public string daughterDialogue;
        
        /// <summary>
        /// 특정 일차에 이 대화가 활성화되는지 확인
        /// </summary>
        public bool IsActiveOnDay(int day)
        {
            return applicableDay == day;
        }
    }
    
    /// <summary>
    /// 런타임 가족 대화 인스턴스
    /// </summary>
    [Serializable]
    public class FamilyDialogueInstance
    {
        public string innerThought;
        public string spouseDialogue;
        public string daughterDialogue;
        
        public FamilyDialogueInstance(FamilyDialogueData data)
        {
            innerThought = data.innerThought;
            spouseDialogue = data.spouseDialogue;
            daughterDialogue = data.daughterDialogue;
        }
    }
}