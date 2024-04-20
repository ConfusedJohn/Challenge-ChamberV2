using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitBoxManager : MonoBehaviour
{
    
    public bool istriggered = false;

    public bool incorrectHit = false;

    public bool active = false;

    public bool danger = false;

    [SerializeField]
    private GameObject activator;
    [SerializeField]
    private GameObject movingObject;

    private BuildingSystemAudioFeedback buildingSystemAudioFeedback;

    [SerializeField]
    AudioClip activateSFX;

    void Awake()
    {
        buildingSystemAudioFeedback = GameObject.FindGameObjectWithTag("Audio").GetComponent<BuildingSystemAudioFeedback>();
    }

    void OnTriggerEnter(Collider col)
    {

        if (col.gameObject.tag == "Trigger")
        {
            if (gameObject.tag == "GetTriggered" && col.gameObject!=activator)
            {
                //buildingSystemAudioFeedback.PlaySFX(buildingSystemAudioFeedback.placeFurnitureClip);

                active = true;
                //Debug.Log("set active");
                StartCoroutine( TriggerAnimations(col));
            }
        }
        if (col.gameObject.tag == "Trigger")
        {
            if (gameObject.tag == "GetTriggeredDanger" && col.gameObject != activator)
            {
                //col.gameObject.SetActive(false);
                //col.gameObject.transform.position = new Vector3(col.gameObject.transform.position.x, -5, col.gameObject.transform.position.z);
                //Debug.Log("set active");
                //col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                //col.gameObject.SetActive(false);
                movingObject.gameObject.GetComponent<MeshRenderer>().enabled = false;
                StartCoroutine(TriggerAnimationDanger(col));
            }
        }

        if (col.gameObject.tag == "Trigger")
        {
            if (gameObject.tag == "GetTriggeredWall" && col.gameObject != activator)
            {
                //col.gameObject.SetActive(false);
                //col.gameObject.transform.position = new Vector3(col.gameObject.transform.position.x, -5, col.gameObject.transform.position.z);
                //Debug.Log("set active");
                //col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                //col.gameObject.SetActive(false);
                //movingObject.gameObject.GetComponent<MeshRenderer>().enabled = false;
                buildingSystemAudioFeedback.PlaySFX(activateSFX);
                danger = true;
                //StartCoroutine(TriggerWall());
            }
        }



    }
    void OnTriggerExit(Collider col)
    {

        if (col.gameObject.tag == "Trigger"&& col.GetComponent<Animator>() != null)
        {
            if (gameObject.tag == "GetTriggered" && col.gameObject != activator && col.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("activation") && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("activation"))
            {
                incorrectHit = true;
                active = false;
                //col.gameObject.transform.position = new Vector3(col.gameObject.transform.position.x,-5, col.gameObject.transform.position.z);
                //col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                movingObject.gameObject.GetComponent<MeshRenderer>().enabled = false;
                //activator.GetComponent<Animator>().Play("inactive");
                Debug.Log("set inactive1" + name +" "+ col.name) ;
            }
        }
       
    }

    private IEnumerator TriggerAnimations(Collider col)
    {
        if(col.GetComponent<Animator>() != null)
        {
            yield return new WaitForSeconds(col.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        } else 
        {
            yield return new WaitForSeconds(1);
        }
        
        if (!istriggered && !incorrectHit) {
            //buildingSystemAudioFeedback.PlaySFX(buildingSystemAudioFeedback.undoClip);
            //Debug.Log("works ");
            activator.SetActive(true);
            activator.GetComponent<Animator>().Play("activation");
            GetComponent<Animator>().Play("activation");
            Invoke(nameof(setInactive), activator.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length+1);
            buildingSystemAudioFeedback.PlaySFX(activateSFX);
            //col.GetComponent<Animator>().Play("launch activator");
            istriggered = true;
        }
        if (incorrectHit)
        {
            //buildingSystemAudioFeedback.PlaySFX(buildingSystemAudioFeedback.removeObjectClip);
            //Debug.Log("works else");
            buildingSystemAudioFeedback.FailedTrap();
            //GetComponent<Animator>().Play("destroy");
        }
    }
    private IEnumerator TriggerAnimationDanger(Collider col)
    {
        buildingSystemAudioFeedback.PlaySFX(buildingSystemAudioFeedback.dogBark);
        GetComponent<Animator>().Play("activation");
        
        yield return new WaitForSeconds(2);
        danger = true;
    }

    private IEnumerator TriggerWall()
    {
        yield return new WaitForSeconds(2);
        danger = true;
    }

    private void setInactive()
    {
        active = false;
        //Debug.Log("set inactive2");
    }


}
