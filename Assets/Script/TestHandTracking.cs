using UnityEngine;
using NatML.Devices;
using NatML.Devices.Outputs;
using NatML.Vision;
using NatML.Visualizers;

namespace NatML
{
    [MLModelDataEmbed("@natml/blazepalm-detector"), MLModelDataEmbed("@natml/blazepalm-landmark")]
    public class TestHandTracking : MonoBehaviour
    {
        /// <summary>
        /// Visualizer
        /// </summary>
        [Header(@"UI"), SerializeField]
        private BlazePalmVisualizer visualizer;

        /// <summary>
        /// Pipeline
        /// </summary>
        private BlazePalmPipeline pipeline;

        /// <summary>
        /// CameraDevice
        /// </summary>
        private CameraDevice cameraDevice;

        /// <summary>
        /// CameraDeviceで写したものをTextureに変換したもの
        /// </summary>
        private TextureOutput previewTextureOutput;

        private void OnDisable() => pipeline?.Dispose();

        [System.Obsolete]
        private async void Start()
        {
            // カメラが許可されているか
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();

            // 許可されていない場合はreturn
            if (permissionStatus != PermissionStatus.Authorized) return;

            // カメラ取得
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;

            // カメラで写したものをTextureに変換
            previewTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(previewTextureOutput);
            var previewTexture = await previewTextureOutput;
            visualizer.image = previewTexture;

            // BlazePalm detectorとpredictorをNatML Hubから取得しPipelineの作成
            var detectorModelData = await MLModelData.FromHub("@natml/blazepalm-detector");
            var predictorModelData = await MLModelData.FromHub("@natml/blazepalm-landmark");
            pipeline = new BlazePalmPipeline(detectorModelData, predictorModelData);
        }

        private void Update()
        {
            // Pipelineが作成されているか
            if (pipeline == null) return;

            // カメラで写している情報から手を予測
            var hands = pipeline.Predict(previewTextureOutput.texture);

            // 予測した情報を描画
            visualizer.Render(hands);
        }
    }
}