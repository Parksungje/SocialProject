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
            Debug.Log("[AudioManager] ===== AWAKE =====");
            
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeAudioSources();
            BuildSFXLibrary();
            
            Debug.Log("[AudioManager] Initialized successfully");
        }
        
        /// <summary>
        /// 오디오 소스 초기화
        /// </summary>
        private void InitializeAudioSources()
        {
            Debug.Log("[AudioManager] Initializing audio sources...");
            
            // BGM Source
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
                Debug.Log("[AudioManager] BGM AudioSource created");
            }
            bgmSource.volume = bgmVolume;
            
            // SFX Source
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                Debug.Log("[AudioManager] SFX AudioSource created");
            }
            sfxSource.volume = sfxVolume;
            
            Debug.Log($"[AudioManager] BGM Volume: {bgmVolume}, SFX Volume: {sfxVolume}");
        }
        
        /// <summary>
        /// SFX 라이브러리 구축
        /// </summary>
        private void BuildSFXLibrary()
        {
            Debug.Log("[AudioManager] Building SFX library...");
            
            sfxLibrary = new Dictionary<string, AudioClip>
            {
                { "stamp_approve", stampApproveSound },
                { "stamp_reject", stampRejectSound },
                { "paper_flip", paperFlipSound },
                { "keyboard", keyboardSound },
                { "notification", notificationSound }
            };
            
            // 디버그: 등록된 사운드 출력
            foreach (var kvp in sfxLibrary)
            {
                if (kvp.Value != null)
                    Debug.Log($"[AudioManager] SFX registered: {kvp.Key} → {kvp.Value.name}");
                else
                    Debug.LogWarning($"[AudioManager] SFX missing: {kvp.Key} → NULL");
            }
        }
        
        /// <summary>
        /// BGM 재생
        /// </summary>
        public void PlayBGM(string bgmName)
        {
            Debug.Log($"[AudioManager] PlayBGM called: {bgmName}");
            
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
                Debug.Log($"[AudioManager] BGM started: {clip.name}");
            }
            else if (clip == null)
            {
                Debug.LogWarning($"[AudioManager] BGM clip not found: {bgmName}");
            }
        }
        
        /// <summary>
        /// BGM 정지
        /// </summary>
        public void StopBGM()
        {
            Debug.Log("[AudioManager] Stopping BGM");
            bgmSource.Stop();
        }
        
        /// <summary>
        /// SFX 재생
        /// </summary>
        public void PlaySFX(string sfxName)
        {
            Debug.Log($"[AudioManager] PlaySFX called: {sfxName}");
            
            if (sfxLibrary.TryGetValue(sfxName, out AudioClip clip))
            {
                if (clip != null)
                {
                    sfxSource.PlayOneShot(clip);
                    Debug.Log($"[AudioManager] SFX played: {sfxName}");
                }
                else
                {
                    Debug.LogError($"[AudioManager] SFX clip is NULL: {sfxName}");
                }
            }
            else
            {
                Debug.LogWarning($"[AudioManager] SFX not found in library: {sfxName}");
            }
        }
        
        /// <summary>
        /// 도장 소리 재생 (승인/거부)
        /// </summary>
        public void PlayStampSound(Core.Decision decision)
        {
            string sfxName = decision == Core.Decision.Approve ? "stamp_approve" : "stamp_reject";
            Debug.Log($"[AudioManager] PlayStampSound: {decision} → {sfxName}");
            PlaySFX(sfxName);
        }
        
        /// <summary>
        /// BGM 볼륨 설정
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            bgmSource.volume = bgmVolume;
            Debug.Log($"[AudioManager] BGM volume set to: {bgmVolume}");
        }
        
        /// <summary>
        /// SFX 볼륨 설정
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            sfxSource.volume = sfxVolume;
            Debug.Log($"[AudioManager] SFX volume set to: {sfxVolume}");
        }
    }
}