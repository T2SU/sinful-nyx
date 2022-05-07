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
        [SerializeField]
        protected float _moveSpeed;

        [SerializeField]
        protected float _moveSpeedInAir;

        [SerializeField]
        protected float _jumpSpeed;

        [SerializeField]
        protected NamedObject<AudioClip>[] _audioClips;
        AudioSource _audioSource;

        [SerializeField]
        protected NamedObject<AnimationReferenceAsset>[] animClip;

        [SerializeField]
        protected Rigidbody2D playerRigidbody;
        [SerializeField]
        protected Collider2D playerCollider;
        [SerializeField]
        protected PlayerGuard playerGuard;

        public SkeletonAnimation skeletonAnimation;

        [SerializeField] private string CurrentAnimation;

        [SerializeField] private SpriteRenderer curtainSprite;

        RaycastHit2D hit;
        [SerializeField]
        private float _rayCastDistance;

        private bool _isGround = true;
        private bool _isJumping = false;
        [SerializeField] private int _jumpCount;
        [SerializeField] private int _jumpCountMax = 1;
        private float xMove;
        private float xDir;

        [SerializeField]
        private float _stamina;

        [SerializeField]
        private float _soul;

        [SerializeField]GameObject combo_1;
        [SerializeField] GameObject combo_2;
        [SerializeField] GameObject combo_3;

        [SerializeField] private int _comboCount;
        [SerializeField] private float _comboDelay;
        [SerializeField] private float _comboTimer;
        [SerializeField] private float AttackDelay;
        private bool _isAttack;

        [SerializeField] private float invincibleTime;
        [SerializeField] private float invincibleTimer;
        [SerializeField] private bool isInvincible;
        [SerializeField] private bool isHitted;
        [SerializeField] private float HittedTime;
        [SerializeField] private float HittedTimer;

        private bool isDash;
        [SerializeField] private float DashDuration;
        [SerializeField] private float DashDelay;
        [SerializeField] private float DashLength;
        private float dashCoolTimer;
        private float dashDurationTimer;

        private bool isDie;

        public float Sin;

        public float Soul { get => _soul; set => _soul = value; }

        public float Stamina { get => _stamina; set => _stamina = value; }

        private bool directionMode = false;

        [SerializeField]
        private bool DebugMode;

        [field: SerializeField]
#if UNITY_EDITOR
        [field: ShowCase]
#endif
        public PlayerState State { get; protected set; }

        public override float EntitySizeX => playerCollider.bounds.size.x / 2;
        public override float EntitySizeY => playerCollider.bounds.size.y / 2;

        private readonly Cooltime _cooltime = new Cooltime();

        protected override void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
            playerCollider = GetComponent<Collider2D>();
            _audioSource = GetComponent<AudioSource>();
            _jumpCount = _jumpCountMax;
            _comboCount = 0;
            _isAttack = false;
            isInvincible = false;
        }

        protected override void FixedUpdate()
        {
            if (directionMode) return;
            if (_isGround)
            {
                Move(xMove);
            }
            else
            {
                AirMove(xMove);
            }
            Jump();
            Dash();
        }

        protected override void OnEnable()
        {

        }

        protected override void Start()
        {
            var mainCam = Camera.main;
            curtainSprite = mainCam.transform.GetComponentInChildren<SpriteRenderer>();
        }

        protected override void Update()
        {
            if (isInvincible)
            {
                invincibleTimer += Time.deltaTime;
                if(invincibleTimer > invincibleTime)
                {
                    invincibleTimer = 0;
                    isInvincible = false;
                }
            }

            if (isHitted)
            {
                HittedTimer += Time.deltaTime;
                if(HittedTimer > HittedTime)
                {
                    HittedTimer = 0;
                    isHitted = false;
                    State = PlayerState.Idle;
                }
            }

            _comboTimer += Time.deltaTime;
            if (_comboTimer > _comboDelay)
            {
                _isAttack = false;
                _comboCount = 0;
                if (playerRigidbody.velocity.x == 0)
                {
                    if (State != PlayerState.Jump && State != PlayerState.Jump2 && _isGround  && State != PlayerState.Dash && State != PlayerState.Hit && !isDie)
                    {
                        State = PlayerState.Idle;
                    }
                }
                else
                {
                    if (State != PlayerState.Jump && State != PlayerState.Jump2 && _isGround  && State != PlayerState.Dash && State != PlayerState.Hit && !isDie)
                    {
                        if (!_audioSource.isPlaying)
                        {
                            _audioSource.clip = _audioClips[0];
                            _audioSource.pitch = 4;
                            _audioSource.Play();
                        }
                        State = PlayerState.Run;
                    }
                }
            }

            if (!directionMode)
                xMove = Input.GetAxisRaw("Horizontal");

            int layerMask = 1 << LayerMask.NameToLayer("Ground");
            if(Physics2D.Raycast(transform.position, -transform.up , _rayCastDistance, layerMask))
            {
                //Debug.Log("isGround");
                _isGround = true;
                _jumpCount = _jumpCountMax;
            }
            else
            {
                //Debug.Log("isNotGround");
                _isGround = false;
            }

            if (Input.GetButtonDown("Fire1") && AttackDelay < _comboTimer && State != PlayerState.Hit && !isDie)
            {
                if (!directionMode)
                    Attack();
            }

            if (xMove > 0 && State != PlayerState.Attack1 && State != PlayerState.Attack2 && State != PlayerState.Attack3 && !isHitted && !isDie)
            {
                transform.localScale = new Vector2(0.1275f, 0.1275f);
                xDir = 1f;
            }
            else if (xMove < 0 && State != PlayerState.Attack1 && State != PlayerState.Attack2 && State != PlayerState.Attack3 && !isHitted && !isDie)
            {
                transform.localScale = new Vector2(-0.1275f, 0.1275f);
                xDir = -1f;
            }

            if (Input.GetButtonDown("Jump") && State != PlayerState.Hit && !isDie)
            {
                _isJumping = true;
            }

            dashCoolTimer += Time.deltaTime;
            if (!directionMode)
            {
                if (Input.GetButtonDown("Dash") && dashCoolTimer > DashDelay && !isHitted && !isDie)
                {
                    _audioSource.pitch = 4;
                    _audioSource.clip = _audioClips[1];
                    _audioSource.Play();
                    isDash = true;
                }
            }

            if(playerRigidbody.velocity.y < 0f && !_isGround && !_isAttack && State != PlayerState.Dash && !isHitted && !isDie)
            {
                State = PlayerState.Fall;
            }

            if (isDie)
            {
                State = PlayerState.Die;
            }
            SetCurrentAnimation(State);
        }

        private void OnDrawGizmos()
        {
            if (!DebugMode) return;
            Debug.DrawRay(transform.position, -transform.up * _rayCastDistance, Color.red);
        }

        public override void OnDamagedBy(Entity source, float damage)
        {
            if (!isInvincible && !isDash && !isDie)
            {
                var result = playerGuard.TryGuard(source, damage);
                Hp = Hp - result.Damage;
                Stamina = Stamina - result.StaminaDamage;
                if (Hp < 0)
                    Hp = 0;

                if (!result.Guarded.HasFlag(PlayerGuardResultType.Guard))
                {
                    isHitted = true;
                    isInvincible = true;
                    State = PlayerState.Hit;
                    _audioSource.clip = _audioClips[6];
                    _audioSource.pitch = 1;
                    _audioSource.Play();
                }

                if (Hp <= 0)
                    OnDeath();
            }
        }

        protected override void OnDeath()
        {
            isDie = true;
            curtainSprite.DOFade(1f, 3f);
            gameObject.GetComponent<MeshRenderer>().sortingLayerName = "Overlay";
            gameObject.GetComponent<MeshRenderer>().sortingOrder = 100;
            StartCoroutine(CoroutineUtility.SlowTime(0.5f, 4));
            GetComponent<PlayerDied>().OnDied();
        }

        private void Move(float xMove)
        {
            if (isDie) return;
            if (isDash) return;
            if (_isAttack)
            {
                playerRigidbody.velocity = Vector2.zero;
            }
            else if (isHitted)
            {
                playerRigidbody.AddForce(new Vector2(1 * -xDir, 1), ForceMode2D.Impulse);
            }
            else
            {
                playerRigidbody.velocity = new Vector2(xMove * _moveSpeed, playerRigidbody.velocity.y);
            }
        }

        private void AirMove(float xMove)
        {
            if (isDie) return;
            if (!isHitted)
            {
                playerRigidbody.velocity = new Vector2(xMove * _moveSpeedInAir, playerRigidbody.velocity.y);
            }
        }

        private void Jump()
        {
            if (isDie) return;
            if (isDash) return;
            if (_isJumping)
            {
                _isJumping = false;
                if (_isGround)
                {
                    playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, _jumpSpeed);
                    State = PlayerState.Jump;
                }
                else if(_jumpCount > 0)
                {
                    _jumpCount--;
                    playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, _jumpSpeed);
                    State= PlayerState.Jump2;
                }
            }
        }

        private void Attack()
        {
            if (isDie) return;
            if (isDash) return;
            GameObject comb = null;
            if (_isGround)
            {
                playerRigidbody.velocity = Vector2.zero;
                if (_comboCount == 0)
                {
                    _audioSource.pitch = 1;
                    _audioSource.clip = _audioClips[2];
                    _audioSource.Play();
                    State = PlayerState.Attack1;
                    if(xDir > 0)
                    {
                        comb = Instantiate(combo_1, new Vector3(transform.position.x + 3f, transform.position.y + 3.4f, transform.position.z), Quaternion.identity);
                    }
                    else
                    {
                        comb = Instantiate(combo_1, new Vector3(transform.position.x - 3f, transform.position.y + 3.4f, transform.position.z), Quaternion.identity);
                    }
                    _comboCount++;
                    _comboTimer = 0;
                }
                else if (_comboCount == 1 && _comboTimer < _comboDelay)
                {
                    _audioSource.pitch = 1;
                    _audioSource.clip = _audioClips[3];
                    _audioSource.Play();
                    State = PlayerState.Attack2;
                    if (xDir > 0)
                    {
                        comb = Instantiate(combo_2, new Vector3(transform.position.x + 3f, transform.position.y + 3.4f, transform.position.z), Quaternion.identity);
                    }
                    else
                    {
                        comb = Instantiate(combo_2, new Vector3(transform.position.x - 3f, transform.position.y + 3.4f, transform.position.z), Quaternion.identity);
                    }
                    _comboCount++;
                    _comboTimer = 0;

                }
                else if (_comboCount == 2 && _comboTimer < _comboDelay)
                {
                    _audioSource.pitch = 1;
                    _audioSource.clip = _audioClips[4];
                    _audioSource.Play();
                    State = PlayerState.Attack3;
                    if (xDir > 0)
                    {
                        comb = Instantiate(combo_3, new Vector3(transform.position.x + 3f, transform.position.y + 3.4f, transform.position.z), Quaternion.identity);
                    }
                    else
                    {
                        comb = Instantiate(combo_3, new Vector3(transform.position.x - 3f, transform.position.y + 3.4f, transform.position.z), Quaternion.identity);
                    }
                    _comboCount = 0;
                    _comboTimer = -0.3f;
                }
            }
            else
            {
                _audioSource.pitch = 1;
                _audioSource.clip = _audioClips[2];
                _audioSource.Play();
                State = PlayerState.AirAttack;
                if (xDir > 0)
                {
                    comb = Instantiate(combo_3, new Vector3(transform.position.x + 3f, transform.position.y + 3.4f, transform.position.z), Quaternion.identity);
                }
                else
                {
                    comb = Instantiate(combo_3, new Vector3(transform.position.x - 3f, transform.position.y + 3.4f, transform.position.z), Quaternion.identity);
                }
                _comboCount = 0;
                _comboTimer = 0;
            }
            if (comb != null)
                Blow.SetAllBlowSourceAs(comb, this);
            _isAttack = true;
        }

        private void Dash()
        {
            if (isDie) return;
            if (!isDash) return;
            dashCoolTimer = 0f;
            dashDurationTimer += Time.deltaTime;
            if(dashDurationTimer < DashDuration)
            {
                playerRigidbody.velocity = new Vector2(xDir * DashLength, 0);
                State = PlayerState.Dash;
            }
            else
            {
                State = PlayerState.Idle;
                dashDurationTimer = 0;
                isDash = false;
            }
        }


        void SetCurrentAnimation(PlayerState _state)
        {
            switch (_state)
            {
                case PlayerState.Idle:
                    AsyncAnimation(animClip[(int)PlayerState.Idle], true, 1f, 0);
                    break;
                case PlayerState.Run:
                    AsyncAnimation(animClip[(int)PlayerState.Run], true, 1f, 0);
                    break;
                case PlayerState.Attack1:
                    AsyncAnimation(animClip[(int)PlayerState.Attack1], false, 1f, 0);
                    break;
                case PlayerState.Attack2:
                    AsyncAnimation(animClip[(int)PlayerState.Attack2], false, 1f, 0);
                    break;
                case PlayerState.Attack3:
                    AsyncAnimation(animClip[(int)PlayerState.Attack3], false, 1f, 0);
                    break;
                case PlayerState.AirAttack:
                    AsyncAnimation(animClip[(int)PlayerState.AirAttack], false, 1f, 0);
                    break;
                case PlayerState.UltimateSkill:
                    AsyncAnimation(animClip[(int)PlayerState.UltimateSkill], false, 1f, 0);
                    break;
                case PlayerState.Jump:
                    AsyncAnimation(animClip[(int)PlayerState.Jump], false, 1f, 0);
                    break;
                case PlayerState.Jump2:
                    AsyncAnimation(animClip[(int)PlayerState.Jump2], false, 1f, 0);
                    break;
                case PlayerState.Fall:
                    AsyncAnimation(animClip[(int)PlayerState.Fall], false, 1f, 0);
                    break;
                case PlayerState.Hit:
                    AsyncAnimation(animClip[(int)PlayerState.Hit], false, 1f, 0);
                    break;
                case PlayerState.Dash:
                    AsyncAnimation(animClip[(int)PlayerState.Dash], false, 1f, 0);
                    break;
                case PlayerState.Die:
                    AsyncAnimation(animClip[(int)PlayerState.Die], false, 1f, 0);
                    break;
            }
        }
        void AsyncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale, int track)
        {
            if (animClip.name.Equals(CurrentAnimation)) return;

            skeletonAnimation.state.SetAnimation(track, animClip, loop).TimeScale = timeScale;
            skeletonAnimation.loop = loop;
            skeletonAnimation.timeScale = timeScale;

            CurrentAnimation = animClip.name;
        }

        public void SetDirectionMode(bool enabled)
        {
            directionMode = enabled;
            xMove = 0f;
        }
    }
}
