using System.Linq;
using UnityEngine;

namespace Sevens.Entities
{
    public class Entity : MonoBehaviour
    {
        protected virtual void Awake()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public bool IsOnLeftBy(Transform other)
        {
            return transform.position.x < other.position.x;
        }

        public bool IsFacingLeft()
        {
            return transform.localScale.x < 0;
        }

        public float GetFacingDirection()
        {
            return Mathf.Sign(transform.localScale.x);
        }

        public string GetClosestSpawnPoint()
        {
            var objs = GameObject.FindGameObjectsWithTag("SpawnPoint");
            var spawnname =
                objs.OrderBy(o =>
                {
                    var gap = o.transform.position - transform.position;
                    return gap.sqrMagnitude;
                })
                .FirstOrDefault()?
                .name;
            if (spawnname != null)
                return spawnname;
            var player = GameObject.FindGameObjectWithTag("Player");
            return player.name;
        }
    }
}