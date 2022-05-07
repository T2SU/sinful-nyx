using UnityEngine;

namespace Sevens.Interfaces
{
    public class PopupWait : CustomYieldInstruction
    {
        public override bool keepWaiting => !Selected;

        public bool Selected = false;
        public int SelectedIndex = -1;
    }
}
