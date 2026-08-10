using System.Collections.Generic;
using UnityEngine;

public static class PrefabPool
{
    private static readonly Dictionary<int, Stack<GameObject>> Pools = new Dictionary<int, Stack<GameObject>>();
    private static Transform _root;

    private static Transform Root
    {
        get
        {
            if (_root == null)
            {
                var go = new GameObject("[PrefabPool]");
                Object.DontDestroyOnLoad(go);
                _root = go.transform;
            }

            return _root;
        }
    }

    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
            return null;

        int id = prefab.GetInstanceID();
        GameObject instance = null;

        if (Pools.TryGetValue(id, out Stack<GameObject> stack))
        {
            while (stack.Count > 0 && instance == null)
                instance = stack.Pop();
        }

        if (instance == null)
        {
            instance = Object.Instantiate(prefab, position, rotation);
            PooledInstance pooled = instance.GetComponent<PooledInstance>();
            if (pooled == null)
                pooled = instance.AddComponent<PooledInstance>();
            pooled.PrefabId = id;
        }
        else
        {
            Transform transform = instance.transform;
            transform.SetParent(null, false);
            transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
        }

        return instance;
    }

    public static GameObject Get(Transform prefab, Vector3 position, Quaternion rotation)
    {
        return prefab == null ? null : Get(prefab.gameObject, position, rotation);
    }

    public static void Release(GameObject instance)
    {
        if (instance == null)
            return;

        PooledInstance pooled = instance.GetComponent<PooledInstance>();
        if (pooled == null)
        {
            Object.Destroy(instance);
            return;
        }

        instance.SetActive(false);
        instance.transform.SetParent(Root, false);

        if (!Pools.TryGetValue(pooled.PrefabId, out Stack<GameObject> stack))
        {
            stack = new Stack<GameObject>();
            Pools[pooled.PrefabId] = stack;
        }

        stack.Push(instance);
    }
}
