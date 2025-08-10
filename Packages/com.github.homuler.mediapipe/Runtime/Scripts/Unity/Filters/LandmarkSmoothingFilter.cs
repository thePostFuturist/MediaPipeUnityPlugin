// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
using UnityEngine;
using mptcc = Mediapipe.Tasks.Components.Containers;

namespace Mediapipe.Unity.Filters
{
  /// <summary>
  /// Exponential Moving Average (EMA) filter for smoothing landmark positions
  /// to reduce jitter in pose tracking.
  /// </summary>
  public class LandmarkSmoothingFilter
  {
    private float _smoothingFactor;
    private List<mptcc.Landmark> _previousLandmarks;
    private List<mptcc.NormalizedLandmark> _previousNormalizedLandmarks;
    private bool _hasInitialized = false;

    /// <summary>
    /// Creates a new smoothing filter.
    /// </summary>
    /// <param name="smoothingFactor">Smoothing factor between 0 and 1. 
    /// 0 = maximum smoothing (no update), 1 = no smoothing (instant update)</param>
    public LandmarkSmoothingFilter(float smoothingFactor = 0.3f)
    {
      _smoothingFactor = Mathf.Clamp01(smoothingFactor);
    }

    /// <summary>
    /// Updates the smoothing factor.
    /// </summary>
    public void SetSmoothingFactor(float factor)
    {
      _smoothingFactor = Mathf.Clamp01(factor);
    }

    /// <summary>
    /// Resets the filter, clearing previous landmark data.
    /// </summary>
    public void Reset()
    {
      _previousLandmarks = null;
      _previousNormalizedLandmarks = null;
      _hasInitialized = false;
    }

    /// <summary>
    /// Applies smoothing to world landmarks.
    /// </summary>
    public List<mptcc.Landmark> SmoothWorldLandmarks(IReadOnlyList<mptcc.Landmark> currentLandmarks)
    {
      if (currentLandmarks == null || currentLandmarks.Count == 0)
      {
        return null;
      }

      // First frame - no smoothing possible
      if (!_hasInitialized || _previousLandmarks == null || _previousLandmarks.Count != currentLandmarks.Count)
      {
        _previousLandmarks = new List<mptcc.Landmark>(currentLandmarks);
        _hasInitialized = true;
        return new List<mptcc.Landmark>(currentLandmarks);
      }

      // Apply EMA filter: smoothed = alpha * current + (1 - alpha) * previous
      var smoothedLandmarks = new List<mptcc.Landmark>(currentLandmarks.Count);
      float oneMinusAlpha = 1f - _smoothingFactor;

      for (int i = 0; i < currentLandmarks.Count; i++)
      {
        var current = currentLandmarks[i];
        var previous = _previousLandmarks[i];

        if (current == null || previous == null)
        {
          smoothedLandmarks.Add(current);
          _previousLandmarks[i] = current;
          continue;
        }

        // Smooth position
        float smoothedX = _smoothingFactor * current.x + oneMinusAlpha * previous.x;
        float smoothedY = _smoothingFactor * current.y + oneMinusAlpha * previous.y;
        float smoothedZ = _smoothingFactor * current.z + oneMinusAlpha * previous.z;

        // Smooth visibility and presence if available
        float? smoothedVisibility = null;
        float? smoothedPresence = null;

        if (current.visibility.HasValue && previous.visibility.HasValue)
        {
          smoothedVisibility = _smoothingFactor * current.visibility.Value + oneMinusAlpha * previous.visibility.Value;
        }
        else if (current.visibility.HasValue)
        {
          smoothedVisibility = current.visibility;
        }

        if (current.presence.HasValue && previous.presence.HasValue)
        {
          smoothedPresence = _smoothingFactor * current.presence.Value + oneMinusAlpha * previous.presence.Value;
        }
        else if (current.presence.HasValue)
        {
          smoothedPresence = current.presence;
        }

        var smoothedLandmark = new mptcc.Landmark(
          smoothedX, smoothedY, smoothedZ,
          smoothedVisibility, smoothedPresence,
          current.name
        );

        smoothedLandmarks.Add(smoothedLandmark);
        _previousLandmarks[i] = smoothedLandmark;
      }

      return smoothedLandmarks;
    }

    /// <summary>
    /// Applies smoothing to normalized landmarks.
    /// </summary>
    public List<mptcc.NormalizedLandmark> SmoothNormalizedLandmarks(IReadOnlyList<mptcc.NormalizedLandmark> currentLandmarks)
    {
      if (currentLandmarks == null || currentLandmarks.Count == 0)
      {
        return null;
      }

      // First frame - no smoothing possible
      if (!_hasInitialized || _previousNormalizedLandmarks == null || _previousNormalizedLandmarks.Count != currentLandmarks.Count)
      {
        _previousNormalizedLandmarks = new List<mptcc.NormalizedLandmark>(currentLandmarks);
        _hasInitialized = true;
        return new List<mptcc.NormalizedLandmark>(currentLandmarks);
      }

      // Apply EMA filter
      var smoothedLandmarks = new List<mptcc.NormalizedLandmark>(currentLandmarks.Count);
      float oneMinusAlpha = 1f - _smoothingFactor;

      for (int i = 0; i < currentLandmarks.Count; i++)
      {
        var current = currentLandmarks[i];
        var previous = _previousNormalizedLandmarks[i];

        // Smooth position
        float smoothedX = _smoothingFactor * current.x + oneMinusAlpha * previous.x;
        float smoothedY = _smoothingFactor * current.y + oneMinusAlpha * previous.y;
        float smoothedZ = _smoothingFactor * current.z + oneMinusAlpha * previous.z;

        // Smooth visibility and presence if available
        float? smoothedVisibility = null;
        float? smoothedPresence = null;

        if (current.visibility.HasValue && previous.visibility.HasValue)
        {
          smoothedVisibility = _smoothingFactor * current.visibility.Value + oneMinusAlpha * previous.visibility.Value;
        }
        else if (current.visibility.HasValue)
        {
          smoothedVisibility = current.visibility;
        }

        if (current.presence.HasValue && previous.presence.HasValue)
        {
          smoothedPresence = _smoothingFactor * current.presence.Value + oneMinusAlpha * previous.presence.Value;
        }
        else if (current.presence.HasValue)
        {
          smoothedPresence = current.presence;
        }

        var smoothedLandmark = new mptcc.NormalizedLandmark(
          smoothedX, smoothedY, smoothedZ,
          smoothedVisibility, smoothedPresence,
          current.name
        );

        smoothedLandmarks.Add(smoothedLandmark);
        _previousNormalizedLandmarks[i] = smoothedLandmark;
      }

      return smoothedLandmarks;
    }

    /// <summary>
    /// Applies smoothing to a collection of world landmarks (multiple poses).
    /// </summary>
    public List<mptcc.Landmarks> SmoothMultipleWorldLandmarks(IReadOnlyList<mptcc.Landmarks> currentPoses)
    {
      if (currentPoses == null || currentPoses.Count == 0)
      {
        return null;
      }

      var smoothedPoses = new List<mptcc.Landmarks>(currentPoses.Count);

      for (int i = 0; i < currentPoses.Count; i++)
      {
        var pose = currentPoses[i];
        if (pose.landmarks != null)
        {
          var smoothedLandmarks = SmoothWorldLandmarks(pose.landmarks);
          smoothedPoses.Add(new mptcc.Landmarks(smoothedLandmarks));
        }
        else
        {
          smoothedPoses.Add(pose);
        }
      }

      return smoothedPoses;
    }
  }
}