using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolutionPuzzle : MonoBehaviour
{
	public bool on = false;
    public List<LightPuzzle> lightPuzzles = new List<LightPuzzle>();
	public Animator doorAnim = null;

	public AudioSource audioSound = null;
	public AudioClip handPadDoor = null;


	public void CheckAndOpen()
	{
		for(int i = 0; i < lightPuzzles.Count; ++i)
		{
			if(!lightPuzzles[i].isEnable)
			{
				if(on)
					audioSound.PlayOneShot(handPadDoor);
				on = false;
				doorAnim.SetBool("character_nearby", false);
				return;
			}
		}

		on = true;
		audioSound.PlayOneShot(handPadDoor);
		doorAnim.SetBool("character_nearby", true);
	}
}
