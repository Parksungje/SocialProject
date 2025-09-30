using System;
using System.Collections.Generic;
using System.Linq;
using KDH.Code.Core;
using KDH.Code.Data;
using UnityEngine;

namespace KDH.Code.Managers
{
    /// <summary>
    /// 가족 시스템 관리 (속마음, 대화, 가계부)
    /// </summary>
    public class FamilyManager : MonoBehaviour
    {
        private static FamilyManager instance;
        public static FamilyManager Instance => instance;
        
        [Header("대화 데이터베이스")]
        [SerializeField] private List<FamilyDialogueData> allDialogues = new List<FamilyDialogueData>();
        
        [Header("현재 대화")]
        [SerializeField] private FamilyDialogueInstance currentDialogue;
        
        // 이벤트
        public event Action<string> OnInnerThoughtShown;
        public event Action<FamilyDialogueInstance> OnFamilyDialogueStarted;
        public event Action OnFamilyDialogueCompleted;
        
        // 프로퍼티
        public FamilyDialogueInstance CurrentDialogue => currentDialogue;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            LoadAllDialogues();
            
            // GameManager 이벤트 구독
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
        }
        
        /// <summary>
        /// 모든 대화 로드
        /// </summary>
        private void LoadAllDialogues()
        {
            FamilyDialogueData[] dialogues = Resources.LoadAll<FamilyDialogueData>("Dialogues/Family");
            allDialogues = dialogues.ToList();
            
            Debug.Log($"Loaded {allDialogues.Count} family dialogue templates");
        }
        
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.InnerThoughts)
            {
                ShowInnerThoughts();
            }
            else if (newState == GameState.FamilyTalk)
            {
                StartFamilyDialogue();
            }
        }
        
        /// <summary>
        /// 속마음 표시
        /// </summary>
        private void ShowInnerThoughts()
        {
            int currentDay = GameManager.Instance.CurrentDay;
            FamilyMood mood = ParameterManager.Instance.GetCurrentFamilyMood();
            
            // 해당 일차와 분위기에 맞는 대화 찾기
            var dialogue = FindApplicableDialogue(currentDay, mood);
            
            if (dialogue != null)
            {
                currentDialogue = new FamilyDialogueInstance(dialogue);
                OnInnerThoughtShown?.Invoke(currentDialogue.innerThought);
                
                Debug.Log($"Inner Thought: {currentDialogue.innerThought}");
                
                // 자동으로 가족 대화로 전환
                Invoke(nameof(MoveToFamilyTalk), 3f);
            }
            else
            {
                Debug.LogWarning($"No dialogue found for Day {currentDay}, Mood {mood}");
                // 대화가 없으면 바로 다음으로
                GameManager.Instance.ChangeState(GameState.FamilyTalk);
            }
        }
        
        private void MoveToFamilyTalk()
        {
            GameManager.Instance.ChangeState(GameState.FamilyTalk);
        }
        
        /// <summary>
        /// 가족 대화 시작
        /// </summary>
        private void StartFamilyDialogue()
        {
            if (currentDialogue == null)
            {
                int currentDay = GameManager.Instance.CurrentDay;
                FamilyMood mood = ParameterManager.Instance.GetCurrentFamilyMood();
                
                var dialogue = FindApplicableDialogue(currentDay, mood);
                if (dialogue != null)
                {
                    currentDialogue = new FamilyDialogueInstance(dialogue);
                }
            }
            
            if (currentDialogue != null)
            {
                OnFamilyDialogueStarted?.Invoke(currentDialogue);
                
                // 경제 시스템의 월간 정산은 이미 EconomyManager에서 처리됨
                
                // 대화 완료 후 자동으로 다음 날로
                float totalDialogueTime = GameConstants.FAMILY_DIALOGUE_DISPLAY_TIME * 2; // 배우자 + 딸
                Invoke(nameof(CompleteFamilyDialogue), totalDialogueTime);
            }
            else
            {
                // 대화가 없으면 바로 다음으로
                GameManager.Instance.ChangeState(GameState.DayEnd);
            }
        }
        
        private void CompleteFamilyDialogue()
        {
            OnFamilyDialogueCompleted?.Invoke();
            currentDialogue = null;
            
            // 다음 날로
            GameManager.Instance.ChangeState(GameState.DayEnd);
        }
        
        /// <summary>
        /// 해당하는 대화 찾기
        /// </summary>
        private FamilyDialogueData FindApplicableDialogue(int currentDay, FamilyMood mood)
        {
            // 정확히 일치하는 대화 찾기
            var exactMatch = allDialogues.FirstOrDefault(d => 
                d.IsActiveOnDay(currentDay) && d.requiredMood == mood);
            
            if (exactMatch != null)
                return exactMatch;
            
            // 일치하는 게 없으면 균형 상태 대화 찾기
            var balancedDialogue = allDialogues.FirstOrDefault(d => 
                d.IsActiveOnDay(currentDay) && d.requiredMood == FamilyMood.Balanced);
            
            return balancedDialogue;
        }
        
        /// <summary>
        /// 가족 관계 평가
        /// </summary>
        public string GetFamilyRelationshipStatus()
        {
            FamilyMood mood = ParameterManager.Instance.GetCurrentFamilyMood();
            
            switch (mood)
            {
                case FamilyMood.HighLoyalty:
                    return "가족이 당신의 성공을 자랑스러워합니다.";
                case FamilyMood.HighConscience:
                    return "가족이 당신을 걱정하고 있습니다.";
                case FamilyMood.Crisis:
                    return "가족과의 관계가 위태롭습니다.";
                default:
                    return "가족과 평범한 일상을 보내고 있습니다.";
            }
        }
    }
}