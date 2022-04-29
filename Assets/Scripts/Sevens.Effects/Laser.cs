using Sevens.Entities;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public GameObject FirePoint;
    public GameObject LaserPrefab;

    private int Prefab;
    private GameObject Instance;
    private Hovl_Laser LaserScript;
    private Hovl_Laser2 LaserScript2;
    private Blow _blow;
    private ContactFilter2D _filter;
    private bool _disabled;
    private float _hittableSince;

    public void DisableLaser()
    {
        if (LaserScript) LaserScript.DisablePrepare();
        if (LaserScript2) LaserScript2.DisablePrepare();
        _disabled = true;
        Destroy(gameObject, 1);
    }

    private void Awake()
    {
        _blow = GetComponent<Blow>();
        _filter = new ContactFilter2D() { useLayerMask = true, layerMask = _blow.DamageLayer };
    }

    private void OnEnable()
    {
        Destroy(Instance);
        Instance = Instantiate(LaserPrefab, FirePoint.transform.position, FirePoint.transform.rotation, FirePoint.transform);
        LaserScript = Instance.GetComponent<Hovl_Laser>();
        LaserScript2 = Instance.GetComponent<Hovl_Laser2>();
        _hittableSince = Time.time + 0.5f;
    }

    private void Update()
    {
        if (_disabled) return;
        var origin = FirePoint.transform.position;
        var dir = FirePoint.transform.forward;
        if (_hittableSince < Time.time)
        {
            var hits = new RaycastHit2D[1];
            var hitCount = Physics2D.Raycast(origin, dir, _filter, hits);
            if (hitCount > 0)
            {
                var comp = hits[0].collider.GetComponent<LivingEntity>();
                _blow.ApplyOnceDamage(comp);
            }
        }
    }

    private void OnDisable()
    {
        DisableLaser();
    }
}
