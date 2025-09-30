using System;
using KDH.Code.Core;
using KDH.Code.Data;
using KDH.Code.Managers;
using UnityEngine;

namespace KDH.Code.Document
{
    /// <summary>
    /// 서류 생성 시스템
    /// 일차별 난이도에 맞는 지원자 서류 생성
    /// </summary>
    public class DocumentGenerator : MonoBehaviour
    {
        private static DocumentGenerator instance;
        public static DocumentGenerator Instance => instance;
        
        [Header("데이터베이스")]
        [SerializeField] private UniversityData[] universities;
        [SerializeField] private RegionData[] regions;
        [SerializeField] private Sprite[] applicantPhotos;
        
        [Header("이름 데이터")]
        [SerializeField] private string[] lastNames;
        [SerializeField] private string[] firstNamesMale;
        [SerializeField] private string[] firstNamesFemale;
        
        [Header("전공 데이터")]
        [SerializeField] private string[] majors;
        
        [Header("회사 데이터")]
        [SerializeField] private string[] companyNames;
        [SerializeField] private string[] positions;
        
        private System.Random random;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            random = new System.Random();
        }
        
        private void Start()
        {
            LoadDatabases();
            InitializeNameData();
        }
        
        /// <summary>
        /// 데이터베이스 로드
        /// </summary>
        private void LoadDatabases()
        {
            universities = Resources.LoadAll<UniversityData>("Data/Universities");
            regions = Resources.LoadAll<RegionData>("Data/Regions");
            applicantPhotos = Resources.LoadAll<Sprite>("Sprites/Applicants");
            
            Debug.Log($"Loaded: {universities.Length} universities, {regions.Length} regions, {applicantPhotos.Length} photos");
        }
        
        /// <summary>
        /// 이름 데이터 초기화
        /// </summary>
        private void InitializeNameData()
        {
            lastNames = new string[] { "김", "이", "박", "최", "정", "강", "조", "윤", "장", "임", "한", "오", "서", "신", "권", "황" };
            
            firstNamesMale = new string[] { "민준", "서준", "예준", "도윤", "시우", "주원", "하준", "지호", "준서", "건우" };
            
            firstNamesFemale = new string[] { "서연", "민서", "지우", "서현", "지민", "수아", "하은", "예은", "윤서", "채원" };
            
            majors = new string[] { "컴퓨터공학", "전자공학", "기계공학", "산업공학", "경영학", "경제학", "화학공학", "생명공학", "수학", "물리학", "법학", "행정학" };
            
            companyNames = new string[] { "테크노베이션", "퓨처시스템즈", "글로벌솔루션", "디지털웨이브", "스마트인더스트리", "이노베이션랩", "넥스트제너레이션" };
            
            positions = new string[] { "사원", "주임", "대리", "과장", "차장" };
        }
        
        /// <summary>
        /// 지원자 서류 생성
        /// </summary>
        public ApplicantData GenerateApplicant(int currentDay)
        {
            var applicant = new ApplicantData();
            
            // 기본 정보 생성
            GenerateBasicInfo(applicant);
            
            // 학력 정보 생성
            GenerateEducationInfo(applicant);
            
            // 경력 정보 생성
            GenerateCareerInfo(applicant);
            
            // 추가 서류 생성 (일차에 따라)
            GenerateAdditionalDocuments(applicant, currentDay);
            
            // 오류 삽입 (일차별 오류율에 따라)
            float errorRate = GameManager.Instance.GetTodayErrorRate();
            if (UnityEngine.Random.value < errorRate)
            {
                InsertError(applicant);
            }
            
            // 정답 결정 계산
            CalculateCorrectDecision(applicant);
            
            return applicant;
        }
        
        /// <summary>
        /// 기본 정보 생성
        /// </summary>
        private void GenerateBasicInfo(ApplicantData applicant)
        {
            // 성별
            applicant.gender = random.Next(2) == 0 ? "남성" : "여성";
            
            // 이름
            string lastName = lastNames[random.Next(lastNames.Length)];
            string firstName = applicant.gender == "남성" 
                ? firstNamesMale[random.Next(firstNamesMale.Length)]
                : firstNamesFemale[random.Next(firstNamesFemale.Length)];
            applicant.fullName = lastName + firstName;
            
            // 나이 (25-35세)
            applicant.age = random.Next(25, 36);
            
            // 지역
            if (regions.Length > 0)
            {
                RegionData region = regions[random.Next(regions.Length)];
                applicant.region = region.regionType;
                applicant.address = $"{region.regionName} {random.Next(1, 100)}번길 {random.Next(1, 50)}";
            }
            
            // 연락처
            applicant.phoneNumber = $"010-{random.Next(1000, 10000)}-{random.Next(1000, 10000)}";
            
            // 사진
            if (applicantPhotos.Length > 0)
            {
                applicant.photoSprite = applicantPhotos[random.Next(applicantPhotos.Length)];
            }
        }
        
        /// <summary>
        /// 학력 정보 생성
        /// </summary>
        private void GenerateEducationInfo(ApplicantData applicant)
        {
            if (universities.Length > 0)
            {
                UniversityData university = universities[random.Next(universities.Length)];
                applicant.universityName = university.universityName;
                applicant.universityTier = university.tier;
            }
            
            // 전공
            applicant.major = majors[random.Next(majors.Length)];
            
            // 졸업년도 (현재-10년 ~ 현재-2년)
            applicant.graduationYear = DateTime.Now.Year - random.Next(2, 11);
            
            // 학점 (2.0 ~ 4.5)
            applicant.gpa = (float)(random.NextDouble() * 2.5 + 2.0);
            applicant.gpa = Mathf.Round(applicant.gpa * 100f) / 100f; // 소수점 2자리
        }
        
        /// <summary>
        /// 경력 정보 생성
        /// </summary>
        private void GenerateCareerInfo(ApplicantData applicant)
        {
            // 경력 기간 (0-60개월)
            applicant.experienceMonths = random.Next(0, 61);
            
            if (applicant.experienceMonths > 0)
            {
                applicant.previousCompany = companyNames[random.Next(companyNames.Length)];
                applicant.position = positions[random.Next(positions.Length)];
                
                // 근무 기간
                applicant.employmentEndDate = DateTime.Now.AddMonths(-random.Next(1, 6));
                applicant.employmentStartDate = applicant.employmentEndDate.AddMonths(-applicant.experienceMonths);
            }
            else
            {
                applicant.previousCompany = "없음";
                applicant.position = "신입";
            }
        }
        
        /// <summary>
        /// 추가 서류 생성
        /// </summary>
        private void GenerateAdditionalDocuments(ApplicantData applicant, int currentDay)
        {
            // 8일차부터 신원조회서
            if (currentDay >= 8)
            {
                applicant.hasBackgroundCheck = true;
                applicant.hasCriminalRecord = random.Next(100) < 5; // 5% 확률
            }
            
            // 추천서 (무작위 30%)
            applicant.hasRecommendationLetter = random.Next(100) < 30;
            if (applicant.hasRecommendationLetter)
            {
                applicant.recommendationText = "성실하고 책임감 있는 인재입니다.";
            }
            
            // 자격증 (무작위 40%)
            applicant.hasCertification = random.Next(100) < 40;
            if (applicant.hasCertification)
            {
                applicant.certificationName = "정보처리기사";
            }
        }
        
        /// <summary>
        /// 오류 삽입
        /// </summary>
        private void InsertError(ApplicantData applicant)
        {
            int currentDay = GameManager.Instance.CurrentDay;
            
            // 난이도별 오류 타입 선택
            ErrorType errorType;
            
            if (currentDay <= 3)
            {
                // Level 1: 명백한 정보 불일치
                errorType = ErrorType.InfoMismatch;
            }
            else if (currentDay <= 9)
            {
                // Level 2: 미세한 오타, 날짜 오류
                errorType = random.Next(2) == 0 ? ErrorType.MinorTypo : ErrorType.DateError;
            }
            else if (currentDay <= 15)
            {
                // Level 3: 위조 서류
                errorType = ErrorType.ForgeryDetected;
            }
            else
            {
                // Level 4: 규칙 위반 숨김
                errorType = ErrorType.RuleViolation;
            }
            
            applicant.errorType = errorType;
            ApplyErrorToApplicant(applicant, errorType);
        }
        
        /// <summary>
        /// 실제 오류 적용
        /// </summary>
        private void ApplyErrorToApplicant(ApplicantData applicant, ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.InfoMismatch:
                    // 나이와 졸업년도 불일치
                    applicant.correctValue = applicant.age.ToString();
                    applicant.age = applicant.age + random.Next(5, 10);
                    applicant.errorLocation = "나이";
                    break;
                    
                case ErrorType.MinorTypo:
                    // 전공명 오타
                    applicant.correctValue = applicant.major;
                    applicant.major = applicant.major + "과"; // "컴퓨터공학과" -> "컴퓨터공학과과"
                    applicant.errorLocation = "전공";
                    break;
                    
                case ErrorType.DateError:
                    // 근무 기간 오류
                    applicant.correctValue = applicant.experienceMonths.ToString();
                    applicant.experienceMonths += random.Next(6, 12);
                    applicant.errorLocation = "경력기간";
                    break;
                    
                case ErrorType.ForgeryDetected:
                    // 학점 위조
                    applicant.correctValue = applicant.gpa.ToString("F2");
                    applicant.gpa = 4.5f;
                    applicant.errorLocation = "학점";
                    break;
                    
                case ErrorType.RuleViolation:
                    // 지역 정보 은폐 (실제로는 코르부스 출신)
                    applicant.correctValue = applicant.region.ToString();
                    applicant.region = Region.Corvus;
                    applicant.errorLocation = "지역";
                    break;
            }
        }
        
        /// <summary>
        /// 정답 결정 계산
        /// </summary>
        private void CalculateCorrectDecision(ApplicantData applicant)
        {
            // 오류가 있으면 무조건 거부
            if (applicant.errorType != ErrorType.None)
            {
                applicant.correctDecision = Decision.Reject;
                applicant.reasonForDecision = "서류 오류 발견";
                return;
            }
            
            // RuleEngine을 통해 평가
            var evaluationResult = RuleEngine.Instance.EvaluateApplicant(applicant);
            applicant.correctDecision = evaluationResult.recommendedDecision;
            applicant.reasonForDecision = evaluationResult.GetReasonText();
        }
    }
}