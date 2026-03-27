
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Manager to apply level based data to the game state before the game loop begins
/// Might contain a list of difficulties, levels, etc.
/// </summary>
public class LevelMgr : Singleton<LevelMgr>
{
    [SerializeField] private string[] _levelSceneNames;
    public string[] LevelSceneNames => _levelSceneNames;

    private int _currentLevelIndex;
    [Serializable]
    public class LevelData
    {
        public string SceneName;
        public string LevelName;
        public Sprite LevelIcon;
    }
    [SerializeField] private LevelData[] _allLevelData;
    public bool IsLevelLoaded { get; private set; }

    public void NextLevel()
    {
        _currentLevelIndex++;
    }
    public void RestartLevelCount()
    {
        _currentLevelIndex = 0;
    } 

    public void LoadCurrentLevel()
    {
        IsLevelLoaded = false;
        StartCoroutine(LoadLevelRoutine());
    }

    public void SetCurrentLevel(int levelIndex)
    {
        _currentLevelIndex = levelIndex;
    }

    private IEnumerator LoadLevelRoutine()
    {
        string levelName = _levelSceneNames[_currentLevelIndex];

        Debug.Log($"LevelMgr: Loading {levelName} additively");

        var asyncOperation =
            SceneManager.LoadSceneAsync(
                levelName, LoadSceneMode.Additive);

        while (asyncOperation is { isDone: false }) yield return null;

        Debug.Log($"LevelMgr: {levelName} loaded");

        IsLevelLoaded = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}