using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace Sevens.Effects
{
    public class SpineAnimationPlayer : MonoBehaviour
    {
        public SkeletonAnimation Skeleton;
        public string AnimName;
        public float Delay;
        public float TimeScale;

        private Spine.AnimationState _animationState;

        private void Awake()
        {
            _animationState = Skeleton.AnimationState;
        }

        private void Start()
        {
            StartCoroutine(Animation());
            Destroy(gameObject, 10);
        }

        private IEnumerator Animation()
        {
            yield return new WaitForSeconds(Delay);
            _animationState.TimeScale = TimeScale;
            _animationState.SetAnimation(0, AnimName, false);
            //Speeches.DialogueManager.Instance.StartDialogue(Hello());
        }

        //private IEnumerator Hello()
        //{
        //    yield return new Speeches.DialogueRunner("안녕0", null, "그래0");
        //    yield return new Speeches.DialogueRunner("안녕1", null, "그래1");
        //    yield return new Speeches.DialogueRunner("안녕2", null, "그래2");
        //    yield return new Speeches.DialogueRunner("안녕3", null, "그래3");
        //    yield return new Speeches.DialogueRunner("안녕4", null, "그래4");
        //}
    }
}
