using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace RusyGameStudio.Tools
{
    /// <summary>
    /// 動画から指定した座標の色を取得するためのツールです。
    /// This is a tool to get colors from specified coordinates in a video.
    /// </summary>
    public class VideoColorPickController : MonoBehaviour
    {
        private const string SHADER_PATH = "Assets/RusyGameStudio/Tools/Shader/PollingColorsComputeShader.compute";

        [SerializeField] private Vector2Int[] _readCoords = default;
        [SerializeField] private RenderTexture _sourceTexture = default;
        [SerializeField] private float _colorPickingTimeSpan = 0.1f;
        private ComputeShader _shader = default;
        private float _currentTime = 0f;
        private int _kernel = 0;
        private GraphicsBuffer _colorBuffer = default;
        private GraphicsBuffer _coordBuffer = default;
        private float3[] _colors = default;

        private void Awake()
        {
            _shader = AssetDatabase.LoadAssetAtPath<ComputeShader>(SHADER_PATH);
            
            if (_shader == null) 
            { 
                Debug.LogError("シェーダーが見つかりませんでした。"); 
                this.enabled = false;
                return; 
            }
            if (!_sourceTexture || _readCoords.Length == 0)
            {
                Debug.LogError("リソースをセットしてください。");
                this.enabled = false;
                return;
            }

            VideoColorMaster.ResetColorList(_readCoords.Length);
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > _colorPickingTimeSpan)
            {
                PollingColorsFromComputeShader();
                _currentTime = 0f;
            }
        }

        private void OnEnable()
        {
            int coordLength = _readCoords.Length;

            _colors = new float3[coordLength];
            _colorBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, coordLength, Marshal.SizeOf<float3>());
            _coordBuffer = SetGraphicsBuffer(_readCoords);
            _colorBuffer.SetData(_colors);

            _kernel = _shader.FindKernel("CSMain");
            _shader.SetInt("BufferCount", coordLength);
            _shader.SetTexture(_kernel, "InputTexture", _sourceTexture);
            _shader.SetBuffer(_kernel, "ColorBuffer", _colorBuffer);
            _shader.SetBuffer(_kernel, "CoordBuffer", _coordBuffer);
        }
        private void OnDisable()
        {
            _colorBuffer?.Dispose();
            _coordBuffer?.Dispose();
        }

        private void PollingColorsFromComputeShader()
        {
            _shader.Dispatch(_kernel, 1, 1, 1);
            _colorBuffer.GetData(_colors);

            for (int i = 0; i < _readCoords.Length; i++)
            {
                VideoColorMaster.SetColor(i, new Color(_colors[i].x, _colors[i].y, _colors[i].z));
            }
        }


        private GraphicsBuffer SetGraphicsBuffer(Vector2Int[] coords)
        {
            var array = new NativeArray<float2>(coords.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = new float2(coords[i].x, coords[i].y);
            }
            var buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, array.Length, Marshal.SizeOf<float2>());
            buffer.SetData(array);
            array.Dispose();
            return buffer;
        }
    }
}