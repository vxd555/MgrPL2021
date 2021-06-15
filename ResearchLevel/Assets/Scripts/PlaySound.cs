using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
	public ChangeMusic lab = null;
	public AudioSource audio = null;
	bool isActive = false;

	private void OnTriggerEnter(Collider other)
	{
		if(isActive && lab == null)
			return;
		if(lab != null && (!lab.isActive || isActive))
			return;

		audio.Play();
		isActive = true;
	}
}
