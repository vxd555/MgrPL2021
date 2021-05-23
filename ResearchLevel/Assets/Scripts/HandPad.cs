using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPad : MonoBehaviour
{
    public Animator doorAnim = null;
    public AudioSource doorSound = null;
	public MeshRenderer meshRenderer = null;
	public Material onMaterial = null;
	public Material offMaterial = null;
	public GameObject elementToOff = null;

	public void Start()
	{
		if(doorAnim.GetBool("character_nearby"))
			meshRenderer.materials[1] = onMaterial;
		else
			meshRenderer.materials[1] = offMaterial;
	}

	public void Use()
	{
		if(elementToOff != null)
			elementToOff.SetActive(false);

		doorAnim.SetBool("character_nearby", !doorAnim.GetBool("character_nearby"));

		if(doorAnim.GetBool("character_nearby"))
		{
			Material[] mats = meshRenderer.materials;
			mats[1] = onMaterial;
			meshRenderer.materials = mats;
		}
		else
		{
			Material[] mats = meshRenderer.materials;
			mats[1] = offMaterial;
			meshRenderer.materials = mats;
		}

		doorSound.Play();

	}
}
