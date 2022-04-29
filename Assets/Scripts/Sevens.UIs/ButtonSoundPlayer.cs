using Assets.Scripts.Sevens.UIs;
using UnityEngine;

namespace Sevens.UIs
{
    public class ButtonSoundPlayer : StateMachineBehaviour
    {
        private int _hash;
        public string Name;
        public bool CanBePlayedAlways;

        public AudioClip AudioClip;

        private void OnEnable()
        {
            _hash = Animator.StringToHash(Name);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var audioSource = animator.GetComponent<AudioSourceLeecher>();

            if (stateInfo.shortNameHash == _hash)
                if (AudioClip != null)
                    audioSource?.Source.PlayOneShot(AudioClip);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
