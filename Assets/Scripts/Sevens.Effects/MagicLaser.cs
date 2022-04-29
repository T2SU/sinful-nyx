using Sevens.Entities;
using Sevens.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Sevens.Effects
{
    public class MagicLaser : MonoBehaviour
    {
        [Header("Point")]
        public GameObject FirePoint;

        [Header("Prefabs")]
        public GameObject beamLineRendererPrefab;
        public GameObject beamStartPrefab;
        public GameObject beamEndPrefab;

        private GameObject beamStart;
        private GameObject beamEnd;
        private GameObject beam;
        private LineRenderer line;
        private ParticleSystem[] _effects;

        [Header("Adjustable Variables")]
        public float beamEndOffset = 1f; //How far from the raycast hit point the end effect is positioned
        public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
        public float textureLengthScale = 3; //Length of the beam texture

        private Blow _blow;
        private ContactFilter2D _filter;
        private bool _disabled;
        private float _hittableSince;

        public void DestroyLaser()
        {
            _disabled = true;
            foreach (var eff in _effects)
                if (eff && eff.isPlaying)
                    eff.Stop();
            if (line && line.enabled)
                line.enabled = false;
            Destroy(beamStart, 1);
            Destroy(beamEnd, 1);
            Destroy(beam, 1);
            Destroy(gameObject, 1);
        }

        private void Start()
        {
            _blow = GetComponent<Blow>();
            _filter = new ContactFilter2D() { useLayerMask = true, layerMask = _blow.DamageLayer };
            beamStart = Instantiate(beamStartPrefab, new Vector3(0, 0, 0), Quaternion.identity, FirePoint.transform);
            beamEnd = Instantiate(beamEndPrefab, new Vector3(0, 0, 0), Quaternion.identity, FirePoint.transform);
            beam = Instantiate(beamLineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity, FirePoint.transform);
            line = beam.GetComponent<LineRenderer>();
            _effects = GetComponentsInChildren<ParticleSystem>();
            _hittableSince = Time.time + 0.5f;
        }

        void Update()
        {
            var origin = FirePoint.transform.position;
            var dir = FirePoint.transform.forward;
            if (!_disabled && _hittableSince < Time.time)
            {
                var hits = new RaycastHit2D[1];
                var hitCount = Physics2D.Raycast(origin, dir, _filter, hits);
                if (hitCount > 0)
                {
                    var comp = hits[0].collider.GetComponent<LivingEntity>();
                    _blow.ApplyOnceDamage(comp);
                }
            }
            ShootBeamInDir(transform.position, dir);
        }

        void ShootBeamInDir(Vector3 start, Vector3 dir)
        {
#if UNITY_5_5_OR_NEWER
            line.positionCount = 2;
#else
		line.SetVertexCount(2); 
#endif
            line.SetPosition(0, start);
            beamStart.transform.position = start;

            Vector3 end = Vector3.zero;
            var hits = new RaycastHit2D[1];
            var hitCount = Physics2D.Raycast(start, dir, PhysicsUtils.GroundContactFilter, hits);
            if (hitCount > 0)
                end = hits[0].point - (Vector2)(dir.normalized * beamEndOffset);
            else
                end = transform.position + (dir * 100);

            beamEnd.transform.position = end;
            line.SetPosition(1, end);

            beamStart.transform.LookAt(beamEnd.transform.position);
            beamEnd.transform.LookAt(beamStart.transform.position);

            float distance = Vector3.Distance(start, end);
            line.sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
            line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
        }
    }
}