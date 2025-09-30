using KDH.Code.Core;
using KDH.Code.Data;
using KDH.Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 엔딩 화면 UI
    /// </summary>
    public class UIEndingPanel : MonoBehaviour
    {
        [Header("엔딩 정보")]
        [SerializeField] private Image endingImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI storyText;
        [SerializeField] private TextMeshProUGUI messageText;
        
        [Header("통계")]
        [SerializeField] private GameObject statisticsPanel;
        [SerializeField] private TextMeshProUGUI statisticsText;
        
        [Header("버튼")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        
        private void Start()
        {
            restartButton.onClick.AddListener(OnRestartClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
            RegisterEvents();
        }
        
        private void OnDestroy()
        {
            UnregisterEvents();
        }
        
        /// <summary>
        /// 이벤트 등록
        /// </summary>
        private void RegisterEvents()
        {
            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.OnEndingShown += DisplayEnding;
            }
            
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
            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.OnEndingShown -= DisplayEnding;
            }
            
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
            gameObject.SetActive(newState == GameState.Ending);
        }
        
        /// <summary>
        /// 엔딩 표시
        /// </summary>
        private void DisplayEnding(EndingData ending)
        {
            // 엔딩 정보 표시
            titleText.text = ending.endingTitle;
            storyText.text = ending.endingStory;
            messageText.text = ending.coreMessage;
            
            if (ending.endingImage != null)
            {
                endingImage.sprite = ending.endingImage;
            }
            
            endingImage.color = ending.themeColor;
            
            // 통계 표시
            if (ending.showStatistics)
            {
                DisplayStatistics();
            }
            else
            {
                statisticsPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 통계 표시
        /// </summary>
        private void DisplayStatistics()
        {
            statisticsPanel.SetActive(true);
            
            if (EndingManager.Instance != null)
            {
                var stats = EndingManager.Instance.GenerateStatistics();
                statisticsText.text = stats.ToString();
            }
        }
        
        /// <summary>
        /// 재시작 버튼 클릭
        /// </summary>
        private void OnRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
            }
        }
        
        /// <summary>
        /// 메인 메뉴 버튼 클릭
        /// </summary>
        private void OnMainMenuClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.MainMenu);
            }
        }
    }
}