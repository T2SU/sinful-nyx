using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;
using Sevens.Utils;

namespace Sevens.Entities.Players
{
    public class Player_ : LivingEntity
    {
        private Rigidbody2D _rigidbody;
        private AudioSource _audioSource;
        private SkeletonAnimation _skeletonAnimation;
        private string _currentAnimation;
        private Collider2D _collider;
        private PlayerGuard _guardComponent;
        private PlayerDied _dieComponent;
        private bool _directionMode;

        public Dictionary<int, Dictionary<string, string>> _questInfo
             = new Dictionary<int, Dictionary<string, string>>();

        // protected 필드
        [SerializeField]
        protected NamedObject<AudioClip>[] _audioClips;

        [SerializeField]
        protected NamedObject<AnimationReferenceAsset>[] animClips;
        // public 필드
        public float Stamina;
        public float StaminaMax;
        public float Sin; // 죄악 수치
        public float Soul; // 영혼 게이지 (궁극기 게이지)

        // 생성자

        // 프로퍼티

        public override float EntitySizeX => _collider.bounds.size.x / 2;

        public override float EntitySizeY => _collider.bounds.size.y / 2;

        public Vector2 Velocity { get; set; }

        public bool IsDirectionMode => _directionMode;

        [field: SerializeField]
        public PlayerState State { get; private set; }
        
        // public 메서드 / 함수
        public void ChangeState(PlayerState state)
        {
            State = state;
        }

        // protected 메서드 / 함수

        // private 메서드 / 함수

        public override void OnDamagedBy(Entity source, float damage)
        {
            var guardResult = _guardComponent.TryGuard(source, damage);
            if (guardResult.Guarded.HasFlag(PlayerGuardResultType.Guard))
            {
                Hp -= guardResult.Damage;
                return;
            }
            
            Hp = Mathf.Max(0f, Hp - damage);

            if (State != PlayerState.Die)
            {
                if (Mathf.Approximately(Hp, 0f))
                    OnDeath();
            }
        }

        public void SetQuestInfo(int questIndex, string key, string value)
        {
            if (!_questInfo.TryGetValue(questIndex, out var dict))
                dict = _questInfo[questIndex] = new Dictionary<string, string>();
            dict[key] = value;
        }

        public string GetQuestInfo(int questIndex, string key)
        {
            if (TryGetQuestInfo(questIndex, key, out var val))
                return val;
            return null;
        }

        public bool TryGetQuestInfo(int questIndex, string key, out string value)
        {
            if (_questInfo.TryGetValue(questIndex, out var dict))
                return dict.TryGetValue(key, out value);
            value = null;
            return false;
        }

        public void TryPlayAnimation(string animName, bool loop, float timeScale, int track)
        {
            AnimationReferenceAsset animClip = animClips.FindByName(animName);

            if (animClip == null) return;
            if (animClip.name.Equals(_currentAnimation)) return;

            _skeletonAnimation.state.SetAnimation(track, animClip, loop).TimeScale = timeScale;
            _skeletonAnimation.loop = loop;
            _skeletonAnimation.timeScale = timeScale;

            _currentAnimation = animClip.name;
        }

        public void PlayAudio(string audioName)
        {
            AudioClip audioClip = _audioClips.FindByName(audioName);
            
            if (audioClip == null) return;
            _audioSource.PlayOneShot(audioClip);
        }

        public void SetDirectionMode(bool mode)
        {
            _directionMode = mode;
        }

        protected override void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();

            _guardComponent = GetComponent<PlayerGuard>();
        }

        protected override void FixedUpdate()
        {
            _rigidbody.velocity = Velocity;
        }

        protected override void OnDeath()
        {
            if (State != PlayerState.Die)
                _dieComponent.OnDied();
        }

// 1. 실제 이동 (키 입력에 연관되어지지 않음)
// 2. 실제 점프 (키 입력에 연관되어지지 않음)
// 3. Spine 애니메이션 재생
// 4. 플레이어 상태(State) 관리
// 5. HP, 스태미나, 죄악 수치
// 6. 피격 처리 (방어 및 패링 컴포넌트가 있다면, 해당 컴포넌트에 처리를 먼저 넘겨봄.)
// 7. 게임 진행 데이터
    }
}