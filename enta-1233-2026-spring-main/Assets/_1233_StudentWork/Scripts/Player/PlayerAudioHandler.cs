using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _footstepSource;
    [SerializeField] private AudioSource _jumpLandingSource;

    public void PlayFootstep()
    {
        _footstepSource?.Play();
    }

    public void PlayJumpLanding()
    {
        _jumpLandingSource?.Play();
    }
}
