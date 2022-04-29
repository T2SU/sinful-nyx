// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Sevens.Speeches
{
    public class SpeechSystem : MonoBehaviour
    {
        [SerializeField]
        private Transform _speechRoot;

        private RectTransform _rectTransform;

        private bool _isDisplaying;
        private List<Tweener> _tweens = new List<Tweener>();
        private int _completedTweens;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void DisplaySpeech(string text, Font font, int fontSize, Vector2 letterSpacing)
        {
            _isDisplaying = true;
            _tweens.Clear();
            _completedTweens = 0;
            foreach (Transform t in _speechRoot)
                GameObject.Destroy(t.gameObject);

            var width = _rectTransform.sizeDelta.x;
            var height = _rectTransform.sizeDelta.y;

            // TODO check performance by profiling
            font.RequestCharactersInTexture(text, fontSize, FontStyle.Normal);
            font.RequestCharactersInTexture(text, fontSize, FontStyle.Bold);

            var i = Vector2.zero;
            var color = Color.white;
            var style = FontStyle.Normal;
            var sequence = 0;

            for (int k = 0; k < text.Length; ++k)
            {
                var ch = text[k];
                if (ch == '#') // custom rich text
                {
                    var rem = text.Length - k;

                    bool CheckMatched(string m)
                    {
                        if (rem < 1 + m.Length) // # 포함
                            return false;
                        if (text.Substring(1 + k, m.Length).ToLower() != m) // # 제외, m 길이
                            return false;
                        k += m.Length; // m 길이
                        return true;
                    }

                    if (CheckMatched("yellow"))     color = Color.yellow;
                    else if (CheckMatched("red"))   color = Color.red;
                    else if (CheckMatched("blue"))  color = Color.blue;
                    else if (CheckMatched("green")) color = Color.green;
                    else if (CheckMatched("gray"))  color = Color.gray;
                    else if (CheckMatched("white")) color = Color.white;
                    else if (CheckMatched("black")) color = Color.black;
                    else if (CheckMatched("bold"))  style = FontStyle.Bold;
                    else if (CheckMatched("normal"))style = FontStyle.Normal;
                    else goto Normal;
                    continue;
                }

                Normal:

                void BreakLine(ref Vector2 i)
                {
                    i.x = 0;
                    i.y -= fontSize + letterSpacing.y;
                }

                if (ch == '\n')
                {
                    BreakLine(ref i);
                    continue;
                }
                
                // should be called 'font.RequestCharactersInTexture()' before this statement.
                if (!font.GetCharacterInfo(ch, out var cinfo, fontSize, style)) 
                    continue;

                var letterWidth = cinfo.advance;
                if (i.x + letterWidth > width)
                    BreakLine(ref i);
                if (i.y > height)
                {
                    Debug.LogWarning("Speech is overflowed!");
                    return;
                }
                NewLetter(i, ch, font, fontSize, style, color, out var txtComp);
                if (ch != ' ')
                {
                    _tweens.Add(
                        txtComp
                        .DOFade(1, 0.25f)
                        .SetDelay(0.08f * sequence++)
                        .SetUpdate(true)
                        .OnComplete(OnLetterFadeInCompleted));
                }
                i.x += letterWidth + letterSpacing.x;
            }
        }

        public bool CompleteSpeechForcely()
        {
            if (_isDisplaying)
            {
                foreach (var tween in _tweens)
                    tween.Complete();
                return false;
            }
            return true;
        }

        private GameObject NewLetter(Vector2 pos, char ch, Font font, int fontSize, FontStyle style, Color color, out Text comp)
        {
            var letter = Instantiate(DialogueManager.Instance.DialogueBase.LetterPrefab, _speechRoot);
            var trans = letter.GetComponent<RectTransform>();
            trans.anchoredPosition = pos;
            trans.sizeDelta = new Vector2(fontSize, fontSize);

            comp = letter.GetComponent<Text>();
            comp.fontSize = fontSize;
            comp.fontStyle = style;
            // comp.alignByGeometry = true;
            if (ch != ' ')
                color.a = 0;
            comp.text = new string(ch, 1);
            comp.color = color;

            return letter;
        }

        private void OnLetterFadeInCompleted()
        {
            if (++_completedTweens < _tweens.Count)
                return;
            _isDisplaying = false;
        }
    }
}
