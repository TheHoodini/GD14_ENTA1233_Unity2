using System;
using UnityEngine;

/// <summary>
/// A singleton for communicating with the player object when it exists
/// </summary>
public class PlayerMgr : Singleton<PlayerMgr>
{
    [SerializeField] private GameObject _playerPrefab;
    public GameObject PlayerObject { get; private set; }
    public bool HasSpawnedPlayer => PlayerObject != null;

    public void SpawnPlayer(Vector3 position, Quaternion rotation)
    {
        if (PlayerObject)
        {
            Debug.LogError("PlayerMgr: Player already spawned!");
            return;
        }

        PlayerObject = Instantiate(_playerPrefab, position, rotation);
        Debug.Log("PlayerMgr: Player spawned");
    }
    
    /// <summary>
    /// Handles the player using the pause input action
    /// TODO move to player input handler separate from player controller
    /// </summary>
    public void PauseInput()
    {
        // Run pause from game manager
        GameMgr.Instance.PauseGameToggle();
    }
}