using UnityEngine;

public class LevelSelectUI : MenuBase
{
    [SerializeField] private Transform _contentParent;
    [SerializeField] private LevelSelectEntryUI _entryPrefab;

    public override GameMenus MenuType()
    {
        return GameMenus.LevelSelectMenu;
    }
    void Start()
    {
        BuildEntries();
    }

    void Update()
    {
        
    }

    private void BuildEntries()
    {
        var levels = LevelMgr.Instance.LevelSceneNames;
        if (levels.Length == 0) return;

        for (int i = 0; i < levels.Length; i++)
        {
            var entry = Instantiate(_entryPrefab, _contentParent);
            entry.Setup(levels[i], i);
        }
    }
}
