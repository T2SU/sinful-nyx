using DG.Tweening;
using Sevens.Cameras;
using Sevens.Effects;
using Sevens.Skills;
using Sevens.Utils;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class Mob : LivingEntity
    {
        public bool Invincible
        {
            get => _invincible;
            set
            {
                if (_invincible = value)
                    Debug.Log($"[Mob] [{name}] Set to invincible infinity!");
                else
                    Debug.Log($"[Mob] [{name}] is not invincible anymore.");
                _invincibleEnd = default;
            }
        }

        [SerializeField]
        protected MobMoveAbility _moveAbility;

        [SerializeField]
        protected NamedObject<AudioClip>[] _audioClips;

        [SerializeField]
        protected NamedObject<AnimationSet>[] _animations;

        [SerializeField]
        protected NamedObject<ActionEffectOption>[] _effects;

        [SerializeField]
        private bool _invincible;

        [SerializeField]
        private bool _immunePushed;

        private SkeletonAnimation _skeletonAnimation;
        private Spine.AnimationState _animationState;
        private Rigidbody2D _rigidbody;
        private CoroutineMan _coroutines;
        private Collider2D _collider;
        private Vector2 _velocity;
        private float _invincibleEnd;
        private Entity _killedBy;
        private Animator _animator;

        [field: SerializeField]
        public MobState State { get; protected set; }

        [field: SerializeField]
        public string Name { get; protected set; }

        public Cooltime Cooltime { get; } = new Cooltime();

        [field: SerializeField]
        public float MoveSpeed { get; protected set; }

        public override float EntitySizeX => _collider.bounds.size.x / 2;

        public override float EntitySizeY => _collider.bounds.size.y / 2;

        public VirtualCameraController Camera { get; private set; }

        public AudioSource AudioSource { get; private set; }
        
        public float HitTime { get; private set; }

        public void SetInvincible(float duration)
        {
            if (_invincible && _invincibleEnd == default)
                return;
            _invincibleEnd = Time.time + duration;
            _invincible = true;
            Debug.Log($"[Mob] [{name}] Set to invincible! time:{duration:F02}secs");
        }

        public void ChangeState(MobState state, bool playLoopAnimationByState = false)
        {
            Debug.Log($"[Mob] [{name}] Changed State {State} -> {state}");
            State = state;

            // 이동 또는 대기 상태가 아닐 경우, 몬스터의 속력을 0으로 만듦.
            if (state != MobState.Move && state != MobState.Idle)
                SetVelocity(Vector2.zero, linearly: false);

            Cooltime.Set("ChangedState");
            if (playLoopAnimationByState)
                PlayAnimation(new AnimationPlayOption(state.ToString(), true));
        }

        public bool IsDelayedByChangedState(float delay)
        {
            if (Cooltime.IsBeing("ChangedState", delay))
                return true;
            return false;
        }

        public float PlayAnimation(AnimationPlayOption opt, bool immediatelyTransition = false)
        {
            if (opt == null)
                return 0f;
            var animName = opt.AnimationName;
            AnimationSet anim = _animations.FindByName(animName);
            if (anim == null)
            {
                Debug.LogWarning($"{name}에서 '{opt.AnimationName}' 애니메이션을 재생하려 하였으나, 해당 애니메이션이 정의되지 않았습니다.");
                return 0f;
            }
            if (anim.SpineAnimation != null)
            {
                var spineAnim = anim.SpineAnimation;
                if (!IsCurrentAnimation(spineAnim, opt.Track))
                {
                    TrackEntry trackEntry;
                    if (immediatelyTransition)
                    {
                        trackEntry = _animationState.SetAnimation(opt.Track, spineAnim, opt.Loop);
                        trackEntry.TimeScale = opt.TimeScale;
                        return trackEntry.Animation.Duration;
                    }
                    else
                    {
                        var curTrack = _animationState.GetCurrent(opt.Track);
                        if (curTrack != null)
                            curTrack.Loop = false;
                        trackEntry = _animationState.AddAnimation(opt.Track, spineAnim, opt.Loop, 0f);
                        trackEntry.TimeScale = opt.TimeScale;
                        return trackEntry.AnimationTime;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(anim.AnimatorAnimation))
            {
                var animatorAnim = anim.AnimatorAnimation;
                if (!IsCurrentAnimation(animatorAnim))
                {
                    _animator.Play(animatorAnim);
                    var infos = _animator.GetCurrentAnimatorClipInfo(0);
                    return infos[0].clip.length;
                }
            }
            return 0f;
        }

        public void ClearSpineAnimations(float mixDuration, float delay, params int[] tracks)
        {
            foreach (var track in tracks)
            {
                if (Mathf.Approximately(delay, 0f) || delay < 0f)
                    _animationState.SetEmptyAnimation(track, mixDuration);
                else
                    _animationState.AddEmptyAnimation(track, mixDuration, delay);
            }
        }

        public Vector2 GetVelocity() => _velocity;

        public void SetVelocity(Vector2 velocity, bool linearly = true)
        {
            if (_coroutines == null) _coroutines = new CoroutineMan(this);
            if (linearly)
                _coroutines.Register("ChangeVelocity",
                    DOTween.To(() => _velocity, v => _velocity = velocity, velocity, 1f));
            else
            {
                _coroutines.KillTweener("ChangeVelocity");
                _velocity = velocity;
            }
        }

        public bool IsVelocityChangingLinearly()
        {
            return _coroutines.IsActive("ChangeVelocity", CoroutineManType.Tweener);
        }

        public override void OnDamagedBy(Entity source, float damage)
        {
            if (Hp == 0)
                return;
            if (source != this)
            {
                if (_invincible)
                    return;
            }
            Hp -= damage;
            if (Hp < 0)
                Hp = 0;
            var pos = GetRandomInPosition();
            if (pos != null)
                PlayEffect("HitBlood", pos.Value);
            if (_moveAbility != MobMoveAbility.None)
                BounceBack(0.3f, IsOnLeftBy(source.transform));
            if (Hp == 0)
            {
                _killedBy = source;
                OnDeath();
            }
            else
            {
                if (!_immunePushed)
                {
                    ChangeState(MobState.Hit);
                    HitTime = PlayAnimation(new AnimationPlayOption("Hit"), immediatelyTransition: true);
                }
            }

        }

        public Vector3? GetRandomInPositionV3(float xOffset = float.NaN)
        {
            var v = GetRandomInPosition(xOffset);
            if (v == null)
                return null;
            return new Vector3(v.Value.x, v.Value.y, transform.position.z);
        }

        public Vector2? GetRandomInPosition(float xOffset = float.NaN)
        {
            float xMin = _collider.bounds.min.x;

            if (float.IsNaN(xOffset))
                xOffset = UnityEngine.Random.Range(0f, _collider.bounds.max.x - xMin);

            // a point on the y axis that is always above our collider
            float yStrictlyAbove = _collider.bounds.max.y + 0.1f;
            float yStrictlyBelow = _collider.bounds.min.y - 0.1f;
            float distance = yStrictlyAbove - yStrictlyBelow;
            float x = xMin + xOffset;

            Vector2? GetBoundY(float originY, Vector2 dir)
            {
                var hits = Physics2D.RaycastAll(
                                new Vector2(x, originY),
                                dir,
                                distance: distance
                            );
                foreach (var h in hits)
                {
                    if (h.collider == _collider)
                        return h.point;
                }
                return null;
            }

            var down = GetBoundY(yStrictlyAbove, Vector2.down);
            var up = GetBoundY(yStrictlyBelow, Vector2.up);

            if (down == null || up == null)
                return null;
            return new Vector2(x, UnityEngine.Random.Range(down.Value.y, up.Value.y));
        }

        public void SetSkelAlpha(float a) => _skeletonAnimation.Skeleton.A = a;

        public float GetSkelAlpha() => _skeletonAnimation.Skeleton.A;

        public void PlayAudio(string name)
        {
            var found = _audioClips.FindByName(name);
            if (found != null)
                PlayAudio(found);
        }

        public void PlayAudio(AudioClip audioClip)
        {
            AudioSource.PlayOneShot(audioClip);
        }

        public void PlayEffect(string key, Vector2 pos, Transform parent = null)
        {
            if (key == null)
            {
                Camera.Shake(new CameraShakeOption() { Time = 0.1f }); // Set To Empty
                return;
            }
            ActionEffectOption effect = _effects.FindByName(key);
            if (effect == null)
            {
                Debug.LogWarning($"Not found an effect '{key}' at '{name}'.");
                return;
            }
            if (effect.Particle != null)
            {
                var obj = Instantiate(effect.Particle);
                obj.transform.position = pos;
                if (parent != null)
                    obj.transform.parent = parent;
            }
            if (effect.Shake.HasValue())
                Camera.Shake(effect.Shake);
            if (effect.Sound != null)
                AudioSource.PlayOneShot(effect.Sound);
        }

        protected override void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            if (_skeletonAnimation != null)
                _animationState = _skeletonAnimation.AnimationState;
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();
            AudioSource = GetComponent<AudioSource>();
            Camera = GetComponent<VirtualCameraLinker>().Camera;
            _coroutines = new CoroutineMan(this);
        }

        protected override void FixedUpdate()
        {
            UpdateVelocity();
        }

        protected override void OnEnable()
        {
        }

        protected override void Start()
        {
        }

        protected override void Update()
        {
            CheckExpireInvincible();
        }

        protected override void OnDisable()
        {
            if (_coroutines == null) _coroutines = new CoroutineMan(this);
            _coroutines.KillAll();
        }

        protected override void OnDeath()
        {
            if (State == MobState.Die)
                return;
            Hp = 0;
            _coroutines.KillAll();
            TryCancelAttack();
            TryCallDestroyCallback();
            ChangeState(MobState.Die);
        }

        private void OnGUI()
        {
            if (Debug.isDebugBuild)
            {
                GUI.Label(new Rect(800, 50, 300, 100), $"{name} HP: {Hp}/{MaxHp}");
            }
        }

        private void TryCancelAttack()
        {
            var attackable = GetComponent<MobAttackable>();
            if (attackable != null)
                attackable.CancelAttack();
        }

        private void TryCallDestroyCallback()
        {
            var callback = GetComponent<EntityDestroyCallbackBase>();
            if (callback != null)
                callback.Execute(this, _killedBy);
            else
                PlayAnimation(new AnimationPlayOption("Die"));
        }

        private void UpdateVelocity()
        {
            _rigidbody.velocity = _velocity;
        }

        private void BounceBack(float power, bool left)
        {
            var dest = PhysicsUtils.CheckBound(this, new Vector2(power, 0), left);
            _coroutines.Register("BounceBack", transform.DOMove(dest, 0.2f).SetLoops(1, LoopType.Yoyo));
        }

        private bool IsCurrentAnimation(Spine.Animation spineAnimation, int trackIndex = 0)
        {
            var current = _animationState.GetCurrent(trackIndex);
            if (current == null)
                return false;
            return current.Animation.Name == spineAnimation.Name;
        }

        private bool IsCurrentAnimation(string animatorAnimation)
        {
            var infos = _animator.GetCurrentAnimatorClipInfo(0);
            if (infos.Length == 0)
                return false;
            return infos[0].clip.name == animatorAnimation;
        }

        private void CheckExpireInvincible()
        {
            if (_invincible && _invincibleEnd != default && Time.time > _invincibleEnd)
            {
                _invincible = false;
                Debug.Log($"[Mob] [{name}] Terminated invincible mode.");
            }
        }
    }
}