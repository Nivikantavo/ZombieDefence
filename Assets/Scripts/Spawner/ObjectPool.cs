using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] protected GameObject _conteiter;

    private readonly List<GameObject> _pool = new List<GameObject>();
    private GameObject _prefab;

    protected virtual void Initialize(GameObject prefab, int capacity)
    {
        _prefab = prefab;
        EnsureCapacity(capacity);
    }

    protected void EnsureCapacity(int capacity)
    {
        if (_prefab == null)
            return;

        int toCreate = capacity - _pool.Count;
        for (int i = 0; i < toCreate; i++)
        {
            CreateInstance();
        }
    }

    public bool TryGetObject(out GameObject result)
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            GameObject candidate = _pool[i];
            if (candidate != null && candidate.activeSelf == false)
            {
                result = candidate;
                return true;
            }
        }

        if (_prefab != null)
        {
            result = CreateInstance();
            return true;
        }

        result = null;
        return false;
    }

    private GameObject CreateInstance()
    {
        Transform parent = _conteiter != null ? _conteiter.transform : transform;
        GameObject spawned = Instantiate(_prefab, parent);
        spawned.SetActive(false);
        _pool.Add(spawned);
        return spawned;
    }
}
