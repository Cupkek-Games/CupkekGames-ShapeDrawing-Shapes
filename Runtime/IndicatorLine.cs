using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PrimeTween;
using Shapes;
using CupkekGames.TimeSystem;

namespace CupkekGames.ShapeDrawing.Shapes
{
    public class IndicatorLine : Indicator
    {
        [SerializeField] private Rectangle _background;
        [SerializeField] private Rectangle _fill;
        [SerializeField] private Line[] _lines;
        [SerializeField] private ShapeRenderer[] _justColor;
        private float _length;
        public void Setup(IndicatorSettings settings, Vector3 position, Quaternion rotation, float length, float width)
        {
            _settings = settings;
            _length = length;

            transform.SetPositionAndRotation(position, rotation);

            float halfWidth = width / 2f; // offset to center the line

            _background.Thickness = width;
            _background.transform.localPosition = new Vector3(-halfWidth, 0, 0);
            _background.Height = width;
            _background.Width = length;

            _fill.Thickness = width;
            _fill.transform.localPosition = new Vector3(-halfWidth, 0, 0);
            _fill.Height = width;
            _fill.Width = 0;

            float lengthUnit = _length / (_lines.Length - 1);
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i].Start = new Vector3(-halfWidth, 0, 0);
                _lines[i].End = new Vector3(halfWidth, 0, 0);
                _lines[i].transform.localPosition = new Vector3(0, 0, lengthUnit * i);
            }
        }

        public void SetColor(Color color)
        {
            _background.Color = new Color(color.r, color.g, color.b, _settings.OpacityBgInner);

            _fill.Color = new Color(color.r, color.g, color.b, _settings.OpacityFillInner);

            Color stroke = new Color(color.r, color.g, color.b, _settings.OpacityStroke);
            foreach (Line line in _lines)
            {
                line.Color = stroke;
            }
            foreach (ShapeRenderer justColor in _justColor)
            {
                justColor.Color = stroke;
            }
        }

        public override async UniTaskVoid AnimateFill(float duration, CancellationToken ct, TimeBundle timeBundle)
        {
            bool isCanceled = await FillTween(duration, ct, timeBundle).SuppressCancellationThrow();

            if (isCanceled && this != null && gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
        protected override void SetFillProgress(float progress)
        {
            _fill.Width = _length * progress;
        }
        protected override async UniTask FillTween(float duration, CancellationToken ct, TimeBundle timeBundle)
        {
            Tween tween = Tween.Custom(0, 1, duration: duration, onValueChange: newVal => SetFillProgress(newVal));

            timeBundle.TimeScaleTween.Add(tween);

            await tween.ToYieldInstruction().ToUniTask(cancellationToken: ct);
        }
    }
}