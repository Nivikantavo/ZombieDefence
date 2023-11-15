//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;
using Plugins.Audio.Core;

namespace InfimaGames.LowPolyShooterPack.Legacy
{
	public class ExplosionScript : MonoBehaviour {
    
    	[Header("Customizable Options")]
    	//How long before the explosion prefab is destroyed
    	public float despawnTime = 10.0f;
    	//How long the light flash is visible
    	public float lightDuration = 0.02f;
    	[Header("Light")]
    	public Light lightFlash;
    
    	[Header("Audio")]
    	public AudioClip[] explosionSounds;
    	public AudioSource audioSource;
        public SourceAudio source;
    
    	private void Start () {
    		//Start the coroutines
    		StartCoroutine (DestroyTimer ());
    		StartCoroutine (LightFlash ());
    
    		//Get a random impact sound from the array
    		AudioClip clip = explosionSounds
    			[Random.Range(0, explosionSounds.Length)];
            //Play the random explosion sound
            source.Play(clip.name);
    	}
    
    	private IEnumerator LightFlash () {
    		//Show the light
    		lightFlash.GetComponent<Light>().enabled = true;
    		//Wait for set amount of time
    		yield return new WaitForSeconds (lightDuration);
    		//Hide the light
    		lightFlash.GetComponent<Light>().enabled = false;
    	}
    
    	private IEnumerator DestroyTimer () {
    		//Destroy the explosion prefab after set amount of seconds
    		yield return new WaitForSeconds (despawnTime);
    		Destroy (gameObject);
    	}
    }
}