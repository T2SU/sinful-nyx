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
        [SerializeField] private float _stamina;
        [SerializeField] private float _soul;
        [SerializeField] private float _sin;

        public float Soul { get => _soul; set => _soul = value; }

        public float Stamina { get => _stamina; set => _stamina = value; }

        public float Sin { get => _sin; set => _sin = value; }


        [Header("Move")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _moveSpeedInAir;

        private float _xMove;


        [Header("Jump")]
        [SerializeField] private float[] _jumpSpeeds;
        [SerializeField] private float _rayCastDistance;

        private int _jumpCount;
        private bool _isGround = true;
        private bool _jumpTrigger = false;

        private int JumpCountMax => _jumpSpeeds.Length;


        [Header("Attack")]
        [SerializeField] private AttackInfo[] _groundCombos;
        [SerializeField] private AttackInfo[] _airCombos;

        private int _currentComboCount;  // ���� ���� ���� �޺� ī��Ʈ
        private int _bufferedComboCount; // �� ���Էµ� �޺� ī��Ʈ
        private int _attackedInAirCount; // ���� ������ Ƚ��
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


        [Header("Misc")]
        [SerializeField] private bool _debugMode;

        private bool _directionMode = false;

        [field: SerializeField]
#if UNITY_EDITOR
        [field: ShowCase]
#endif
        public PlayerState State { get; set; }

        public override float EntitySizeX => _playerCollider.bounds.size.x / 2;
        public override float EntitySizeY => _playerCollider.bounds.size.y / 2;

        private readonly Cooltime _cooltime = new Cooltime();
        private TimeElapsingRecord _invincibleTimer;
        private TimeElapsingRecord _hitTimer;
        private TimeElapsingRecord _comboAttackTimer;
        private TimeElapsingRecord _comboFinishDelayTimer;
        private TimeElapsingRecord _beingDashTimer;

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
            _invincibleTimer = new TimeElapsingRecord();
            _hitTimer = new TimeElapsingRecord();
            _comboAttackTimer = new TimeElapsingRecord();
            _comboFinishDelayTimer = new TimeElapsingRecord();
            _beingDashTimer = new TimeElapsingRecord();
            Invincible = false;
        }

        protected override void FixedUpdate()
        {
            if (_directionMode) return;
            UpdateVelocity();
        }

        private void AirMove(float xMove)
        {
            if (State == PlayerState.Die) return;
            if (State == PlayerState.Hit) return;
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
        }

        protected override void OnEnable()
        {

        }

        protected override void Start()
        {
            var mainCam = Camera.main;
            var curtainPrefab = Resources.Load<GameObject>("Sprite/CurtainBase");
            _curtainSprite = Instantiate(curtainPrefab, mainCam.transform).GetComponent<SpriteRenderer>();

            // ���� ������
            _curtainSprite.color = new Color(0f, 0f, 0f, 0f); 
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

            if (Physics2D.Raycast(transform.position, -transform.up, _rayCastDistance, PhysicsUtils.GroundLayerMask))
            {
                //Debug.Log("isGround");
                _isGround = true;
                if (!PlayerStates.IsAirState(State))
                {
                    _jumpCount = 0;
                    _attackedInAirCount = 0;
                }
            }
            else
            {
                //Debug.Log("isNotGround");
                _isGround = false;
            }

            if (PlayerStates.IsAttackableState(State))
            {
                if (Input.GetButtonDown("Fire1") && !_directionMode)
                    EnqueueAttack();
                if (State == PlayerState.Attack)
                    ExecuteAttack();
            }

            if (State != PlayerState.Attack && State != PlayerState.Hit && State != PlayerState.Die)
            {
                if (!Mathf.Approximately(0f, _xMove))
                    transform.SetFacingLeft(_xMove < 0);
            }

            if (CanChangeDefaultState(State))
            {
                if (!Mathf.Approximately(0f, _playerRigidbody.velocity.x))
                    State = PlayerState.Run;
                else
                    State = PlayerState.Idle;
            }

            if (PlayerStates.IsJumpableState(State))
            {
                if (Input.GetButtonDown("Jump"))
                {
                    if (_jumpCount < JumpCountMax)
                    {
                        ++_jumpCount;
                        _jumpTrigger = true;
                        State = PlayerState.Jump;
                        TryPlayAnimation(_animClip.FindByName($"Jump{_jumpCount}"), false, 1f, 0);
                    }
                }
            }

            UpdateDash();

            if (_playerRigidbody.velocity.y < 0f && !_isGround && !PlayerStates.HasUniqueAnimationState(State))
            {
                if (!_jumpTrigger)
                    State = PlayerState.Fall;
            }

            if (State == PlayerState.Die)
            {
                State = PlayerState.Die;
            }
            SetCurrentAnimation(State);
        }


        private bool CanChangeDefaultState(PlayerState state)
        {
            // Hit, Guard, Attack, Hit, Die ��,
            // ���� �ִ��� Idle �Ǵ� Run �ִϸ��̼����� ��ȯ�ؼ��� �ȵǴ� ����
            if (PlayerStates.HasUniqueAnimationState(state))
                return false;

            // ��� ���� ��
            if (state == PlayerState.Jump)
                return false;

            // ���߿� ������?
            if (!_isGround)
                return false;

            return true;
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
                    PlayAudio("Dash", 4f);
                    State = PlayerState.Dash;
                    _beingDashTimer.UpdateAsNow();
                }
            }
        }

        void SetCurrentAnimation(PlayerState _state)
        {
            switch (_state)
            {
                case PlayerState.Idle:
                case PlayerState.Run:
                    AsyncAnimation(_state, true, 1f, 0);
                    break;
                case PlayerState.UltimateSkill:
                case PlayerState.Fall:
                case PlayerState.Hit:
                case PlayerState.Dash:
                case PlayerState.Die:
                case PlayerState.Guard:
                    AsyncAnimation(_state, false, 1f, 0);
                    break;
            }
        }
        private void AsyncAnimation(PlayerState state, bool loop, float timeScale, int track)
        {
            TryPlayAnimation(_animClip.FindByName(state.ToString()), loop, timeScale, track);
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
            => PlayAudio(state.ToString());

        private void PlayAudio(string audioName, float pitch = 1.0f)
        {
            AudioClip clip = _audioClips.FindByName(audioName);
            if (clip == null) return;
            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(clip);
        }
    }
}
