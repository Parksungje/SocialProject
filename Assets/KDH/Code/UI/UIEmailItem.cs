using System;
using KDH.Code.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 이메일 목록 아이템 UI
    /// </summary>
    public class UIEmailItem : MonoBehaviour
    {
        [Header("UI 요소")]
        [SerializeField] private TextMeshProUGUI senderText;
        [SerializeField] private TextMeshProUGUI subjectText;
        [SerializeField] private GameObject unreadIndicator;
        [SerializeField] private GameObject mandatoryIndicator;
        [SerializeField] private Button selectButton;
        
        [Header("색상")]
        [SerializeField] private Color readColor = Color.gray;
        [SerializeField] private Color unreadColor = Color.white;
        
        private EmailInstance email;
        private Action onClickCallback;
        
        /// <summary>
        /// 이메일 아이템 설정
        /// </summary>
        public void Setup(EmailInstance emailInstance, Action onClick)
        {
            email = emailInstance;
            onClickCallback = onClick;
            
            senderText.text = emailInstance.emailData.senderName;
            subjectText.text = emailInstance.emailData.subject;
            
            UpdateReadState();
            
            mandatoryIndicator.SetActive(emailInstance.emailData.isMandatory);
            
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnItemClicked);
        }
        
        /// <summary>
        /// 읽음 상태 업데이트
        /// </summary>
        private void UpdateReadState()
        {
            bool isRead = email.isRead;
            unreadIndicator.SetActive(!isRead);
            
            Image bgImage = GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = isRead ? readColor : unreadColor;
            }
        }
        
        /// <summary>
        /// 아이템 클릭 처리
        /// </summary>
        private void OnItemClicked()
        {
            onClickCallback?.Invoke();
            UpdateReadState();
        }
    }
}