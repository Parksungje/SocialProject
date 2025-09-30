using System.Collections;
using UnityEngine;

namespace KDH.Code.Utils
{
    /// <summary>
    /// 화면 전환 및 효과 관리
    /// </summary>
    public class ScreenManager : MonoBehaviour
    {
        private static ScreenManager instance;
        public static ScreenManager Instance => instance;
        
        [Header("페이드 설정")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 페이드 캔버스 그룹 설정
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }
        
        /// <summary>
        /// 화면 페이드아웃
        /// </summary>
        public void FadeOut(System.Action onComplete = null)
        {
            StartCoroutine(FadeCoroutine(0f, 1f, fadeDuration, onComplete));
        }
        
        /// <summary>
        /// 화면 페이드인
        /// </summary>
        public void FadeIn(System.Action onComplete = null)
        {
            StartCoroutine(FadeCoroutine(1f, 0f, fadeDuration, onComplete));
        }
        
        /// <summary>
        /// 페이드 코루틴
        /// </summary>
        private IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration, System.Action onComplete)
        {
            if (fadeCanvasGroup == null)
            {
                onComplete?.Invoke();
                yield break;
            }
            
            fadeCanvasGroup.alpha = startAlpha;
            fadeCanvasGroup.blocksRaycasts = true;
            
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                yield return null;
            }
            
            fadeCanvasGroup.alpha = endAlpha;
            fadeCanvasGroup.blocksRaycasts = endAlpha > 0.5f;
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 화면 전환 (페이드아웃 -> 액션 -> 페이드인)
        /// </summary>
        public void TransitionScreen(System.Action transitionAction)
        {
            StartCoroutine(TransitionCoroutine(transitionAction));
        }
        
        private IEnumerator TransitionCoroutine(System.Action transitionAction)
        {
            // 페이드아웃
            yield return StartCoroutine(FadeCoroutine(0f, 1f, fadeDuration, null));
            
            // 전환 액션 실행
            transitionAction?.Invoke();
            
            // 약간의 딜레이
            yield return new WaitForSeconds(0.2f);
            
            // 페이드인
            yield return StartCoroutine(FadeCoroutine(1f, 0f, fadeDuration, null));
        }
    }
}