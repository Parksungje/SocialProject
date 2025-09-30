using System;
using UnityEngine;

namespace KDH.Code.Data
{
    /// <summary>
    /// 지원자 정보 클래스 (런타임 데이터)
    /// </summary>
    [Serializable]
    public class ApplicantData
    {
        [Header("기본 정보")]
        public string fullName;
        public int age;
        public string gender;
        public string address;
        public Core.Region region;
        public string phoneNumber;
        public Sprite photoSprite;
        
        [Header("학력 정보")]
        public string universityName;
        public Core.UniversityTier universityTier;
        public string major;
        public int graduationYear;
        [Range(0f, 4.5f)]
        public float gpa;
        
        [Header("경력 정보")]
        public string previousCompany;
        public string position;
        public int experienceMonths;
        public DateTime employmentStartDate;
        public DateTime employmentEndDate;
        
        [Header("추가 서류")]
        public bool hasBackgroundCheck;
        public bool hasCriminalRecord;
        public bool hasRecommendationLetter;
        public string recommendationText;
        public bool hasCertification;
        public string certificationName;
        
        [Header("오류 정보")]
        public Core.ErrorType errorType;
        public string errorLocation;      // 오류가 있는 필드명
        public string correctValue;       // 정답 값
        
        [Header("정답 정보")]
        public Core.Decision correctDecision;
        public string reasonForDecision;
        
        /// <summary>
        /// 지원자의 완전한 주소 반환
        /// </summary>
        public string GetFullAddress()
        {
            return $"{region.ToString()} 지역, {address}";
        }
        
        /// <summary>
        /// 경력 기간을 년.월 형식으로 반환
        /// </summary>
        public string GetExperienceDuration()
        {
            int years = experienceMonths / 12;
            int months = experienceMonths % 12;
            
            if (years > 0)
                return $"{years}년 {months}개월";
            return $"{months}개월";
        }
        
        /// <summary>
        /// 학점을 백분율로 변환
        /// </summary>
        public float GetGPAPercentage()
        {
            return (gpa / 4.5f) * 100f;
        }
    }
}