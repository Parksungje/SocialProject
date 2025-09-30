using System;
using KDH.Code.Core;
using UnityEngine;

namespace KDH.Code.Data
{
    /// <summary>
    /// 이메일 데이터 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New Email", menuName = "Equality Corp/Email Data")]
    public class EmailData : ScriptableObject
    {
        [Header("기본 정보")]
        public string emailId;
        public EmailType emailType;
        public int applicableDay;          // 해당 일차 (-1이면 모든 일차)
        
        [Header("발신자")]
        public string senderName;
        public string senderTitle;
        
        [Header("내용")]
        public string subject;
        [TextArea(5, 10)]
        public string body;
        
        [Header("첨부 파일")]
        public bool hasAttachment;
        public string attachmentName;
        
        [Header("중요도")]
        public bool isMandatory;           // 필수 읽기 여부
        
        /// <summary>
        /// 특정 일차에 이 이메일이 활성화되는지 확인
        /// </summary>
        public bool IsActiveOnDay(int day)
        {
            return applicableDay == -1 || applicableDay == day;
        }
    }
    
    /// <summary>
    /// 런타임 이메일 인스턴스
    /// </summary>
    [Serializable]
    public class EmailInstance
    {
        public EmailData emailData;
        public bool isRead;
        public DateTime receivedTime;
        
        public EmailInstance(EmailData data)
        {
            emailData = data;
            isRead = false;
            receivedTime = DateTime.Now;
        }
    }
}