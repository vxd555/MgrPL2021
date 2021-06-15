using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPadPuzzle : MonoBehaviour
{
    public List<LightPuzzle> lightPuzzles = new List<LightPuzzle>();
	public SolutionPuzzle solution = null;

	public Material off = null;
	public Material on = null;
	public MeshRenderer mesh = null;
	private bool isEnable = false;

	public void Use()
	{
		for(int i = 0; i < lightPuzzles.Count; ++i)
		{
			lightPuzzles[i].Switch();
		}

		isEnable = !isEnable;

		if(isEnable)
		{
			Material[] mats = mesh.materials;
			mats[1] = on;
			mesh.materials = mats;
		}
		else
		{
			Material[] mats = mesh.materials;
			mats[1] = off;
			mesh.materials = mats;
		}

		solution.CheckAndOpen();
	}
}
