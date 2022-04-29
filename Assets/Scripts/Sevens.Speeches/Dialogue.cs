// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using UnityEngine;

namespace Sevens.Speeches
{
    public class Dialogue : CustomYieldInstruction
    {
        public static bool Running { get; private set; }

        public Dialogue(string speaker, Sprite speakerAvatar, string text)
        {
            Running = true;
            DialogueManager.Instance.DialogueBase.SendSpeech(speaker, speakerAvatar, text);
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
