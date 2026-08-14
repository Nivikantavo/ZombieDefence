//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Weapon Attachment Manager. Handles equipping and storing a Weapon's Attachments.
    /// </summary>
    public class WeaponAttachmentManager : WeaponAttachmentManagerBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "Scope")]

        [Tooltip("Determines if the ironsights should be shown on the weapon model.")]
        [SerializeField]
        private bool scopeDefaultShow = true;
        
        [Tooltip("Default Scope!")]
        [SerializeField]
        private ScopeBehaviour scopeDefaultBehaviour;

        [Tooltip("Selected Scope Index. If you set this to a negative number, ironsights will be selected as the enabled scope.")]
        [SerializeField]
        private int scopeIndex = -1;

        [Tooltip("First scope index when using random scopes.")]
        [SerializeField]
        private int scopeIndexFirst = -1;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool scopeIndexRandom;

        [HideInInspector]
        [SerializeField]
        private int upgradeScopeIndex;

        [Tooltip("All possible Scope Attachments that this Weapon can use!")]
        [SerializeField]
        private ScopeBehaviour[] scopeArray;
        
        [Title(label: "Muzzle")]

        [Tooltip("Selected Muzzle Index.")]
        [SerializeField]
        private int muzzleIndex;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool muzzleIndexRandom = true;

        [HideInInspector]
        [SerializeField]
        private int upgradeMuzzleIndex;

        [Tooltip("All possible Muzzle Attachments that this Weapon can use!")]
        [SerializeField]
        private MuzzleBehaviour[] muzzleArray;
        
        [Title(label: "Laser")]

        [Tooltip("Selected Laser Index.")]
        [SerializeField]
        private int laserIndex = -1;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool laserIndexRandom = true;

        [HideInInspector]
        [SerializeField]
        private int upgradeLaserIndex;

        [Tooltip("All possible Laser Attachments that this Weapon can use!")]
        [SerializeField]
        private LaserBehaviour[] laserArray;
        
        [Title(label: "Grip")]

        [Tooltip("Selected Grip Index.")]
        [SerializeField]
        private int gripIndex = -1;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool gripIndexRandom = true;

        [HideInInspector]
        [SerializeField]
        private int upgradeGripIndex;

        [Tooltip("All possible Grip Attachments that this Weapon can use!")]
        [SerializeField]
        private GripBehaviour[] gripArray;
        
        [Title(label: "Magazine")]

        [Tooltip("Selected Magazine Index.")]
        [SerializeField]
        private int magazineIndex;
        
        [Tooltip("Should we pick a random index when starting the game?")]
        [SerializeField]
        private bool magazineIndexRandom = true;

        [Tooltip("All possible Magazine Attachments that this Weapon can use!")]
        [SerializeField]
        private Magazine[] magazineArray;

        [Title(label: "Upgrade Levels")]

        [Tooltip("Attachment indices for weapon levels 1-5. Level 1 is the base loadout. Level 3 matches the old single upgrade. If empty, values are filled from the default and upgrade indices.")]
        [SerializeField]
        private WeaponAttachmentLevel[] attachmentLevels;

        #endregion

        #region FIELDS

        /// <summary>
        /// Equipped Scope.
        /// </summary>
        private ScopeBehaviour scopeBehaviour;
        /// <summary>
        /// Equipped Muzzle.
        /// </summary>
        private MuzzleBehaviour muzzleBehaviour;
        /// <summary>
        /// Equipped Laser.
        /// </summary>
        private LaserBehaviour laserBehaviour; 
        /// <summary>
        /// Equipped Grip.
        /// </summary>
        private GripBehaviour gripBehaviour;
        /// <summary>
        /// Equipped Magazine.
        /// </summary>
        private MagazineBehaviour magazineBehaviour;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake.
        /// </summary>
        protected override void Awake()
        {
            if (magazineBehaviour == null)
            {
                SetDefultOrRandomAttachments();
            }
        }

        public override void ApplyLevel(int level)
        {
            level = WeaponUpgradeLevels.Clamp(level);
            EnsureAttachmentLevels();

            if (level <= 1)
            {
                SetDefultOrRandomAttachments();
            }
            else
            {
                ApplyAttachmentLevel(attachmentLevels[WeaponUpgradeLevels.ToIndex(level)]);
            }

            ApplyMagazineLevel(level);
        }

        public override void SetUpgradeAttachments()
        {
            ApplyLevel(WeaponUpgradeLevels.LegacyMappedLevel);
        }

        private void ApplyAttachmentLevel(WeaponAttachmentLevel attachmentLevel)
        {
            scopeBehaviour = scopeArray.SelectAndSetActive(attachmentLevel.ScopeIndex);

            if (scopeBehaviour == null)
            {
                scopeBehaviour = scopeDefaultBehaviour;
                if (scopeBehaviour != null)
                    scopeBehaviour.gameObject.SetActive(scopeDefaultShow);
            }

            muzzleBehaviour = muzzleArray.SelectAndSetActive(attachmentLevel.MuzzleIndex);
            laserBehaviour = laserArray.SelectAndSetActive(attachmentLevel.LaserIndex);
            gripBehaviour = gripArray.SelectAndSetActive(attachmentLevel.GripIndex);

            magazineBehaviour = magazineArray.SelectAndSetActive(magazineIndex);
            if (magazineBehaviour == null)
            {
                magazineBehaviour = magazineArray.SelectAndSetActive(0);
            }
        }

        private void ApplyMagazineLevel(int level)
        {
            Magazine magazine = magazineBehaviour as Magazine;
            if (magazine == null && magazineArray != null && magazineIndex >= 0 && magazineIndex < magazineArray.Length)
            {
                magazine = magazineArray[magazineIndex];
            }

            magazine?.ApplyLevel(level);
        }

        private void EnsureAttachmentLevels()
        {
            if (WeaponUpgradeLevels.NeedsFill(attachmentLevels) == false)
            {
                return;
            }

            WeaponAttachmentLevel level1 = new WeaponAttachmentLevel
            {
                ScopeIndex = scopeIndex,
                MuzzleIndex = muzzleIndex,
                LaserIndex = laserIndex,
                GripIndex = gripIndex
            };

            WeaponAttachmentLevel level3 = new WeaponAttachmentLevel
            {
                ScopeIndex = upgradeScopeIndex,
                MuzzleIndex = upgradeMuzzleIndex,
                LaserIndex = upgradeLaserIndex,
                GripIndex = upgradeGripIndex
            };

            attachmentLevels = WeaponUpgradeLevels.CreateAttachmentLevels(level1, level3);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureAttachmentLevels();
        }

        [ContextMenu("Rebuild Attachment Levels From Legacy")]
        private void RebuildAttachmentLevelsFromLegacy()
        {
            attachmentLevels = null;
            EnsureAttachmentLevels();
        }
#endif


        public void SetDefultOrRandomAttachments()
        {
            if (scopeIndexRandom && scopeArray != null && scopeArray.Length > 0)
                scopeIndex = Random.Range(scopeIndexFirst, scopeArray.Length);
            scopeBehaviour = scopeArray.SelectAndSetActive(scopeIndex);
            if (scopeBehaviour == null)
            {
                scopeBehaviour = scopeDefaultBehaviour;
                if (scopeBehaviour != null)
                    scopeBehaviour.gameObject.SetActive(scopeDefaultShow);
            }

            if (muzzleIndexRandom && muzzleArray != null && muzzleArray.Length > 0)
                muzzleIndex = Random.Range(0, muzzleArray.Length);
            muzzleBehaviour = muzzleArray.SelectAndSetActive(muzzleIndex);

            if (laserIndexRandom && laserArray != null && laserArray.Length > 0)
                laserIndex = Random.Range(0, laserArray.Length);
            laserBehaviour = laserArray.SelectAndSetActive(laserIndex);

            if (gripIndexRandom && gripArray != null && gripArray.Length > 0)
                gripIndex = Random.Range(0, gripArray.Length);
            gripBehaviour = gripArray.SelectAndSetActive(gripIndex);

            if (magazineIndexRandom && magazineArray != null && magazineArray.Length > 0)
                magazineIndex = Random.Range(0, magazineArray.Length);
            magazineBehaviour = magazineArray.SelectAndSetActive(magazineIndex);
        }
        #endregion

        #region GETTERS

        public override ScopeBehaviour GetEquippedScope() => scopeBehaviour;
        public override ScopeBehaviour GetEquippedScopeDefault() => scopeDefaultBehaviour;

        public override MagazineBehaviour GetEquippedMagazine() => magazineBehaviour;
        public override MuzzleBehaviour GetEquippedMuzzle() => muzzleBehaviour;

        public override LaserBehaviour GetEquippedLaser() => laserBehaviour;
        public override GripBehaviour GetEquippedGrip() => gripBehaviour;

        #endregion
    }
}