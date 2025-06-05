using UnityEngine;

namespace RusyGameStudio.Tools
{
    /// <summary>
    /// 
    /// リフレクションプローブの更新頻度を制御するツールです。
    /// リフレクションプローブをリアルタイム描画する必要があり、その上で最適化したい場合に有効です。
    /// リフレクションプローブがセットされているオブジェクトに一緒にセットしてご利用ください。
    /// 
    /// This is a tool to control the frequency of reflection probe updates.
    /// It is useful when the reflection probe needs to be rendered in realtime and you want to optimize them.
    /// Set this together on the object which set the reflection probe. 
    /// 
    /// </summary>
    public class ReflectionProbeController : MonoBehaviour
    {
        [SerializeField, Range(1f, 120f)] private float _refreshFPS = 30f;
        private ReflectionProbe _probe = default;
        private float _currentTime = 0f;

        private void Start()
        {
            _probe = GetComponent<ReflectionProbe>();
            if (_probe == null)
            {
                this.enabled = false;
                return;
            }
            else
            {
                _probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
            }
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > (1f / _refreshFPS))
            {
                _probe.RenderProbe();
                _currentTime = 0f;
            }
        }
    }
}