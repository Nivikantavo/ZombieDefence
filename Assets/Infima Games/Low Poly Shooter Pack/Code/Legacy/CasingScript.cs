//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;
using Plugins.Audio.Core;

namespace InfimaGames.LowPolyShooterPack.Legacy
{
	public class CasingScript : MonoBehaviour
	{
		[Header("Force X")]
		[Tooltip("Minimum force on X axis")]
		public float minimumXForce;

		[Tooltip("Maimum force on X axis")]
		public float maximumXForce;

		[Header("Force Y")]
		[Tooltip("Minimum force on Y axis")]
		public float minimumYForce;

		[Tooltip("Maximum force on Y axis")]
		public float maximumYForce;

		[Header("Force Z")]
		[Tooltip("Minimum force on Z axis")]
		public float minimumZForce;

		[Tooltip("Maximum force on Z axis")]
		public float maximumZForce;

		[Header("Rotation Force")]
		[Tooltip("Minimum initial rotation value")]
		public float minimumRotation;

		[Tooltip("Maximum initial rotation value")]
		public float maximumRotation;

		[Header("Despawn Time")]
		[Tooltip("How long after spawning that the casing is destroyed")]
		public float despawnTime;

		[Header("Audio")]
		public AudioClip[] casingSounds;

		public AudioSource audioSource;
		public SourceAudio Source;

		[Header("Spin Settings")]
		[Tooltip("How fast the casing spins over time")]
		public float speed = 2500.0f;

		private Rigidbody _rigidbody;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void OnEnable()
		{
			transform.rotation = Random.rotation;

			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;

			_rigidbody.AddRelativeTorque(
				Random.Range(minimumRotation, maximumRotation),
				Random.Range(minimumRotation, maximumRotation),
				Random.Range(minimumRotation, maximumRotation)
				* Time.deltaTime);

			_rigidbody.AddRelativeForce(
				Random.Range(minimumXForce, maximumXForce),
				Random.Range(minimumYForce, maximumYForce),
				Random.Range(minimumZForce, maximumZForce));

			StopAllCoroutines();
			StartCoroutine(RemoveCasing());
			StartCoroutine(PlaySound());
		}

		private void FixedUpdate()
		{
			transform.Rotate(Vector3.right, speed * Time.deltaTime);
			transform.Rotate(Vector3.down, speed * Time.deltaTime);
		}

		private IEnumerator PlaySound()
		{
			yield return new WaitForSeconds(Random.Range(0.25f, 0.85f));
			if (casingSounds == null || casingSounds.Length == 0 || audioSource == null || Source == null)
				yield break;

			audioSource.clip = casingSounds[Random.Range(0, casingSounds.Length)];
			Source.Play(audioSource.clip.name);
		}

		private IEnumerator RemoveCasing()
		{
			yield return new WaitForSeconds(despawnTime);
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
			PrefabPool.Release(gameObject);
		}
	}
}
