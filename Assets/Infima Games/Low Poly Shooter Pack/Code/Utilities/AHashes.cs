//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Animator Hashes.
    /// </summary>
    public static class AHashes
    {
        /// <summary>
        /// Stop Trigger Hash.
        /// </summary>
        public static readonly int Stop = Animator.StringToHash("Stop");
        
        /// <summary>
        /// Reloading Bool Hash.
        /// </summary>
        public static readonly int Reloading = Animator.StringToHash("Reloading");
        /// <summary>
        /// Inspecting Bool Hash.
        /// </summary>
        public static readonly int Inspecting = Animator.StringToHash("Inspecting");
        
        /// <summary>
        /// Meleeing Bool Hash.
        /// </summary>
        public static readonly int Meleeing = Animator.StringToHash("Meleeing");
        /// <summary>
        /// Grenading Bool Hash.
        /// </summary>
        public static readonly int Grenading = Animator.StringToHash("Grenading");
        
        /// <summary>
        /// Bolt Action Bool Hash.
        /// </summary>
        public static readonly int Bolt = Animator.StringToHash("Bolt Action");
        
        /// <summary>
        /// Holstering Bool Hash.
        /// </summary>
        public static readonly int Holstering = Animator.StringToHash("Holstering");
        /// <summary>
        /// Holstered Bool Hash.
        /// </summary>
        public static readonly int Holstered = Animator.StringToHash("Holstered");

        /// <summary>
        /// Running Bool Hash.
        /// </summary>
        public static readonly int Running = Animator.StringToHash("Running");
        /// <summary>
        /// Lowered Bool Hash.
        /// </summary>
        public static readonly int Lowered = Animator.StringToHash("Lowered");
    }
}