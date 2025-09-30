using KDH.Code.Document;
using KDH.Code.Managers;
using KDH.Code.Utils;
using UnityEngine;

namespace KDH.Code.Core
{
    /// <summary>
    /// 게임 초기화 및 부트스트랩
    /// 씬의 첫 번째 GameObject에 부착하여 모든 매니저 초기화
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("매니저 프리팹")]
        [SerializeField] private GameObject gameManagerPrefab;
        [SerializeField] private GameObject parameterManagerPrefab;
        [SerializeField] private GameObject economyManagerPrefab;
        [SerializeField] private GameObject documentManagerPrefab;
        [SerializeField] private GameObject emailManagerPrefab;
        [SerializeField] private GameObject familyManagerPrefab;
        [SerializeField] private GameObject endingManagerPrefab;
        [SerializeField] private GameObject saveManagerPrefab;
        [SerializeField] private GameObject ruleEnginePrefab;
        [SerializeField] private GameObject documentGeneratorPrefab;
        [SerializeField] private GameObject audioManagerPrefab;
        [SerializeField] private GameObject screenManagerPrefab;
        
        [Header("초기화 순서")]
        [SerializeField] private bool initializeOnAwake = true;
        
        private void Awake()
        {
            if (initializeOnAwake)
            {
                InitializeGame();
            }
        }
        
        /// <summary>
        /// 게임 초기화
        /// </summary>
        public void InitializeGame()
        {
            Debug.Log("=== Game Bootstrap Started ===");
            
            // 핵심 매니저 초기화
            InitializeManager<GameManager>(gameManagerPrefab, "GameManager");
            InitializeManager<ParameterManager>(parameterManagerPrefab, "ParameterManager");
            InitializeManager<EconomyManager>(economyManagerPrefab, "EconomyManager");
            
            // 게임 시스템 매니저 초기화
            InitializeManager<DocumentManager>(documentManagerPrefab, "DocumentManager");
            InitializeManager<EmailManager>(emailManagerPrefab, "EmailManager");
            InitializeManager<FamilyManager>(familyManagerPrefab, "FamilyManager");
            InitializeManager<EndingManager>(endingManagerPrefab, "EndingManager");
            InitializeManager<SaveManager>(saveManagerPrefab, "SaveManager");
            
            // 규칙 및 생성 시스템
            InitializeManager<RuleEngine>(ruleEnginePrefab, "RuleEngine");
            InitializeManager<DocumentGenerator>(documentGeneratorPrefab, "DocumentGenerator");
            
            // 유틸리티 매니저 초기화
            InitializeManager<AudioManager>(audioManagerPrefab, "AudioManager");
            InitializeManager<ScreenManager>(screenManagerPrefab, "ScreenManager");
            
            Debug.Log("=== Game Bootstrap Completed ===");
        }
        
        /// <summary>
        /// 매니저 초기화 헬퍼
        /// </summary>
        private void InitializeManager<T>(GameObject prefab, string managerName) where T : MonoBehaviour
        {
            // 이미 존재하는지 확인
            T existingManager = FindObjectOfType<T>();
            
            if (existingManager != null)
            {
                Debug.Log($"[Bootstrap] {managerName} already exists");
                return;
            }
            
            // 프리팹이 있으면 인스턴스화
            if (prefab != null)
            {
                GameObject managerObj = Instantiate(prefab);
                managerObj.name = managerName;
                Debug.Log($"[Bootstrap] {managerName} instantiated from prefab");
            }
            else
            {
                // 프리팹이 없으면 새 GameObject 생성
                GameObject managerObj = new GameObject(managerName);
                managerObj.AddComponent<T>();
                Debug.Log($"[Bootstrap] {managerName} created as new GameObject");
            }
        }
    }
}