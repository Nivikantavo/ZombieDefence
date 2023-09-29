﻿//Copyright 2022, Infima Games. All Rights Reserved.

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace InfimaGames.LowPolyShooterPack
{
	/// <summary>
	/// Main Character Component. This component handles the most important functions of the character, and interfaces
	/// with basically every part of the asset, it is the hub where it all converges.
	/// </summary>
	[RequireComponent(typeof(CharacterKinematics))]
	public sealed class Character : CharacterBehaviour
	{
		#region FIELDS SERIALIZED

		[SerializeField] private Player _player;
		private float _lookSensetive;

		[Title(label: "References")]

		[Tooltip("The character's LowerWeapon component.")]
		[SerializeField]
		private LowerWeapon lowerWeapon;
		
		[Title(label: "Inventory")]
		
		[Tooltip("Determines the index of the weapon to equip when the game starts.")]
		[SerializeField]
		private int weaponIndexEquippedAtStart;
		
		[Tooltip("Inventory.")]
		[SerializeField]
		private InventoryBehaviour inventory;

		[Title(label: "Grenade")]

		[Tooltip("If true, the character's grenades will never run out.")]
		[SerializeField]
		private bool grenadesUnlimited;

		[Tooltip("Total amount of grenades at start.")]
		[SerializeField]
		private int grenadeTotal = 10;
		
		[Tooltip("Grenade spawn offset from the character's camera.")]
		[SerializeField]
		private float grenadeSpawnOffset = 1.0f;
		
		[Tooltip("Grenade Prefab. Spawned when throwing a grenade.")]
		[SerializeField]
		private GameObject grenadePrefab;
		
		[Title(label: "Knife")]
		
		[Tooltip("Knife GameObject.")]
		[SerializeField]
		private GameObject knife;

		[Title(label: "Cameras")]

		[Tooltip("Normal Camera.")]
		[SerializeField]
		private Camera cameraWorld;

		[Tooltip("Weapon-Only Camera. Depth.")]
		[SerializeField]
		private Camera cameraDepth;

		[Title(label: "Animation")]
		
		[Tooltip("Determines how smooth the turning animation is.")]
		[SerializeField]
		private float dampTimeTurning = 0.4f;

		[Tooltip("Determines how smooth the locomotion blendspace is.")]
		[SerializeField]
		private float dampTimeLocomotion = 0.15f;

		[Tooltip("How smoothly we play aiming transitions. Beware that this affects lots of things!")]
		[SerializeField]
		private float dampTimeAiming = 0.3f;

		[Tooltip("Interpolation speed for the running offsets.")]
		[SerializeField]
		private float runningInterpolationSpeed = 12.0f;

		[Tooltip("Determines how fast the character's weapons are aimed.")]
		[SerializeField]
		private float aimingSpeedMultiplier = 1.0f;
		
		[Title(label: "Animation Procedural")]

		[Tooltip("Weapon Bone.")]
		[SerializeField]
		private Transform boneWeapon;
		
		[Tooltip("Character Animator.")]
		[SerializeField]
		private Animator characterAnimator;

		[SerializeField]
		private bool enableWeaponSway = true;

		[SerializeField]
		private float weaponSwaySmoothValueInput = 8.0f;

		[Title(label: "Field Of View")]

		[Tooltip("Normal world field of view.")]
		[SerializeField]
		private float fieldOfView = 100.0f;

		[Tooltip("Multiplier for the field of view while running.")]
		[SerializeField]
		private float fieldOfViewRunningMultiplier = 1.05f;

		[Tooltip("Weapon-specific field of view.")]
		[SerializeField]
		private float fieldOfViewWeapon = 55.0f;

		[Title(label: "Audio Clips")]
		
		[Tooltip("Melee Audio Clips.")]
		[SerializeField]
		private AudioClip[] audioClipsMelee;

		[Tooltip("Grenade Throw Audio Clips.")]
		[SerializeField]
		private AudioClip[] audioClipsGrenadeThrow;

		#endregion

		#region FIELDS

		/// <summary>
		/// True if the character is aiming.
		/// </summary>
		private bool aiming;
		/// <summary>
		/// Last Frame's Aiming Value.
		/// </summary>
		private bool wasAiming;
		/// <summary>
		/// True if the character is running.
		/// </summary>
		private bool running;
		/// <summary>
		/// True if the character has its weapon holstered.
		/// </summary>
		private bool holstered;
		
		/// <summary>
		/// Last Time.time at which we shot.
		/// </summary>
		private float lastShotTime;
		
		/// <summary>
		/// Overlay Layer Index. Useful for playing things like firing animations.
		/// </summary>
		private int layerOverlay;
		/// <summary>
		/// Holster Layer Index. Used to play holster animations.
		/// </summary>
		private int layerHolster;
		/// <summary>
		/// Actions Layer Index. Used to play actions like reloading.
		/// </summary>
		private int layerActions;

		/// <summary>
		/// Cached Movement Component. Used in order to access some of the movement-related properties.
		/// </summary>
		private MovementBehaviour movementBehaviour;
		
		/// <summary>
		/// The currently equipped weapon.
		/// </summary>
		private WeaponBehaviour equippedWeapon;
		/// <summary>
		/// The equipped weapon's attachment manager.
		/// </summary>
		private WeaponAttachmentManagerBehaviour weaponAttachmentManager;
		
		/// <summary>
		/// The scope equipped on the character's weapon.
		/// </summary>
		private ScopeBehaviour equippedWeaponScope;
		/// <summary>
		/// The magazine equipped on the character's weapon.
		/// </summary>
		private MagazineBehaviour equippedWeaponMagazine;
		
		/// <summary>
		/// True if the character is reloading.
		/// </summary>
		private bool reloading;
		
		/// <summary>
		/// True if the character is inspecting its weapon.
		/// </summary>
		private bool inspecting;
		/// <summary>
		/// True if the character is throwing a grenade.
		/// </summary>
		private bool throwingGrenade;
		
		/// <summary>
		/// True if the character is meleeing.
		/// </summary>
		private bool meleeing;

		/// <summary>
		/// Sway Location Value.
		/// </summary>
		private Vector3 swayLocation;
		/// <summary>
		/// Sway Rotation Value.
		/// </summary>
		private Vector3 swayRotation;

		/// <summary>
		/// True if the character is in the middle of holstering a weapon.
		/// </summary>
		private bool holstering;
		/// <summary>
		/// Alpha Aiming Value. Zero to one value representing aiming. Zero if we're not aiming, and one if we are
		/// fully aiming.
		/// </summary>
		private float aimingAlpha;

		/// <summary>
		/// Crouching Alpha. This value dictates how visible the crouching state is at any given time.
		/// </summary>
		private float crouchingAlpha;
		/// <summary>
		/// Running Alpha. This value dictates how visible the running state is at any given time.
		/// </summary>
		private float runningAlpha;

		/// <summary>
		/// Look Axis Values.
		/// </summary>
		private Vector2 axisLook;
		/// <summary>
		/// Smoothed Look Axis Values. Used for Sway.
		/// </summary>
		private Vector2 axisLookSmooth;
		
		/// <summary>
		/// Look Axis Values.
		/// </summary>
		private Vector2 axisMovement;
		/// <summary>
		/// Smoothed Movement Axis Values. Used for Sway.
		/// </summary>
		private Vector2 axisMovementSmooth;

		/// <summary>
		/// True if the character is playing the bolt-action animation.
		/// </summary>
		private bool bolting;

		/// <summary>
		/// Current grenades left.
		/// </summary>
		private int grenadeCount;

		/// <summary>
		/// True if the player is holding the aiming button.
		/// </summary>
		private bool holdingButtonAim;
		/// <summary>
		/// True if the player is holding the running button.
		/// </summary>
		private bool holdingButtonRun;
		/// <summary>
		/// True if the player is holding the firing button.
		/// </summary>
		private bool holdingButtonFire;

		/// <summary>
		/// If true, the tutorial text should be visible on screen.
		/// </summary>
		private bool tutorialTextVisible;

		/// <summary>
		/// True if the game cursor is locked! Used when pressing "Escape" to allow developers to more easily access the editor.
		/// </summary>
		private bool cursorLocked;
		/// <summary>
		/// Amount of shots fired in succession. We use this value to increase the spread, and also to apply recoil
		/// </summary>
		private int shotsFired;

		/// <summary>
		/// Time.time at which we started aiming.
		/// </summary>
		private float aimStartTime;

		#endregion

		#region CONSTANTS
		
		//TODO: Get rid of all of these, and move them to the AHashes file.

		/// <summary>
		/// Aiming Alpha Value.
		/// </summary>
		private static readonly int HashAimingAlpha = Animator.StringToHash("Aiming");
		/// <summary>
		/// Bolt Action Value.
		/// </summary>
		private static readonly int HashBoltAction = Animator.StringToHash("Bolt Action");

		/// <summary>
		/// Hashed "Movement".
		/// </summary>
		private static readonly int HashMovement = Animator.StringToHash("Movement");
		/// <summary>
		/// Hashed "Leaning".
		/// </summary>
		private static readonly int HashLeaning = Animator.StringToHash("Leaning");
		
		/// <summary>
		/// Hashed "Aiming Speed Multiplier".
		/// </summary>
		private static readonly int HashAimingSpeedMultiplier = Animator.StringToHash("Aiming Speed Multiplier");
		/// <summary>
		/// Hashed "Turning".
		/// </summary>
		private static readonly int HashTurning = Animator.StringToHash("Turning");
		
		/// <summary>
		/// Hashed "Horizontal".
		/// </summary>
		private static readonly int HashHorizontal = Animator.StringToHash("Horizontal");
		/// <summary>
		/// Hashed "Vertical".
		/// </summary>
		private static readonly int HashVertical = Animator.StringToHash("Vertical");
		
		/// <summary>
		/// Hashed "Play Rate Locomotion Forward".
		/// </summary>
		private static readonly int HashPlayRateLocomotionForward = Animator.StringToHash("Play Rate Locomotion Forward");
		/// <summary>
		/// Hashed "Play Rate Locomotion Sideways".
		/// </summary>
		private static readonly int HashPlayRateLocomotionSideways = Animator.StringToHash("Play Rate Locomotion Sideways");
		/// <summary>
		/// Hashed "Play Rate Locomotion Backwards".
		/// </summary>
		private static readonly int HashPlayRateLocomotionBackwards = Animator.StringToHash("Play Rate Locomotion Backwards");

		/// <summary>
		/// Hashed "Alpha Action Offset".
		/// </summary>
		private static readonly int HashAlphaActionOffset = Animator.StringToHash("Alpha Action Offset");

		#endregion

		#region UNITY

		/// <summary>
		/// Awake.
		/// </summary>
		protected override void Awake()
		{
			#region Lock Cursor

			//Always make sure that our cursor is locked when the game starts!
			cursorLocked = true;
			//Update the cursor's state.
			UpdateCursorState();

			#endregion

			//Cache the movement behaviour.
			movementBehaviour = GetComponent<MovementBehaviour>();
			SetGranadesCount();
			SetSavedSensetive();
        }

        private void OnEnable()
        {
			_player.PushForceUsed += OnPushForceUsed;
        }

        private void OnDisable()
        {
            _player.PushForceUsed -= OnPushForceUsed;
        }

        /// <summary>
        /// Start.
        /// </summary>
        protected override void Start()
		{
			//Max out the grenades.
			grenadeCount = grenadeTotal;
			
			//Hide knife. We do this so we don't see a giant knife stabbing through the character's hands all the time!
			if (knife != null)
				knife.SetActive(false);
			
			//Cache a reference to the holster layer's index.
			layerHolster = characterAnimator.GetLayerIndex("Layer Holster");
			//Cache a reference to the action layer's index.
			layerActions = characterAnimator.GetLayerIndex("Layer Actions");
			//Cache a reference to the overlay layer's index.
			layerOverlay = characterAnimator.GetLayerIndex("Layer Overlay");
		}

		/// <summary>
		/// Update.
		/// </summary>
		protected override void Update()
		{
			//Match Aim.
			aiming = holdingButtonAim && CanAim();
			//Match Run.
			running = holdingButtonRun && CanRun();

			//Check if we're aiming.
			switch (aiming)
			{
				//Just Started.
				case true when !wasAiming:
					equippedWeaponScope.OnAim();
					break;
				//Just Stopped.
				case false when wasAiming:
					equippedWeaponScope.OnAimStop();
					break;
			}

			//Holding the firing button.
			if (holdingButtonFire)
			{
				//Check.
				if (CanPlayAnimationFire() && equippedWeapon.HasAmmunition() && equippedWeapon.IsAutomatic())
				{
					//Has fire rate passed.
					if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
						Fire();
				}
				else
				{
					//Reset fired shots, so recoil/spread does not just stay at max when we've run out
					//of ammo already!
					shotsFired = 0;
				}
			}

			//Try and calculate the sway so we can apply it.
			if(enableWeaponSway)
				CalculateSway();

			//Interpolate Movement Axis.
			axisMovementSmooth = Vector2.Lerp(axisMovementSmooth, axisMovement, Time.deltaTime * weaponSwaySmoothValueInput);
			//Interpolate Look Axis.
			axisLookSmooth = Vector2.Lerp(axisLookSmooth, axisLook, Time.deltaTime * weaponSwaySmoothValueInput);

			//Update Animator.
			UpdateAnimator();

			//Update Aiming Alpha. We need to get this here because we're using the Animator to interpolate the aiming value.
			aimingAlpha = characterAnimator.GetFloat(HashAimingAlpha);
			
			//Interpolate the crouching alpha. We do this here as a quick and dirty shortcut, but there's definitely better ways to do this.
			crouchingAlpha = Mathf.Lerp(crouchingAlpha, movementBehaviour.IsCrouching() ? 1.0f : 0.0f, Time.deltaTime * 12.0f);
			//Interpolate the running alpha. We do this here as a quick and dirty shortcut, but there's definitely better ways to do this.
			runningAlpha = Mathf.Lerp(runningAlpha, running ? 1.0f : 0.0f, Time.deltaTime * runningInterpolationSpeed);

			//Running Field Of View Multiplier.
			float runningFieldOfView = Mathf.Lerp(1.0f, fieldOfViewRunningMultiplier, runningAlpha);
			
			//Interpolate the world camera's field of view based on whether we are aiming or not.
			cameraWorld.fieldOfView = Mathf.Lerp(fieldOfView, fieldOfView * equippedWeapon.GetFieldOfViewMultiplierAim(), aimingAlpha) * runningFieldOfView;
			//Interpolate the depth camera's field of view based on whether we are aiming or not.
			cameraDepth.fieldOfView = Mathf.Lerp(fieldOfViewWeapon, fieldOfViewWeapon * equippedWeapon.GetFieldOfViewMultiplierAimWeapon(), aimingAlpha);
			
			//Save Aiming Value.
			wasAiming = aiming;
		}

		/// <summary>
		/// Late Update.
		/// </summary>
		protected override void LateUpdate()
		{
			//Ignore if we don't have a weapon bone assigned.
			if (boneWeapon == null)
				return;

			//We need a weapon for this!
			if (equippedWeapon == null)
				return;

			//Weapons without a scope should not be a thing! Ironsights are a scope too!
			if (equippedWeaponScope == null)
				return;

			//Get the weapon offsets.
			Offsets weaponOffsets = equippedWeapon.GetWeaponOffsets();

			//Frame Location Local.
			Vector3 frameLocationLocal = swayLocation;
			//Offset Location.
			frameLocationLocal += Vector3.Lerp(weaponOffsets.StandingLocation, weaponOffsets.AimingLocation, aimingAlpha);
			//Scope Aiming Location.
			frameLocationLocal += Vector3.Lerp(default, equippedWeaponScope.GetOffsetAimingLocation(), aimingAlpha);
			//Crouching Location.
			frameLocationLocal += Vector3.Lerp(default, weaponOffsets.CrouchingLocation, crouchingAlpha * (1 - aimingAlpha));
			//Running Location.
			frameLocationLocal += Vector3.Lerp(default, weaponOffsets.RunningLocation, runningAlpha * (1 - aimingAlpha));
			//Action Offset Location. This is a helping value to make actions like throwing a grenade different per-weapon.
			frameLocationLocal += Vector3.Lerp(weaponOffsets.ActionLocation * characterAnimator.GetFloat(HashAlphaActionOffset), default, aimingAlpha);

			//Frame Rotation Local.
			Vector3 frameRotationLocal = swayRotation;
			//Offset Rotation.
			frameRotationLocal += Vector3.Lerp(weaponOffsets.StandingRotation, weaponOffsets.AimingRotation, aimingAlpha);
			//Scope Aiming Rotation.
			frameRotationLocal += Vector3.Lerp(default, equippedWeaponScope.GetOffsetAimingRotation(), aimingAlpha);
			//Crouching Rotation.
			frameRotationLocal += Vector3.Lerp(default, weaponOffsets.CrouchingRotation, crouchingAlpha * (1 - aimingAlpha));
			//Running Rotation.
			frameRotationLocal += Vector3.Lerp(default, weaponOffsets.RunningRotation, runningAlpha * (1 - aimingAlpha));
			//Action Offset Rotation. This is a helping value to make actions like throwing a grenade different per-weapon.
			frameRotationLocal += Vector3.Lerp(weaponOffsets.ActionRotation * characterAnimator.GetFloat(HashAlphaActionOffset), default, aimingAlpha);
			
			#region Automatic Aim Offsets
			
			// Transform socketScopeCorrected = equippedWeaponScope.transform.GetChild(0).GetChild(0).GetChild(0);
			// Transform socketScopes = equippedWeaponScope.transform.parent.parent;
			//
			// Log.wtf(equippedWeaponScope.transform.parent.parent.parent.localPosition);
			// Vector3 localPosition = equippedWeaponScope.transform.GetChild(0).GetChild(0).localPosition;
			// Log.wtf(localPosition);
			// boneWeapon.localPosition -= Vector3.Lerp(default, localPosition, aimingAlpha);

			#endregion

			//Add to the weapon location.
			boneWeapon.localPosition += frameLocationLocal;
			//Add to the weapon rotation.
			boneWeapon.localEulerAngles += frameRotationLocal;
		}

		#endregion

		#region GETTERS
		
		/// <summary>
		/// GetShotsFired.
		/// </summary>
		public override int GetShotsFired() => shotsFired;

		/// <summary>
		/// IsLowered.
		/// </summary>
		public override bool IsLowered()
		{
			//Weapons are never lowered if we don't even have a LowerWeapon component.
			if (lowerWeapon == null)
				return false;

			//Return.
			return lowerWeapon.IsLowered();
		}

		/// <summary>
		/// GetCameraWorld.
		/// </summary>
		public override Camera GetCameraWorld() => cameraWorld;
		/// <summary>
		/// GetCameraDepth.
		/// </summary>
		/// <returns></returns>
		public override Camera GetCameraDepth() => cameraDepth;
		
		/// <summary>
		/// Returns the looking sway values for this frame.
		/// </summary>
		/// <param name="sway">Sway values to use for the calculation.</param>
		private (Vector3 location, Vector3 rotation) GetSwayLook(Sway sway)
		{
			//Horizontal Axis.
			float horizontalAxis = Mathf.Clamp(axisLookSmooth.x, -1.0f, 1.0f);
			//Vertical Axis.
			float verticalAxis = Mathf.Clamp(axisLookSmooth.y, -1.0f, 1.0f);

			//Horizontal Axis Location.
			Vector3 horizontalLocation = horizontalAxis * sway.Look.Location.Horizontal;
			//Horizontal Axis Rotation.
			Vector3 horizontalRotation = horizontalAxis * sway.Look.Rotation.Horizontal;

			//Vertical Axis Location.
			Vector3 verticalLocation = verticalAxis * sway.Look.Location.Vertical;
			//Vertical Axis Rotation.
			Vector3 verticalRotation = verticalAxis * sway.Look.Rotation.Vertical;

			//Return.
			return (horizontalLocation + verticalLocation, horizontalRotation + verticalRotation);
		}
		/// <summary>
		/// Returns the movement sway values for this frame.
		/// </summary>
		/// <param name="sway">Sway values to use for the calculation.</param>
		private (Vector3 location, Vector3 rotation) GetSwayMovement(Sway sway)
		{
			//Horizontal Axis.
			float horizontalAxis = Mathf.Clamp(axisMovementSmooth.x, -1.0f, 1.0f);
			//Vertical Axis.
			float verticalAxis = Mathf.Clamp(axisMovementSmooth.y, -1.0f, 1.0f);
			
			//Horizontal Axis Location.
			Vector3 horizontalLocation = horizontalAxis * sway.Movement.Location.Horizontal;
			//Horizontal Axis Rotation.
			Vector3 horizontalRotation = horizontalAxis * sway.Movement.Rotation.Horizontal;

			//Vertical Axis Location.
			Vector3 verticalLocation = verticalAxis * sway.Movement.Location.Vertical;
			//Vertical Axis Rotation.
			Vector3 verticalRotation = verticalAxis * sway.Movement.Rotation.Vertical;

			//Return.
			return (horizontalLocation + verticalLocation, horizontalRotation + verticalRotation);
		}

		/// <summary>
		/// GetInventory.
		/// </summary>
		public override InventoryBehaviour GetInventory() => inventory;

		/// <summary>
		/// GetGrenadesCurrent.
		/// </summary>
		public override int GetGrenadesCurrent() => grenadeCount;
		/// <summary>
		/// GetGrenadesTotal.
		/// </summary>
		public override int GetGrenadesTotal() => grenadeTotal;

		/// <summary>
		/// IsRunning.
		/// </summary>
		/// <returns></returns>
		public override bool IsRunning() => running;
		/// <summary>
		/// IsHolstered.
		/// </summary>
		public override bool IsHolstered() => holstered;

		/// <summary>
		/// Is Crouching.
		/// </summary>
		public override bool IsCrouching() => movementBehaviour.IsCrouching();

		/// <summary>
		/// IsReloading.
		/// </summary>
		public override bool IsReloading() => reloading;

		/// <summary>
		/// IsThrowingGrenade.
		/// </summary>
		public override bool IsThrowingGrenade() => throwingGrenade;
		
		/// <summary>
		/// IsMeleeing.
		/// </summary>
		/// <returns></returns>
		public override bool IsMeleeing() => meleeing;

		/// <summary>
		/// IsAiming.
		/// </summary>
		public override bool IsAiming() => aiming;
		/// <summary>
		/// IsCursorLocked.
		/// </summary>
		public override bool IsCursorLocked() => cursorLocked;
		
		/// <summary>
		/// IsTutorialTextVisible.
		/// </summary>
		public override bool IsTutorialTextVisible() => tutorialTextVisible;
		
		/// <summary>
		/// GetInputMovement.
		/// </summary>
		public override Vector2 GetInputMovement() => axisMovement;
		/// <summary>
		/// GetInputLook.
		/// </summary>
		public override Vector2 GetInputLook() => axisLook;

		/// <summary>
		/// GetAudioClipsGrenadeThrow.
		/// </summary>
		public override AudioClip[] GetAudioClipsGrenadeThrow() => audioClipsGrenadeThrow;
		/// <summary>
		/// GetAudioClipsMelee.
		/// </summary>
		public override AudioClip[] GetAudioClipsMelee() => audioClipsMelee;
		
		/// <summary>
		/// IsInspecting.
		/// </summary>
		public override bool IsInspecting() => inspecting;
		/// <summary>
		/// IsHoldingButtonFire. 
		/// </summary>
		public override bool IsHoldingButtonFire() => holdingButtonFire;

		#endregion

		#region METHODS

		/// <summary>
		/// Updates all the animator properties for this frame.
		/// </summary>
		private void UpdateAnimator()
		{
			#region Reload Stop

			//Check if we're currently reloading cycled.
			const string boolNameReloading = "Reloading";
			if (characterAnimator.GetBool(boolNameReloading))
			{
				//If we only have one more bullet to reload, then we can change the boolean already.
				if (equippedWeapon.GetAmmunitionTotal() - equippedWeapon.GetAmmunitionCurrent() < 1)
				{
					//Update the character animator.
					characterAnimator.SetBool(boolNameReloading, false);
					//Update the weapon animator.
					equippedWeapon.GetAnimator().SetBool(boolNameReloading, false);
				}	
			}

			#endregion

			//Leaning. Affects how much the character should apply of the leaning additive animation.
			float leaningValue = Mathf.Clamp01(axisMovement.y);
			characterAnimator.SetFloat(HashLeaning, leaningValue, 0.5f, Time.deltaTime);

			//Movement Value. This value affects absolute movement. Aiming movement uses this, as opposed to per-axis movement.
			float movementValue = Mathf.Clamp01(Mathf.Abs(axisMovement.x) + Mathf.Abs(axisMovement.y));
			characterAnimator.SetFloat(HashMovement, movementValue, dampTimeLocomotion, Time.deltaTime);
			
			//Aiming Speed Multiplier.
			characterAnimator.SetFloat(HashAimingSpeedMultiplier, aimingSpeedMultiplier);
			
			//Turning Value. This determines how much of the turning animation to play based on our current look rotation.
			characterAnimator.SetFloat(HashTurning, Mathf.Abs(axisLook.x), dampTimeTurning, Time.deltaTime);

			//Horizontal Movement Float.
			characterAnimator.SetFloat(HashHorizontal, axisMovementSmooth.x, dampTimeLocomotion, Time.deltaTime);
			//Vertical Movement Float.
			characterAnimator.SetFloat(HashVertical, axisMovementSmooth.y, dampTimeLocomotion, Time.deltaTime);
			
			//Update the aiming value, but use interpolation. This makes sure that things like firing can transition properly.
			characterAnimator.SetFloat(HashAimingAlpha, Convert.ToSingle(aiming), dampTimeAiming, Time.deltaTime);

			//Set the locomotion play rate. This basically stops movement from happening while in the air.
			const string playRateLocomotionBool = "Play Rate Locomotion";
			characterAnimator.SetFloat(playRateLocomotionBool, movementBehaviour.IsGrounded() ? 1.0f : 0.0f, 0.2f, Time.deltaTime);

			#region Movement Play Rates

			//Update Forward Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
			characterAnimator.SetFloat(HashPlayRateLocomotionForward, movementBehaviour.GetMultiplierForward(), 0.2f, Time.deltaTime);
			//Update Sideways Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
			characterAnimator.SetFloat(HashPlayRateLocomotionSideways, movementBehaviour.GetMultiplierSideways(), 0.2f, Time.deltaTime);
			//Update Backwards Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
			characterAnimator.SetFloat(HashPlayRateLocomotionBackwards, movementBehaviour.GetMultiplierBackwards(), 0.2f, Time.deltaTime);

			#endregion
			
			//Update Animator Aiming.
			const string boolNameAim = "Aim";
			characterAnimator.SetBool(boolNameAim, aiming);
			
			//Update Animator Running.
			const string boolNameRun = "Running";
			characterAnimator.SetBool(boolNameRun, running);
			
			//Update Animator Crouching.
			const string boolNameCrouch = "Crouching";
			characterAnimator.SetBool(boolNameCrouch, movementBehaviour.IsCrouching());
		}
		/// <summary>
		/// Plays the inspect animation.
		/// </summary>
		private void Inspect()
		{
			//State.
			inspecting = true;
			//Play.
			characterAnimator.CrossFade("Inspect", 0.0f, layerActions, 0);
		}
		
		/// <summary>
		/// Calculates the sway values.
		/// </summary>
		private void CalculateSway()
		{
			//We need a scope!
			if (equippedWeaponScope == null)
				return;

			//Weapon Sway Values.
			Sway sway = equippedWeapon.GetSway();
			//Weapon Sway Smooth Value.
			float swaySmoothValue = equippedWeapon.GetSwaySmoothValue();
			
			(Vector3 location, Vector3 rotation) swayLookStanding = GetSwayLook(sway);
			(Vector3 location, Vector3 rotation) swayLookAiming = GetSwayLook(equippedWeaponScope.GetSwayAiming());
			
			(Vector3 location, Vector3 rotation) swayMovementStanding = GetSwayMovement(sway);
			(Vector3 location, Vector3 rotation) swayMovementAiming = GetSwayMovement(equippedWeaponScope.GetSwayAiming());
			
			//Get Look Sway.
			(Vector3 location, Vector3 rotation) swayLook = default;
			swayLook.location = Vector3.Lerp(swayLookStanding.location, swayLookAiming.location, aimingAlpha);
			swayLook.rotation = Vector3.Lerp(swayLookStanding.rotation, swayLookAiming.rotation, aimingAlpha);

			//Get Movement Sway.
			(Vector3 location, Vector3 rotation) swayMovement = default;
			swayMovement.location = Vector3.Lerp(swayMovementStanding.location, swayMovementAiming.location, aimingAlpha);
			swayMovement.rotation = Vector3.Lerp(swayMovementStanding.rotation, swayMovementAiming.rotation, aimingAlpha);
			
			//Calculate Sway Location.
			Vector3 frameLocation = swayLook.location + swayMovement.location;
			//Interpolate.
			swayLocation = Vector3.LerpUnclamped(swayLocation, frameLocation, Time.deltaTime * swaySmoothValue);

			//Calculate Sway Rotation.
			Vector3 frameRotation = swayLook.rotation + swayMovement.rotation;
			//Interpolate.
			swayRotation = Vector3.LerpUnclamped(swayRotation, frameRotation, Time.deltaTime * swaySmoothValue);
		}
		/// <summary>
		/// Fires the character's weapon.
		/// </summary>
		private void Fire()
		{
			//Increase shots fired. We use this value to increase the spread, and also to apply recoil, so
			//it is very important that we keep it up to date.
			shotsFired++;
			
			//Save the shot time, so we can calculate the fire rate correctly.
			lastShotTime = Time.time;
			//Fire the weapon! Make sure that we also pass the scope's spread multiplier if we're aiming.
			equippedWeapon.Fire(aiming ? equippedWeaponScope.GetMultiplierSpread() : 1.0f);

			//Play firing animation.
			const string stateName = "Fire";
			characterAnimator.CrossFade(stateName, 0.05f, layerOverlay, 0);

			//Play bolt actioning animation if needed, and if we have ammunition. We don't play this for the last shot.
			if (equippedWeapon.IsBoltAction() && equippedWeapon.HasAmmunition())
				UpdateBolt(true);

			//Automatically reload the weapon if we need to. This is very helpful for things like grenade launchers or rocket launchers.
			if (!equippedWeapon.HasAmmunition() && equippedWeapon.GetAutomaticallyReloadOnEmpty())
				StartCoroutine(nameof(TryReloadAutomatic));
		}

		private void PlayReloadAnimation()
		{
			#region Animation

			//Get the name of the animation state to play, which depends on weapon settings, and ammunition!
			string stateName = equippedWeapon.HasCycledReload() ? "Reload Open" :
				(equippedWeapon.HasAmmunition() ? "Reload" : "Reload Empty");
			
			//Play the animation state!
			characterAnimator.Play(stateName, layerActions, 0.0f);

			#endregion

			//Set Reloading Bool. This helps cycled reloads know when they need to stop cycling.
			const string boolName = "Reloading";
			characterAnimator.SetBool(boolName, reloading = true);
			
			//Reload.
			equippedWeapon.Reload();
		}

        private void SetGranadesCount()
        {
            grenadeTotal = SaveSystem.Instance.GetData().GranadesCount;
        }

		public void ChangeSensetive(float newSensetive)
		{
			_lookSensetive = newSensetive;

        }

		private void SetSavedSensetive()
		{
			_lookSensetive = SaveSystem.Instance.GetData().Sensetive;
        }

        /// <summary>
        /// Plays The Reload Animation After A Delay. Helpful to reload automatically after running out of ammunition.
        /// </summary>
        private IEnumerator TryReloadAutomatic()
		{
			//Yield.
			yield return new WaitForSeconds(equippedWeapon.GetAutomaticallyReloadOnEmptyDelay());

			//Play Reload Animation.
			PlayReloadAnimation();
		}

		/// <summary>
		/// Equip Weapon Coroutine.
		/// </summary>
		private IEnumerator Equip(int index = 0)
		{
			//Only if we're not holstered, holster. If we are already, we don't need to wait.
			if(!holstered)
			{
				//Holster.
				SetHolstered(holstering = true);
				//Wait.
				yield return new WaitUntil(() => holstering == false);
			}
			//Unholster. We do this just in case we were holstered.
			SetHolstered(false);
			//Play Unholster Animation.
			characterAnimator.Play("Unholster", layerHolster, 0);
			
			//Equip The New Weapon.
			inventory.Equip(index);
			//Refresh.
			RefreshWeaponSetup();
		}
		/// <summary>
		/// Refresh all weapon things to make sure we're all set up!
		/// </summary>
		public void RefreshWeaponSetup()
		{
			//Make sure we have a weapon. We don't want errors!
			if ((equippedWeapon = inventory.GetEquipped()) == null)
			{
				Debug.Log("inventory.GetEquipped()) == null");
                return;
            }
				
			
			//Update Animator Controller. We do this to update all animations to a specific weapon's set.
			characterAnimator.runtimeAnimatorController = equippedWeapon.GetAnimatorController();

			//Get the attachment manager so we can use it to get all the attachments!
			weaponAttachmentManager = equippedWeapon.GetAttachmentManager();
			if (weaponAttachmentManager == null) 
				return;
			
			//Get equipped scope. We need this one for its settings!
			equippedWeaponScope = weaponAttachmentManager.GetEquippedScope();
			//Get equipped magazine. We need this one for its settings!
			equippedWeaponMagazine = weaponAttachmentManager.GetEquippedMagazine();
		}

		private void FireEmpty()
		{
			/*
			 * Save Time. Even though we're not actually firing, we still need this for the fire rate between
			 * empty shots.
			 */
			lastShotTime = Time.time;
			//Play.
			characterAnimator.CrossFade("Fire Empty", 0.05f, layerOverlay, 0);
		}
		/// <summary>
		/// Updates the cursor state based on the value of the cursorLocked variable.
		/// </summary>
		private void UpdateCursorState()
		{
			Cursor.visible = !cursorLocked;
			Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
		}

		/// <summary>
		/// Plays The Grenade Throwing Animation.
		/// </summary>
		private void PlayGrenadeThrow()
		{
			//Start State.
			throwingGrenade = true;
			
			//Play Normal.
			characterAnimator.CrossFade("Grenade Throw", 0.15f,
				characterAnimator.GetLayerIndex("Layer Actions Arm Left"), 0.0f);
					
			//Play Additive.
			characterAnimator.CrossFade("Grenade Throw", 0.05f,
				characterAnimator.GetLayerIndex("Layer Actions Arm Right"), 0.0f);
		}
		/// <summary>
		/// Play The Melee Animation.
		/// </summary>
		private void PlayMelee()
		{
			//Start State.
			meleeing = true;
			
			//Play Normal.
			characterAnimator.CrossFade("Knife Attack", 0.05f,
				characterAnimator.GetLayerIndex("Layer Actions Arm Left"), 0.0f);
			
			//Play Additive.
			characterAnimator.CrossFade("Knife Attack", 0.05f,
				characterAnimator.GetLayerIndex("Layer Actions Arm Right"), 0.0f);
		}
		
		/// <summary>
		/// Changes the value of bolting, and updates the animator.
		/// </summary>
		private void UpdateBolt(bool value)
		{
			//Update.
			characterAnimator.SetBool(HashBoltAction, bolting = value);
		}
		/// <summary>
		/// Updates the "Holstered" variable, along with the Character's Animator value.
		/// </summary>
		private void SetHolstered(bool value = true)
		{
			//Update value.
			holstered = value;
			
			//Update Animator.
			const string boolName = "Holstered";
			characterAnimator.SetBool(boolName, holstered);	
		}
		
		#region ACTION CHECKS

		/// <summary>
		/// Can Fire.
		/// </summary>
		private bool CanPlayAnimationFire()
		{
			//Block.
			if (holstered || holstering)
				return false;

			//Block.
			if (meleeing || throwingGrenade)
				return false;

			//Block.
			if (reloading || bolting)
				return false;

			//Block.
			if (inspecting)
				return false;

			//Return.
			return true;
		}

		/// <summary>
		/// Determines if we can play the reload animation.
		/// </summary>
		private bool CanPlayAnimationReload()
		{
			//No reloading!
			if (reloading)
				return false;

			//No meleeing!
			if (meleeing)
				return false;

			//Not actioning a bolt.
			if (bolting)
				return false;

			//Can't reload while throwing a grenade.
			if (throwingGrenade)
				return false;

			//Block while inspecting.
			if (inspecting)
				return false;
			
			//Block Full Reloading if needed.
			if (!equippedWeapon.CanReloadWhenFull() && equippedWeapon.IsFull())
				return false;
			
			//Return.
			return true;
		}
		
		/// <summary>
		/// Returns true if the character is able to throw a grenade.
		/// </summary>
		private bool CanPlayAnimationGrenadeThrow()
		{
			//Block.
			if (holstered || holstering)
				return false;

			//Block.
			if (meleeing || throwingGrenade)
				return false;

			//Block.
			if (reloading || bolting)
				return false;

			//Block.
			if (inspecting)
				return false;
			
			//We need to have grenades!
			if (!grenadesUnlimited && grenadeCount == 0)
				return false;
			
			//Return.
			return true;
		}

		/// <summary>
		/// Returns true if the Character is able to melee attack.
		/// </summary>
		private bool CanPlayAnimationMelee()
		{
			//Block.
			if (holstered || holstering)
				return false;

			//Block.
			if (meleeing || throwingGrenade)
				return false;

			//Block.
			if (reloading || bolting)
				return false;

			//Block.
			if (inspecting)
				return false;
			
			//Return.
			return true;
		}

		/// <summary>
		/// Returns true if the character is able to holster their weapon.
		/// </summary>
		/// <returns></returns>
		private bool CanPlayAnimationHolster()
		{
			//Block.
			if (meleeing || throwingGrenade)
				return false;

			//Block.
			if (reloading || bolting)
				return false;

			//Block.
			if (inspecting)
				return false;
			
			//Return.
			return true;
		}

		/// <summary>
		/// Returns true if the Character can change their Weapon.
		/// </summary>
		/// <returns></returns>
		private bool CanChangeWeapon()
		{
			//Block.
			if (holstering)
				return false;

			//Block.
			if (meleeing || throwingGrenade)
				return false;

			//Block.
			if (reloading || bolting)
				return false;

			//Block.
			if (inspecting)
				return false;
			
			//Return.
			return true;
		}

		/// <summary>
		/// Returns true if the Character can play the Inspect animation.
		/// </summary>
		private bool CanPlayAnimationInspect()
		{
			//Block.
			if (holstered || holstering)
				return false;

			//Block.
			if (meleeing || throwingGrenade)
				return false;

			//Block.
			if (reloading || bolting)
				return false;

			//Block.
			if (inspecting)
				return false;
			
			//Return.
			return true;
		}

		/// <summary>
		/// Returns true if the Character can Aim.
		/// </summary>
		/// <returns></returns>
		private bool CanAim()
		{
			//Block.
			if (holstered || inspecting)
				return false;

			//Block.
			if (meleeing || throwingGrenade)
				return false;

			//Block.
			if (reloading || holstering)
				return false;
			
			//Return.
			return true;
		}
		
		/// <summary>
		/// Returns true if the character can run.
		/// </summary>
		/// <returns></returns>
		private bool CanRun()
		{
			//Block.
			if (inspecting || bolting)
				return false;

			//No running while crouching.
			if (movementBehaviour.IsCrouching())
				return false;

			//Block.
			if (meleeing || throwingGrenade)
				return false;

			//Block.
			if (reloading || aiming)
				return false;

			//While trying to fire, we don't want to run. We do this just in case we do fire.
			if (holdingButtonFire && equippedWeapon.HasAmmunition())
				return false;

			//This blocks running backwards, or while fully moving sideways.
			if (axisMovement.y <= 0 || Math.Abs(Mathf.Abs(axisMovement.x) - 1) < 0.01f)
				return false;
			
			//Return.
			return true;
		}

		#endregion

		#region INPUT

		/// <summary>
		/// Fire.
		/// </summary>
		public void OnTryFire(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;

			//Switch.
			switch (context)
			{
				//Started.
				case {phase: InputActionPhase.Started}:
					//Hold.
					holdingButtonFire = true;
					
					//Restart the shots.
					shotsFired = 0;
					break;
				//Performed.
				case {phase: InputActionPhase.Performed}:
					//Ignore if we're not allowed to actually fire.
					if (!CanPlayAnimationFire())
						break;
					
					//Check.
					if (equippedWeapon.HasAmmunition())
					{
						//Check.
						if (equippedWeapon.IsAutomatic())
						{
							//Reset fired shots, so recoil/spread does not just stay at max when we've run out
							//of ammo already!
							shotsFired = 0;
							
							//Break.
							break;
						}
							
						//Has fire rate passed.
						if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
							Fire();
					}
					//Fire Empty.
					else
						FireEmpty();
					break;
				//Canceled.
				case {phase: InputActionPhase.Canceled}:
					//Stop Hold.
					holdingButtonFire = false;

					//Reset shotsFired.
					shotsFired = 0;
					break;
			}
		}
		/// <summary>
		/// Reload.
		/// </summary>
		public void OnTryPlayReload(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;
			
			//Block.
			if (!CanPlayAnimationReload())
				return;
			
			//Switch.
			switch (context)
			{
				//Performed.
				case {phase: InputActionPhase.Performed}:
					//Play Animation.
					PlayReloadAnimation();
					break;
			}
		}

		/// <summary>
		/// Inspect.
		/// </summary>
		public void OnTryInspect(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;
			
			//Block.
			if (!CanPlayAnimationInspect())
				return;
			
			//Switch.
			switch (context)
			{
				//Performed.
				case {phase: InputActionPhase.Performed}:
					//Play Animation.
					Inspect();
					break;
			}
		}
		/// <summary>
		/// Aiming.
		/// </summary>
		public void OnTryAiming(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;

			//Switch.
			switch (context.phase)
			{
				case InputActionPhase.Started:
					//Started.
					holdingButtonAim = true;
					//Save Time.
					aimStartTime = Time.time;
					break;
				case InputActionPhase.Canceled:
					//Canceled.
					holdingButtonAim = false;
					break;
			}
		}

		/// <summary>
		/// Holster.
		/// </summary>
		public void OnTryHolster(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;
			
			//Switch.
			switch (context.phase)
			{
				//Performed.
				case InputActionPhase.Performed:
					//Check.
					if (CanPlayAnimationHolster())
					{
						//Set.
						SetHolstered(!holstered);
						//Holstering.
						holstering = true;
					}
					break;
			}
		}
		/// <summary>
		/// Throw Grenade. 
		/// </summary>
		public void OnTryThrowGrenade(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;
			
			//Switch.
			switch (context.phase)
			{
				//Performed.
				case InputActionPhase.Performed:
					//Try Play.
					if (CanPlayAnimationGrenadeThrow())
						PlayGrenadeThrow();
					break;
			}
		}
		
		/// <summary>
		/// Melee.
		/// </summary>
		public void OnTryMelee(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;
			
			//Switch.
			switch (context.phase)
			{
				//Performed.
				case InputActionPhase.Performed:
					//Try Play.
					if (CanPlayAnimationMelee())
						PlayMelee();
					break;
			}
		}

		private void OnPushForceUsed()
		{
            if (CanPlayAnimationMelee())
                PlayMelee();
        }
		/// <summary>
		/// Run. 
		/// </summary>
		public void OnTryRun(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;
			
			//Switch.
			switch (context.phase)
			{
				//Started.
				case InputActionPhase.Started:
					//Start.
					holdingButtonRun = true;
					break;
				//Canceled.
				case InputActionPhase.Canceled:
					//Stop.
					holdingButtonRun = false;
					break;
			}
		}

		/// <summary>
		/// Jump. 
		/// </summary>
		public void OnTryJump(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;

			//Switch.
			switch (context.phase)
			{
				//Performed.
				case InputActionPhase.Performed:
					//Jump.
					movementBehaviour.Jump();
					break;
			}
		}
		/// <summary>
		/// Next Inventory Weapon.
		/// </summary>
		public void OnTryInventoryNext(InputAction.CallbackContext context)
		{
			//Block while the cursor is unlocked.
			if (!cursorLocked)
				return;
			
			//Null Check.
			if (inventory == null)
				return;
			
			//Switch.
			switch (context)
			{
				//Performed.
				case {phase: InputActionPhase.Performed}:
					//Get the index increment direction for our inventory using the scroll wheel direction. If we're not
					//actually using one, then just increment by one.
					float scrollValue = context.valueType.IsEquivalentTo(typeof(Vector2)) ? Mathf.Sign(context.ReadValue<Vector2>().y) : 1.0f;
					
					//Get the next index to switch to.
					int indexNext = scrollValue > 0 ? inventory.GetNextIndex() : inventory.GetLastIndex();
					//Get the current weapon's index.
					int indexCurrent = inventory.GetEquippedIndex();
					
					//Make sure we're allowed to change, and also that we're not using the same index, otherwise weird things happen!
					if (CanChangeWeapon() && (indexCurrent != indexNext))
						StartCoroutine(nameof(Equip), indexNext);
					break;
			}
		}

        public void OnTryEquipWeapun(InputAction.CallbackContext context)
        {
            if (!cursorLocked)
                return;

            //Null Check.
            if (inventory == null)
                return;

            //Switch.
            switch (context)
            {
                //Performed.
                case { phase: InputActionPhase.Performed }:

					float weaponIndex = context.ReadValue<Single>() - 1;

                    int indexCurrent = inventory.GetEquippedIndex();
					int weaponsLength = inventory.GetWeaponsLength();

                    if (CanChangeWeapon() && (indexCurrent != weaponIndex) && (weaponIndex < weaponsLength))
					{
                        StartCoroutine(nameof(Equip), weaponIndex);
                    }
                        
                    break;
            }
        }

        public void OnLockCursor(InputAction.CallbackContext context)
		{
			//Debug.Log("Character: Esc pressed");
			////Switch.
			//switch (context)
			//{
			//	//Performed.
			//	case { phase: InputActionPhase.Performed }:
			//		//Toggle the cursor locked value.
			//		cursorLocked = !cursorLocked;
			//		//Update the cursor's state.
			//		UpdateCursorState();
			//		break;
			//}
		}
		
		/// <summary>
		/// Movement.
		/// </summary>
		public void OnMove(InputAction.CallbackContext context)
		{
			//Read.
			axisMovement = cursorLocked ? context.ReadValue<Vector2>() : default;
		}
		/// <summary>
		/// Look.
		/// </summary>
		public void OnLook(InputAction.CallbackContext context)
		{
            //Read.
            axisLook = cursorLocked ? context.ReadValue<Vector2>() : default;

			//Make sure that we have a weapon.
			if (equippedWeapon == null)
				return;

			//Make sure that we have a scope.
			if (equippedWeaponScope == null)
				return;

			//If we're aiming, multiply by the mouse sensitivity multiplier of the equipped weapon's scope!
			axisLook *= aiming ? equippedWeaponScope.GetMultiplierMouseSensitivity() : 1.0f;
			axisLook *= _lookSensetive;
		}

		/// <summary>
		/// Called in order to update the tutorial text value.
		/// </summary>
		public void OnUpdateTutorial(InputAction.CallbackContext context)
		{
			//Switch.
			tutorialTextVisible = context switch
			{
				//Started. Show the tutorial.
				{phase: InputActionPhase.Started} => true,
				//Canceled. Hide the tutorial.
				{phase: InputActionPhase.Canceled} => false,
				//Default.
				_ => tutorialTextVisible
			};
		}

		public void OnTryUseForce(InputAction.CallbackContext context)
		{
            switch (context)
            {
                case { phase: InputActionPhase.Performed }:
                    _player.TryUsePushForce();
                    break;
            }
        }

		#endregion

		#region ANIMATION EVENTS

		public override void EjectCasing()
		{
			//Notify the weapon.
			if(equippedWeapon != null)
				equippedWeapon.EjectCasing();
		}
		public override void FillAmmunition(int amount)
		{
			//Notify the weapon to fill the ammunition by the amount.
			if(equippedWeapon != null)
				equippedWeapon.FillAmmunition(amount);
		}
		
		public override void Grenade()
		{
			//Make sure that the grenade is valid, otherwise we'll get errors.
			if (grenadePrefab == null)
				return;

			//Make sure we have a camera!
			if (cameraWorld == null)
				return;
			
			//Remove Grenade.
			if(!grenadesUnlimited)
				grenadeCount--;
			
			//Get Camera Transform.
			Transform cTransform = cameraWorld.transform;
			//Calculate the throwing location.
			Vector3 position = cTransform.position;
			position += cTransform.forward * grenadeSpawnOffset;
			//Throw.
			Instantiate(grenadePrefab, position, cTransform.rotation);
		}
		public override void SetActiveMagazine(int active)
		{
			//Set magazine gameObject active.
			equippedWeaponMagazine.gameObject.SetActive(active != 0);
		}

		public override void AnimationEndedBolt()
		{
			//Update.
			UpdateBolt(false);
		}
		public override void AnimationEndedReload()
		{
			//Stop reloading!
			reloading = false;
		}

		public override void AnimationEndedGrenadeThrow()
		{
			//Stop Grenade Throw.
			throwingGrenade = false;
		}
		public override void AnimationEndedMelee()
		{
			//Stop Melee.
			meleeing = false;
		}

		public override void AnimationEndedInspect()
		{
			//Stop Inspecting.
			inspecting = false;
		}
		public override void AnimationEndedHolster()
		{
			//Stop Holstering.
			holstering = false;
		}

		public override void SetSlideBack(int back)
		{
			//Set slide back.
			if (equippedWeapon != null)
				equippedWeapon.SetSlideBack(back);
		}

		public override void SetActiveKnife(int active)
		{
			//Set Active.
			knife.SetActive(active != 0);
		}

		public override void LockCursor(bool needlock)
		{
            cursorLocked = needlock;
            UpdateCursorState();
        }

		#endregion

		#endregion
	}
}