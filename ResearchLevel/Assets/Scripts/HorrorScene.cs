using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorScene : MonoBehaviour
{
	public GameObject offChest = null;
	public GameObject onChest = null;

	private bool isActive = false;

	private IEnumerator coroutine;

	public void OnTriggerEnter(Collider other)
	{
		if(isActive)
			return;

		offChest.SetActive(false);
		onChest.SetActive(true);
		isActive = true;

		coroutine = Fade();
		StartCoroutine(coroutine);

	}

	IEnumerator Fade()
	{
		yield return new WaitForSeconds(8f);
		for(int c = 190; c > 28; c -= 3)
		{
			if(c < 28)
				c = 28;

			Color color = new Color(c / 255f, c / 255f, c / 255f, 1f);
			RenderSettings.ambientLight = color;
			yield return new WaitForSeconds(.04f);
		}
	}
}
