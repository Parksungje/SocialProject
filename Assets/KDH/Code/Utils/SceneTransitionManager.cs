using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KDH.Code.Utils
{
    /// <summary>
    /// 씬 전환 관리 시스템
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        private static SceneTransitionManager instance;
        public static SceneTransitionManager Instance => instance;
        
        [Header("씬 이름")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string gameSceneName = "GameScene";
        
        [Header("전환 설정")]
        [SerializeField] private float transitionDelay = 0.5f;
        
        private bool isTransitioning = false;
        
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
        
        /// <summary>
        /// 메인 메뉴로 이동
        /// </summary>
        public void LoadMainMenu()
        {
            if (isTransitioning) return;
            
            StartCoroutine(LoadSceneAsync(mainMenuSceneName));
        }
        
        /// <summary>
        /// 게임 씬으로 이동
        /// </summary>
        public void LoadGameScene()
        {
            if (isTransitioning) return;
            
            StartCoroutine(LoadSceneAsync(gameSceneName));
        }
        
        /// <summary>
        /// 씬 비동기 로드
        /// </summary>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            isTransitioning = true;
            
            // 페이드 아웃
            if (Utils.ScreenManager.Instance != null)
            {
                Utils.ScreenManager.Instance.FadeOut();
                yield return new WaitForSeconds(transitionDelay);
            }
            
            // 씬 로드 시작
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            
            // 로딩 대기
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            
            // 씬 활성화
            asyncLoad.allowSceneActivation = true;
            
            // 씬 로드 완료 대기
            yield return new WaitUntil(() => asyncLoad.isDone);
            
            // 페이드 인
            if (Utils.ScreenManager.Instance != null)
            {
                Utils.ScreenManager.Instance.FadeIn();
            }
            
            isTransitioning = false;
            
            Debug.Log($"Scene loaded: {sceneName}");
        }
        
        /// <summary>
        /// 현재 씬 이름 반환
        /// </summary>
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
        
        /// <summary>
        /// 게임 씬인지 확인
        /// </summary>
        public bool IsGameScene()
        {
            return GetCurrentSceneName() == gameSceneName;
        }
        
        /// <summary>
        /// 메인 메뉴 씬인지 확인
        /// </summary>
        public bool IsMainMenuScene()
        {
            return GetCurrentSceneName() == mainMenuSceneName;
        }
    }
}