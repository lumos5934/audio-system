using System.Collections;
using UnityEngine;

namespace LLib
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioData _data;
        [SerializeField] private float _fadeInDuration   = 0f;
        [SerializeField] private float _fadeOutDuration  = 0f;
        [SerializeField] private bool _isSourceRoot = false;

        private AudioHandle _handle = AudioHandle.Invalid;

        private void OnDisable()
        {
            Stop();
        }
        
        public void Play()
        {
            _handle = _isSourceRoot ? 
                AudioManager.Instance.Play(_data, transform, _fadeInDuration) : 
                AudioManager.Instance.Play(_data, _fadeInDuration);
        }

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


