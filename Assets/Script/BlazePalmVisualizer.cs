using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using NatML.Vision;

namespace NatML.Visualizers
{
    [RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
    public sealed class BlazePalmVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Image keypoint;

        [SerializeField]
        private UILineRenderer bones;

        private RawImage rawImage;
        private AspectRatioFitter aspectFitter;
        private readonly List<GameObject> currentHands = new List<GameObject>();

        public Texture2D image
        {
            get => rawImage.texture as Texture2D;
            set
            {
                rawImage.texture = value;
                aspectFitter.aspectRatio = (float)value.width / value.height;
            }
        }

        void Awake()
        {
            rawImage = GetComponent<RawImage>();
            aspectFitter = GetComponent<AspectRatioFitter>();
        }

        /// <summary>
        /// KeyPointの追加
        /// </summary>
        private void AddKeypoint(Vector2 point)
        {
            // KeyPointの生成
            var prefab = Instantiate(keypoint, transform);
            prefab.gameObject.SetActive(true);
            // pointの座標にKeyPointを移動
            var prefabTransform = prefab.transform as RectTransform;
            var imageTransform = rawImage.transform as RectTransform;
         
            prefabTransform.anchorMin = 0.5f * Vector2.one;
            prefabTransform.anchorMax = 0.5f * Vector2.one;
            prefabTransform.pivot = 0.5f * Vector2.one;
            prefabTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, point);

            currentHands.Add(prefab.gameObject);
        }

        //private void AddBones(Vector2 point)
        //{
        //    var bonePrefab = Instantiate(bones, transform);
        //    bonePrefab.gameObject.SetActive(true);
        //    // pointの座標にKeyPointを移動
        //    var bonesPrefabTransform = bonePrefab.transform as RectTransform;
        //    var boneTransform = bones.transform as RectTransform;
            
        //    bonesPrefabTransform.anchoredPosition = Rect.NormalizedToPoint(boneTransform.rect, point);
        //}

        /// <summary>
        /// KeyPointとboneの描画
        /// </summary>
        public void Render(params BlazePalmPredictor.Hand[] hands)
        {
            // 現在のKeyPoint削除
            foreach (var t in currentHands)
            {
                Destroy(t.gameObject);
            }

            currentHands.Clear();

            // 描画
            var segments = new List<Vector2[]>();
            foreach (var hand in hands)
            {
                // Keypoints
                foreach (var p in hand.keypoints)
                {
                    AddKeypoint((Vector2)p);
                }

                // Bones
                segments.AddRange(new List<Vector3[]>
                {
                    new[]
                    {
                        hand.keypoints.wrist, hand.keypoints.thumbCMC, hand.keypoints.thumbMCP, hand.keypoints.thumbIP,
                        hand.keypoints.thumbTip
                    },
                    new[]
                    {
                        hand.keypoints.wrist, hand.keypoints.indexMCP, hand.keypoints.indexPIP, hand.keypoints.indexDIP,
                        hand.keypoints.indexTip
                    },
                    new[]
                    {
                        hand.keypoints.middleMCP, hand.keypoints.middlePIP, hand.keypoints.middleDIP,
                        hand.keypoints.middleTip
                    },
                    new[]
                    {
                        hand.keypoints.ringMCP, hand.keypoints.ringPIP, hand.keypoints.ringDIP, hand.keypoints.ringTip
                    },
                    new[]
                    {
                        hand.keypoints.wrist, hand.keypoints.pinkyMCP, hand.keypoints.pinkyPIP, hand.keypoints.pinkyDIP,
                        hand.keypoints.pinkyTip
                    },
                    new[]
                    {
                        hand.keypoints.indexMCP, hand.keypoints.middleMCP, hand.keypoints.ringMCP,
                        hand.keypoints.pinkyMCP
                    },
                }.Select(points => points.Select(p => GetFixedPoint(p)).ToArray()));
            }

            bones.Points = null;
            bones.Segments = segments;
        }

        private Vector2 GetFixedPoint(Vector2 vec)
        {
            return new Vector2(vec.x * Screen.width - Screen.width / 2, vec.y * Screen.height - Screen.height / 2);
        }
    }
}