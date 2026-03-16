using UnityEngine;

public class BudAnimationHandler : MonoBehaviour
{
    [SerializeField] private BudBrain _budBrain;

    public void OnDieAnimationEnd()
    {
        _budBrain.ShrinkAndDestroy();
    }
}
