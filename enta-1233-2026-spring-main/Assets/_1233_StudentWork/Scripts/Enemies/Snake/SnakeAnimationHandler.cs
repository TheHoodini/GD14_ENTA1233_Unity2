using UnityEngine;

public class SnakeAnimationHandler : MonoBehaviour
{
    [SerializeField] private SnakeBrain _snakeBrain;
    [SerializeField] private SnakeAttackState _snakeAttackState;

    public void OnDieAnimationEnd()
    {
        _snakeBrain.ShrinkAndDestroy();
    }

    public void OnAttackHit()
    {
         _snakeAttackState.ApplyMeleeDamage();
    }
}
