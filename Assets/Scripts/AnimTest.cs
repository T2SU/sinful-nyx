using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class AnimTest : MonoBehaviour
    {
        private AudioSource _audioSource;
        private Animator _animController;

        public AudioClip AudioClip;
        public string StateName;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _animController = GetComponent<Animator>();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(30, 30, 200, 100), "테스트"))
            {
                if (AudioClip != null)
                    _audioSource.PlayOneShot(AudioClip);
                _animController.Play(StateName);
            }
            if (GUI.Button(new Rect(30, 250, 200, 100), "리셋"))
            {
                _animController.Play("elite idle");
            }
        }
    }
}
