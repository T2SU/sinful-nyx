using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public abstract class EntityDestroyCallbackBase : MonoBehaviour
    {
        public abstract void Execute(Entity entity, Entity killedBy);
    }
}
