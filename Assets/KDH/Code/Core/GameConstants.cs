namespace KDH.Code.Core
{
    /// <summary>
    /// 게임 전역 상수 정의
    /// </summary>
    public static class GameConstants
    {
        // 게임 설정
        public const int TOTAL_DAYS = 16;
        public const int TARGET_FPS = 60;
        
        // 경제 시스템
        public const int BASE_DAILY_SALARY = 150000;
        public const int CORRECT_BONUS = 5000;
        public const int ERROR_PENALTY = 10000;
        public const int MONTHLY_PERFORMANCE_BONUS = 500000;
        public const float PERFORMANCE_BONUS_THRESHOLD = 0.95f;
        
        // 지출
        public const int MONTHLY_RENT = 1200000;
        public const int MONTHLY_LIVING = 1000000;
        public const int MONTHLY_EDUCATION = 400000;
        public const int TOTAL_MONTHLY_EXPENSE = MONTHLY_RENT + MONTHLY_LIVING + MONTHLY_EDUCATION;
        
        // 파라미터 범위
        public const int PARAM_MIN = 0;
        public const int PARAM_MAX = 100;
        public const int INEQUALITY_INDEX_MAX = 200;
        
        // 일차별 서류 목표량
        public static readonly int[] DAILY_DOCUMENT_TARGETS = { 
            0, 8, 8, 8,      // 1-3일차
            10, 10, 10,      // 4-6일차
            12, 12, 12,      // 7-9일차
            15, 15, 15,      // 10-12일차
            18, 18, 18,      // 13-15일차
            20               // 16일차
        };
        
        // 일차별 오류율
        public static readonly float[] DAILY_ERROR_RATES = {
            0f, 0.2f, 0.2f, 0.2f,    // 1-3일차
            0.3f, 0.3f, 0.3f,        // 4-6일차
            0.35f, 0.35f, 0.35f,     // 7-9일차
            0.4f, 0.4f, 0.4f,        // 10-12일차
            0.45f, 0.45f, 0.45f,     // 13-15일차
            0.5f                      // 16일차
        };
        
        // 가족 대화 빈도 (3일마다)
        public const int FAMILY_TALK_INTERVAL = 3;
        
        // UI 타이밍
        public const float DOCUMENT_SPAWN_INTERVAL = 3f;
        public const float FEEDBACK_DISPLAY_TIME = 1.5f;
        public const float FAMILY_DIALOGUE_DISPLAY_TIME = 3f;
        
        // 엔딩 조건 임계값
        public const int ENDING_HIGH_THRESHOLD = 80;
        public const int ENDING_MID_HIGH_THRESHOLD = 70;
        public const int ENDING_MID_THRESHOLD = 50;
        public const int ENDING_LOW_THRESHOLD = 40;
        public const int ENDING_VERY_LOW_THRESHOLD = 30;
    }
}