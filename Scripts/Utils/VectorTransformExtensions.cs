using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    static class VectorTransformExtensions
    {
        public static IEnumerator<Vector3> LerpToAction(this Transform actor, Vector3 start, Vector3 end, float time,
            Action callback = null)
        {
            for(var t = 0.0f; t<time; t += Time.deltaTime)
            {
                yield return actor.eulerAngles = Vector3.Lerp(start, end, t / time);
            }
            if (callback != null)
            {
                callback();
            }
        }

        public static bool LessThanMax(this Vector3 vec, float maxVal)
        {
            return vec.x < maxVal && vec.y < maxVal && vec.z < maxVal;
        }

        public static bool LessThanMax(this Vector3 vec, Vector3 maxVals)
        {
            return vec.x < maxVals.x && vec.y < maxVals.y && vec.z < maxVals.z;
        }

        public static bool GreaterThan(this Vector3 vec, float maxVal)
        {
            return vec.x > maxVal && vec.y > maxVal && vec.z > maxVal;
        }

        public static Vector3 Abs(this Vector3 vec)
        {
            return new Vector3(Mathf.Abs(vec.x),Mathf.Abs(vec.y), Mathf.Abs(vec.z));
        }

        public static Vector3 Clamp(this Vector3 vec, int clamp)
        {
            return new Vector3(vec.x%clamp, vec.y%clamp,vec.z%clamp);
        }

    }
}
