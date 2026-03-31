using UnityEngine;

public class SpikeAnimationHandler : MonoBehaviour
{
    [SerializeField] private SpikeBrain _spikeBrain;

    public void OnDieAnimationEnd()
    {
        _spikeBrain.ShrinkAndDestroy();
    }

}
