using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour


{

    [SerializeField]
    GameObject start;
    [SerializeField]
    GameObject end;

    [SerializeField]
    PlacementManager placementManager;

    [SerializeField]
    GameObject endCanvas;
    [SerializeField]
    Image endImage;

    private bool gameStarted = false;

    private HitBoxManager endHitBoxManager;

    private float nextActionTime = 0.0f;
    private float period = 2.0f;


    private BuildingSystemAudioFeedback buildingSystemAudioFeedback;

    private void Start()
    {
        buildingSystemAudioFeedback = GameObject.FindGameObjectWithTag("Audio").GetComponent<BuildingSystemAudioFeedback>();
        Application.targetFrameRate=60;
        endHitBoxManager = end.GetComponent<HitBoxManager>();
    }


    public void triggerStart()
    {
        placementManager.CancelState();
        start.SetActive(true);
        start.GetComponent<Animator>().Play("activation");
        gameStarted = true;
        nextActionTime = Time.time + period;
        buildingSystemAudioFeedback.PlaySFX(buildingSystemAudioFeedback.start);

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Preview"))
        {
            go.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void ResetGame()
    {
        start.SetActive(true);
        start.GetComponent<Animator>().Play("inactive");
        end.GetComponent<Animator>().Play("inactive");
        endHitBoxManager.active=false;
        endHitBoxManager.istriggered = false;
        endHitBoxManager.incorrectHit = false;
        endHitBoxManager.danger = false;
        gameStarted = false;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GetTriggered"))
        {
            go.GetComponent<Animator>().Play("inactive");
            //maybe set color to original
            go.GetComponent<HitBoxManager>().active =false;
            go.GetComponent<HitBoxManager>().istriggered = false;
            go.GetComponent<HitBoxManager>().incorrectHit = false;
            go.GetComponent<HitBoxManager>().danger = false;
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Trigger"))
        {

            if (go.GetComponent<Animator>() != null)
            {
                go.GetComponent<Animator>().Play("inactive");
            }
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GetTriggeredDanger"))
        {
            go.GetComponent<HitBoxManager>().danger = false;
        }
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("GetTriggeredWall"))
        {
            go.GetComponent<MeshRenderer>().enabled = true;
            go.GetComponent<HitBoxManager>().danger = false;
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("movingObject"))
        {
            go.GetComponent<MeshRenderer>().enabled = true;
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Preview"))
        {
            go.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void Update()
    {
        if (gameStarted) 
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime = Time.time + period;
                checkIfDanger();
                StartCoroutine(CheckIfWin());
                checkIfActive();               
            }
        }
        
    }
    private IEnumerator CheckIfWin()
    {
        yield return new WaitForSeconds(1);

        if (endHitBoxManager.istriggered && !endHitBoxManager.incorrectHit && gameStarted)
        {
            //Debug.Log("win");
            end.GetComponent<Animator>().Play("activation");
            gameStarted = false;
            //trigger end scene, UI element edit chamber,replay the trap, start over, try other levels
            endImage.sprite = Resources.Load<Sprite>("Win");
            endCanvas.SetActive(true);
            buildingSystemAudioFeedback.PlaySFX(buildingSystemAudioFeedback.victory);
        }

    }
    private void checkIfActive()
    {

        List<bool> trapsActivation = new List<bool>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GetTriggered"))
        {
            trapsActivation.Add(go.GetComponent<HitBoxManager>().active);
        }

        if (!trapsActivation.Contains(true) && gameStarted)
        {
            gameStarted = false;
            endImage.sprite = Resources.Load<Sprite>("Loss");
            endCanvas.SetActive(true);
            buildingSystemAudioFeedback.PlaySFX(buildingSystemAudioFeedback.loss);
        }
    }
    private void checkIfDanger()
    {

        List<bool> trapsActivation = new List<bool>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GetTriggeredDanger"))
        {
            trapsActivation.Add(go.GetComponent<HitBoxManager>().danger);
        }

        if (trapsActivation.Contains(true) && gameStarted)
        {
            gameStarted = false;
            endImage.sprite = Resources.Load<Sprite>("LossDanger");
            endCanvas.SetActive(true);
        }
    }
}
