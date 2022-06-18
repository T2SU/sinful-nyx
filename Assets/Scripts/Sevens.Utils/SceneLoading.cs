using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sevens.Utils
{
    public class SceneLoading
    {
        public string NextScene { get; }

        public GameObject LoadingScreen { get; }

        public bool IsDone { get; private set; }

        private readonly CoroutineMan _coroutine;

        public SceneLoading(string nextScene, CoroutineMan coroutineMan)
        {
            _coroutine = coroutineMan;
            NextScene = nextScene;
            var prefab = Resources.Load<GameObject>(Prefabs.LoadingScreen);
            LoadingScreen = GameObject.Instantiate(prefab);
            PrepareLoadingScreen();
            Begin();
        }

        private void PrepareLoadingScreen()
        {
            GameObject.DontDestroyOnLoad(LoadingScreen);
            var canvasGroup = LoadingScreen.GetComponentInChildren<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = LoadingScreen.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.0f;
        }

        private void Begin()
        {
            _coroutine.Register("ScreenTransfer", LoadingOperation());
        }

        private IEnumerator LoadingOperation()
        {
            var canvasGroup = LoadingScreen.GetComponent<CanvasGroup>();
            var loadingComponent = LoadingScreen.GetComponent<SceneLoadingComponent>();

            yield return canvasGroup.DOFade(1.0f, 0.5f).WaitForCompletion();

            var operations = new List<AsyncOperation>()
            {
                SceneManager.LoadSceneAsync(NextScene, LoadSceneMode.Single)
            };
            foreach (var operation in operations)
                operation.allowSceneActivation = false;

            // 1초짜리 가짜
            float fakeDuration = 0.3f;
            float begin = Time.time;
            float end = begin + fakeDuration;

            loadingComponent.Progress = 0.0f;
            while (!operations.TrueForAll(o => o.progress >= 0.9f) || end > Time.time)
            {
                float now = operations.Select(o => o.progress).Sum() + Mathf.Min(fakeDuration, Time.time - begin);
                float total = operations.Count + fakeDuration;

                float progress = now / total;
                loadingComponent.Progress = progress;
                yield return null;
            }
            loadingComponent.Progress = 1.0f;

            foreach (var operation in operations)
                operation.allowSceneActivation = true;

            loadingComponent.SetDone();
            yield return new WaitForSeconds(0.25f);
            yield return canvasGroup.DOFade(0.0f, 0.25f).WaitForCompletion();
            GameObject.Destroy(LoadingScreen.gameObject);

            IsDone = true;
        }
    }
}
