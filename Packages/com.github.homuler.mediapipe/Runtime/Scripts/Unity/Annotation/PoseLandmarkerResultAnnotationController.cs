// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using UnityEngine;

namespace Mediapipe.Unity
{
  public class PoseLandmarkerResultAnnotationController : AnnotationController<MultiPoseLandmarkListWithMaskAnnotation>
  {
    [SerializeField] private bool _visualizeZ = false;
    [SerializeField] private bool _useWorldCoordinates = false;

    private readonly object _currentTargetLock = new object();
    private PoseLandmarkerResult _currentTarget;
    
    // Debug logging variables
    private float _lastDebugLogTime = 0f;
    private const float DEBUG_LOG_INTERVAL = 5f; // Log every 5 seconds

    public void InitScreen(int maskWidth, int maskHeight) => annotation.InitMask(maskWidth, maskHeight);

    public void SetUseWorldCoordinates(bool useWorldCoordinates) => _useWorldCoordinates = useWorldCoordinates;

    public void DrawNow(PoseLandmarkerResult target)
    {
      target.CloneTo(ref _currentTarget);
      if (_currentTarget.segmentationMasks != null)
      {
        ReadMask(_currentTarget.segmentationMasks);
        // NOTE: segmentationMasks can still be accessed from newTarget.
        _currentTarget.segmentationMasks.Clear();
      }
      SyncNow();
    }

    public void DrawLater(PoseLandmarkerResult target) => UpdateCurrentTarget(target);

    private void ReadMask(IReadOnlyList<Image> segmentationMasks) => annotation.ReadMask(segmentationMasks, isMirrored);

    protected void UpdateCurrentTarget(PoseLandmarkerResult newTarget)
    {
      lock (_currentTargetLock)
      {
        newTarget.CloneTo(ref _currentTarget);
        if (_currentTarget.segmentationMasks != null)
        {
          ReadMask(_currentTarget.segmentationMasks);
          // NOTE: segmentationMasks can still be accessed from newTarget.
          _currentTarget.segmentationMasks.Clear();
        }
        isStale = true;
      }
    }

    protected override void SyncNow()
    {
      lock (_currentTargetLock)
      {
        isStale = false;
        if (_useWorldCoordinates && _currentTarget.poseWorldLandmarks != null)
        {
          // Always use visualizeZ=true for world coordinates to preserve 3D positions
          annotation.DrawWorldLandmarks(_currentTarget.poseWorldLandmarks, true);
          LogWorldLandmarkPositions();
        }
        else
        {
          annotation.Draw(_currentTarget.poseLandmarks, _visualizeZ);
        }
      }
    }
    
    private void LogWorldLandmarkPositions()
    {
      // Check if enough time has passed since last log
      float currentTime = Time.time;
      if (currentTime - _lastDebugLogTime < DEBUG_LOG_INTERVAL)
      {
        return;
      }
      
      _lastDebugLogTime = currentTime;
      
      if (_currentTarget.poseWorldLandmarks == null || _currentTarget.poseWorldLandmarks.Count == 0)
      {
        Debug.Log("[World Landmarks Debug] No world landmarks available");
        return;
      }
      
      Debug.Log($"[World Landmarks Debug] ===== LOGGING WORLD POSITIONS AT TIME {currentTime:F2} =====");
      Debug.Log($"[World Landmarks Debug] Total poses detected: {_currentTarget.poseWorldLandmarks.Count}");
      
      // Landmark names for better readability
      string[] landmarkNames = new string[] {
        "Nose", "Left Eye Inner", "Left Eye", "Left Eye Outer",
        "Right Eye Inner", "Right Eye", "Right Eye Outer",
        "Left Ear", "Right Ear", "Mouth Left", "Mouth Right",
        "Left Shoulder", "Right Shoulder", "Left Elbow", "Right Elbow",
        "Left Wrist", "Right Wrist", "Left Pinky", "Right Pinky",
        "Left Index", "Right Index", "Left Thumb", "Right Thumb",
        "Left Hip", "Right Hip", "Left Knee", "Right Knee",
        "Left Ankle", "Right Ankle", "Left Heel", "Right Heel",
        "Left Foot Index", "Right Foot Index"
      };
      
      for (int poseIdx = 0; poseIdx < _currentTarget.poseWorldLandmarks.Count; poseIdx++)
      {
        var worldLandmarks = _currentTarget.poseWorldLandmarks[poseIdx];
        if (worldLandmarks.landmarks == null)
        {
          Debug.Log($"[World Landmarks Debug] Pose {poseIdx}: NULL landmarks");
          continue;
        }
        
        Debug.Log($"[World Landmarks Debug] ---- Pose {poseIdx} ---- ({worldLandmarks.landmarks.Count} landmarks)");
        
        for (int i = 0; i < worldLandmarks.landmarks.Count; i++)
        {
          var landmark = worldLandmarks.landmarks[i];
          string name = i < landmarkNames.Length ? landmarkNames[i] : $"Point_{i}";
          
          if (landmark == null)
          {
            Debug.Log($"[World Landmarks Debug]   [{i:D2}] {name,-20}: NULL");
          }
          else
          {
            Debug.Log($"[World Landmarks Debug]   [{i:D2}] {name,-20}: X={landmark.x,8:F4}, Y={landmark.y,8:F4}, Z={landmark.z,8:F4}, Visibility={landmark.visibility ?? -1f:F3}, Presence={landmark.presence ?? -1f:F3}");
          }
        }
        
        // Calculate some statistics
        if (worldLandmarks.landmarks.Count > 0)
        {
          float minX = float.MaxValue, maxX = float.MinValue;
          float minY = float.MaxValue, maxY = float.MinValue;
          float minZ = float.MaxValue, maxZ = float.MinValue;
          
          foreach (var landmark in worldLandmarks.landmarks)
          {
            if (landmark != null)
            {
              minX = Mathf.Min(minX, landmark.x);
              maxX = Mathf.Max(maxX, landmark.x);
              minY = Mathf.Min(minY, landmark.y);
              maxY = Mathf.Max(maxY, landmark.y);
              minZ = Mathf.Min(minZ, landmark.z);
              maxZ = Mathf.Max(maxZ, landmark.z);
            }
          }
          
          Debug.Log($"[World Landmarks Debug] Pose {poseIdx} Bounds:");
          Debug.Log($"[World Landmarks Debug]   X range: [{minX:F4}, {maxX:F4}] (spread: {maxX - minX:F4})");
          Debug.Log($"[World Landmarks Debug]   Y range: [{minY:F4}, {maxY:F4}] (spread: {maxY - minY:F4})");
          Debug.Log($"[World Landmarks Debug]   Z range: [{minZ:F4}, {maxZ:F4}] (spread: {maxZ - minZ:F4})");
        }
      }
      
      Debug.Log($"[World Landmarks Debug] ===== END OF LOG =====");
    }
  }
}
