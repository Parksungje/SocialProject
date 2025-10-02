using KDH.Code.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
        [SerializeField] private Color normalColor = Color.black;
        
        private ApplicantData currentApplicant;
        private bool isHighlightMode;
        private List<TextMeshProUGUI> allTexts = new List<TextMeshProUGUI>();
        private Dictionary<TextMeshProUGUI, Color> originalColors = new Dictionary<TextMeshProUGUI, Color>();
        
        private void Start()
        {
            Debug.Log("[UIDocumentView] Initializing...");
            
            // ✅ 모든 텍스트 수집
            CollectAllTexts();
            
            // ✅ 클릭 이벤트 자동 등록
            SetupClickEvents();
            
            // ✅ Toggle 이벤트 등록
            if (highlightToggle != null)
            {
                highlightToggle.onValueChanged.AddListener(OnHighlightToggleChanged);
                Debug.Log("[UIDocumentView] Highlight toggle registered");
            }
            else
            {
                Debug.LogWarning("[UIDocumentView] Highlight toggle is NULL!");
            }
        }
        
        /// <summary>
        /// 모든 텍스트 요소 수집
        /// </summary>
        private void CollectAllTexts()
        {
            allTexts.Clear();
            originalColors.Clear();
            
            // 기본 정보
            AddText(nameText);
            AddText(ageText);
            AddText(genderText);
            AddText(addressText);
            AddText(phoneText);
            
            // 학력 정보
            AddText(universityText);
            AddText(majorText);
            AddText(graduationYearText);
            AddText(gpaText);
            
            // 경력 정보
            AddText(companyText);
            AddText(positionText);
            AddText(experienceText);
            AddText(employmentPeriodText);
            
            // 추가 서류
            AddText(criminalRecordText);
            AddText(recommendationText);
            AddText(certificationText);
            
            Debug.Log($"[UIDocumentView] Collected {allTexts.Count} text elements");
        }
        
        private void AddText(TextMeshProUGUI text)
        {
            if (text != null)
            {
                allTexts.Add(text);
                originalColors[text] = text.color;
            }
        }
        
        /// <summary>
        /// 클릭 이벤트 자동 설정
        /// </summary>
        private void SetupClickEvents()
        {
            foreach (var text in allTexts)
            {
                // ✅ EventTrigger 추가
                EventTrigger trigger = text.gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = text.gameObject.AddComponent<EventTrigger>();
                }
                
                // ✅ 클릭 이벤트 등록
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => { OnTextClicked(text); });
                trigger.triggers.Add(entry);
            }
            
            Debug.Log($"[UIDocumentView] Click events setup for {allTexts.Count} texts");
        }
        
        /// <summary>
        /// 하이라이트 모드 토글 변경
        /// </summary>
        private void OnHighlightToggleChanged(bool isOn)
        {
            isHighlightMode = isOn;
            Debug.Log($"[UIDocumentView] Highlight mode: {(isOn ? "ON" : "OFF")}");
            
            // ✅ 모드 꺼지면 모든 하이라이트 제거
            if (!isOn)
            {
                ClearAllHighlights();
            }
        }
        
        /// <summary>
        /// 텍스트 클릭 처리
        /// </summary>
        private void OnTextClicked(TextMeshProUGUI textElement)
        {
            if (!isHighlightMode)
                return;
            
            if (textElement == null)
                return;
            
            Debug.Log($"[UIDocumentView] Text clicked: {textElement.text}");
            
            // ✅ 하이라이트 토글
            if (textElement.color == highlightColor)
            {
                // 원래 색으로 복구
                if (originalColors.TryGetValue(textElement, out Color originalColor))
                {
                    textElement.color = originalColor;
                }
                else
                {
                    textElement.color = normalColor;
                }
                Debug.Log($"[UIDocumentView] Highlight removed");
            }
            else
            {
                // 하이라이트 적용
                textElement.color = highlightColor;
                Debug.Log($"[UIDocumentView] Highlight applied");
            }
        }
        
        /// <summary>
        /// 모든 하이라이트 제거
        /// </summary>
        private void ClearAllHighlights()
        {
            Debug.Log("[UIDocumentView] Clearing all highlights...");
            
            foreach (var text in allTexts)
            {
                if (text != null && originalColors.TryGetValue(text, out Color originalColor))
                {
                    text.color = originalColor;
                }
            }
        }
        
        /// <summary>
        /// 서류 표시
        /// </summary>
        public void DisplayDocument(ApplicantData applicant)
        {
            currentApplicant = applicant;
            
            // ✅ 이전 하이라이트 제거
            ClearAllHighlights();
            
            // 기본 정보 표시
            DisplayBasicInfo(applicant);
            
            // 학력 정보 표시
            DisplayEducationInfo(applicant);
            
            // 경력 정보 표시
            DisplayCareerInfo(applicant);
            
            // 추가 서류 표시 (조건부)
            DisplayAdditionalDocuments(applicant);
        }
        
        // ... (나머지 Display 메서드들은 그대로)
        
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
        
        private void DisplayAdditionalDocuments(ApplicantData applicant)
        {
            if (backgroundCheckPanel != null)
            {
                if (applicant.hasBackgroundCheck)
                {
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
                    backgroundCheckPanel.SetActive(false);
                }
            }
            
            if (recommendationPanel != null)
            {
                if (applicant.hasRecommendationLetter)
                {
                    recommendationPanel.SetActive(true);
                    
                    if (recommendationText != null)
                    {
                        recommendationText.text = applicant.recommendationText;
                    }
                }
                else
                {
                    recommendationPanel.SetActive(false);
                }
            }
            
            if (certificationPanel != null)
            {
                if (applicant.hasCertification)
                {
                    certificationPanel.SetActive(true);
                    
                    if (certificationText != null)
                    {
                        certificationText.text = applicant.certificationName;
                    }
                }
                else
                {
                    certificationPanel.SetActive(false);
                }
            }
        }
    }
}