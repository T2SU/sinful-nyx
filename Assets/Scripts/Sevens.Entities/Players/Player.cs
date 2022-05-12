using Sevens.Skills;
using Sevens.Utils;
#if UNITY_EDITOR
using Sevens.Utils.Editors;
#endif
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace Sevens.Entities.Players
{
    public class Player : LivingEntity
    {
        private const string AttackCooltimeKey = "공격 쿹타임";
        private const string DashCooltimeKey = "대쉬 쿹타임";

        [Header("Clips")]
        [SerializeField] private NamedObject<AudioClip>[] _audioClips;
        [SerializeField] private NamedObject<AnimationReferenceAsset>[] _animClip;


        [Header("Components")]
        [SerializeField] private Rigidbody2D _playerRigidbody;
        [SerializeField] private Collider2D _playerCollider;
        [SerializeField] private PlayerGuard _playerGuard;
        [SerializeField] private string _currentAnimation;

        private SpriteRenderer _curtainSprite;
        private SkeletonAnimation _skeletonAnimation;
        private AudioSource _audioSource;


        [Header("BasicStat")]
        [SerializeField] private float _stamina;
        [SerializeField] private float _soul;
        [SerializeField] private float _sin;

        public float Soul { get => _soul; set => _soul = value; }

        public float Stamina
        {
            get => _stamina;
            set
            {
                _stamina = value;
                OnStaminaRatioChanged?.Invoke(StaminaRatio);
            }
        }

        public float Sin { get => _sin; set => _sin = value; }

        [field: SerializeField]
        public float MaxStamina { get; protected set; }

        public float StaminaRatio => (float)_stamina / MaxStamina;

        public UnityEvent<float> OnStaminaRatioChanged;
        public UnityEvent<float> OnStaminaRatioInitialSet;


        [Header("Move")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _moveSpeedInAir;

        private float _xMove;


        [Header("Jump")]
        [SerializeField] private float[] _jumpSpeeds;
        [SerializeField] private float _rayCastDistance;

        private int _jumpCount;
        private bool _lastFrameIsGround;
        private bool _isGround;
        private bool _jumpTrigger = false;
        private float _lastGravityScale;

        private int JumpCountMax => _jumpSpeeds.Length;


        [Header("Attack")]
        [SerializeField] private AttackInfo[] _groundCombos;
        [SerializeField] private AttackInfo[] _airCombos;

        private int _currentComboCount;  // 현재 진행 중인 콤보 카운트
        private int _bufferedComboCount; // 총 선입력된 콤보 카운트
        private int _attackedInAirCount; // 공중 공격한 횟수
        private bool _isExecutedComboOnce;

        private AttackInfo CurrentAttackInfo
            => _isGround ? _groundCombos[_currentComboCount] : _airCombos[_currentComboCount];

        private int MaxComboCount
            => _isGround ? _groundCombos.Length : _airCombos.Length;


        [Header("Invincible")]
        [SerializeField] private float _invincibleTime;

        private bool _isInvincible;
        public bool Invincible
        {
            get => _isInvincible;
            set
            {
                if (_isInvincible = value)
                    _invincibleTimer.UpdateAsNow();
            }
        }


        [Header("Hit")]
        [SerializeField] private float _hitTime;


        [Header("Dash")]
        [SerializeField] private float _dashCooltime;
        [SerializeField] private float _dashHorizontalSpeed;
        [SerializeField] private float _dashDuration;

        private int _dashedInAirCount;


        [Header("Misc")]
        [SerializeField] private bool _debugMode;

        private bool _directionMode = false;

        [field: SerializeField]
#if UNITY_EDITOR
        [field: ShowCase]
#endif

        private PlayerState _state;
        public PlayerState State
        {
            get => _state;
            set
            {
                switch (_state = value)
                {
                    case PlayerState.Idle:
                    case PlayerState.Run:
                        TryPlayAnimation(value, true, 1f, 0);
                        break;
                    case PlayerState.UltimateSkill:
                    case PlayerState.Hit:
                    case PlayerState.Dash:
                    case PlayerState.Die:
                    case PlayerState.Guard:
                        TryPlayAnimation(value, false, 1f, 0);
                        break;
                }
                Debug.Log($"State = {_state}");
            }
        }

        public override float EntitySizeX => _playerCollider.bounds.size.x / 2;
        public override float EntitySizeY => _playerCollider.bounds.size.y / 2;

        private readonly Cooltime _cooltime = new Cooltime();
        private TimeElapsingRecord _invincibleTimer;
        private TimeElapsingRecord _hitTimer;
        private TimeElapsingRecord _comboAttackTimer;
        private TimeElapsingRecord _comboFinishDelayTimer;
        private TimeElapsingRecord _beingDashTimer;

        public virtual void SetInitialStamina(float stamina)
        {
            Stamina = stamina;
            OnStaminaRatioInitialSet?.Invoke(StaminaRatio);
        }

        public void SetDirectionMode(bool enabled)
        {
            _directionMode = enabled;
            _xMove = 0f;
        }

        protected override void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody2D>();
            _playerCollider = GetComponent<Collider2D>();
            _audioSource = GetComponent<AudioSource>();
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _jumpCount = 0;
            _currentComboCount = 0;
            _bufferedComboCount = 0;
            _attackedInAirCount = 0;
            _dashedInAirCount = 0;
            _invincibleTimer = new TimeElapsingRecord();
            _hitTimer = new TimeElapsingRecord();
            _comboAttackTimer = new TimeElapsingRecord();
            _comboFinishDelayTimer = new TimeElapsingRecord();
            _beingDashTimer = new TimeElapsingRecord();
            Invincible = false;
            _lastGravityScale = _playerRigidbody.gravityScale;
        }

        protected override void Start()
        {
            var mainCam = Camera.main;
            var curtainPrefab = Resources.Load<GameObject>("Sprite/CurtainBase");
            _curtainSprite = Instantiate(curtainPrefab, mainCam.transform).GetComponent<SpriteRenderer>();

            // 투명 검정색
            _curtainSprite.color = new Color(0f, 0f, 0f, 0f);

            var colliders = GetComponents<Collider2D>();
            foreach (var coll in colliders)
            {
                if (coll.isTrigger)
                {
                    var results = new Collider2D[1];
                    _isGround = Physics2D.OverlapCollider(coll, PhysicsUtils.GroundContactFilter, results) != 0;
                    _lastFrameIsGround = _isGround;
                    break;
                }
            }
        }

        protected override void FixedUpdate()
        {
            if (_directionMode) return;
            UpdateVelocity();
        }

        private void UpdateVelocity()
        {
            if (State == PlayerState.Die) return;

            var velocity = _playerRigidbody.velocity;

            switch (State)
            {
                case PlayerState.Attack:
                    velocity = Vector2.zero;
                    break;
                case PlayerState.Hit:
                    _playerRigidbody.AddForce(new Vector2(GetFacingDirection(), 1), ForceMode2D.Impulse);
                    break;
                case PlayerState.Guard:
                    velocity = new Vector2(0, _playerRigidbody.velocity.y);
                    break;
                case PlayerState.Dash:
                    velocity = new Vector2(GetFacingDirection() * _dashHorizontalSpeed, 0);
                    break;
                default:
                    velocity.x = _xMove * (_isGround ? _moveSpeed : _moveSpeedInAir);
                    break;
            }
            if (_jumpTrigger)
            {
                _jumpTrigger = false;
                velocity.y = _jumpSpeeds[_jumpCount - 1];
            }

            _playerRigidbody.velocity = velocity;

            // 공중 부양일 때는 중력을 0으로.
            if (PlayerStates.IsLevitationState(State))
            {
                _playerRigidbody.gravityScale = 0f;
            }
            else
                _playerRigidbody.gravityScale = _lastGravityScale;
        }

        protected override void OnEnable()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == PhysicsUtils.GroundLayer)
            {
                _isGround = true;
                _jumpCount = 0;
                _attackedInAirCount = 0;
                _dashedInAirCount = 0;
            }
            Debug.Log($"OnTriggerEnter2D layer={collision.gameObject.layer} (Ground? {collision.gameObject.layer == PhysicsUtils.GroundLayer})");
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == PhysicsUtils.GroundLayer)
            {
                _isGround = false;
            }
            Debug.Log($"OnTriggerExit2D layer={collision.gameObject.layer} (Ground? {collision.gameObject.layer == PhysicsUtils.GroundLayer})");
        }

        protected override void Update()
        {
            if (!_directionMode)
                _xMove = Input.GetAxisRaw("Horizontal");

            if (_isInvincible)
            {
                if (_invincibleTimer.IsElapsed(_invincibleTime))
                {
                    Invincible = false;
                }
            }

            if (State == PlayerState.Hit)
            {
                if (_hitTimer.IsElapsed(_hitTime))
                {
                    State = PlayerState.Idle;
                }
            }

            if (PlayerStates.IsAttackableState(State))
            {
                if (Input.GetButtonDown("Fire1") && !_directionMode)
                    EnqueueAttack();
                if (State == PlayerState.Attack)
                    ExecuteAttack();
            }

            if (State != PlayerState.Attack && State != PlayerState.Hit && State != PlayerState.Guard && State != PlayerState.Die)
            {
                if (!Mathf.Approximately(0f, _xMove))
                    transform.SetFacingLeft(_xMove < 0);
            }

            //if (CanChangeDefaultState(State))
            //{
            //    if (!Mathf.Approximately(0f, _playerRigidbody.velocity.x))
            //        State = PlayerState.Run;
            //    else
            //        State = PlayerState.Idle;
            //}

            if (_isGround && (State == PlayerState.Idle || State == PlayerState.Run))
            {
                if (!Mathf.Approximately(_xMove, 0f))
                {
                    if (State != PlayerState.Run)
                        State = PlayerState.Run;
                }
                else
                {
                    if (State != PlayerState.Idle)
                        State = PlayerState.Idle;
                }
            }
            if (State == PlayerState.Air)
            {
                if (!_isGround && _playerRigidbody.velocity.y < -0.2f)
                {
                    if (!_jumpTrigger)
                        TryPlayAnimation("Fall", false, 1f, 0);
                }
                if (_isGround)
                {
                    if (!_lastFrameIsGround)
                    {
                        State = PlayerState.Idle;
                    }
                }
            }
            else if (State == PlayerState.Idle || State == PlayerState.Run)
            {
                if (!_isGround && _playerRigidbody.velocity.y < -0.2f)
                {
                    State = PlayerState.Air;
                    _jumpCount++;
                }
            }
            _lastFrameIsGround = _isGround;

            if (PlayerStates.IsJumpableState(State))
            {
                if (Input.GetButtonDown("Jump"))
                {
                    if (_jumpCount < JumpCountMax)
                    {
                        ++_jumpCount;
                        _jumpTrigger = true;
                        State = PlayerState.Air;
                        TryPlayAnimation($"Jump{_jumpCount}", false, 1f, 0);
                    }
                }
            }

            UpdateDash();
        }

        private void OnDrawGizmos()
        {
            if (!_debugMode) return;
            Debug.DrawRay(transform.position, -transform.up * _rayCastDistance, Color.red);
        }

        public override void OnDamagedBy(Entity source, float damage)
        {
            if (!_isInvincible && State != PlayerState.Dash && State != PlayerState.Die)
            {
                var result = _playerGuard.TryGuard(source, damage);
                Hp -= result.Damage;
                Stamina -= result.StaminaDamage;
                if (Hp < 0)
                    Hp = 0;

                if (!result.Guarded.HasFlag(PlayerGuardResultType.Guard))
                {
                    Invincible = true;
                    State = PlayerState.Hit;
                    PlayAudio(State);
                }

                if (Hp <= 0)
                    OnDeath();
            }
        }

        protected override void OnDeath()
        {
            State = PlayerState.Die;
            _curtainSprite.DOFade(1f, 3f);
            gameObject.GetComponent<MeshRenderer>().sortingLayerName = "Overlay";
            gameObject.GetComponent<MeshRenderer>().sortingOrder = 100;
            StartCoroutine(CoroutineUtility.SlowTime(0.5f, 4));
            GetComponent<PlayerDied>().OnDied();
        }

        private void EnqueueAttack()
        {
            if (State == PlayerState.Die) return;
            if (State == PlayerState.Dash) return;

            var currentAttack = CurrentAttackInfo;
            var maxComboCount = MaxComboCount;

            // 공격 쿨타임 중에 공격 불가
            if (_cooltime.IsBeing(AttackCooltimeKey, currentAttack.AttackCooltime))
                return;

            // 최대 콤보 카운트 도달 시 입력을 더 이상 받지 않음.
            if (_bufferedComboCount + 1 > maxComboCount)
                return;

            // 점프 중일 경우
            if (!_isGround)
            {
                // 최대 공격 횟수 1회 도달시
                if (_attackedInAirCount >= 1)
                    // 입력을 받지 않음.
                    return;

                // 공중 공격 횟수 증가
                ++_attackedInAirCount;
            }

            // 선입력 반영
            _bufferedComboCount++;

            // 최초 공격일 경우, 현재 콤보 카운트를 0으로 초기화 및 현재 이동 속도를 멈춤.
            if (State != PlayerState.Attack)
            {
                State = PlayerState.Attack;
                _playerRigidbody.velocity = Vector2.zero;
            }
        }

        private void ExecuteAttack()
        {
            // 공격을 실행
            if (_currentComboCount < _bufferedComboCount)
            {
                var currentAttack = CurrentAttackInfo;
                AnimationReferenceAsset anim = _animClip.FindByName(currentAttack.AnimationName);
                var animDuration = anim.Animation.Duration;

                if (!_isExecutedComboOnce)
                {
                    PlayAudio(currentAttack.AudioName);
                    var comb = InstantiateComboAttackPrefab(currentAttack.AttackPrefab);
                    Blow.SetAllBlowSourceAs(comb, this);
                    TryPlayAnimation(anim, false, 1f, 0);
                    _comboAttackTimer.UpdateAsNow();
                    _isExecutedComboOnce = true;
                }

                // 경과 시간 < 콤보 애니메이션 시간 + 콤보 딜레이
                var comboDuration = animDuration + currentAttack.ComboDelay;
                if (!_comboAttackTimer.IsElapsed(comboDuration))
                    // --> 기다림
                    return;

                if (_currentComboCount < _bufferedComboCount)
                {
                    var hasEnqueuedAttack = _currentComboCount + 1 < _bufferedComboCount;
                    // 더 연계할 것이 없다면
                    if (!hasEnqueuedAttack)
                    {
                        // 막타 공격 후딜레이 적용
                        if (!_comboFinishDelayTimer.IsElapsed(comboDuration + currentAttack.ComboFinishDelay))
                            return;
                    }

                    // 다음 콤보 카운트로 올림.
                    _comboFinishDelayTimer.UpdateAsNow();
                    ++_currentComboCount;
                    _isExecutedComboOnce = false;
                }
            }

            // 공격을 종료
            else
            {
                State = PlayerState.Idle;
                _currentComboCount = 0;
                _bufferedComboCount = 0;
                _cooltime.Set(AttackCooltimeKey);
            }
        }

        private GameObject InstantiateComboAttackPrefab(GameObject comboPrefab)
        {
            var pos = comboPrefab.transform.position;
            pos.x *= GetFacingDirection();
            return Instantiate(comboPrefab, transform.position + pos, comboPrefab.transform.rotation);
        }


        private void UpdateDash()
        {
            // 대쉬 중
            if (State == PlayerState.Dash)
            {
                // 대쉬 지속 시간이 모두 지났으면, 쿨타임 적용 및 Idle로 상태로 전환.
                if (_beingDashTimer.IsElapsed(_dashDuration))
                {
                    _cooltime.Set(DashCooltimeKey);
                    State = PlayerState.Idle;
                }
            }

            // 키 입력 시 대쉬 상태로 전환
            else
            {
                // 연출 모드일 경우 대쉬 하면 안됨!
                if (_directionMode) return;

                if (State == PlayerState.Hit) return;
                if (State == PlayerState.Die) return;
                if (_cooltime.IsBeing(DashCooltimeKey, _dashCooltime)) return;

                if (Input.GetButtonDown("Dash"))
                {
                    // 점프 중일 경우
                    if (!_isGround)
                    {
                        // 최대 공중 대쉬 횟수 1회 도달시
                        if (_dashedInAirCount >= 1)
                            // 입력을 받지 않음.
                            return;

                        // 공중 공격 횟수 증가
                        ++_dashedInAirCount;
                    }

                    PlayAudio("Dash", 4f);
                    State = PlayerState.Dash;
                    _beingDashTimer.UpdateAsNow();
                }
            }
        }

        private void TryPlayAnimation(PlayerState state, bool loop, float timeScale, int track)
        {
            TryPlayAnimation(state.ToString(), loop, timeScale, track);
        }

        private void TryPlayAnimation(string animName, bool loop, float timeScale, int track)
        {
            TryPlayAnimation(_animClip.FindByName(animName), loop, timeScale, track);
        }

        private float TryPlayAnimation(AnimationReferenceAsset anim, bool loop, float timeScale, int track)
        {
            if (anim.name.Equals(_currentAnimation)) return 0f;

            var trackEntry = _skeletonAnimation.state.SetAnimation(track, anim, loop);
            trackEntry.TimeScale = timeScale;
            _skeletonAnimation.loop = loop;
            _skeletonAnimation.timeScale = timeScale;

            _currentAnimation = anim.name;

            return trackEntry.AnimationTime;
        }

        private void PlayAudio(PlayerState state, float pitch = 1.0f)
            => PlayAudio(state.ToString(), pitch);

        private void PlayAudio(string audioName, float pitch = 1.0f)
        {
            AudioClip clip = _audioClips.FindByName(audioName);
            if (clip == null) return;
            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(clip);
        }
    }
}
