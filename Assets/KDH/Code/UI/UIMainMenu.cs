using KDH.Code.Core;
using KDH.Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [Header("버튼")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;
        
        [Header("저장 파일 정보")]
        [SerializeField] private TextMeshProUGUI saveFileInfoText;
        
        private void Awake()
        {
            // Start 대신 Awake에서 초기화
            Debug.Log("[UIMainMenu] Awake - Initializing");
        }
        
        private void Start()
        {
            Debug.Log("[UIMainMenu] Start - Setting up buttons");
            
            InitializeButtons();
            UpdateContinueButton();
            
            // 메인 메뉴 씬에서만 이벤트 등록하지 않음
            bool isMainMenuScene = Utils.SceneTransitionManager.Instance == null || 
                                   Utils.SceneTransitionManager.Instance.IsMainMenuScene();
            
            if (!isMainMenuScene && GameManager.Instance != null)
            {
                RegisterEvents();
            }
            else
            {
                // 메인 메뉴 씬에서는 강제 활성화
                gameObject.SetActive(true);
                Debug.Log("[UIMainMenu] Main menu scene detected - staying active");
            }
        }
        
        private void OnDestroy()
        {
            UnregisterEvents();
        }
        
        private void InitializeButtons()
        {
            // NULL 체크
            if (newGameButton == null)
            {
                Debug.LogError("[UIMainMenu] NewGameButton is not assigned!");
                return;
            }
            if (continueButton == null)
            {
                Debug.LogError("[UIMainMenu] ContinueButton is not assigned!");
                return;
            }
            if (creditsButton == null)
            {
                Debug.LogError("[UIMainMenu] CreditsButton is not assigned!");
                return;
            }
            if (quitButton == null)
            {
                Debug.LogError("[UIMainMenu] QuitButton is not assigned!");
                return;
            }
            
            // 기존 리스너 제거 후 추가
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(OnNewGameClicked);
            
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
            
            creditsButton.onClick.RemoveAllListeners();
            creditsButton.onClick.AddListener(OnCreditsClicked);
            
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitClicked);
            
            Debug.Log("[UIMainMenu] All buttons initialized successfully");
        }
        
        private void RegisterEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                Debug.Log("[UIMainMenu] Events registered");
            }
        }
        
        private void UnregisterEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            // 메인 메뉴 씬이 아닐 때만 상태 확인
            bool isMainMenuScene = Utils.SceneTransitionManager.Instance != null && 
                                   Utils.SceneTransitionManager.Instance.IsMainMenuScene();
            
            if (!isMainMenuScene)
            {
                gameObject.SetActive(newState == GameState.MainMenu);
            }
            
            if (newState == GameState.MainMenu)
            {
                UpdateContinueButton();
            }
        }
        
        private void UpdateContinueButton()
        {
            if (SaveManager.Instance != null)
            {
                bool hasSave = SaveManager.Instance.HasSaveFile();
                continueButton.interactable = hasSave;
                
                if (hasSave && saveFileInfoText != null)
                {
                    saveFileInfoText.text = SaveManager.Instance.GetSaveFileInfo();
                }
                else if (saveFileInfoText != null)
                {
                    saveFileInfoText.text = "";
                }
            }
        }
        
        private void OnNewGameClicked()
        {
            Debug.Log("[UIMainMenu] ===== NEW GAME CLICKED =====");
            
            // 저장 파일 확인
            if (SaveManager.Instance != null && SaveManager.Instance.HasSaveFile())
            {
                Debug.LogWarning("기존 저장 파일을 덮어쓰시겠습니까?");
            }
            
            // 모든 매니저 초기화
            if (ParameterManager.Instance != null)
                ParameterManager.Instance.ResetParameters();
            
            if (EconomyManager.Instance != null)
                EconomyManager.Instance.ResetEconomy();
            
            // 게임 씬으로 전환
            if (Utils.SceneTransitionManager.Instance != null)
            {
                Debug.Log("[UIMainMenu] Loading game scene...");
                Utils.SceneTransitionManager.Instance.LoadGameScene();
            }
            else
            {
                Debug.LogError("[UIMainMenu] SceneTransitionManager not found!");
            }
        }
        
        private void OnContinueClicked()
        {
            Debug.Log("[UIMainMenu] ===== CONTINUE CLICKED =====");
            
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
            }
            
            if (Utils.SceneTransitionManager.Instance != null)
            {
                Utils.SceneTransitionManager.Instance.LoadGameScene();
            }
            else
            {
                Debug.LogError("[UIMainMenu] SceneTransitionManager not found!");
            }
        }
        
        private void OnCreditsClicked()
        {
            Debug.Log("[UIMainMenu] ===== CREDITS CLICKED =====");
            // TODO: 크레딧 화면 표시
        }
        
        private void OnQuitClicked()
        {
            Debug.Log("[UIMainMenu] ===== QUIT CLICKED =====");
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}