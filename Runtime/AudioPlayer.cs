using System.Collections;
using UnityEngine;

namespace LLib
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private float _fadeInDuration   = 0f;
        [SerializeField] private float _fadeOutDuration  = 0f;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _followTransform = false;

        private AudioHandle _handle = AudioHandle.Invalid;

  
        private void OnEnable()
        {
            if (_playOnEnable)
            {
                StartCoroutine(PlayOnEnableDelayRoutine());
            }
        }

        private void OnDisable()
        {
            Stop();   
        }

        private void Update()
        {
            if (_followTransform &&
                _handle.IsValid &&
                _handle.AudioSource != null)
            {
                _handle.AudioSource.transform.position = transform.position;
            }
        }

        
        [ContextMenu("Play")]
        public void Play()
        {
            if (_handle.IsValid) 
                return;
            
            _handle = AudioManager.Instance.PlayAt(_id, transform.position, _fadeInDuration);
        }

        
        [ContextMenu("Stop")]
        public void Stop()
        {
            _handle.Stop(_fadeOutDuration);
            _handle = AudioHandle.Invalid;
        }

        public void Pause()
        {
            _handle.Pause();
        }

        public void UnPause()
        {
            _handle.UnPause();
        }

        public void SetVolume(float normalizedVolume)
        {
            _handle.Volume = normalizedVolume;
        }


        private IEnumerator PlayOnEnableDelayRoutine()
        {
            yield return new WaitForEndOfFrame();
            
            Play();
        }
    }
}


