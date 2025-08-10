// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Mediapipe.Unity.CoordinateSystem;
using UnityEngine;

using mplt = Mediapipe.LocationData.Types;
using mptcc = Mediapipe.Tasks.Components.Containers;

namespace Mediapipe.Unity
{
#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
#pragma warning restore IDE0065

  public class PointAnnotation : HierarchicalAnnotation
  {
    [SerializeField] private Color _color = Color.green;
    [SerializeField] private float _radius = 15.0f;

    private void OnEnable()
    {
      ApplyColor(_color);
      ApplyRadius(_radius);
    }

    private void OnDisable()
    {
      ApplyRadius(0.0f);
    }

    public void SetColor(Color color)
    {
      _color = color;
      ApplyColor(_color);
    }

    public void SetRadius(float radius)
    {
      _radius = radius;
      ApplyRadius(_radius);
    }

    public void Draw(Vector3 position)
    {
      SetActive(true); // Vector3 is not nullable
      transform.localPosition = position;
    }

    public void Draw(Landmark target, Vector3 scale, bool visualizeZ = true)
    {
      if (ActivateFor(target))
      {
        var position = GetScreenRect().GetPoint(target, scale, rotationAngle, isMirrored);
        if (!visualizeZ)
        {
          position.z = 0.0f;
        }
        transform.localPosition = position;
      }
    }

    public void Draw(NormalizedLandmark target, bool visualizeZ = true)
    {
      if (ActivateFor(target))
      {
        var position = GetScreenRect().GetPoint(target, rotationAngle, isMirrored);
        if (!visualizeZ)
        {
          position.z = 0.0f;
        }
        transform.localPosition = position;
      }
    }

    public void Draw(in mptcc.NormalizedLandmark target, bool visualizeZ = true)
    {
      if (ActivateFor(target))
      {
        var position = GetScreenRect().GetPoint(in target, rotationAngle, isMirrored);
        if (!visualizeZ)
        {
          position.z = 0.0f;
        }
        transform.localPosition = position;
      }
    }

    private static float _lastWorldDebugTime = 0f;
    private static int _worldDrawCallCount = 0;
    
    public void Draw(in mptcc.Landmark target, Vector3 scale, bool visualizeZ = true)
    {
      if (ActivateFor(target))
      {
        var position = new Vector3(target.x * scale.x, target.y * scale.y, target.z * scale.z);
        if (!visualizeZ)
        {
          position.z = 0.0f;
        }
        
        // Debug logging for world coordinates
        _worldDrawCallCount++;
        float currentTime = UnityEngine.Time.time;
        bool shouldLog = (currentTime - _lastWorldDebugTime) >= 5f;
        
        if (shouldLog && _worldDrawCallCount <= 33) // Log first 33 points (one pose)
        {
          UnityEngine.Debug.Log($"[PointAnnotation.Draw] Point {_worldDrawCallCount}: Raw=({target.x:F4}, {target.y:F4}, {target.z:F4}) Scale={scale} Final=({position.x:F4}, {position.y:F4}, {position.z:F4}) visualizeZ={visualizeZ}");
        }
        
        if (shouldLog && _worldDrawCallCount >= 33)
        {
          _lastWorldDebugTime = currentTime;
          _worldDrawCallCount = 0;
        }
        
        transform.localPosition = position;
      }
    }

    public void Draw(mplt.RelativeKeypoint target, float threshold = 0.0f)
    {
      if (ActivateFor(target))
      {
        Draw(GetScreenRect().GetPoint(target, rotationAngle, isMirrored));
        SetColor(GetColor(target.Score, threshold));
      }
    }

    public void Draw(mptcc.NormalizedKeypoint target, float threshold = 0.0f)
    {
      if (ActivateFor(target))
      {
        Draw(GetScreenRect().GetPoint(target, rotationAngle, isMirrored));
        SetColor(GetColor(target.score ?? 1.0f, threshold));
      }
    }

    private void ApplyColor(Color color)
    {
      GetComponent<Renderer>().material.color = color;
    }

    private void ApplyRadius(float radius)
    {
      transform.localScale = radius * Vector3.one;
    }

    private Color GetColor(float score, float threshold)
    {
      var t = (score - threshold) / (1 - threshold);
      var h = Mathf.Lerp(90, 0, t) / 360; // from yellow-green to red
      return Color.HSVToRGB(h, 1, 1);
    }
  }
}
