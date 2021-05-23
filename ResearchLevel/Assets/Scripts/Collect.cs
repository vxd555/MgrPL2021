using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collect : MonoBehaviour
{
    [Header("Control")]
    public FirstPersonController fpc;
    public Transform direction;
    private RaycastHit hit;
    private Vector3 boxSize = new Vector3(0.2f, 0.2f, 0.2f);

    [Header("Interface")]
    public GameObject uiCollect = null;
    public Text uiCollectText = null;

    //ilość beczek i kolejność ich zbierania
    private int barrelAmount = 0;
    private List<int> barrelOrder = new List<int>();

    private bool fly = false; //koniec gry
    private float timeFromStart = 0f; //licznik czasu gry
    private int mapType = 0; //jaka mapa się wylosowała

    //korytarz, schodki, wierza, linie, rury
    public List<GameObject> elementsToChange = new List<GameObject>();

    [Header("Skybox")]
    public Material earth = null;
    public Material space = null;

    [Header("Audio")]
    public AudioSource audioSpeak = null;
    public AudioSource audioStep = null;
    public AudioSource audioSound = null;
    public AudioClip[] steps = new AudioClip[10];
    public AudioClip openDoor = null;
    public AudioClip closeDoor = null;
    public AudioClip collectBarrel = null;
    public AudioClip handPadDoor = null;
    public AudioClip endSpeak = null;
    public float stepTimeSpan = 0.65f;
    public float stepSprintTimeSpan = 0.45f;
    private float timeToNextStep;


	private void Start()
	{
        mapType = Random.Range(0, 3);

        if(mapType == 0)
        {
            RenderSettings.skybox = space;

        }
        else if(mapType == 1)
        {
            elementsToChange[0].SetActive(false);
        }
        else
        {
            elementsToChange[1].SetActive(false);
        }

        /*
        //research2
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
        }*/

        timeToNextStep = 0f;

        Cursor.visible = false;
    }

	void Update()
    {
        if(fly)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
			{
                Application.Quit();
			}
            return;
        }

        if(Physics.BoxCast(direction.position, boxSize, direction.forward, out hit, transform.rotation, 2.5f))
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
                    audioSound.PlayOneShot(collectBarrel);
                }
            }
            else if (hit.collider.tag == "Ship")
            {
                if (barrelAmount > 2)
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
                            uiCollectText.text = $"Zapisz dane poniżej będą potrzebne do wypełnienia ankiety \n\n Mapa: {mapType + 1} \n\n Czas przejścia: {(int)(timeFromStart / 60)}:{(int)(timeFromStart % 60)}\n\n Kolejność: {result} \n\nKliknij Esc aby wyjść";
                        }
                        else
                        {
                            uiCollectText.text = $"Zapisz dane poniżej będą potrzebne do wypełnienia ankiety \n\n Mapa: {mapType + 1} \n\n Czas przejścia: {(int)(timeFromStart / 60)}:0{(int)(timeFromStart % 60)}\n\n Kolejność: {result} \n\nKliknij Esc aby wyjść";

                        }
                        uiCollect.SetActive(true);

                        audioSpeak.PlayOneShot(endSpeak);

                        fly = true;
                        return;
                    }
                }
                else uiCollectText.text = "";
            }
            else if(hit.collider.tag == "Door")
            {
                uiCollectText.text = "Wciśnij E aby otworzyć/zamknąć";
                uiCollect.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    Animator anim = hit.collider.GetComponent<Animator>();
                    if(anim == null)
					{
                        anim = hit.collider.GetComponentInParent<Animator>();
					}
                    if(anim != null)
					{
                        anim.SetBool("character_nearby", !anim.GetBool("character_nearby"));
                        if(anim.GetBool("character_nearby"))
                            audioSound.PlayOneShot(openDoor);
                        if(anim.GetBool("character_nearby"))
                            audioSound.PlayOneShot(closeDoor);
                    }
                }
            }
            else if(hit.collider.tag == "HandPad")
            {
                uiCollectText.text = "Wciśnij E aby użyć";
                uiCollect.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.GetComponent<HandPad>().Use();
                    audioSound.PlayOneShot(handPadDoor);
                }
            }
            else
            {
                uiCollect.SetActive(false);
                uiCollectText.text = "";
            }
        }
        else uiCollect.SetActive(false);

        if(fpc.IsWalking)
		{
            timeToNextStep -= Time.deltaTime;
            if(timeToNextStep <= 0f)
			{
                if(fpc.IsSprinting)
                    timeToNextStep = stepSprintTimeSpan;
                else
                    timeToNextStep = stepTimeSpan;
                audioStep.PlayOneShot(steps[Random.RandomRange(0, 10)]);
			}
        }
    }
}
