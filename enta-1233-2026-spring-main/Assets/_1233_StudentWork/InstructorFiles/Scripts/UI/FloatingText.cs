using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField][TextArea] private string _textToDisplay;

    private void Start()
    {
        _camera = Camera.main;
        _textMeshPro.text = _textToDisplay;
    }

    private void LateUpdate()
    {
        if (_camera == null) return;
        transform.forward = _camera.transform.forward;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_textMeshPro != null)
            _textMeshPro.text = _textToDisplay;
    }
#endif
}