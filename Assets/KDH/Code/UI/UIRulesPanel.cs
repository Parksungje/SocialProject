using System.Collections.Generic;
using KDH.Code.Core;
using KDH.Code.Data;
using KDH.Code.Document;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDH.Code.UI
{
    /// <summary>
    /// 채용 규칙 표시 패널
    /// </summary>
    public class UIRulesPanel : MonoBehaviour
    {
        [Header("UI 요소")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Transform rulesContainer;
        [SerializeField] private GameObject ruleItemPrefab;
        [SerializeField] private Button closeButton;
        
        [Header("규칙 색상")]
        [SerializeField] private Color basicRuleColor = Color.white;
        [SerializeField] private Color preferenceRuleColor = new Color(1f, 0.6f, 0f); // 주황색
        [SerializeField] private Color priorityRuleColor = Color.green;
        [SerializeField] private Color prohibitionRuleColor = Color.red;
        
        private void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePanel);
            }
            
            // 초기에는 숨김
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 규칙 패널 표시
        /// </summary>
        public void ShowRules()
        {
            Debug.Log("[UIRulesPanel] Showing rules...");
            
            // 기존 규칙 아이템 제거
            ClearRules();
            
            // 현재 활성 규칙 가져오기
            List<HiringRule> activeRules = RuleEngine.Instance?.GetActiveRules();
            
            if (activeRules == null || activeRules.Count == 0)
            {
                Debug.LogWarning("[UIRulesPanel] No active rules found!");
                DisplayNoRulesMessage();
                gameObject.SetActive(true);
                return;
            }
            
            // 타이틀 설정
            int currentDay = Managers.GameManager.Instance?.CurrentDay ?? 1;
            if (titleText != null)
            {
                titleText.text = $"📋 채용 규칙 (Day {currentDay})";
            }
            
            // 규칙 타입별로 분류
            List<HiringRule> basicRules = new List<HiringRule>();
            List<HiringRule> preferenceRules = new List<HiringRule>();
            List<HiringRule> priorityRules = new List<HiringRule>();
            List<HiringRule> prohibitionRules = new List<HiringRule>();
            
            foreach (var rule in activeRules)
            {
                switch (rule.ruleType)
                {
                    case RuleType.Basic:
                        basicRules.Add(rule);
                        break;
                    case RuleType.Preference:
                        preferenceRules.Add(rule);
                        break;
                    case RuleType.Priority:
                        priorityRules.Add(rule);
                        break;
                    case RuleType.Prohibition:
                        prohibitionRules.Add(rule);
                        break;
                }
            }
            
            // 각 카테고리별로 표시
            DisplayRuleCategory("기본 규칙", basicRules, basicRuleColor);
            DisplayRuleCategory("편견 규칙 ⚠️", preferenceRules, preferenceRuleColor);
            DisplayRuleCategory("우대 규칙 ⭐", priorityRules, priorityRuleColor);
            DisplayRuleCategory("금지 규칙 ❌", prohibitionRules, prohibitionRuleColor);
            
            // 패널 활성화
            gameObject.SetActive(true);
            
            Debug.Log($"[UIRulesPanel] Displayed {activeRules.Count} rules");
        }
        
        /// <summary>
        /// 규칙 카테고리 표시
        /// </summary>
        private void DisplayRuleCategory(string categoryName, List<HiringRule> rules, Color color)
        {
            if (rules.Count == 0)
                return;
            
            // 카테고리 헤더 생성
            CreateCategoryHeader(categoryName, color);
            
            // 각 규칙 아이템 생성
            foreach (var rule in rules)
            {
                CreateRuleItem(rule, color);
            }
            
            // 구분선 추가
            CreateSeparator();
        }
        
        /// <summary>
        /// 카테고리 헤더 생성
        /// </summary>
        private void CreateCategoryHeader(string categoryName, Color color)
        {
            if (ruleItemPrefab == null || rulesContainer == null)
                return;
            
            GameObject headerObj = Instantiate(ruleItemPrefab, rulesContainer);
            TextMeshProUGUI headerText = headerObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (headerText != null)
            {
                headerText.text = $"<b>{categoryName}</b>";
                headerText.color = color;
                headerText.fontSize += 4;
            }
        }
        
        /// <summary>
        /// 규칙 아이템 생성
        /// </summary>
        private void CreateRuleItem(HiringRule rule, Color color)
        {
            if (ruleItemPrefab == null || rulesContainer == null)
                return;
            
            GameObject itemObj = Instantiate(ruleItemPrefab, rulesContainer);
            TextMeshProUGUI itemText = itemObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (itemText != null)
            {
                // 비윤리적 규칙은 경고 표시
                string warning = rule.isEthical ? "" : " ⚠️";
                itemText.text = $"  • {rule.ruleDescription}{warning}";
                itemText.color = color;
            }
        }
        
        /// <summary>
        /// 구분선 생성
        /// </summary>
        private void CreateSeparator()
        {
            if (ruleItemPrefab == null || rulesContainer == null)
                return;
            
            GameObject separatorObj = Instantiate(ruleItemPrefab, rulesContainer);
            TextMeshProUGUI separatorText = separatorObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (separatorText != null)
            {
                separatorText.text = "";
                separatorText.fontSize = 10;
            }
        }
        
        /// <summary>
        /// 규칙 없음 메시지
        /// </summary>
        private void DisplayNoRulesMessage()
        {
            if (titleText != null)
            {
                titleText.text = "📋 채용 규칙";
            }
            
            if (ruleItemPrefab != null && rulesContainer != null)
            {
                GameObject itemObj = Instantiate(ruleItemPrefab, rulesContainer);
                TextMeshProUGUI itemText = itemObj.GetComponentInChildren<TextMeshProUGUI>();
                
                if (itemText != null)
                {
                    itemText.text = "현재 적용 중인 규칙이 없습니다.";
                    itemText.color = Color.gray;
                }
            }
        }
        
        /// <summary>
        /// 기존 규칙 제거
        /// </summary>
        private void ClearRules()
        {
            if (rulesContainer == null)
                return;
            
            foreach (Transform child in rulesContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        /// <summary>
        /// 패널 닫기
        /// </summary>
        public void ClosePanel()
        {
            Debug.Log("[UIRulesPanel] Closing panel");
            gameObject.SetActive(false);
        }
    }
}