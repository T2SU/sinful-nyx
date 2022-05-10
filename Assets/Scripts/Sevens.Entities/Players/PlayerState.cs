using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevens.Entities.Players
{
    public enum PlayerState
    {
        Idle, Run, Attack, UltimateSkill, Jump, Fall, Hit, Dash ,Die, Guard
    }

    public static class PlayerStates
    {
        public static bool IsJumpableState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                case PlayerState.Run:
                case PlayerState.Jump:
                case PlayerState.Fall:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAttackableState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Attack:
                case PlayerState.Idle:
                case PlayerState.Run:
                case PlayerState.Jump:
                case PlayerState.Fall:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAirState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Jump:
                case PlayerState.Fall:
                    return true;
                default:
                    return false;
            }
        }

        public static bool HasUniqueAnimationState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Attack:
                case PlayerState.Dash:
                case PlayerState.Hit:
                case PlayerState.Die:
                case PlayerState.Guard:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsLevitationState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Attack:
                case PlayerState.Dash:
                    return true;
                default:
                    return false;
            }
        }
    }
}
