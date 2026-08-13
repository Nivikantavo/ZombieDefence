//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using Random = UnityEngine.Random;

namespace InfimaGames.LowPolyShooterPack.Legacy
{
	public class Projectile : MonoBehaviour
	{
		public float Damage => _damage;

		private static readonly List<Collider> PlayerColliders = new List<Collider>();
		private static Transform _playerRoot;

		private float _damage;
		private float _hitForce = 20;
		private Rigidbody _rigidbody;
		private Collider _collider;
		private bool _released;

		[Range(5, 100)]
		[Tooltip("After how long time should the bullet prefab be destroyed?")]
		public float destroyAfter;

		[Tooltip("If enabled the bullet destroys on impact")]
		public bool destroyOnImpact = false;

		[Tooltip("Minimum time after impact that the bullet is destroyed")]
		public float minDestroyTime;

		[Tooltip("Maximum time after impact that the bullet is destroyed")]
		public float maxDestroyTime;

		[Header("Impact Effect Prefabs")]
		public Transform[] bloodImpactPrefabs;

		public Transform[] metalImpactPrefabs;
		public Transform[] dirtImpactPrefabs;
		public Transform[] concreteImpactPrefabs;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_collider = GetComponent<Collider>();
		}

		private void OnEnable()
		{
			_released = false;
			IgnorePlayerCollision();
			StopAllCoroutines();
			StartCoroutine(DestroyAfter());
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (_released)
				return;

			if (IsPlayer(collision.transform))
			{
				if (_collider != null)
					Physics.IgnoreCollision(collision.collider, _collider);

				return;
			}

			if (collision.gameObject.GetComponent<Projectile>() != null)
				return;

			if (!destroyOnImpact)
			{
				StartCoroutine(DestroyTimer());
			}
			else
			{
				Release();
			}

			var rigidbody = collision.rigidbody;
			if (rigidbody != null)
			{
				rigidbody.AddForceAtPosition(transform.forward * _hitForce, transform.position, ForceMode.Impulse);
			}

			if (collision.gameObject.TryGetComponent(out HitBox hitBox))
			{
				hitBox.OnHit(_damage);
			}

			if (collision.gameObject.TryGetComponent(out Idamageable damageable))
			{
				damageable.TakeDamage(_damage);
			}

			if (collision.transform.tag == "Blood")
			{
				SpawnImpact(bloodImpactPrefabs, collision);
				Release();
			}
			else if (collision.transform.tag == "Metal")
			{
				SpawnImpact(metalImpactPrefabs, collision);
				Release();
			}
			else if (collision.transform.tag == "Dirt")
			{
				SpawnImpact(dirtImpactPrefabs, collision);
				Release();
			}
			else if (collision.transform.tag == "Concrete")
			{
				SpawnImpact(concreteImpactPrefabs, collision);
				Release();
			}
			else if (collision.transform.tag == "Target")
			{
				collision.transform.gameObject.GetComponent<TargetScript>().isHit = true;
				Release();
			}
			else if (collision.transform.tag == "ExplosiveBarrel")
			{
				collision.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
				Release();
			}
			else if (collision.transform.tag == "GasTank")
			{
				collision.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
				Release();
			}
		}

		public void SetDamage(float damage)
		{
			_damage = damage;
		}

		private void SpawnImpact(Transform[] prefabs, Collision collision)
		{
			if (prefabs == null || prefabs.Length == 0)
				return;

			PrefabPool.Get(prefabs[Random.Range(0, prefabs.Length)], transform.position,
				Quaternion.LookRotation(collision.contacts[0].normal));
		}

		//Physics drops ignored collider pairs as soon as one of them is disabled, so pooled projectiles have to register again on every spawn.
		private void IgnorePlayerCollision()
		{
			if (_collider == null || !TryCachePlayer())
				return;

			foreach (Collider playerCollider in PlayerColliders)
			{
				if (playerCollider != null && playerCollider.enabled && playerCollider.gameObject.activeInHierarchy)
					Physics.IgnoreCollision(playerCollider, _collider);
			}
		}

		private static bool IsPlayer(Transform hit)
		{
			return _playerRoot != null && hit.IsChildOf(_playerRoot);
		}

		private static bool TryCachePlayer()
		{
			if (_playerRoot != null)
				return true;

			if (ServiceLocator.Current == null)
				return false;

			CharacterBehaviour playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
			if (playerCharacter == null)
				return false;

			_playerRoot = playerCharacter.transform.root;
			PlayerColliders.Clear();
			_playerRoot.GetComponentsInChildren(true, PlayerColliders);

			return true;
		}

		private IEnumerator DestroyTimer()
		{
			yield return new WaitForSeconds(Random.Range(minDestroyTime, maxDestroyTime));
			Release();
		}

		private IEnumerator DestroyAfter()
		{
			yield return new WaitForSeconds(destroyAfter);
			Release();
		}

		private void Release()
		{
			if (_released)
				return;

			_released = true;
			StopAllCoroutines();

			if (_rigidbody != null)
			{
				_rigidbody.velocity = Vector3.zero;
				_rigidbody.angularVelocity = Vector3.zero;
			}

			PrefabPool.Release(gameObject);
		}
	}
}
