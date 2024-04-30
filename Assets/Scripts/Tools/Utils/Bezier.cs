using UnityEngine;

namespace Common
{
    public static class Bezier
    {
        // P = (1−t)2P1 + 2(1−t)tP2 + t2P3
        public static Vector3 GetPosition(Vector3 start, Vector3 inflection, Vector3 end, float t)
        {
            return ((1-t) * (1-t)) * start 
                   + (2 * (1 - t) * t) * inflection 
                   + (t*t) * end;
        }
    }
}