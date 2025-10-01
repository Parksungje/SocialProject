using KDH.Code.Core;
using KDH.Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 메인 메뉴 UI
    /// </summary>
    public class UIMainMenu : MonoBehaviour
    {
        [Header("버튼")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;
        
        [Header("저장 파일 정보")]
        [SerializeField] private TextMeshProUGUI saveFileInfoText;
        
        private void Start()
        {
            InitializeButtons();
            UpdateContinueButton();
            RegisterEvents();
        }
        
        private void OnDestroy()
        {
            UnregisterEvents();
        }
        
        /// <summary>
        /// 버튼 초기화
        /// </summary>
        private void InitializeButtons()
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
            continueButton.onClick.AddListener(OnContinueClicked);
            creditsButton.onClick.AddListener(OnCreditsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }
        
        /// <summary>
        /// 이벤트 등록
        /// </summary>
        private void RegisterEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
        }
        
        /// <summary>
        /// 이벤트 해제
        /// </summary>
        private void UnregisterEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }
        
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            gameObject.SetActive(newState == GameState.MainMenu);
            
            if (newState == GameState.MainMenu)
            {
                UpdateContinueButton();
            }
        }
        
        /// <summary>
        /// Continue 버튼 상태 업데이트
        /// </summary>
        private void UpdateContinueButton()
        {
            if (SaveManager.Instance != null)
            {
                bool hasSave = SaveManager.Instance.HasSaveFile();
                continueButton.interactable = hasSave;
                
                if (hasSave)
                {
                    saveFileInfoText.text = SaveManager.Instance.GetSaveFileInfo();
                }
                else
                {
                    saveFileInfoText.text = "";
                }
            }
        }
        
        /// <summary>
        /// 새 게임 버튼 클릭
        /// </summary>
        private void OnNewGameClicked()
        {
            // 저장 파일이 있으면 경고
            if (SaveManager.Instance != null && SaveManager.Instance.HasSaveFile())
            {
                // TODO: 확인 다이얼로그 표시
                Debug.Log("기존 저장 파일을 덮어쓰시겠습니까?");
            }
            
            // 모든 매니저 초기화
            if (ParameterManager.Instance != null)
                ParameterManager.Instance.ResetParameters();
            
            if (EconomyManager.Instance != null)
                EconomyManager.Instance.ResetEconomy();
            
            // 게임 씬으로 전환
            if (Utils.SceneTransitionManager.Instance != null)
            {
                Utils.SceneTransitionManager.Instance.LoadGameScene();
            }
            else
            {
                Debug.LogError("SceneTransitionManager not found!");
            }
        }
        
        /// <summary>
        /// 계속하기 버튼 클릭
        /// </summary>
        private void OnContinueClicked()
        {
            // 저장 파일 로드
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
            }
            
            // 게임 씬으로 전환
            if (Utils.SceneTransitionManager.Instance != null)
            {
                Utils.SceneTransitionManager.Instance.LoadGameScene();
            }
            else
            {
                Debug.LogError("SceneTransitionManager not found!");
            }
        }
        
        /// <summary>
        /// 크레딧 버튼 클릭
        /// </summary>
        private void OnCreditsClicked()
        {
            // TODO: 크레딧 화면 표시
            Debug.Log("Credits");
        }
        
        /// <summary>
        /// 종료 버튼 클릭
        /// </summary>
        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}