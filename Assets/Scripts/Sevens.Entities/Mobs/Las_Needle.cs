// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using System.Collections;
using DG.Tweening;
using UnityEngine;
using Sevens.Utils;

#if UNITY_EDITOR
using Sevens.Utils.Editors;
#endif

namespace Sevens.Entities.Mobs
{
    public class Las_Needle : MonoBehaviour
    {
        [SerializeField]
        private GameObject _needle;

        [SerializeField]
        private Transform _beginPosition;

        [SerializeField]
        private float _startDelay = 0.2f;

        [SerializeField]
        private float _needleRisingInterval = 0.2f;

        [SerializeField]
        private float _needleRisingDuration = 0.5f;

        [SerializeField]
        private float _needleWaitingDuration = 0.3f;

        [SerializeField]
        private Vector2 _needleGap;

        [SerializeField]
        private float _needleMovingUp;

        [SerializeField]
        private int _needleNumber = 1;

        [SerializeField]
#if UNITY_EDITOR
        [ShowCase]
#endif
        private float _totalDuration;
        public float TotalDuration => _totalDuration;

        private CoroutineMan _coroutine;
        private Mob _mob;

        private void Awake()
        {
            _coroutine = new CoroutineMan(this);
        }

        public WaitWhile ActivateAndWait(Mob mob, Transform parent, bool left)
        {
            _mob = mob;
            _coroutine.Register("NeedleMaster", NeedleCoroutine(parent, left));
            return new WaitWhile(() =>
                {
                    var active = _coroutine.IsActive("NeedleMaster", CoroutineManType.Coroutine);
                    var child = transform.childCount;
                    return active || child > 0;
                }
            );
        }

        public void Kill()
        {
            _coroutine.KillAll();
        }

        private IEnumerator NeedleCoroutine(Transform parent, bool left)
        {
            Vector3 gap = _needleGap;
            var pos = _beginPosition.position;
            yield return new WaitForSeconds(_startDelay);
            for (int i = 0; i < _needleNumber; ++i)
            {
                yield return new WaitForSeconds(_needleRisingInterval);
                var obj = Instantiate(_needle, pos, Quaternion.identity, parent);
                if (!obj.activeSelf)
                    obj.SetActive(true);
                obj.transform.SetFacingLeft(left);
                var originPos = pos;
                _coroutine.Register($"Needle{i}",
                    DOTween.Sequence()
                    .Append(obj.transform.DOMoveY(originPos.y + _needleMovingUp, _needleRisingDuration))
                    .AppendInterval(_needleWaitingDuration)
                    .Append(obj.transform.DOMoveY(originPos.y, _needleRisingDuration))
                    .AppendCallback(() => Destroy(obj))
                , alwaysComplete: true);
                pos += gap * (left ? -1 : 1);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_needleNumber == 0)
            {
                _totalDuration = 0;
                return;
            }
            _totalDuration = _startDelay + (_needleNumber - 1) * _needleRisingInterval + _needleRisingDuration + _needleWaitingDuration;
        }
#endif
    }
}
