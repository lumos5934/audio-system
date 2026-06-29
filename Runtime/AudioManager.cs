using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace LLib
{
    public partial class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer _mixer;

        private Dictionary<AudioMixerGroup, List<AudioHandle>> _activeHandles = new();
        private Pool _pool;
        
        public static AudioManager Instance { get; private set; }
        
        private void Awake()    
        {
            if (_mixer == null)
            {
                enabled = false;
                return;
            }
            
            if (Instance != null)
            {
                Destroy(gameObject); 
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _pool = new Pool();
        }
   
        public AudioHandle Play(AudioData data, float fadeIn = 0f)
        {
            return Play(data, null, Vector3.zero, fadeIn);
        }

        public AudioHandle Play(AudioData data, Vector3 position, float fadeIn = 0f)
        {
            return Play(data, null, position, fadeIn);
        }
        
        public AudioHandle Play(AudioData data, Transform root, float fadeIn = 0f)
        {
            return Play(data, root, Vector3.zero, fadeIn);
        }
        
        private AudioHandle Play(AudioData data, Transform root, Vector3 pos, float fadeDuration = 0f)
        {
            var handle = GetHandle(data);
            if(handle == null)
                return AudioHandle.Invalid;
            
            var audioSource = handle.AudioSource;
            if (root != null)
            {
                audioSource.transform.SetParent(root);
                audioSource.transform.localPosition = Vector3.zero;
            }
            else
            {
                audioSource.transform.position = pos;
            }
            audioSource.Play();
            
            if (fadeDuration > 0f)
            {
                audioSource.volume = 0f;
                FadeTo(handle, handle.BaseVolume, fadeDuration);
            }

            if (!data.Loop)
            {
                StartCoroutine(AutoReleaseRoutine(handle));
            }
            
            return handle;
        }

        private AudioHandle GetHandle(AudioData data)
        {
            var handle = _pool.Get(data);
            if(handle == null)
                return AudioHandle.Invalid;
            
            var mixerGroup = data.MixerGroup;
            if (!_activeHandles.ContainsKey(mixerGroup))
            {
                _activeHandles[mixerGroup] = new();
            }
          
            _activeHandles[mixerGroup].Add(handle);
            
            return handle;
        }
        
        public void Stop(AudioMixerGroup mixerGroup, float fade = 0f)
        {
            ForeachGroup(mixerGroup, handle => handle.Stop(fade));
        }

        public void Pause(AudioMixerGroup mixerGroup)
        {
            ForeachGroup(mixerGroup, handle => handle.Pause());
        }

        public void UnPause(AudioMixerGroup mixerGroup)
        {
            ForeachGroup(mixerGroup, handle => handle.UnPause());
        }

        private void ForeachGroup(AudioMixerGroup mixerGroup, Action<AudioHandle> handleAction)
        {
            foreach (var pair in _activeHandles)
            {
                if (pair.Key == mixerGroup)
                {
                    foreach (var handle in pair.Value)
                    {
                        handleAction?.Invoke(handle);
                    }

                    return;
                }
            }
        }

        public void FadeTo(AudioHandle handle, float target, float duration, Action onComplete = null)
        {
            StartCoroutine(FadeRoutine(handle, target, duration, onComplete));
        }

        public void SetVolume(AudioMixerGroup mixerGroup, float normalizedVolume)
        {
            float value = normalizedVolume > 0.0001f ? Mathf.Log10(normalizedVolume) * 20f : -80f;
            _mixer.SetFloat(mixerGroup.name, value);
        }

        public float GetVolume(AudioMixerGroup mixerGroup)
        {
            _mixer.GetFloat(mixerGroup.name, out var dB);
            return Mathf.Pow(10, dB / 20f);
        }
        
        private IEnumerator FadeRoutine(AudioHandle handle, float targetVolume, float duration, Action onComplete)
        {
            float start = handle.Volume;
            for (float t = 0; t < duration && handle.IsValid; t += Time.unscaledDeltaTime)
            {
                handle.Volume = Mathf.Lerp(start, targetVolume, t / duration);
                yield return null;
            }

            if (handle.IsValid)
            {
                handle.Volume = targetVolume;
            }
            
            onComplete?.Invoke();
        }

        private IEnumerator AutoReleaseRoutine(AudioHandle handle)
        {
            while (handle.IsValid)
            {
                yield return null;
            }

            _activeHandles[handle.Data.MixerGroup].Remove(handle);
            _pool.Release(handle);
        }
    }
}
