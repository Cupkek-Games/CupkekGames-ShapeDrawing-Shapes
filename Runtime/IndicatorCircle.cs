using UnityEngine;
using Shapes;
using Cysharp.Threading.Tasks;
using System.Threading;
using PrimeTween;
using CupkekGames.TimeSystem;

namespace CupkekGames.ShapeDrawing.Shapes
{
    public class IndicatorCircle : Indicator
    {
        [SerializeField] private Disc _background;
        [SerializeField] private Disc _fill;
        [SerializeField] private Disc[] _lines;
        [SerializeField] private ShapeRenderer[] _justColor;
        private float _radius;
        public void Setup(IndicatorSettings settings, Vector3 position, Quaternion rotation, float radius, float angle)
        {
            _settings = settings;
            _radius = radius;

            transform.SetPositionAndRotation(position, rotation);

            _background.Radius = radius;
            _fill.Radius = 0;

            float halfAngle = angle / 2;
            float start = -halfAngle * Mathf.Deg2Rad;
            float end = halfAngle * Mathf.Deg2Rad;

            _background.AngRadiansStart = start;
            _background.AngRadiansEnd = end;

            _fill.AngRadiansStart = start;
            _fill.AngRadiansEnd = end;

            float radiusUnit = radius / _lines.Length;
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i].AngRadiansStart = -halfAngle * Mathf.Deg2Rad;
                _lines[i].AngRadiansEnd = halfAngle * Mathf.Deg2Rad;
                _lines[i].Radius = radiusUnit * (i + 1);
            }
        }

        public void SetColor(Color color)
        {
            _background.ColorInner = new Color(color.r, color.g, color.b, _settings.OpacityBgInner);
            _background.ColorOuter = new Color(color.r, color.g, color.b, _settings.OpacityBgOuter);

            _fill.ColorInner = new Color(color.r, color.g, color.b, _settings.OpacityFillInner);
            _fill.ColorOuter = new Color(color.r, color.g, color.b, _settings.OpacityFillOuter);

            Color stroke = new Color(color.r, color.g, color.b, _settings.OpacityStroke);
            foreach (Disc line in _lines)
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
            _fill.Radius = _radius * progress;
        }
        protected override async UniTask FillTween(float duration, CancellationToken ct, TimeBundle timeBundle)
        {
            Tween tween = Tween.Custom(0, 1, duration: duration, onValueChange: newVal => SetFillProgress(newVal));

            timeBundle.TimeScaleTween.Add(tween);

            await tween.ToYieldInstruction().ToUniTask(cancellationToken: ct);
        }
    }
}