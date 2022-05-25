// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using UnityEngine;

namespace Sevens.Speeches
{
    public class DialogueRunner : CustomYieldInstruction
    {
        public static bool Running { get; private set; }

        public DialogueRunner(Dialogue dialogue)
        {
            Running = true;
            DialogueManager.Instance.DialogueBase.SendSpeech(dialogue);
        }

        public override bool keepWaiting
        {
            get
            {
                if (!DialogueManager.Instance.DialogueBase.Completed)
                    return true;
                Running = false;
                return false;
            }
        }
    }
}
