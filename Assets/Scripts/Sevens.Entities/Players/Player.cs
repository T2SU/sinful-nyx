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
using Sevens.Entities.Mobs;
using System.Linq;
using EasyParallax;
using Sevens.Cameras;
using UnityEngine.VFX;

namespace Sevens.Entities.Players
{
    public class Player : LivingEntity
    {
        private const string AttackCooltimeKey = "���� �lŸ��";
        private const string DashCooltimeKey = "�뽬 �lŸ��";

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
        [SerializeField, Range(0, 100)] private float _stamina;
        [SerializeField, Range(0, 1000)] private float _soul;
        [SerializeField, Range(0, 1000)] private float _sin;
        
        [SerializeField] private float _staminaRecoveryInterval;
        [SerializeField] private float _staminaRecovery;

        private Vector3 _prevPosition;
        private readonly List<SpriteScroller> _spriteScrollers = new List<SpriteScroller>();

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
        
        public float Sin 
        { 
            get => _sin; 
            set
            {
                _sin = value;
                OnSinRatioChanged?.Invoke(SinRatio);
            }
        }

        [field: SerializeField]
        public float MaxStamina { get; protected set; }

        public float StaminaRatio => (float)_stamina / MaxStamina;

        [field: SerializeField]
        public float MaxSin { get; protected set; }

        public float SinRatio => (float)_sin / MaxSin;

        public UnityEvent<float> OnStaminaRatioChanged;
        public UnityEvent<float> OnStaminaRatioInitialSet;

        public UnityEvent<float> OnSinRatioChanged;
        public UnityEvent<float> OnSinRatioInitialSet;


        [Header("Move")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _moveSpeedInAir;

        private float _xMove;


        [Header("Jump")]
        [SerializeField] private float[] _jumpSpeeds;
        [SerializeField] private float _rayCastDistance;
        [SerializeField] private float _risingGravityScale;
        [SerializeField] private float _fallingGravityScale;

        private int _jumpCount;
        private bool _lastFrameIsGround;
        #if UNITY_EDITOR
        [ShowCase]
        #endif
        private bool _isGround;
        private bool _jumpTrigger = false;

        private int JumpCountMax => _jumpSpeeds.Length;


        [Header("Attack")]
        [SerializeField] private AttackInfo[] _groundCombos;
        [SerializeField] private AttackInfo[] _airCombos;
        [SerializeField] private AttackInfo[] _counterCombos;


        private int _currentComboCount;  // ���� ���� ���� �޺� ī��Ʈ
        private int _bufferedComboCount; // �� ���Էµ� �޺� ī��Ʈ
        private int _attackedInAirCount; // ���� ������ Ƚ��
        private bool _isExecutedComboOnce;
        private bool _isCounterAttack;

        private AttackInfo CurrentAttackInfo
        {
            get
            {
                if (_isCounterAttack)
                    return _counterCombos[_currentComboCount];
                else if (_isGround)
                    return _groundCombos[_currentComboCount];
                else
                    return _airCombos[_currentComboCount];
            }
        }

        private int MaxComboCount
        {
            get
            {
                if (_isCounterAttack)
                    return _counterCombos.Length;
                else if (_isGround)
                    return _groundCombos.Length;
                else
                    return _airCombos.Length;
            }
        }


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
        [SerializeField] private Vector2 _hitKnockback;

        private float? _hitDirection;


        [Header("Dash")]
        [SerializeField] private float _dashCooltime;
        [SerializeField] private float _dashHorizontalSpeed;
        [SerializeField] private float _dashDuration;

        private int _dashedInAirCount;


        [Header("Camera")]
        [SerializeField] private VirtualCameraController _virtualCameraController;


        [Header("Visual Effects")]
        [SerializeField] private VisualEffect _dustWhileMovingVFX;


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
                if (_state == PlayerState.Attack && value != PlayerState.Attack)
                    ResetCombo();
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
        private TimeElapsingRecord _staminaRecoveryTimer;

        public Achievements Achievements { get; } = new Achievements();

        public virtual void SetInitialStamina(float stamina)
        {
            Stamina = stamina;
            OnStaminaRatioInitialSet?.Invoke(StaminaRatio);
        }

        public virtual void SetInitialSin(float sin)
        {
            Sin = sin;
            OnSinRatioInitialSet?.Invoke(SinRatio);
        }

        public bool IsDirectionMode()
        {
            return _directionMode;
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
            _staminaRecoveryTimer = new TimeElapsingRecord();
            _virtualCameraController = FindObjectOfType<VirtualCameraController>();
            Invincible = false;

            var stashedPlayerData = Singleton<PlayerData>.Data;
            if (stashedPlayerData != null)
                Load(stashedPlayerData);
        }

        protected override void Start()
        {
            var mainCam = Camera.main;
            var curtainPrefab = Resources.Load<GameObject>(Prefabs.CurtainSprite);
            _curtainSprite = Instantiate(curtainPrefab, mainCam.transform).GetComponent<SpriteRenderer>();

            // ���� ������
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

            _staminaRecoveryTimer.UpdateAsNow();

            _prevPosition = transform.position;
            _spriteScrollers.Clear();
            foreach (var obj in GameObject.FindGameObjectsWithTag("ScrollableBackground"))
            {
                var s = obj.GetComponent<SpriteScroller>();
                if (s != null)
                    _spriteScrollers.Add(s);
            }
        }

        protected override void FixedUpdate()
        {
            switch (State)
            {
                case PlayerState.Hit:
                    UpdateVelocityOnHit();
                    break;
                default:
                    UpdateVelocity();
                    break;
            }
            UpdateGravityScale();
        }

        private void UpdateGravityScale()
        {
            // ���� �ξ��� ���� �߷��� 0����.
            if (PlayerStates.IsLevitationState(State))
            {
                _playerRigidbody.gravityScale = 0f;
            }
            else if (_playerRigidbody.velocity.y < 0f)
            {
                _playerRigidbody.gravityScale = _fallingGravityScale;
            }
            else
            {
                _playerRigidbody.gravityScale = _risingGravityScale;
            }
        }

        private void UpdateVelocityOnHit()
        {
            if (_hitDirection.HasValue)
            {
                var velocity = _playerRigidbody.velocity;
                velocity.x = 0f;
                if (velocity.y > 0f)
                    velocity.y = 0f;
                _playerRigidbody.velocity = velocity;
                var knockback = _hitKnockback;
                knockback.x *= _hitDirection.Value;
                //Debug.Log($"Current velocity = {_playerRigidbody.velocity} | Knockback = {knockback}");
                _playerRigidbody.AddForce(knockback, ForceMode2D.Impulse);
                _hitDirection = null;
            }
            else
            {
                //Debug.Log($"Current velocity ? {_playerRigidbody.velocity}");
            }
        }

        private void UpdateVelocity()
        {
            var velocity = _playerRigidbody.velocity;

            switch (State)
            {
                case PlayerState.Attack:
                    velocity = Vector2.zero;
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

            if (State == PlayerState.Die || _directionMode)
            {
                velocity.x = 0f;
                if (velocity.y > 0f)
                    velocity.y = 0f;
            }
            else if (_jumpTrigger)
            {
                _jumpTrigger = false;
                velocity.y = _jumpSpeeds[_jumpCount - 1];
            }

            _playerRigidbody.velocity = velocity;
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

            if (collision.gameObject.layer == PhysicsUtils.InstaDeath)
            {
                OnDeath();
            }
            //Debug.Log($"OnTriggerEnter2D layer={collision.gameObject.layer} (Ground? {collision.gameObject.layer == PhysicsUtils.GroundLayer})");
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == PhysicsUtils.GroundLayer)
            {
                _isGround = false;
            }
            //Debug.Log($"OnTriggerExit2D layer={collision.gameObject.layer} (Ground? {collision.gameObject.layer == PhysicsUtils.GroundLayer})");
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
                    {
                        State = PlayerState.Run;
                        _dustWhileMovingVFX.Play();
                    }
                }
                else
                {
                    _dustWhileMovingVFX.Stop();
                    if (State != PlayerState.Idle)
                        State = PlayerState.Idle;
                }
            }
            if (State == PlayerState.Air)
            {
                _dustWhileMovingVFX.Stop();
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

            if (PlayerStates.IsJumpableState(State) && !_directionMode)
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
            if (PlayerStates.IsStaminaRecoveryableState(State))
            {
                RecoveryStamina();
            }

            UpdateBackgroundScrolling();
        }

        private void UpdateBackgroundScrolling()
        {
            var nowPos = transform.position;
            var diff = nowPos - _prevPosition;

            foreach (var s in _spriteScrollers)
                s.Move(diff);

            _prevPosition = nowPos;
        }

        private void OnDrawGizmos()
        {
            if (!_debugMode) return;
            Debug.DrawRay(transform.position, -transform.up * _rayCastDistance, Color.red);
        }

        public bool CheckDirection(Entity source)
        {
            return IsOnLeftBy(source.transform) != IsFacingLeft();
        }

        public override void OnDamagedBy(Entity source, float damage)
        {
            if (!_isInvincible && State != PlayerState.Dash && State != PlayerState.Die)
            {
                var result = _playerGuard.TryGuard(source, damage);
                Hp -= result.Damage;
                Stamina -= result.StaminaDamage;
                Debug.Log("���� ������" + result.Damage);
                Debug.Log("���� ���׹̳�������" + result.StaminaDamage);
                if (Hp < 0)
                    Hp = 0;
                if (result.Guarded.HasFlag(PlayerGuardResultType.Parry))
                {
                    CounterAttack(source);
                }
                if (!result.Guarded.HasFlag(PlayerGuardResultType.Guard))
                {
                    Invincible = true;
                    _hitDirection = IsOnLeftBy(source.transform) ? -1f : 1f;
                    State = PlayerState.Hit;
                    _hitTimer.UpdateAsNow();
                    _invincibleTimer.UpdateAsNow();
                    PlayAudio(State);
                }

                bool shakeResult = result.Guarded.HasFlag(PlayerGuardResultType.Guard) || result.Guarded.HasFlag(PlayerGuardResultType.Parry);

                _virtualCameraController.Shake(new Effects.CameraShakeOption()
                {
                    Amplitude = shakeResult ? 2f : 6f,
                    Frequency = 2.0f,
                    Time = 0.5f
                }) ;

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

        private void RecoveryStamina()
        {
            if (_staminaRecoveryTimer.Next(_staminaRecoveryInterval))
            {
                if (Stamina >= MaxStamina)
                    return;
                Stamina += _staminaRecovery;
            }
        }

        private void EnqueueAttack()
        {
            if (State == PlayerState.Die) return;
            if (State == PlayerState.Dash) return;

            var currentAttack = CurrentAttackInfo;
            var maxComboCount = MaxComboCount;

            // ���� ��Ÿ�� �߿� ���� �Ұ�
            if (_cooltime.IsBeing(AttackCooltimeKey, currentAttack.AttackCooltime))
                return;

            // �ִ� �޺� ī��Ʈ ���� �� �Է��� �� �̻� ���� ����.
            if (_bufferedComboCount + 1 > maxComboCount)
                return;

            // ���� ���� ���
            if (!_isGround)
            {
                // �ִ� ���� Ƚ�� 1ȸ ���޽�
                if (_attackedInAirCount >= 1)
                    // �Է��� ���� ����.
                    return;

                // ���� ���� Ƚ�� ����
                ++_attackedInAirCount;
            }

            // ���Է� �ݿ�
            _bufferedComboCount++;

            // ���� ������ ���, ���� �޺� ī��Ʈ�� 0���� �ʱ�ȭ �� ���� �̵� �ӵ��� ����.
            if (State != PlayerState.Attack)
            {
                State = PlayerState.Attack;
                _playerRigidbody.velocity = Vector2.zero;
            }
        }

        private void ExecuteAttack()
        {
            // ������ ����
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

                // ��� �ð� < �޺� �ִϸ��̼� �ð� + �޺� ������
                var comboDuration = animDuration + currentAttack.ComboDelay;
                if (!_comboAttackTimer.IsElapsed(comboDuration))
                    // --> ��ٸ�
                    return;

                if (_currentComboCount < _bufferedComboCount)
                {
                    var hasEnqueuedAttack = _currentComboCount + 1 < _bufferedComboCount;
                    // �� ������ ���� ���ٸ�
                    if (!hasEnqueuedAttack)
                    {
                        // ��Ÿ ���� �ĵ����� ����
                        if (!_comboFinishDelayTimer.IsElapsed(comboDuration + currentAttack.ComboFinishDelay))
                            return;
                    }

                    // ���� �޺� ī��Ʈ�� �ø�.
                    _comboFinishDelayTimer.UpdateAsNow();
                    ++_currentComboCount;
                    _isExecutedComboOnce = false;
                }
            }

            // ������ ����
            else
            {
                State = PlayerState.Idle;
                ResetCombo();
            }
        }



        private void ResetCombo()
        {
            _isCounterAttack = false;
            _currentComboCount = 0;
            _bufferedComboCount = 0;
            _cooltime.Set(AttackCooltimeKey);
        }

        private GameObject InstantiateComboAttackPrefab(GameObject comboPrefab)
        {
            var pos = comboPrefab.transform.position;
            pos.x *= GetFacingDirection();
            return Instantiate(comboPrefab, transform.position + pos, comboPrefab.transform.rotation);
        }

        private void CounterAttack(Entity source)
        {
            
            State = PlayerState.Attack;
            _isCounterAttack = true;
            _bufferedComboCount = 1;
            _cooltime.Reset(AttackCooltimeKey);
            
            var attackable = source.GetComponent<MobAttackable>();
            if (attackable != null)
                attackable.CancelAttack();
        }

        private void UpdateDash()
        {
            // ����Ƽ �������� �÷��̸�忡���� �뽬 �׻� �ر� ���·�..
#if !UNITY_EDITOR
            // �뽬 �ر� �ƴϸ� ��� �Ұ�
            if (Achievements.GetData(PlayerDataKeyType.UnlockedDash) != "1")
                return;
#endif

            // �뽬 ��
            if (State == PlayerState.Dash)
            {
                // �뽬 ���� �ð��� ��� ��������, ��Ÿ�� ���� �� Idle�� ���·� ��ȯ.
                if (_beingDashTimer.IsElapsed(_dashDuration))
                {
                    _cooltime.Set(DashCooltimeKey);
                    State = PlayerState.Idle;
                }
            }

            // Ű �Է� �� �뽬 ���·� ��ȯ
            else
            {
                // ���� ����� ��� �뽬 �ϸ� �ȵ�!
                if (_directionMode) return;

                if (State == PlayerState.Hit) return;
                if (State == PlayerState.Die) return;
                if (_cooltime.IsBeing(DashCooltimeKey, _dashCooltime)) return;

                if (Input.GetButtonDown("Dash"))
                {
                    // ���� ���� ���
                    if (!_isGround)
                    {
                        // �ִ� ���� �뽬 Ƚ�� 1ȸ ���޽�
                        if (_dashedInAirCount >= 1)
                            // �Է��� ���� ����.
                            return;

                        // ���� ���� Ƚ�� ����
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

        private void Load(PlayerData data)
        {
            MaxHp = data.MaxHP;
            MaxSin = data.MaxSin;
            MaxStamina = data.MaxStamina;
            SetInitialHp(data.HP);
            SetInitialSin(data.Sin);
            SetInitialStamina(data.Stamina);
            Soul = data.Soul;
            Achievements.Datas = data.Achievements.Datas.ToArray();

            if (!string.IsNullOrEmpty(data.SpawnPointName))
            {
                var pointObj = GameObject.Find(data.SpawnPointName);
                if (pointObj == null)
                    return;

                var pos = pointObj.transform.position;
                transform.position = pos;
            }
        }
    }
}
