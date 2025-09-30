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
            
            // 추가 서류 표시
            DisplayAdditionalDocuments(applicant);
        }
        
        /// <summary>
        /// 기본 정보 표시
        /// </summary>
        private void DisplayBasicInfo(ApplicantData applicant)
        {
            nameText.text = applicant.fullName;
            ageText.text = $"{applicant.age}세";
            genderText.text = applicant.gender;
            addressText.text = applicant.GetFullAddress();
            phoneText.text = applicant.phoneNumber;
            
            if (applicant.photoSprite != null)
            {
                photoImage.sprite = applicant.photoSprite;
            }
        }
        
        /// <summary>
        /// 학력 정보 표시
        /// </summary>
        private void DisplayEducationInfo(ApplicantData applicant)
        {
            universityText.text = applicant.universityName;
            majorText.text = applicant.major;
            graduationYearText.text = $"{applicant.graduationYear}년 졸업";
            gpaText.text = $"{applicant.gpa:F2} / 4.5";
        }
        
        /// <summary>
        /// 경력 정보 표시
        /// </summary>
        private void DisplayCareerInfo(ApplicantData applicant)
        {
            if (applicant.experienceMonths > 0)
            {
                companyText.text = applicant.previousCompany;
                positionText.text = applicant.position;
                experienceText.text = applicant.GetExperienceDuration();
                employmentPeriodText.text = $"{applicant.employmentStartDate:yyyy.MM} ~ {applicant.employmentEndDate:yyyy.MM}";
            }
            else
            {
                companyText.text = "없음";
                positionText.text = "신입";
                experienceText.text = "경력 없음";
                employmentPeriodText.text = "-";
            }
        }
        
        /// <summary>
        /// 추가 서류 표시
        /// </summary>
        private void DisplayAdditionalDocuments(ApplicantData applicant)
        {
            // 신원조회서
            if (applicant.hasBackgroundCheck)
            {
                backgroundCheckPanel.SetActive(true);
                criminalRecordText.text = applicant.hasCriminalRecord 
                    ? "범죄 기록 있음" 
                    : "범죄 기록 없음";
                criminalRecordText.color = applicant.hasCriminalRecord 
                    ? Color.red 
                    : Color.green;
            }
            else
            {
                backgroundCheckPanel.SetActive(false);
            }
            
            // 추천서
            if (applicant.hasRecommendationLetter)
            {
                recommendationPanel.SetActive(true);
                recommendationText.text = applicant.recommendationText;
            }
            else
            {
                recommendationPanel.SetActive(false);
            }
            
            // 자격증
            if (applicant.hasCertification)
            {
                certificationPanel.SetActive(true);
                certificationText.text = applicant.certificationName;
            }
            else
            {
                certificationPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 하이라이트 모드 토글
        /// </summary>
        public void ToggleHighlightMode()
        {
            isHighlightMode = highlightToggle.isOn;
            Debug.Log($"Highlight mode: {isHighlightMode}");
        }
        
        /// <summary>
        /// 텍스트 요소 클릭 처리 (하이라이트용)
        /// </summary>
        public void OnTextClicked(TextMeshProUGUI textElement)
        {
            if (!isHighlightMode)
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