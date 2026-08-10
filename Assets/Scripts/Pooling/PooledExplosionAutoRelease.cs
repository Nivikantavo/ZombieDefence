using System.Collections;
using UnityEngine;

public class PooledExplosionAutoRelease : MonoBehaviour
{
    [SerializeField] private float _lifetime = 3f;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(ReleaseAfterDelay());
    }

    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(_lifetime);
        PrefabPool.Release(gameObject);
    }
}
