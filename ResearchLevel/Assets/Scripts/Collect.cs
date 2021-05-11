using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collect : MonoBehaviour
{
    public FirstPersonController fpc;
    public Transform direction;
    private RaycastHit hit;
    private Vector3 boxSize = new Vector3(0.3f, 0.3f, 0.3f);

    public GameObject uiCollect = null;
    public Text uiCollectText = null;

    private int barrelAmount = 0;

    private List<int> barrelOrder = new List<int>();

    private bool fly = false;
    private float timeFromStart = 0f;

    private int mapType = 0;

    //korytarz, schodki, wierza, linie, rury
    public List<GameObject> elementsToChange = new List<GameObject>();

    public Material earth = null;
    public Material space = null;

	private void Start()
	{
        mapType = Random.Range(0, 3);

        if(mapType == 0)
		{
            RenderSettings.skybox = space;
            elementsToChange[0].SetActive(false);
            elementsToChange[1].SetActive(false);

        }
        else if(mapType == 1)
		{
            elementsToChange[3].SetActive(false);
            elementsToChange[0].SetActive(false);
            elementsToChange[2].SetActive(false);
        }
        else
		{
            elementsToChange[4].SetActive(false);
            elementsToChange[1].SetActive(false);
        }

    }

	void Update()
    {
        if (fly) return;

        if(Physics.BoxCast(direction.position, boxSize, direction.forward, out hit, transform.rotation, 2f))
		{
            if (hit.collider.tag == "Barrel")
            {
                uiCollectText.text = "Wciśnij E aby zebrać";
                uiCollect.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    barrelOrder.Add(hit.collider.GetComponent<Barrel>().index);
                    ++barrelAmount;
                    hit.collider.gameObject.SetActive(false);
                    uiCollect.SetActive(false);
                }
            }
            else if (hit.collider.tag == "Ship")
            {
                if (barrelAmount > 1)
                {
                    uiCollect.SetActive(true);
                    uiCollectText.text = "Wciśnij E aby odlecieć";

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        fpc.playerCanMove = false;
                        fpc.cameraCanMove = false;
                        fpc.enableHeadBob = false;
                        fpc.enabled = false;

                        string result = "";
                        for (int i = 0; i < barrelOrder.Count; ++i)
                        {
                            result += barrelOrder[i] + " ";
                        }

                        timeFromStart = Time.timeSinceLevelLoad;

                        if (timeFromStart % 60 >= 10)
                        {
                            uiCollectText.text = $"Zapisz dane poniżej będą potrzebne do wypełnienia ankiety \n\n Mapa: {mapType + 1} \n\n Czas przejścia: {(int)(timeFromStart / 60)}:{(int)(timeFromStart % 60)}\n\n Kolejność: {result}";
                        }
                        else
                        {
                            uiCollectText.text = $"Zapisz dane poniżej będą potrzebne do wypełnienia ankiety \n\n Mapa: {mapType + 1} \n\n Czas przejścia: {(int)(timeFromStart / 60)}:0{(int)(timeFromStart % 60)}\n\n Kolejność{result}";

                        }
                        uiCollect.SetActive(true);


                        fly = true;
                        return;
                    }
                }
                else uiCollectText.text = "";
            }
            else
            {
                uiCollect.SetActive(false);
                uiCollectText.text = "";
            }
        }
        else uiCollect.SetActive(false);
    }
}
