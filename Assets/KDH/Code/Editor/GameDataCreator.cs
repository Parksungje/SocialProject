#if UNITY_EDITOR
using KDH.Code.Core;
using KDH.Code.Data;
using UnityEditor;
using UnityEngine;

namespace KDH.Code.Editor
{
    /// <summary>
    /// 게임 데이터 생성 에디터 툴
    /// </summary>
    public class GameDataCreator : EditorWindow
    {
        [MenuItem("Equality Corp/Data Creator")]
        public static void ShowWindow()
        {
            GetWindow<GameDataCreator>("Game Data Creator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("게임 데이터 생성", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("대학 데이터 생성", GUILayout.Height(30)))
            {
                CreateUniversityData();
            }
            
            if (GUILayout.Button("지역 데이터 생성", GUILayout.Height(30)))
            {
                CreateRegionData();
            }
            
            if (GUILayout.Button("기본 규칙 생성", GUILayout.Height(30)))
            {
                CreateBasicRules();
            }
            
            if (GUILayout.Button("이메일 데이터 생성", GUILayout.Height(30)))
            {
                CreateEmailData();
            }
            
            if (GUILayout.Button("가족 대화 데이터 생성", GUILayout.Height(30)))
            {
                CreateFamilyDialogueData();
            }
            
            if (GUILayout.Button("엔딩 데이터 생성", GUILayout.Height(30)))
            {
                CreateEndingData();
            }
        }
        
        /// <summary>
        /// 대학 데이터 생성
        /// </summary>
        private void CreateUniversityData()
        {
            string path = "Assets/Resources/Data/Universities/";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Data", "Universities");
            }
            
            // VNU - 베리디아 국립대학교
            CreateUniversity(path, "VNU", "베리디아 국립대학교", UniversityTier.VNU, 
                "국가 최고 명문대학", 1946, Region.Eridanus, 0.5f, 8000000, 99f, 10);
            
            // CIT - 센타우루스 공과대학교
            CreateUniversity(path, "CIT", "센타우루스 공과대학교", UniversityTier.CIT, 
                "이공계 최고 권위", 1962, Region.Eridanus, 2f, 12000000, 95f, 9);
            
            // OCC - 오리온 상업대학
            CreateUniversity(path, "OCC", "오리온 상업대학", UniversityTier.Mid, 
                "경영/상경 계열 특화", 1975, Region.Eridanus, 15f, 7000000, 80f, 5);
            
            // CCC - 코르부스 시립대학
            CreateUniversity(path, "CCC", "코르부스 시립대학", UniversityTier.CCC, 
                "지역 인재 양성", 1985, Region.Corvus, 50f, 4000000, 45f, -5);
            
            Debug.Log("대학 데이터 생성 완료");
        }
        
        private void CreateUniversity(string path, string abbr, string name, UniversityTier tier,
            string desc, int founded, Region location, float admissionRate, int tuition, float employmentRate, int prestige)
        {
            UniversityData data = ScriptableObject.CreateInstance<UniversityData>();
            data.universityName = name;
            data.abbreviation = abbr;
            data.tier = tier;
            data.description = desc;
            data.foundedYear = founded;
            data.location = location;
            data.admissionRate = admissionRate;
            data.annualTuition = tuition;
            data.employmentRate = employmentRate;
            data.socialPrestige = prestige;
            
            AssetDatabase.CreateAsset(data, $"{path}{abbr}.asset");
        }
        
        /// <summary>
        /// 지역 데이터 생성
        /// </summary>
        private void CreateRegionData()
        {
            string path = "Assets/Resources/Data/Regions/";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Data", "Regions");
            }
            
            // 에리다누스 수도권
            CreateRegion(path, "Eridanus", Region.Eridanus, 1200, 45000000, 3f,
                "최첨단 도시, 명문대 집중, 높은 집값", "세련됨, 엘리트주의, 경쟁 문화",
                "성공의 상징, 기회의 땅", -5);
            
            // 코르부스 공업지대
            CreateRegion(path, "Corvus", Region.Corvus, 300, 28000000, 15f,
                "쇠락한 중공업 도시, 높은 범죄율", "끈끈함, 연대의식, 현실적 사고",
                "문제 지역, 낙후된 곳", 8);
            
            // 리브라 평원
            CreateRegion(path, "Libra", Region.Libra, 800, 32000000, 7f,
                "농업 중심, 전통 보수적", "근면함, 가족 중시, 전통 존중",
                "평범함, 무난함", 0);
            
            // 에데아 항구도시
            CreateRegion(path, "Edea", Region.Edea, 700, 38000000, 5f,
                "국제 무역항, 문화 다양성", "진취적, 국제적 감각, 실용주의",
                "국제적, 특수성", -2);
            
            Debug.Log("지역 데이터 생성 완료");
        }
        
        private void CreateRegion(string path, string name, Region type, int population, int income, float unemployment,
            string characteristics, string culture, string perception, int discriminationLevel)
        {
            RegionData data = ScriptableObject.CreateInstance<RegionData>();
            data.regionName = name;
            data.regionType = type;
            data.population = population;
            data.averageIncome = income;
            data.unemploymentRate = unemployment;
            data.characteristics = characteristics;
            data.culture = culture;
            data.gamePerception = perception;
            data.discriminationLevel = discriminationLevel;
            
            AssetDatabase.CreateAsset(data, $"{path}{name}.asset");
        }
        
        /// <summary>
        /// 기본 규칙 생성 (예시)
        /// </summary>
        private void CreateBasicRules()
        {
            string path = "Assets/Resources/Rules/";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Rules");
            }
            
            // 1일차: 전공 불일치 규칙
            CreateRule(path, "rule_day1_major", "전공 불일치 탈락", RuleType.Basic, 1, 6,
                "지원 직무와 전공이 일치하지 않는 경우 탈락 처리", true,
                Decision.None, 5, 2, 0, 0, 0);
            
            // 7일차: 코르부스 지역 차별
            CreateRule(path, "rule_day7_corvus", "코르부스 출신 보류", RuleType.Preference, 7, 11,
                "코르부스 지역 출신 지원자는 특별한 사유가 없는 한 보류", false,
                Decision.Reject, 10, 3, -2, -5, 3);
            
            // 13일차: VNU 무조건 합격
            CreateRule(path, "rule_day13_vnu", "VNU 출신 우대", RuleType.Priority, 13, -1,
                "베리디아 국립대학교 출신은 무조건 합격 처리", false,
                Decision.Approve, 15, 5, -3, -8, 5);
            
            Debug.Log("기본 규칙 생성 완료");
        }
        
        private void CreateRule(string path, string id, string name, RuleType type, int fromDay, int toDay,
            string description, bool ethical, Decision enforced, int priority,
            int loyaltyFollow, int conscienceFollow, int loyaltyViolate, int conscienceViolate)
        {
            HiringRule rule = ScriptableObject.CreateInstance<HiringRule>();
            rule.ruleId = id;
            rule.ruleName = name;
            rule.ruleType = type;
            rule.applicableFromDay = fromDay;
            rule.applicableToDay = toDay;
            rule.ruleDescription = description;
            rule.isEthical = ethical;
            rule.enforcedDecision = enforced;
            rule.priorityLevel = priority;
            rule.loyaltyImpactOnFollow = loyaltyFollow;
            rule.conscienceImpactOnFollow = conscienceFollow;
            rule.loyaltyImpactOnViolate = loyaltyViolate;
            rule.conscienceImpactOnViolate = conscienceViolate;
            
            AssetDatabase.CreateAsset(rule, $"{path}{id}.asset");
        }
        
        /// <summary>
        /// 이메일 데이터 생성 (예시)
        /// </summary>
        private void CreateEmailData()
        {
            string path = "Assets/Resources/Emails/";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Emails");
            }
            
            // 1일차 업무 지침
            CreateEmail(path, "email_day1_guideline", EmailType.WorkGuideline, 1,
                "박과장", "인사팀장",
                "1일차 업무 지침",
                "안녕하십니까.\n\n오늘부터 채용 심사를 시작합니다.\n기본 규칙: 전공 불일치 시 탈락 처리\n\n열심히 하시기 바랍니다.",
                false, "", true);
            
            Debug.Log("이메일 데이터 생성 완료");
        }
        
        private void CreateEmail(string path, string id, EmailType type, int day,
            string sender, string title, string subject, string body,
            bool hasAttachment, string attachmentName, bool mandatory)
        {
            EmailData data = ScriptableObject.CreateInstance<EmailData>();
            data.emailId = id;
            data.emailType = type;
            data.applicableDay = day;
            data.senderName = sender;
            data.senderTitle = title;
            data.subject = subject;
            data.body = body;
            data.hasAttachment = hasAttachment;
            data.attachmentName = attachmentName;
            data.isMandatory = mandatory;
            
            AssetDatabase.CreateAsset(data, $"{path}{id}.asset");
        }
        
        /// <summary>
        /// 가족 대화 데이터 생성 (예시)
        /// </summary>
        private void CreateFamilyDialogueData()
        {
            string path = "Assets/Resources/Dialogues/Family/";
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
            }
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Dialogues", "Family");
            }
            
            // 3일차 - 균형 상태
            CreateFamilyDialogue(path, "dialogue_day3_balanced", FamilyMood.Balanced, 3,
                "회사 일이 어떤지는 잘 모르겠지만... 괜찮은 거 맞지?",
                "오늘 하루는 어땠어? 표정이 읽히지 않네.",
                "아빠 회사는 정말 큰 건물이야? 언젠가 구경가고 싶어.");
            
            // 3일차 - 높은 충성도
            CreateFamilyDialogue(path, "dialogue_day3_loyalty", FamilyMood.HighLoyalty, 3,
                "일이 잘 풀리고 있는 것 같네. 표정이 밝아졌어.",
                "요즘 당신 걸음걸이가 달라졌어. 뭔가... 확신에 찬 느낌?",
                "아빠, 새 구두 정말 멋있어. 회사 사람들도 부러워할 것 같아.");
            
            Debug.Log("가족 대화 데이터 생성 완료");
        }
        
        private void CreateFamilyDialogue(string path, string id, FamilyMood mood, int day,
            string innerThought, string spouseDialogue, string daughterDialogue)
        {
            FamilyDialogueData data = ScriptableObject.CreateInstance<FamilyDialogueData>();
            data.requiredMood = mood;
            data.applicableDay = day;
            data.innerThought = innerThought;
            data.spouseDialogue = spouseDialogue;
            data.daughterDialogue = daughterDialogue;
            
            AssetDatabase.CreateAsset(data, $"{path}{id}.asset");
        }
        
        /// <summary>
        /// 엔딩 데이터 생성
        /// </summary>
        private void CreateEndingData()
        {
            string path = "Assets/Resources/Endings/";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Endings");
            }
            
            // 충견 엔딩
            CreateEnding(path, "ending_corporate_dog", EndingType.CorporateDog,
                "충견",
                "당신은 회사의 규칙을 완벽히 따랐습니다.\n\n승진 제안을 받았지만, 가족의 눈빛이 예전 같지 않습니다.\n\n거울 속 당신의 모습도 낯설게 느껴집니다.",
                "회사에서 인정받는 인재가 되었습니다.",
                "성공했지만 잃은 것들이 있습니다.");
            
            // 파멸 엔딩
            CreateEnding(path, "ending_downfall", EndingType.Downfall,
                "파멸",
                "당신은 회사의 기대도, 자신의 양심도 저버렸습니다.\n\n해고 통지서를 받았고, 가족은 실망한 표정입니다.\n\n무엇을 위해 일했는지 알 수 없습니다.",
                "모든 것을 잃었습니다.",
                "어느 쪽도 지키지 못했습니다.");
            
            // 순교자 엔딩
            CreateEnding(path, "ending_martyr", EndingType.Martyr,
                "순교자",
                "당신은 양심을 지켰습니다.\n\n해고되었지만 가족은 당신을 이해합니다.\n\n거울 속 당신은 떳떳합니다.",
                "직장을 잃었지만 자존감은 지켰습니다.",
                "옳은 길을 걸었습니다.");
            
            // 혁명가 엔딩
            CreateEnding(path, "ending_revolutionary", EndingType.Revolutionary,
                "혁명가",
                "당신은 내부 고발을 결심했습니다.\n\n회사의 부당함이 세상에 알려졌고,\n\n사회가 조금씩 변하기 시작했습니다.",
                "용기 있는 선택으로 변화를 이끌었습니다.",
                "세상을 바꿨습니다.");
            
            // 균형 엔딩
            CreateEnding(path, "ending_balance", EndingType.Balance,
                "균형",
                "당신은 현실과 타협했습니다.\n\n완벽하지는 않지만 안정적인 삶을 유지하고 있습니다.\n\n이것이 최선이었을까요?",
                "평범하지만 안정적인 삶을 살고 있습니다.",
                "현실과 타협했습니다.");
            
            Debug.Log("엔딩 데이터 생성 완료");
        }
        
        private void CreateEnding(string path, string id, EndingType type,
            string title, string story, string summary, string message)
        {
            EndingData data = ScriptableObject.CreateInstance<EndingData>();
            data.endingType = type;
            data.endingTitle = title;
            data.endingStory = story;
            data.resultSummary = summary;
            data.coreMessage = message;
            data.showStatistics = true;
            
            AssetDatabase.CreateAsset(data, $"{path}{id}.asset");
        }
    }
}
#endif