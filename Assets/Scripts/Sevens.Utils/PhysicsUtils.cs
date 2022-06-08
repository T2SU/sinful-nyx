using Sevens.Entities;
using UnityEngine;

namespace Sevens.Utils
{
    public static class PhysicsUtils
    {
        private static int _groundLayerMask;
        private static int _groundLayer;
        private static ContactFilter2D? _filter;

        private static int _instaDeathLayer;

        public static int GroundLayerMask
        {
            get
            {
                if (_groundLayerMask == 0)
                    _groundLayerMask = LayerMask.GetMask("Ground");
                return _groundLayerMask;
            }
        }

        public static int GroundLayer
        {
            get
            {
                if (_groundLayer == 0)
                    _groundLayer = LayerMask.NameToLayer("Ground");
                return _groundLayer;
            }
        }

        public static int InstaDeath
        {
            get
            {
                if (_instaDeathLayer == 0)
                    _instaDeathLayer = LayerMask.NameToLayer("InstaDeath");
                return _instaDeathLayer;
            }
        }

        public static ContactFilter2D GroundContactFilter
        {
            get
            {
                if (_filter == null)
                    _filter = new ContactFilter2D() { useLayerMask = true, layerMask = GroundLayerMask };
                return _filter.Value;
            }
        }

        public static float GetXByDirection(float x, bool facingLeft)
        {
            if (facingLeft)
                return -x;
            return x;
        }

        public static Bounds ArrangeZ(Bounds bounds)
        {
            var center = bounds.center;
            var size = bounds.size;
            center.z = 0;
            size.z = 1;
            return new Bounds(center, size);
        }

        public static Vector3 CheckBound(LivingEntity entity, Vector2 move, bool left)
        {
            float distanceAgainstWall = 1f;

            var vec = new Vector2(move.x * (left ? -1 : 1), move.y);
            var tf = entity.transform;
            var origin = tf.position;
            origin.y += 0.005f;
            var distance = entity.EntitySizeX + Mathf.Abs(vec.x);
            var maxforward = entity.EntitySizeX + distanceAgainstWall;
            var vectorX = distance * (left ? -1 : 1);
            var maxVectorX = maxforward * (left ? -1 : 1);
            var hit = Physics2D.Raycast(origin, vec.normalized, distance, GroundLayerMask);
            var dest = new Vector3(vectorX + tf.position.x, vec.y + tf.position.y, tf.position.z);
            if (hit)
                dest.x = hit.point.x - maxVectorX;
            //dest.y -= 0.005f;
            return dest;
        }

        public static bool IsFacingLeft(this Transform tf)
        {
            var s = tf.localScale;
            return s.x < 0;
        }

        public static void SetFacingLeft(this Transform tf, bool left, Transform _newPivot = null)
        {
            var s = tf.localScale;
            s.x = (left ? -1 : 1) * Mathf.Abs(s.x);

            if (_newPivot != null && tf.localScale != s)
            {
                var gap = tf.position - _newPivot.position;
                tf.position = new Vector2(_newPivot.position.x - Mathf.Abs(gap.x) * (left ? -1 : 1), tf.position.y);
            }

            tf.localScale = s;
        }

        public static bool IsLayerMatched(this GameObject obj, int layer)
        {
            return obj.layer == layer;
        }

        public static bool IsLayerMatched(this GameObject obj, LayerMask layer)
        {
            return ((1 << obj.layer) & layer) != 0;
        }

        public static Vector2? GetLowestPivot(this Transform transform)
        {
            var coll = transform.GetComponent<Collider2D>();
            if (coll == null)
                return null;
            return new Vector2(coll.bounds.center.x, coll.bounds.min.y);
        }
    }
}
