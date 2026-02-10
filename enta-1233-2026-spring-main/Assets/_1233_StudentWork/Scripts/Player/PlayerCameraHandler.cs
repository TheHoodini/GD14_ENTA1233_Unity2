using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField] private  CinemachineCamera _normalCamera;
    [SerializeField] private CinemachineCamera _zoomedCamera;
    
    public void NormalView()
    {
        _zoomedCamera.gameObject.SetActive(false);
        _normalCamera.gameObject.SetActive(true);
    }
    public void Zoom()
    {
        _normalCamera.gameObject.SetActive(false);
        _zoomedCamera.gameObject.SetActive(true);
    }
}
