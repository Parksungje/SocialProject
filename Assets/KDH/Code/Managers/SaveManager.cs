using System;
using System.IO;
using UnityEngine;

namespace KDH.Code.Managers
{
    /// <summary>
    /// 저장/로드 시스템 관리
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager instance;
        public static SaveManager Instance => instance;
        
        private const string SAVE_FILE_NAME = "savegame.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        
        // 이벤트
        public event Action OnGameSaved;
        public event Action OnGameLoaded;
        
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
        /// 게임 저장
        /// </summary>
        public void SaveGame()
        {
            try
            {
                GameSaveData saveData = new GameSaveData
                {
                    currentDay = GameManager.Instance.CurrentDay,
                    saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    
                    // 파라미터 저장
                    parameterData = ParameterManager.Instance?.GetSaveData(),
                    
                    // 경제 저장
                    economyData = EconomyManager.Instance?.GetSaveData()
                };
                
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SavePath, json);
                
                OnGameSaved?.Invoke();
                Debug.Log($"Game saved: {SavePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed: {e.Message}");
            }
        }
        
        /// <summary>
        /// 게임 로드
        /// </summary>
        public void LoadGame()
        {
            if (!HasSaveFile())
            {
                Debug.LogWarning("No save file found");
                return;
            }
            
            try
            {
                string json = File.ReadAllText(SavePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                
                // 데이터 복원
                GameManager.Instance.SetCurrentDay(saveData.currentDay);
                
                if (saveData.parameterData != null)
                {
                    ParameterManager.Instance?.LoadSaveData(saveData.parameterData);
                }
                
                if (saveData.economyData != null)
                {
                    EconomyManager.Instance?.LoadSaveData(saveData.economyData);
                }
                
                OnGameLoaded?.Invoke();
                Debug.Log($"Game loaded: Day {saveData.currentDay}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Load failed: {e.Message}");
            }
        }
        
        /// <summary>
        /// 저장 파일 존재 여부
        /// </summary>
        public bool HasSaveFile()
        {
            return File.Exists(SavePath);
        }
        
        /// <summary>
        /// 저장 파일 삭제
        /// </summary>
        public void DeleteSaveFile()
        {
            if (HasSaveFile())
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted");
            }
        }
        
        /// <summary>
        /// 저장 파일 정보 가져오기
        /// </summary>
        public string GetSaveFileInfo()
        {
            if (!HasSaveFile())
                return "저장된 게임이 없습니다.";
            
            try
            {
                string json = File.ReadAllText(SavePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                
                return $"Day {saveData.currentDay}\n저장 시간: {saveData.saveTime}";
            }
            catch
            {
                return "저장 파일을 읽을 수 없습니다.";
            }
        }
    }
    
    /// <summary>
    /// 게임 저장 데이터
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public int currentDay;
        public string saveTime;
        
        public ParameterSaveData parameterData;
        public EconomySaveData economyData;
    }
}