using UnityEngine;
using UnityEngine.Rendering;

namespace RusyGameStudio.Tools
{
    /// <summary>
    /// 疑似レイトレーシングによって、リアルな反射を作るためのコントローラーです。
    /// 通常のリフレクションより処理が重い点に注意してください。
    /// This is a controller for creating realistic reflections with pseudo ray tracing.
    /// Note that this is a heavier process than normal reflection process.
    /// </summary>
    public class RealisticReflectionController : MonoBehaviour
    {
        [SerializeField] private Camera _refCamera = default;
        [SerializeField] private float _offsetHeight = 0.1f;
        [SerializeField, Range(1f, 120f)] private float _reflectionFPS = 30f;
        private float _currentTime = 0f;
        private Camera _mainCamera;

        #region Calculate Reflection Transform
        private Matrix4x4 refMatrix = default;
        private Vector3 normal = Vector3.up;
        private Vector3 pos = Vector3.zero;
        private Vector3 cpos = default;
        private Vector3 cnormal = default;
        private Vector4 clipPlane = default;
        #endregion

        void WriteLogMessage(ScriptableRenderContext context, Camera camera) => _mainCamera = camera;

        private void Start()
        {
            _mainCamera = Camera.main;
            RenderPipelineManager.beginCameraRendering += WriteLogMessage;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= WriteLogMessage;
        }

        private void LateUpdate()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > (1f / _reflectionFPS))
            {
                SetReflectionCamera();
                
                GL.invertCulling = true;
                _refCamera.Render();
                GL.invertCulling = false;

                _currentTime = 0f;
            }
        }

        private void SetReflectionCamera()
        {
            pos.y         = _offsetHeight;
            float d       = -Vector3.Dot(normal, pos);

            refMatrix.m00 = 1f - 2f * normal.x * normal.x;
            refMatrix.m01 =    - 2f * normal.x * normal.y;
            refMatrix.m02 =    - 2f * normal.x * normal.z;
            refMatrix.m03 =    - 2f * normal.x * d;
            refMatrix.m10 =    - 2f * normal.x * normal.y;
            refMatrix.m11 = 1f - 2f * normal.y * normal.y;
            refMatrix.m12 =    - 2f * normal.y * normal.z;
            refMatrix.m13 =    - 2f * normal.y * d;
            refMatrix.m20 =    - 2f * normal.x * normal.z;
            refMatrix.m21 =    - 2f * normal.y * normal.z;
            refMatrix.m22 = 1f - 2f * normal.z * normal.z;
            refMatrix.m23 =    - 2f * normal.z * d;
            refMatrix.m30 = 0f;
            refMatrix.m31 = 0f;
            refMatrix.m32 = 0f;
            refMatrix.m33 = 1f;

            _refCamera.worldToCameraMatrix = _mainCamera.worldToCameraMatrix * refMatrix;

            cpos        = _refCamera.worldToCameraMatrix.MultiplyPoint(pos);
            cnormal     = _refCamera.worldToCameraMatrix.MultiplyVector(normal).normalized;
            clipPlane.x = cnormal.x;
            clipPlane.y = cnormal.y;
            clipPlane.z = cnormal.z;
            clipPlane.w = -Vector3.Dot(cpos, cnormal);

            _refCamera.projectionMatrix = _mainCamera.CalculateObliqueMatrix(clipPlane);
        }
    }
}