namespace Sevens.Entities
{
    public class AnimationPlayOption
    {
        public int Track;
        public bool Loop;
        public float TimeScale;
        public string AnimationName = null;

        public AnimationPlayOption(string animationName, bool loop = false, int track = 0, float timeScale = 1f)
        {
            AnimationName = animationName;
            Loop = loop;
            Track = track;
            TimeScale = timeScale;
        }
    }
}
