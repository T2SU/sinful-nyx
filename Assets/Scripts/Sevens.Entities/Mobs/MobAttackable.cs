using DG.Tweening;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    [RequireComponent(typeof(Mob))]
    public class MobAttackable : MonoBehaviour
    {
        private const string NextAttackKey = "NextAttack";

        [SerializeField]
        private MobAttackBase[] _attacks;

        [SerializeField]
        private MobAttackBase[] _evasions;

        private MobAttackBase _currentAttack;
        private PlayerState _lastPlayerState;

        public CoroutineMan AttackCoroutines { get; private set; }

        public Mob Mob { get; private set; }

        public Player Player { get; private set; }

        public void CancelAttack()
        {
            if (_currentAttack == null)
                return;
            _currentAttack.ClearObjects();
            EndAttack(true);
            _currentAttack = null;
        }

        private void Awake()
        {
            Mob = GetComponent<Mob>();
            AttackCoroutines = new CoroutineMan(this);
        }

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        private void Update()
        {
            if (Mob.State != MobState.Move && Mob.State != MobState.Idle)
                return;
            ThinkEvasion();
            ThinkAttack();
        }

        private void ThinkEvasion()
        {
            if (_lastPlayerState != PlayerState.Attack && Player.State == PlayerState.Attack)
                AttemptAttack(_evasions);
            _lastPlayerState = Player.State;
        }

        private void ThinkAttack()
        {
            if (Mob.IsDelayedByChangedState(1.0f))
                return;
            if (Mob.Cooltime.IsBeing(NextAttackKey, 2.0f))
                return;
            AttemptAttack(_attacks);
        }

        private void AttemptAttack(MobAttackBase[] attacks)
        {
            if (attacks.Any(a => a == null))
                Debug.LogWarning($"{name} 객체에 MobAttack이 할당되지 않은 요소가 있습니다. 모든 요소는 반드시 None이 아니어야 합니다.");
            var filtered = attacks
                   .Where(atk => atk && atk.IsAvailable(Player, Mob))
                   .GroupBy(atk => atk.Priority)
                   .OrderByDescending(group => group.Key)
                   .FirstOrDefault();
            if (filtered == null)
                return;
            _currentAttack = Randomizer.PickOneRand(filtered);
            if (_currentAttack == null)
                return;
            Mob.ChangeState(MobState.Attack);
            Mob.Cooltime.Set(NextAttackKey);
            _currentAttack.Mob = Mob;

            if (_currentAttack.InvincibleWhileAttack)
                Mob.Invincible = true;

            AttackCoroutines.Register(_currentAttack.Name, ExecuteCoroutine());
        }

        private IEnumerator ExecuteCoroutine()
        {
            _currentAttack.OnExecute(Player, this);
            yield return _currentAttack.Attack(Player, Mob, AttackCoroutines);
            _currentAttack.ClearObjects();
            EndAttack(false);
            _currentAttack = null;
        }

        private void OnDisable()
        {
            AttackCoroutines.KillAll();
        }

        private void EndAttack(bool canceled)
        {
            if (canceled)
                AttackCoroutines.KillAll();
            Mob.Cooltime.Set(_currentAttack.Name);
            Mob.ChangeState(MobState.Idle, true);
            if (_currentAttack.InvincibleWhileAttack)
                Mob.Invincible = false;
            if (canceled)
                _currentAttack.OnCancel(this);
            _currentAttack.OnFinish(this);
        }
    }
}
