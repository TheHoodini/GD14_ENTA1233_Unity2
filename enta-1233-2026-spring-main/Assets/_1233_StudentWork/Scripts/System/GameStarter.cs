using System.Collections;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
   
    void Start()
    {
        StartCoroutine(StartWhenReady());
    }

    private IEnumerator StartWhenReady() 
    {
        Debug.Log("GameStarter: Requesting level load");
        LevelMgr.Instance.LoadCurrentLevel();

        Debug.Log("GameStarter: Waiting for level to load");
        yield return new WaitUntil(() => LevelMgr.Instance.IsLevelLoaded);
        
        Debug.Log("GameStarter: Spawning player");
        PlayerSpawnPoint spawnPoint = PlayerSpawnPoint.Instance;

        if (spawnPoint == null)
        {
            Debug.LogError("GameStarter: No PlayerSpawnPoint found in the level!");
        }
        else
        {
            PlayerMgr.Instance.SpawnPlayer(
                spawnPoint.transform.position,
                spawnPoint.transform.rotation
                );
        }

        Debug.Log("GameStarter: Waiting for player spawn");
        yield return new WaitUntil(() => PlayerMgr.Instance.HasSpawnedPlayer);   

        Debug.Log("GameStarter: Game starting in 3 seconds");
        yield return new WaitForSeconds(3f);
    }
}
