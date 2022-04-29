// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

namespace Sevens.Utils
{
    using TE = UnityEngine.Time;

    public class TimeElapsingRecord
    {
        public float Time
        {
            get; private set;
        }

        public TimeElapsingRecord()
            => Time = TE.time;

        public TimeElapsingRecord(float time)
            => Time = time;

        public void Set(float time)
            => Time = time;

        public void UpdateAsNow()
            => Set(TE.time);

        public bool IsElapsed(float gap)
            => (TE.time - Time) >= gap;

        public bool Next(float gap)
        {
            if (!IsElapsed(gap))
                return false;
            UpdateAsNow();
            return true;
        }
    }
}
