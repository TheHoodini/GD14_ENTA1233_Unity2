using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// In game HUD shown when not paused
/// </summary>
public class GameUI : MenuBase
{
    public override GameMenus MenuType()
    {
        return GameMenus.InGameUI;
    }

    [SerializeField] private Image _healthFillImage;
    [SerializeField] private TextMeshProUGUI _potionAmountText;

    private Health _playerHealth;
    private int _playerPotionAmount;

    private void OnEnable()
    {
        if (PlayerMgr.Instance == null)
        {
            Debug.LogError("GameUI: PlayerMgr is null");
            return;
        }

        //if the player was set already
        if (PlayerMgr.Instance.HasSpawnedPlayer)
        {
            HandlePlayerAssigned(PlayerMgr.Instance.PlayerObject);
            return;
        }

        //otherwise wait for the player to spawn
        PlayerMgr.Instance.OnPlayerAssigned += HandlePlayerAssigned;
    }

    private void OnDisable()
    {
        if (PlayerMgr.Instance != null) PlayerMgr.Instance.OnPlayerAssigned -= HandlePlayerAssigned;
    }

    private void HandlePlayerAssigned(GameObject playerObject)
    {
        if (playerObject == null)
        {
            RefreshHealthBar(null);
            return;
        }

        _playerHealth = playerObject.GetComponentInChildren<Health>();
        _playerPotionAmount = playerObject.GetComponentInChildren<PlayerController>()?.BombAmmo ?? 0;

        if (_playerHealth == null)
        {
            Debug.LogError("GameUI: Player object does not have a Health Component.");
            return;
        } if (_playerPotionAmount < 0)
        {
            Debug.LogError("GameUI: Player object does not have a PlayerController Component or BombAmmo is negative");
            return;
        }

        _playerHealth.OnHealthChanged += RefreshHealthBar;
        playerObject.GetComponentInChildren<PlayerController>().OnBombAmmoChanged += RefreshPotionBar;

        RefreshHealthBar(_playerHealth);
        RefreshPotionBar(_playerPotionAmount);
    }

    private void RefreshHealthBar(Health health)
    {
        if (_healthFillImage == null) return;

        _healthFillImage.fillAmount = health != null ? health.NormalizedHealth : 0f;
    }

    private void RefreshPotionBar(int potionAmount)
    {
        if (_potionAmountText == null) return;

        _potionAmountText.text = potionAmount.ToString();
    }
}