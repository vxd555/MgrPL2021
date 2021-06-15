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
    [HideInInspector]
    public List<int> roomOrder = new List<int>();

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
    public AudioClip creakingAudio = null;
    public AudioClip beepAudio = null;
    public AudioClip[] woodAudio = new AudioClip[6];
    public AudioClip[] scaryAudio = new AudioClip[4];
    public float stepTimeSpan = 0.65f;
    public float stepSprintTimeSpan = 0.45f;
    private float timeToNextStep;

    [Header("Interaction/Wheel")]
    public ParticleSystemRenderer wheelSmoke = null;
    public GameObject wheelBlock = null;
    public Animator wheelAnim = null;
    private int wheelLevel = 0;
    private float wheelDealy = 0f;

    [Header("Interaction/VendingMachine")]
    public Animator vendingAnim = null;
    private float vendingLiftingLevel = 0f;
    private float vendingFallDealy = 0.05f;
    private float vendingFallPower = 0.03f;
    private float vendingLiftingPower = 0.15f;
    public GameObject vendingDoor = null;
    public BoxCollider vendingCollider = null;
    public BoxCollider vendingLiftingCollider = null;



    private void Start()
	{
        mapType = Random.Range(0, 3);

        if(mapType == 0)
        {
            elementsToChange[1].SetActive(false);
            elementsToChange[2].SetActive(false);

        }
        else if(mapType == 1)
        {
            elementsToChange[0].SetActive(false);
            elementsToChange[2].SetActive(false);
        }
        else
        {
            elementsToChange[0].SetActive(false);
            elementsToChange[1].SetActive(false);
            RenderSettings.skybox = space;
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
            if(hit.collider.tag == "Barrel")
            {
                uiCollectText.text = "Wciśnij E aby zebrać";
                uiCollect.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    barrelOrder.Add(hit.collider.GetComponent<Barrel>().index);
                    ++barrelAmount;
                    hit.collider.gameObject.SetActive(false);
                    uiCollect.SetActive(false);
                    audioSound.PlayOneShot(collectBarrel);
                }
            }
            else if(hit.collider.tag == "Ship")
            {
                if(barrelAmount > 2)
                {
                    uiCollect.SetActive(true);
                    uiCollectText.text = "Wciśnij E aby odlecieć";

                    if(Input.GetKeyDown(KeyCode.E))
                    {
                        fpc.playerCanMove = false;
                        fpc.cameraCanMove = false;
                        fpc.enableHeadBob = false;
                        fpc.enabled = false;

                        string result = "";
                        for(int i = 0; i < barrelOrder.Count; ++i)
                        {
                            result += barrelOrder[i] + " ";
                        }

                        string resultRoom = "";
                        for(int i = 0; i < roomOrder.Count; ++i)
                        {
                            resultRoom += roomOrder[i] + " ";
                        }

                        timeFromStart = Time.timeSinceLevelLoad;

                        if(timeFromStart % 60 >= 10)
                        {
                            uiCollectText.text = $"Zapisz dane poniżej będą potrzebne do wypełnienia ankiety \n\n Mapa: {mapType + 1} \n\n Czas przejścia: {(int)(timeFromStart / 60)}:{(int)(timeFromStart % 60)}\n\n Zebrane beczki: {result} \n\n Kolejność odwiedzania: {resultRoom} \n\nKliknij Esc aby wyjść";
                        }
                        else
                        {
                            uiCollectText.text = $"Zapisz dane poniżej będą potrzebne do wypełnienia ankiety \n\n Mapa: {mapType + 1} \n\n Czas przejścia: {(int)(timeFromStart / 60)}:0{(int)(timeFromStart % 60)}\n\n Zebrane beczki: {result} \n\n Kolejność odwiedzania: {resultRoom} \n\nKliknij Esc aby wyjść";

                        }
                        uiCollect.SetActive(true);

                        audioSpeak.PlayOneShot(endSpeak);

                        fly = true;
                        return;
                    }
                }
                else
                    uiCollectText.text = "";
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
            else if(hit.collider.tag == "HandPadPuzzle")
            {
                uiCollectText.text = "Wciśnij E aby użyć";
                uiCollect.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.GetComponent<HandPadPuzzle>().Use();
                    audioSound.PlayOneShot(beepAudio);
                }
            }
            else if(hit.collider.tag == "Wheel" && wheelDealy <= 0f && wheelLevel < 3)
            {
                uiCollectText.text = "Wciśnij E aby przekręcić";
                uiCollect.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    wheelDealy = 0.7f;
                    ++wheelLevel;
                    wheelAnim.SetInteger("WheelUse", wheelLevel);
                    if(wheelLevel == 1)
                        wheelSmoke.maxParticleSize = 0.25f;
                    else if(wheelLevel == 2)
                        wheelSmoke.maxParticleSize = 0.1f;
                    else if(wheelLevel >= 3)
                    {
                        wheelSmoke.maxParticleSize = 0f;
                        wheelBlock.SetActive(false);
                    }

                    audioSound.PlayOneShot(creakingAudio);
                }
            }
            else if(hit.collider.tag == "VendingMachine" && vendingLiftingLevel < 0.98f)
            {
                uiCollectText.text = "Wciskaj E aby unieść";
                uiCollect.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    vendingLiftingLevel += vendingLiftingPower;
                    if(vendingLiftingLevel > 1f)
                        vendingLiftingLevel = 1f;

                    if(vendingLiftingLevel > 0.98f)
					{
                        vendingDoor.tag = "Door";
                        vendingCollider.enabled = true;
                        vendingLiftingCollider.enabled = false;
                    }

                    vendingAnim.SetFloat("Blend", vendingLiftingLevel);
                    audioSound.PlayOneShot(woodAudio[Random.Range(0, woodAudio.Length)]);
                }
            }
            else
            {
                uiCollect.SetActive(false);
                uiCollectText.text = "";
            }

        }
        else uiCollect.SetActive(false);

        if(wheelDealy > 0f)
            wheelDealy -= Time.deltaTime;

        if(vendingFallDealy <= 0f && vendingLiftingLevel < 0.98f)
		{
            vendingLiftingLevel -= vendingFallPower;
            if(vendingLiftingLevel < 0f)
                vendingLiftingLevel = 0f;

            vendingFallDealy = 0.05f;

            vendingAnim.SetFloat("Blend", vendingLiftingLevel);
        }
        if(vendingFallDealy > 0f)
            vendingFallDealy -= Time.deltaTime;

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
