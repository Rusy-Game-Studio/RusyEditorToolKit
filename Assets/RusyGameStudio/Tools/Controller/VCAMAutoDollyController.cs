using Cinemachine;
using UnityEngine;

namespace RusyGameStudio.Tools
{
    public class VCAMAutoDollyController : MonoBehaviour
    {
        [SerializeField] private float _timeScale = 1.0f;
        private CinemachineTrackedDolly _vcamSettings = default;
        private float _position = 0f;


        private void Start()
        {
            _vcamSettings = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
            if (_vcamSettings == null)
            {
                Debug.LogError("Please set references");
                enabled = false;
                return;
            }

            _vcamSettings.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
        }

        private void Update()
        {
            _position += Time.deltaTime * _timeScale;
            _vcamSettings.m_PathPosition = _position;
            if (_position > 1f) _position = 0f;
        }
    }
}