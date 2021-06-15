using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour
{
    public AudioSource audio = null;
    public AudioClip clip = null;

	[HideInInspector]
	public bool isActive = false;

	public void OnTriggerEnter(Collider other)
	{
		if(isActive)
			return;

		audio.clip = clip;
		audio.Play();
		isActive = true;
	}
}
