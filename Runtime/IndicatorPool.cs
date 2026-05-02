using UnityEngine;
using CupkekGames.Pool;

namespace CupkekGames.ShapeDrawing.Shapes
{
  public class IndicatorPool : MonoBehaviour, IIndicatorPool
  {
    [SerializeField] private GameObject _arcRegionPrefab;
    private GameObjectPool _arcRegionPool;
    [SerializeField] private GameObject _circleRegionPrefab;
    private GameObjectPool _circleRegionPool;
    [SerializeField] private GameObject _lineIndicatorPrefab;
    private GameObjectPool _lineIndicatorPool;
    [SerializeField] private IndicatorSettings _settings;

    private void Awake()
    {
      _arcRegionPool = new GameObjectPool(_arcRegionPrefab, 2, 4);
      _circleRegionPool = new GameObjectPool(_circleRegionPrefab, 2, 4);
      _lineIndicatorPool = new GameObjectPool(_lineIndicatorPrefab, 2, 4);
    }

    public Indicator ShowArcRegion(Vector3 position, Quaternion rotation, float radius, float angle, Color? color = null)
    {
      GameObject go = _arcRegionPool.Pool.Get();
      IndicatorCircle indicator = go.GetComponent<IndicatorCircle>();

      indicator.Setup(_settings, position, rotation, radius, angle);

      if (color != null)
      {
        indicator.SetColor(color.Value);
      }

      go.SetActive(true);

      return indicator;
    }
    public Indicator ShowCircleRegion(Vector3 position, float radius, Color? color = null)
    {
      return ShowArcRegion(position, Quaternion.identity, radius, 360, color);
    }
    public Indicator ShowLineRegion(Vector3 position, Quaternion rotation, float length, float width, Color? color = null)
    {
      GameObject go = _lineIndicatorPool.Pool.Get();
      IndicatorLine indicator = go.GetComponent<IndicatorLine>();

      indicator.Setup(_settings, position, rotation, length, width);

      if (color != null)
      {
        indicator.SetColor(color.Value);
      }

      go.SetActive(true);

      return indicator;
    }
  }
}