using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("텍스트 설정")]
        [Tooltip("원본 텍스트 (자동으로 TMP_Text에서 가져옴)")]
        private string originalText;
    
        [Tooltip("호버 시 앞에 붙을 문자")]
        [SerializeField] private string prefixChar = "> ";
    
        [Tooltip("호버 시 뒤에 붙을 문자")]
        [SerializeField] private string suffixChar = " <";
    
        [Header("사운드 설정")]
        [Tooltip("마우스 호버 시 재생할 사운드")]
        [SerializeField] private AudioClip hoverSound;
    
        [Tooltip("마우스 클릭 시 재생할 사운드")]
        [SerializeField] private AudioClip clickSound;
    
        [Tooltip("사운드 볼륨")]
        [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    
        [Header("컴포넌트 참조")]
        private TMP_Text tmpText;
        private AudioSource audioSource;
        private Button button;
    
        [Tooltip("AudioSource를 자동으로 생성할지 여부")]
        [SerializeField] private bool createAudioSource = true;

        private void Awake()
        {
            // TMP_Text 컴포넌트 가져오기
            tmpText = GetComponent<TMP_Text>();
        
            if (tmpText == null)
            {
                Debug.LogError($"[TMPButtonInteraction] TMP_Text 컴포넌트를 찾을 수 없습니다: {gameObject.name}");
                enabled = false;
                return;
            }
        
            // 원본 텍스트 저장
            originalText = tmpText.text;
        
            // Button 컴포넌트 찾기 (부모 또는 같은 GameObject에서)
            button = GetComponentInParent<Button>();
            if (button == null)
            {
                button = GetComponent<Button>();
            }
            
            // Button에 클릭 리스너 추가 (기존 onClick은 유지)
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
        
            // AudioSource 설정
            SetupAudioSource();
        }

        private void SetupAudioSource()
        {
            // 기존 AudioSource 찾기
            audioSource = GetComponent<AudioSource>();
        
            // AudioSource가 없고 자동 생성이 활성화된 경우
            if (audioSource == null && createAudioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // 2D 사운드
            }
        }

        /// <summary>
        /// 마우스 포인터가 버튼 위로 올라갔을 때
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tmpText != null)
            {
                tmpText.text = prefixChar + originalText + suffixChar;
            }
        
            // 호버 사운드 재생
            PlaySound(hoverSound);
        }

        /// <summary>
        /// 마우스 포인터가 버튼에서 벗어났을 때
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (tmpText != null)
            {
                tmpText.text = originalText;
            }
        }

        /// <summary>
        /// 버튼 클릭 시 호출 (Button.onClick을 통해)
        /// IPointerClickHandler 대신 Button의 onClick 이벤트 사용
        /// </summary>
        private void OnButtonClicked()
        {
            // 클릭 사운드 재생
            PlaySound(clickSound);
            
            Debug.Log($"[TMPButtonInteraction] Button clicked: {gameObject.name}");
        }

        /// <summary>
        /// 사운드 재생 메서드
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;
        
            if (audioSource != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }
            else
            {
                // AudioSource가 없는 경우 정적 메서드 사용
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
            }
        }

        /// <summary>
        /// 런타임에 원본 텍스트를 변경할 때 사용
        /// </summary>
        public void SetOriginalText(string newText)
        {
            originalText = newText;
            if (tmpText != null)
            {
                tmpText.text = originalText;
            }
        }

        /// <summary>
        /// 호버 문자 설정 변경
        /// </summary>
        public void SetHoverCharacters(string prefix, string suffix)
        {
            prefixChar = prefix;
            suffixChar = suffix;
        }

        /// <summary>
        /// 사운드 변경
        /// </summary>
        public void SetSounds(AudioClip hover, AudioClip click)
        {
            hoverSound = hover;
            clickSound = click;
        }
        
        private void OnDestroy()
        {
            // Button 리스너 제거
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClicked);
            }
        }
    }
}