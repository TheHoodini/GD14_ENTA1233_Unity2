using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    public void OnAttackEnd()
    {
        _playerController.OnAttackAnimationEnd();
    }
}
