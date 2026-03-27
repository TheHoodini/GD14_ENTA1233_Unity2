using TMPro;
using UnityEngine;

public class LevelSelectEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;

    private int _levelIndex;
    public void Setup (string level, int levelIndex)
    {
        _levelIndex = levelIndex;

        if (_titleText != null) _titleText.text = level;
    }

    public void ButtonPressed()
    {
        LevelMgr.Instance.SetCurrentLevel(_levelIndex);
        SceneMgr.Instance.LoadScene(GameScenes.Gameplay, GameMenus.InGameUI);
    }
}
