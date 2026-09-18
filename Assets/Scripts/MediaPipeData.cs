using System;

[Serializable]
public class MediaPipeData
{
    public string protocolVersion;
    public int sequenceNumber;
    public double timestamp;
    public bool tracking;
    public string controlStatus;
    public string poseStatus;
    public string headHand;
    public string tailHand;
    public float headX;
    public float headY;
    public float tailX;
    public float tailY;
    public float directionX;
    public float directionY;
    public float angle;
    public float shoulderWidth;
    public float handDistance;
    public float sizeRatio;
    public string fishSize; // "SMALL", "MEDIUM", "LARGE"
    public string leftGesture;
    public string rightGesture;
    public float leftHandDetectionConfidence;
    public float rightHandDetectionConfidence;
    public float leftGestureConfidence;
    public float rightGestureConfidence;
}
