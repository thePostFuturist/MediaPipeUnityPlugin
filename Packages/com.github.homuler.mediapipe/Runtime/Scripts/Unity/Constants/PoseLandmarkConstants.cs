// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
using UnityEngine;
using mptcc = Mediapipe.Tasks.Components.Containers;

namespace Mediapipe.Unity.Constants
{
  /// <summary>
  /// Contains reference poses and constants for pose landmark system.
  /// </summary>
  public static class PoseLandmarkConstants
  {
    /// <summary>
    /// A-Pose reference positions in world coordinates (meters).
    /// Arms horizontal at shoulder level, legs straight.
    /// </summary>
    public static readonly List<Vector3> APoseWorldPositions = new List<Vector3>
    {
      // 0: Nose
      new Vector3(0f, 1.6f, 0f),
      // 1-3: Left eye (inner, center, outer)
      new Vector3(0.03f, 1.65f, -0.05f),
      new Vector3(0.04f, 1.65f, -0.05f),
      new Vector3(0.05f, 1.65f, -0.05f),
      // 4-6: Right eye (inner, center, outer)
      new Vector3(-0.03f, 1.65f, -0.05f),
      new Vector3(-0.04f, 1.65f, -0.05f),
      new Vector3(-0.05f, 1.65f, -0.05f),
      // 7: Left ear
      new Vector3(0.08f, 1.62f, -0.03f),
      // 8: Right ear
      new Vector3(-0.08f, 1.62f, -0.03f),
      // 9: Mouth left
      new Vector3(0.02f, 1.55f, -0.03f),
      // 10: Mouth right
      new Vector3(-0.02f, 1.55f, -0.03f),
      // 11: Left shoulder
      new Vector3(0.15f, 1.4f, 0f),
      // 12: Right shoulder
      new Vector3(-0.15f, 1.4f, 0f),
      // 13: Left elbow (arm extended horizontally)
      new Vector3(0.4f, 1.4f, 0f),
      // 14: Right elbow (arm extended horizontally)
      new Vector3(-0.4f, 1.4f, 0f),
      // 15: Left wrist
      new Vector3(0.65f, 1.4f, 0f),
      // 16: Right wrist
      new Vector3(-0.65f, 1.4f, 0f),
      // 17: Left pinky
      new Vector3(0.7f, 1.38f, 0f),
      // 18: Right pinky
      new Vector3(-0.7f, 1.38f, 0f),
      // 19: Left index
      new Vector3(0.7f, 1.42f, 0f),
      // 20: Right index
      new Vector3(-0.7f, 1.42f, 0f),
      // 21: Left thumb
      new Vector3(0.68f, 1.43f, 0.02f),
      // 22: Right thumb
      new Vector3(-0.68f, 1.43f, 0.02f),
      // 23: Left hip
      new Vector3(0.1f, 1.0f, 0f),
      // 24: Right hip
      new Vector3(-0.1f, 1.0f, 0f),
      // 25: Left knee
      new Vector3(0.1f, 0.5f, 0f),
      // 26: Right knee
      new Vector3(-0.1f, 0.5f, 0f),
      // 27: Left ankle
      new Vector3(0.1f, 0.1f, 0f),
      // 28: Right ankle
      new Vector3(-0.1f, 0.1f, 0f),
      // 29: Left heel
      new Vector3(0.1f, 0f, -0.05f),
      // 30: Right heel
      new Vector3(-0.1f, 0f, -0.05f),
      // 31: Left foot index
      new Vector3(0.1f, 0f, 0.1f),
      // 32: Right foot index
      new Vector3(-0.1f, 0f, 0.1f)
    };
    
    /// <summary>
    /// Creates A-Pose landmarks with specified visibility and presence.
    /// </summary>
    public static mptcc.Landmarks CreateAPoseLandmarks(float visibility = 1f, float presence = 1f)
    {
      var landmarks = new List<mptcc.Landmark>();
      
      foreach (var position in APoseWorldPositions)
      {
        landmarks.Add(new mptcc.Landmark(
          position.x,
          position.y,
          position.z,
          visibility,
          presence,
          null // name
        ));
      }
      
      return new mptcc.Landmarks(landmarks);
    }
    
    /// <summary>
    /// Blends between detected landmarks and A-Pose based on confidence.
    /// </summary>
    public static mptcc.Landmarks BlendWithAPose(mptcc.Landmarks detectedLandmarks, float blendFactor, float confidenceThreshold = 0.3f)
    {
      if (detectedLandmarks.landmarks == null)
      {
        return CreateAPoseLandmarks();
      }
      
      var blendedLandmarks = new List<mptcc.Landmark>();
      var aPose = CreateAPoseLandmarks();
      
      for (int i = 0; i < detectedLandmarks.landmarks.Count && i < aPose.landmarks.Count; i++)
      {
        var detected = detectedLandmarks.landmarks[i];
        var reference = aPose.landmarks[i];
        
        if (detected == null)
        {
          blendedLandmarks.Add(reference);
          continue;
        }
        
        // Check confidence (using visibility as confidence metric)
        float confidence = detected.visibility ?? 0f;
        float actualBlend = confidence < confidenceThreshold ? blendFactor : 0f;
        
        // Blend positions
        float blendedX = Mathf.Lerp(detected.x, reference.x, actualBlend);
        float blendedY = Mathf.Lerp(detected.y, reference.y, actualBlend);
        float blendedZ = Mathf.Lerp(detected.z, reference.z, actualBlend);
        
        blendedLandmarks.Add(new mptcc.Landmark(
          blendedX,
          blendedY,
          blendedZ,
          detected.visibility,
          detected.presence,
          detected.name
        ));
      }
      
      return new mptcc.Landmarks(blendedLandmarks);
    }
    
    /// <summary>
    /// Calculates average confidence for a set of landmarks.
    /// </summary>
    public static float CalculateAverageConfidence(mptcc.Landmarks landmarks)
    {
      if (landmarks.landmarks == null || landmarks.landmarks.Count == 0)
      {
        return 0f;
      }
      
      float totalConfidence = 0f;
      int validCount = 0;
      
      foreach (var landmark in landmarks.landmarks)
      {
        if (landmark != null && landmark.visibility.HasValue)
        {
          totalConfidence += landmark.visibility.Value;
          validCount++;
        }
      }
      
      return validCount > 0 ? totalConfidence / validCount : 0f;
    }
  }
}