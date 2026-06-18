using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

internal class AudioHandlePool
{
    public static AudioHandlePool Instance { get; private set; }

    private readonly HashSet<AudioHandle> _active = new();
    private readonly Transform _root;
    
    private readonly Dictionary<AudioSource, IObjectPool<AudioSource>> _sourcePools = new();
    private readonly IObjectPool<AudioHandle> _handlePool;
    
    internal AudioHandlePool(Transform root)
    {
        _root = root;
        
        _handlePool = new ObjectPool<AudioHandle>(
            createFunc: () => new AudioHandle(this),
            actionOnGet: handle => {},
            actionOnRelease: handle => handle.Unbind(),
            actionOnDestroy: null
        );
        
    }


    public AudioHandle Get(AudioGroup group, AudioData data)
    {
        var source = GetOrCreateSource(data.sourcePrefab);
        
        var handle = _handlePool.Get();
        handle.Bind(group, source, data);

        _active.Add(handle);
        
        return handle;
    }


    public void Release(AudioHandle handle)
    {
        if (_active.Contains(handle))
        {
            _active.Remove(handle);

            if (_sourcePools.TryGetValue(handle.Data.sourcePrefab, out var pool))
            {
                pool.Release(handle.AudioSource);
            }

            _handlePool.Release(handle);
        }
    }
    
    
    private AudioSource GetOrCreateSource(AudioSource prefab)
    {
        if (!_sourcePools.TryGetValue(prefab, out var pool))
        {
            pool = new ObjectPool<AudioSource>(
                createFunc: () => Object.Instantiate(prefab, _root),
                actionOnGet: source => source.gameObject.SetActive(true),
                actionOnRelease: source => source.gameObject.SetActive(false),
                actionOnDestroy: source => Object.Destroy(source.gameObject)
            );
            
            _sourcePools[prefab] = pool;
        }
      
        return pool.Get();
    }
}
