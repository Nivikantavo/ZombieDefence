//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Magazine.
    /// </summary>
    public class Magazine : MagazineBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "Settings")]
        
        [Tooltip("Total Ammunition.")]
        [SerializeField]
        private int ammunitionTotal = 10;

        [HideInInspector]
        [SerializeField]
        private int upgradeAmmunitionTotal = 10;

        [Tooltip("Magazine size for weapon levels 1-5. Level 1 is the base magazine. Level 3 matches the old single upgrade. If empty, values are filled from Ammunition Total / Upgrade Ammunition Total.")]
        [SerializeField]
        private int[] ammunitionByLevel;

        [Title(label: "Interface")]

        [Tooltip("Interface Sprite.")]
        [SerializeField]
        private Sprite sprite;

        #endregion

        private int currentLevel = 1;

        public override void ApplyLevel(int level)
        {
            EnsureAmmunitionByLevel();
            currentLevel = WeaponUpgradeLevels.Clamp(level);
        }

        public override void SetUpgradeAmmunition()
        {
            ApplyLevel(WeaponUpgradeLevels.LegacyMappedLevel);
        }

        private void EnsureAmmunitionByLevel()
        {
            if (WeaponUpgradeLevels.NeedsFill(ammunitionByLevel) == false)
            {
                return;
            }

            ammunitionByLevel = WeaponUpgradeLevels.CreateInterpolated(ammunitionTotal, upgradeAmmunitionTotal);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureAmmunitionByLevel();
        }

        [ContextMenu("Rebuild Ammunition Levels From Legacy")]
        private void RebuildAmmunitionLevelsFromLegacy()
        {
            ammunitionByLevel = WeaponUpgradeLevels.CreateInterpolated(ammunitionTotal, upgradeAmmunitionTotal);
        }
#endif

        #region GETTERS

        /// <summary>
        /// Ammunition Total.
        /// </summary>
        public override int GetAmmunitionTotal()
        {
            EnsureAmmunitionByLevel();
            int index = WeaponUpgradeLevels.ToIndex(currentLevel);
            if (ammunitionByLevel == null || index < 0 || index >= ammunitionByLevel.Length)
                return ammunitionTotal;

            return ammunitionByLevel[index];
        }
        /// <summary>
        /// Sprite.
        /// </summary>
        public override Sprite GetSprite() => sprite;

        #endregion
    }
}