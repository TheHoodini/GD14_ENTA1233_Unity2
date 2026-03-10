using UnityEngine;

public class LevelCompleteTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameMgr.Instance.NextLevel();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
