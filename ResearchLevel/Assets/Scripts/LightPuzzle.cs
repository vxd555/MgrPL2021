using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPuzzle : MonoBehaviour
{
    public bool isEnable = false;
	public Material off = null;
	public Material on = null;
	public MeshRenderer mesh = null;

    public void Switch()
	{
		isEnable = !isEnable;

		if(isEnable)
			mesh.material = on;
		else
			mesh.material = off;
	}
}
