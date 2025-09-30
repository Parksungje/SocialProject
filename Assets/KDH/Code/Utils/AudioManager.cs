using System.Collections.Generic;
using UnityEngine;

namespace KDH.Code.Utils
{
    /// <summary>
    /// 오디오 시스템 관리
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance => instance;
        
        [Header("오디오 소스")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("BGM")]
        [SerializeField] private AudioClip officeBGM;
        [SerializeField] private AudioClip familyBGM;
        [SerializeField] private AudioClip endingBGM;
        
        [Header("SFX")]
        [SerializeField] private AudioClip stampApproveSound;
        [SerializeField] private AudioClip stampRejectSound;
        [SerializeField] private AudioClip paperFlipSound;
        [SerializeField] private AudioClip keyboardSound;
        [SerializeField] private AudioClip notificationSound;
        
        [Header("볼륨 설정")]
        [SerializeField] private float bgmVolume = 0.5f;
        [SerializeField] private float sfxVolume = 0.7f;
        
        private Dictionary<string, AudioClip> sfxLibrary;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeAudioSources();
            BuildSFXLibrary();
        }
        
        /// <summary>
        /// 오디오 소스 초기화
        /// </summary>
        private void InitializeAudioSources()
        {
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
            }
            
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
            }
            
            bgmSource.volume = bgmVolume;
            sfxSource.volume = sfxVolume;
        }
        
        /// <summary>
        /// SFX 라이브러리 구축
        /// </summary>
        private void BuildSFXLibrary()
        {
            sfxLibrary = new Dictionary<string, AudioClip>
            {
                { "stamp_approve", stampApproveSound },
                { "stamp_reject", stampRejectSound },
                { "paper_flip", paperFlipSound },
                { "keyboard", keyboardSound },
                { "notification", notificationSound }
            };
        }
        
        /// <summary>
        /// BGM 재생
        /// </summary>
        public void PlayBGM(string bgmName)
        {
            AudioClip clip = null;
            
            switch (bgmName.ToLower())
            {
                case "office":
                    clip = officeBGM;
                    break;
                case "family":
                    clip = familyBGM;
                    break;
                case "ending":
                    clip = endingBGM;
                    break;
            }
            
            if (clip != null && bgmSource.clip != clip)
            {
                bgmSource.clip = clip;
                bgmSource.Play();
            }
        }
        
        /// <summary>
        /// BGM 정지
        /// </summary>
        public void StopBGM()
        {
            bgmSource.Stop();
        }
        
        /// <summary>
        /// BGM 페이드아웃
        /// </summary>
        public void FadeOutBGM(float duration = 1f)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }
        
        private System.Collections.IEnumerator FadeOutCoroutine(float duration)
        {
            float startVolume = bgmSource.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }
            
            bgmSource.volume = 0f;
            bgmSource.Stop();
            bgmSource.volume = startVolume;
        }
        
        /// <summary>
        /// SFX 재생
        /// </summary>
        public void PlaySFX(string sfxName)
        {
            if (sfxLibrary.TryGetValue(sfxName, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"SFX not found: {sfxName}");
            }
        }
        
        /// <summary>
        /// 도장 소리 재생 (승인/거부)
        /// </summary>
        public void PlayStampSound(Core.Decision decision)
        {
            string sfxName = decision == Core.Decision.Approve ? "stamp_approve" : "stamp_reject";
            PlaySFX(sfxName);
        }
        
        /// <summary>
        /// BGM 볼륨 설정
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            bgmSource.volume = bgmVolume;
        }
        
        /// <summary>
        /// SFX 볼륨 설정
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            sfxSource.volume = sfxVolume;
        }
    }
}