namespace KDH.Code.Core
{
    /// <summary>
    /// 지역 열거형
    /// </summary>
    public enum Region
    {
        Eridanus,      // 에리다누스 수도권
        Corvus,        // 코르부스 공업지대
        Libra,         // 리브라 평원
        Edea           // 에데아 항구도시
    }
    
    /// <summary>
    /// 대학 등급 열거형
    /// </summary>
    public enum UniversityTier
    {
        VNU,           // 베리디아 국립대학교 (1등급)
        CIT,           // 센타우루스 공과대학교 (2등급)
        Mid,           // 중위권 대학 (3등급)
        CCC            // 코르부스 시립대학 (4등급)
    }
    
    /// <summary>
    /// 서류 오류 타입
    /// </summary>
    public enum ErrorType
    {
        None,                  // 오류 없음
        InfoMismatch,         // 정보 불일치
        MinorTypo,            // 미세한 오타
        DateError,            // 날짜 오류
        ForgeryDetected,      // 위조 서류
        RuleViolation         // 규칙 위반
    }
    
    /// <summary>
    /// 규칙 타입
    /// </summary>
    public enum RuleType
    {
        Basic,            // 기본 규칙 (객관적)
        Preference,       // 편견 규칙 (차별적)
        Priority,         // 우대 규칙 (무조건 합격)
        Prohibition       // 금지 규칙 (무조건 탈락)
    }
    
    /// <summary>
    /// 심사 결정
    /// </summary>
    public enum Decision
    {
        None,
        Approve,          // 승인
        Reject            // 거부
    }
    
    /// <summary>
    /// 게임 엔딩 타입
    /// </summary>
    public enum EndingType
    {
        CorporateDog,     // 충견 엔딩
        Downfall,         // 파멸 엔딩
        Martyr,           // 순교자 엔딩
        Revolutionary,    // 혁명가 엔딩
        Balance           // 균형 엔딩
    }
    
    /// <summary>
    /// 게임 상태
    /// </summary>
    public enum GameState
    {
        MainMenu,
        DayStart,
        EmailBriefing,
        DocumentReview,
        DailyReport,
        InnerThoughts,
        FamilyTalk,
        DayEnd,
        FinalChoice,
        Ending
    }
    
    /// <summary>
    /// 가족 관계 상태
    /// </summary>
    public enum FamilyMood
    {
        HighLoyalty,      // 높은 충성도 상황
        HighConscience,   // 높은 양심 상황
        Balanced,         // 균형 상태
        Crisis            // 위기 상태
    }
    
    /// <summary>
    /// 메일 타입
    /// </summary>
    public enum EmailType
    {
        WorkGuideline,    // 업무 지침 (필수)
        CompanyNews,      // 사내 뉴스
        ExternalNews,     // 외부 뉴스
        PersonalMail      // 개인 메일
    }
}