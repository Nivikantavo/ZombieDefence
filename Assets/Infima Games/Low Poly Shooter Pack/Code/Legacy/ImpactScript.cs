//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;

namespace InfimaGames.LowPolyShooterPack.Legacy
{
	public class ImpactScript : MonoBehaviour
	{
		[Header("Impact Despawn Timer")]
		public float despawnTimer = 10.0f;

		[Header("Audio")]
		public AudioClip[] impactSounds;

		public AudioSource audioSource;

		private void OnEnable()
		{
			StopAllCoroutines();
			StartCoroutine(DespawnTimer());

			if (audioSource != null && impactSounds != null && impactSounds.Length > 0)
			{
				audioSource.clip = impactSounds[Random.Range(0, impactSounds.Length)];
				audioSource.Play();
			}
		}

		private IEnumerator DespawnTimer()
		{
			yield return new WaitForSeconds(despawnTimer);
			PrefabPool.Release(gameObject);
		}
	}
}
