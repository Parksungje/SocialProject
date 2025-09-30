using System.Collections.Generic;
using UnityEngine;

namespace KDH.Code.Utils
{
    /// <summary>
    /// 다국어 지원 시스템 (향후 확장용)
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager instance;
        public static LocalizationManager Instance => instance;
        
        [Header("언어 설정")]
        [SerializeField] private SystemLanguage currentLanguage = SystemLanguage.Korean;
        
        private Dictionary<string, Dictionary<SystemLanguage, string>> localizationData;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeLocalization();
        }
        
        /// <summary>
        /// 로컬라이제이션 초기화
        /// </summary>
        private void InitializeLocalization()
        {
            localizationData = new Dictionary<string, Dictionary<SystemLanguage, string>>();
            
            // 현재는 한국어만 지원
            currentLanguage = SystemLanguage.Korean;
        }
        
        /// <summary>
        /// 텍스트 가져오기
        /// </summary>
        public string GetText(string key)
        {
            if (localizationData.TryGetValue(key, out var translations))
            {
                if (translations.TryGetValue(currentLanguage, out string text))
                {
                    return text;
                }
            }
            
            return key; // 키를 찾지 못하면 키 자체를 반환
        }
        
        /// <summary>
        /// 언어 변경
        /// </summary>
        public void SetLanguage(SystemLanguage language)
        {
            currentLanguage = language;
            // TODO: UI 갱신 이벤트 발생
        }
    }
}