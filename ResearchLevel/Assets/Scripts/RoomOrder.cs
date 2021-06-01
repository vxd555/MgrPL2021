using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomOrder : MonoBehaviour
{
	public int roomIndex = 0;

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			Collect c = other.GetComponent<Collect>();
			if(!c.roomOrder.Contains(roomIndex))
				c.roomOrder.Add(roomIndex);
		}
	}
}
