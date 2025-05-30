using UnityEngine;

public static class TransformExtensions
{
    public static void SetLocalProperties(this Transform t, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        t.localPosition = position;
        t.localRotation = rotation;
        t.localScale = scale;
    }
}