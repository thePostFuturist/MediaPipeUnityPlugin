// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Mediapipe.Unity.ScriptableObjects
{
  /// <summary>
  /// ScriptableObject for storing and exporting pose landmark data.
  /// Allows for runtime recording and playback of pose tracking results.
  /// </summary>
  [CreateAssetMenu(fileName = "PoseLandmarkData", menuName = "MediaPipe/Pose Landmark Data")]
  public class PoseLandmarkData : ScriptableObject
  {
    /// <summary>
    /// Individual landmark data containing position and confidence values.
    /// </summary>
    [Serializable]
    public struct LandmarkData
    {
      public Vector3 position;
      public float visibility;
      public float presence;
      
      public LandmarkData(Vector3 pos, float vis, float pres)
      {
        position = pos;
        visibility = vis;
        presence = pres;
      }
    }
    
    /// <summary>
    /// Complete set of 33 pose landmarks with named access.
    /// </summary>
    [Serializable]
    public struct PoseLandmarkSet
    {
      // Face landmarks (0-10)
      public LandmarkData nose;              // 0
      public LandmarkData leftEyeInner;      // 1
      public LandmarkData leftEye;           // 2
      public LandmarkData leftEyeOuter;      // 3
      public LandmarkData rightEyeInner;     // 4
      public LandmarkData rightEye;          // 5
      public LandmarkData rightEyeOuter;     // 6
      public LandmarkData leftEar;           // 7
      public LandmarkData rightEar;          // 8
      public LandmarkData mouthLeft;         // 9
      public LandmarkData mouthRight;        // 10
      
      // Upper body landmarks (11-22)
      public LandmarkData leftShoulder;      // 11
      public LandmarkData rightShoulder;     // 12
      public LandmarkData leftElbow;         // 13
      public LandmarkData rightElbow;        // 14
      public LandmarkData leftWrist;         // 15
      public LandmarkData rightWrist;        // 16
      public LandmarkData leftPinky;         // 17
      public LandmarkData rightPinky;        // 18
      public LandmarkData leftIndex;         // 19
      public LandmarkData rightIndex;        // 20
      public LandmarkData leftThumb;         // 21
      public LandmarkData rightThumb;        // 22
      
      // Lower body landmarks (23-32)
      public LandmarkData leftHip;           // 23
      public LandmarkData rightHip;          // 24
      public LandmarkData leftKnee;          // 25
      public LandmarkData rightKnee;         // 26
      public LandmarkData leftAnkle;         // 27
      public LandmarkData rightAnkle;        // 28
      public LandmarkData leftHeel;          // 29
      public LandmarkData rightHeel;         // 30
      public LandmarkData leftFootIndex;     // 31
      public LandmarkData rightFootIndex;    // 32
      
      /// <summary>
      /// Gets landmark by index (0-32).
      /// </summary>
      public LandmarkData GetByIndex(int index)
      {
        switch (index)
        {
          case 0: return nose;
          case 1: return leftEyeInner;
          case 2: return leftEye;
          case 3: return leftEyeOuter;
          case 4: return rightEyeInner;
          case 5: return rightEye;
          case 6: return rightEyeOuter;
          case 7: return leftEar;
          case 8: return rightEar;
          case 9: return mouthLeft;
          case 10: return mouthRight;
          case 11: return leftShoulder;
          case 12: return rightShoulder;
          case 13: return leftElbow;
          case 14: return rightElbow;
          case 15: return leftWrist;
          case 16: return rightWrist;
          case 17: return leftPinky;
          case 18: return rightPinky;
          case 19: return leftIndex;
          case 20: return rightIndex;
          case 21: return leftThumb;
          case 22: return rightThumb;
          case 23: return leftHip;
          case 24: return rightHip;
          case 25: return leftKnee;
          case 26: return rightKnee;
          case 27: return leftAnkle;
          case 28: return rightAnkle;
          case 29: return leftHeel;
          case 30: return rightHeel;
          case 31: return leftFootIndex;
          case 32: return rightFootIndex;
          default: return new LandmarkData();
        }
      }
      
      /// <summary>
      /// Sets landmark by index (0-32).
      /// </summary>
      public void SetByIndex(int index, LandmarkData data)
      {
        switch (index)
        {
          case 0: nose = data; break;
          case 1: leftEyeInner = data; break;
          case 2: leftEye = data; break;
          case 3: leftEyeOuter = data; break;
          case 4: rightEyeInner = data; break;
          case 5: rightEye = data; break;
          case 6: rightEyeOuter = data; break;
          case 7: leftEar = data; break;
          case 8: rightEar = data; break;
          case 9: mouthLeft = data; break;
          case 10: mouthRight = data; break;
          case 11: leftShoulder = data; break;
          case 12: rightShoulder = data; break;
          case 13: leftElbow = data; break;
          case 14: rightElbow = data; break;
          case 15: leftWrist = data; break;
          case 16: rightWrist = data; break;
          case 17: leftPinky = data; break;
          case 18: rightPinky = data; break;
          case 19: leftIndex = data; break;
          case 20: rightIndex = data; break;
          case 21: leftThumb = data; break;
          case 22: rightThumb = data; break;
          case 23: leftHip = data; break;
          case 24: rightHip = data; break;
          case 25: leftKnee = data; break;
          case 26: rightKnee = data; break;
          case 27: leftAnkle = data; break;
          case 28: rightAnkle = data; break;
          case 29: leftHeel = data; break;
          case 30: rightHeel = data; break;
          case 31: leftFootIndex = data; break;
          case 32: rightFootIndex = data; break;
        }
      }
      
      /// <summary>
      /// Creates a PoseLandmarkSet from lists of positions, visibilities, and presences.
      /// </summary>
      public static PoseLandmarkSet FromLists(List<Vector3> positions, List<float> visibilities, List<float> presences)
      {
        var set = new PoseLandmarkSet();
        
        for (int i = 0; i < 33 && i < positions.Count; i++)
        {
          var pos = positions[i];
          var vis = i < visibilities.Count ? visibilities[i] : -1f;
          var pres = i < presences.Count ? presences[i] : -1f;
          set.SetByIndex(i, new LandmarkData(pos, vis, pres));
        }
        
        return set;
      }
    }
    
    /// <summary>
    /// Single frame of landmark data.
    /// </summary>
    [Serializable]
    public class LandmarkFrame
    {
      public float timestamp;
      public int frameNumber;
      public bool isUsingWorldCoordinates;
      public PoseLandmarkSet landmarks;
      
      public LandmarkFrame()
      {
        // Don't access Time in constructor - will be set when used
        timestamp = 0f;
        frameNumber = 0;
        landmarks = new PoseLandmarkSet();
      }
      
      public LandmarkFrame(float time, int frame)
      {
        timestamp = time;
        frameNumber = frame;
        landmarks = new PoseLandmarkSet();
      }
    }
    
    [Header("Current Frame Data")]
    [SerializeField] private LandmarkFrame _currentFrame;
    
    [Header("Recording Settings")]
    [SerializeField] private bool _isRecording = false;
    [SerializeField] private int _maxFramesToRecord = 1000;
    
    [Header("Recorded Data")]
    [SerializeField] private List<LandmarkFrame> _recordedFrames = new List<LandmarkFrame>();
    
    // Landmark names for debugging and export
    private static readonly string[] LandmarkNames = new string[] {
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
    
    // Properties
    public LandmarkFrame CurrentFrame
    {
      get
      {
        if (_currentFrame == null)
        {
          _currentFrame = new LandmarkFrame();
        }
        return _currentFrame;
      }
    }
    public bool IsRecording => _isRecording;
    public List<LandmarkFrame> RecordedFrames => _recordedFrames;
    public int RecordedFrameCount => _recordedFrames.Count;
    
    private void OnEnable()
    {
      // Initialize current frame if needed
      if (_currentFrame == null)
      {
        _currentFrame = new LandmarkFrame();
      }
    }
    
    /// <summary>
    /// Updates the current frame with new landmark data.
    /// </summary>
    public void UpdateCurrentFrame(List<Vector3> positions, List<float> visibilities, List<float> presences, bool isWorldCoords)
    {
      _currentFrame = new LandmarkFrame(Time.time, Time.frameCount)
      {
        isUsingWorldCoordinates = isWorldCoords,
        landmarks = PoseLandmarkSet.FromLists(positions, visibilities, presences)
      };
      
      // If recording, add to recorded frames
      if (_isRecording && _recordedFrames.Count < _maxFramesToRecord)
      {
        _recordedFrames.Add(_currentFrame);
      }
    }
    
    /// <summary>
    /// Starts recording landmark frames.
    /// </summary>
    public void StartRecording(bool clearExisting = true)
    {
      _isRecording = true;
      if (clearExisting)
      {
        _recordedFrames.Clear();
      }
      Debug.Log($"[PoseLandmarkData] Started recording. Max frames: {_maxFramesToRecord}");
    }
    
    /// <summary>
    /// Stops recording landmark frames.
    /// </summary>
    public void StopRecording()
    {
      _isRecording = false;
      Debug.Log($"[PoseLandmarkData] Stopped recording. Total frames: {_recordedFrames.Count}");
    }
    
    /// <summary>
    /// Clears all recorded frames.
    /// </summary>
    public void ClearRecording()
    {
      _recordedFrames.Clear();
      Debug.Log("[PoseLandmarkData] Cleared all recorded frames");
    }
    
    /// <summary>
    /// Gets a specific recorded frame by index.
    /// </summary>
    public LandmarkFrame GetFrame(int index)
    {
      if (index >= 0 && index < _recordedFrames.Count)
      {
        return _recordedFrames[index];
      }
      return null;
    }
    
    /// <summary>
    /// Exports recorded data to CSV format.
    /// </summary>
    public void ExportToCSV(string filePath)
    {
      if (_recordedFrames.Count == 0)
      {
        Debug.LogWarning("[PoseLandmarkData] No frames to export");
        return;
      }
      
      StringBuilder csv = new StringBuilder();
      
      // Header
      csv.AppendLine("Frame,Timestamp,CoordType,LandmarkIndex,LandmarkName,X,Y,Z,Visibility,Presence");
      
      // Data rows
      for (int frameIdx = 0; frameIdx < _recordedFrames.Count; frameIdx++)
      {
        var frame = _recordedFrames[frameIdx];
        string coordType = frame.isUsingWorldCoordinates ? "World" : "Normalized";
        
        for (int landmarkIdx = 0; landmarkIdx < 33; landmarkIdx++)
        {
          var landmark = frame.landmarks.GetByIndex(landmarkIdx);
          string name = landmarkIdx < LandmarkNames.Length ? LandmarkNames[landmarkIdx] : $"Point_{landmarkIdx}";
          
          csv.AppendLine($"{frame.frameNumber},{frame.timestamp:F3},{coordType},{landmarkIdx},{name},{landmark.position.x:F6},{landmark.position.y:F6},{landmark.position.z:F6},{landmark.visibility:F3},{landmark.presence:F3}");
        }
      }
      
      try
      {
        File.WriteAllText(filePath, csv.ToString());
        Debug.Log($"[PoseLandmarkData] Exported {_recordedFrames.Count} frames to: {filePath}");
      }
      catch (Exception e)
      {
        Debug.LogError($"[PoseLandmarkData] Failed to export CSV: {e.Message}");
      }
    }
    
    /// <summary>
    /// Exports only the current frame to CSV.
    /// </summary>
    public void ExportCurrentFrameToCSV(string filePath)
    {
      StringBuilder csv = new StringBuilder();
      
      // Header
      csv.AppendLine("LandmarkIndex,Name,X,Y,Z,Visibility,Presence");
      
      // Data rows
      for (int i = 0; i < 33; i++)
      {
        var landmark = _currentFrame.landmarks.GetByIndex(i);
        string name = i < LandmarkNames.Length ? LandmarkNames[i] : $"Point_{i}";
        
        csv.AppendLine($"{i},{name},{landmark.position.x:F6},{landmark.position.y:F6},{landmark.position.z:F6},{landmark.visibility:F3},{landmark.presence:F3}");
      }
      
      try
      {
        File.WriteAllText(filePath, csv.ToString());
        Debug.Log($"[PoseLandmarkData] Exported current frame to: {filePath}");
      }
      catch (Exception e)
      {
        Debug.LogError($"[PoseLandmarkData] Failed to export current frame: {e.Message}");
      }
    }
    
    /// <summary>
    /// Quick access methods for common landmark groups.
    /// </summary>
    public Vector3 GetCenterOfMass()
    {
      var frame = CurrentFrame;
      var hips = (frame.landmarks.leftHip.position + frame.landmarks.rightHip.position) * 0.5f;
      var shoulders = (frame.landmarks.leftShoulder.position + frame.landmarks.rightShoulder.position) * 0.5f;
      return (hips + shoulders) * 0.5f;
    }
    
    public float GetShoulderWidth()
    {
      var frame = CurrentFrame;
      return Vector3.Distance(frame.landmarks.leftShoulder.position, frame.landmarks.rightShoulder.position);
    }
    
    public float GetHipWidth()
    {
      var frame = CurrentFrame;
      return Vector3.Distance(frame.landmarks.leftHip.position, frame.landmarks.rightHip.position);
    }
    
    public float GetHeight()
    {
      var frame = CurrentFrame;
      // Approximate height from nose to feet
      var nose = frame.landmarks.nose.position.y;
      var leftFoot = frame.landmarks.leftAnkle.position.y;
      var rightFoot = frame.landmarks.rightAnkle.position.y;
      var lowestFoot = Mathf.Min(leftFoot, rightFoot);
      return nose - lowestFoot;
    }
  }
}