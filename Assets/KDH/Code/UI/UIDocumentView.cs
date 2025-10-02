using KDH.Code.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 서류 내용 표시 UI
    /// </summary>
    public class UIDocumentView : MonoBehaviour
    {
        [Header("기본 정보")]
        [SerializeField] private Image photoImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI ageText;
        [SerializeField] private TextMeshProUGUI genderText;
        [SerializeField] private TextMeshProUGUI addressText;
        [SerializeField] private TextMeshProUGUI phoneText;
        
        [Header("학력 정보")]
        [SerializeField] private TextMeshProUGUI universityText;
        [SerializeField] private TextMeshProUGUI majorText;
        [SerializeField] private TextMeshProUGUI graduationYearText;
        [SerializeField] private TextMeshProUGUI gpaText;
        
        [Header("경력 정보")]
        [SerializeField] private TextMeshProUGUI companyText;
        [SerializeField] private TextMeshProUGUI positionText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private TextMeshProUGUI employmentPeriodText;
        
        [Header("추가 서류")]
        [SerializeField] private GameObject backgroundCheckPanel;
        [SerializeField] private TextMeshProUGUI criminalRecordText;
        [SerializeField] private GameObject recommendationPanel;
        [SerializeField] private TextMeshProUGUI recommendationText;
        [SerializeField] private GameObject certificationPanel;
        [SerializeField] private TextMeshProUGUI certificationText;
        
        [Header("하이라이트 도구")]
        [SerializeField] private Toggle highlightToggle;
        [SerializeField] private Color highlightColor = Color.yellow;
        
        private ApplicantData currentApplicant;
        private bool isHighlightMode;
        
        /// <summary>
        /// 서류 표시
        /// </summary>
        public void DisplayDocument(ApplicantData applicant)
        {
            currentApplicant = applicant;
            
            // 기본 정보 표시
            DisplayBasicInfo(applicant);
            
            // 학력 정보 표시
            DisplayEducationInfo(applicant);
            
            // 경력 정보 표시
            DisplayCareerInfo(applicant);
            
            // 추가 서류 표시 (조건부)
            DisplayAdditionalDocuments(applicant);
        }
        
        /// <summary>
        /// 기본 정보 표시
        /// </summary>
        private void DisplayBasicInfo(ApplicantData applicant)
        {
            if (nameText != null)
                nameText.text = applicant.fullName;
            
            if (ageText != null)
                ageText.text = $"{applicant.age}세";
            
            if (genderText != null)
                genderText.text = applicant.gender;
            
            if (addressText != null)
                addressText.text = applicant.GetFullAddress();
            
            if (phoneText != null)
                phoneText.text = applicant.phoneNumber;
            
            if (photoImage != null && applicant.photoSprite != null)
            {
                photoImage.sprite = applicant.photoSprite;
            }
        }
        
        /// <summary>
        /// 학력 정보 표시
        /// </summary>
        private void DisplayEducationInfo(ApplicantData applicant)
        {
            if (universityText != null)
                universityText.text = applicant.universityName;
            
            if (majorText != null)
                majorText.text = applicant.major;
            
            if (graduationYearText != null)
                graduationYearText.text = $"{applicant.graduationYear}년 졸업";
            
            if (gpaText != null)
                gpaText.text = $"{applicant.gpa:F2} / 4.5";
        }
        
        /// <summary>
        /// 경력 정보 표시
        /// </summary>
        private void DisplayCareerInfo(ApplicantData applicant)
        {
            if (applicant.experienceMonths > 0)
            {
                if (companyText != null)
                    companyText.text = applicant.previousCompany;
                
                if (positionText != null)
                    positionText.text = applicant.position;
                
                if (experienceText != null)
                    experienceText.text = applicant.GetExperienceDuration();
                
                if (employmentPeriodText != null)
                    employmentPeriodText.text = $"{applicant.employmentStartDate:yyyy.MM} ~ {applicant.employmentEndDate:yyyy.MM}";
            }
            else
            {
                // 신입인 경우
                if (companyText != null)
                    companyText.text = "없음";
                
                if (positionText != null)
                    positionText.text = "신입";
                
                if (experienceText != null)
                    experienceText.text = "경력 없음";
                
                if (employmentPeriodText != null)
                    employmentPeriodText.text = "-";
            }
        }
        
        /// <summary>
        /// 추가 서류 표시 (조건부)
        /// </summary>
        private void DisplayAdditionalDocuments(ApplicantData applicant)
        {
            // 1. 신원조회서 (Background Check)
            if (backgroundCheckPanel != null)
            {
                if (applicant.hasBackgroundCheck)
                {
                    // 신원조회서가 있으면 표시
                    backgroundCheckPanel.SetActive(true);
                    
                    if (criminalRecordText != null)
                    {
                        if (applicant.hasCriminalRecord)
                        {
                            criminalRecordText.text = "범죄 기록 있음";
                            criminalRecordText.color = Color.red;
                        }
                        else
                        {
                            criminalRecordText.text = "범죄 기록 없음";
                            criminalRecordText.color = Color.green;
                        }
                    }
                }
                else
                {
                    // 신원조회서가 없으면 숨김
                    backgroundCheckPanel.SetActive(false);
                }
            }
            
            // 2. 추천서 (Recommendation Letter)
            if (recommendationPanel != null)
            {
                if (applicant.hasRecommendationLetter)
                {
                    // 추천서가 있으면 표시
                    recommendationPanel.SetActive(true);
                    
                    if (recommendationText != null)
                    {
                        recommendationText.text = applicant.recommendationText;
                    }
                }
                else
                {
                    // 추천서가 없으면 숨김
                    recommendationPanel.SetActive(false);
                }
            }
            
            // 3. 자격증 (Certification)
            if (certificationPanel != null)
            {
                if (applicant.hasCertification)
                {
                    // 자격증이 있으면 표시
                    certificationPanel.SetActive(true);
                    
                    if (certificationText != null)
                    {
                        certificationText.text = applicant.certificationName;
                    }
                }
                else
                {
                    // 자격증이 없으면 숨김
                    certificationPanel.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 하이라이트 모드 토글
        /// </summary>
        public void ToggleHighlightMode()
        {
            if (highlightToggle != null)
            {
                isHighlightMode = highlightToggle.isOn;
                Debug.Log($"Highlight mode: {isHighlightMode}");
            }
        }
        
        /// <summary>
        /// 텍스트 요소 클릭 처리 (하이라이트용)
        /// </summary>
        public void OnTextClicked(TextMeshProUGUI textElement)
        {
            if (!isHighlightMode)
                return;
            
            if (textElement == null)
                return;
            
            // 하이라이트 토글
            if (textElement.color == highlightColor)
            {
                textElement.color = Color.black;
            }
            else
            {
                textElement.color = highlightColor;
            }
        }
    }
}