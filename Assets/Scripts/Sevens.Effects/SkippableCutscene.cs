using Sevens.UIs;
using Sevens.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sevens.Effects
{
    public abstract class SkippableCutscene : MonoBehaviour
    {
        public string NextScene;

        private RadialGrowingGauge _gauge;
        private float _pressedCountdown;
        private Coroutine _cutscene;
        private bool _alreadySkipped;

        private const float _maxPressingCountdown = 1.5f;

        protected virtual void Awake()
        {
            var skipButton = Instantiate(Resources.Load<GameObject>(Prefabs.SkipButton));
            _gauge = skipButton.transform.GetComponentInChildren<RadialGrowingGauge>();
            _alreadySkipped = false;
        }

        protected Coroutine StartCutscene(IEnumerator coroutine)
        {
            return StartCoroutine(CutsceneCoroutine(coroutine));
        }

        protected Coroutine StartCutscene(string coroutine)
        {
            return StartCoroutine(CutsceneCoroutine(coroutine));
        }

        private IEnumerator CutsceneCoroutine(IEnumerator coroutine)
        {
            yield return (_cutscene = StartCoroutine(coroutine));
            OnFinishCutscene();
        }

        private IEnumerator CutsceneCoroutine(string coroutine)
        {
            yield return (_cutscene = StartCoroutine(coroutine));
            OnFinishCutscene();
        }

        private void OnFinishCutscene()
        {
            // Load Scene
            if (!string.IsNullOrEmpty(NextScene))
                SceneManagement.Instance.LoadScene(NextScene);
        }

        private void Update()
        {
            if (_pressedCountdown < _maxPressingCountdown)
            {
                if (Input.GetButton("Interaction"))
                {
                    _pressedCountdown += Time.deltaTime;
                }
                else
                    _pressedCountdown = 0;
            }
            else
            {
                if (!_alreadySkipped)
                {
                    StopCoroutine(_cutscene);
                    OnFinishCutscene();
                    _alreadySkipped = true;
                }
            }

            _gauge.UpdateRatio(Mathf.Min(_maxPressingCountdown, _pressedCountdown) / _maxPressingCountdown);
        }
    }
}
