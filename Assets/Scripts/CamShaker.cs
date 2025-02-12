using UnityEngine;
using Cinemachine;
public class CamShaker : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
CinemachineBasicMultiChannelPerlin _camNoise;
    float _gainLerpValue = 0;
  [SerializeField]  float _targetGain = 4;
    bool _shaking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        _camNoise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _camNoise.m_AmplitudeGain = 0;
    }

    [ContextMenu("Shake Camera")]
    public void ShakeCam()
    {
        if (!_shaking)
        {
            _shaking = true;
            _gainLerpValue = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (_shaking)
        {
            _gainLerpValue += Time.deltaTime;
            _camNoise.m_AmplitudeGain = Mathf.Lerp(0, _targetGain, _gainLerpValue);
            if (_gainLerpValue >= 1)
            {
                _shaking = false;
                _camNoise.m_AmplitudeGain = 0;
            }
        }
    }
}
