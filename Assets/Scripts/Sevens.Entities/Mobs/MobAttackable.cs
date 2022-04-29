using Sevens.Entities.Players;
using Sevens.Utils;
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
            if (_currentAttack != null)
                _currentAttack.Cancel(this);
        }

        public void EndAttack(bool canceled)
        {
            if (canceled)
                AttackCoroutines.KillAll();
            Mob.Cooltime.Set(_currentAttack.Name);
            Mob.ChangeState(MobState.Idle, true);
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

        private bool IsAttackingState(PlayerState state) =>
            state == PlayerState.Attack1
            || state == PlayerState.Attack2
            || state == PlayerState.Attack3;

        private void ThinkEvasion()
        {
            if (!IsAttackingState(_lastPlayerState) && IsAttackingState(Player.State))
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
            var filtered = attacks
                   .Where(atk => atk.IsAvailable(Player, Mob))
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
            _currentAttack.Execute(Player, this);
        }

        private void OnDisable()
        {
            AttackCoroutines.KillAll();
        }
    }
}
