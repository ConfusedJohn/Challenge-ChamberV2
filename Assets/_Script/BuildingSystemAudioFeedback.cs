using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio feedback system for the Placement Manager
/// </summary>
public class BuildingSystemAudioFeedback : MonoBehaviour
{
    [SerializeField] 
    private AudioSource musicSource,SFXSource;

    [SerializeField]
    public AudioClip undoClip, placeConstructionObjectClip, placeFurnitureClip, removeObjectClip, rotateClip,backGround, failedTrap,dogBark,start,victory,loss;

    public void PlayUndoSound() => PlaySFX(undoClip);

    public void PlayPlaceFurniture() => PlaySFX(placeFurnitureClip);
    public void PlayPlaceConstruction() => PlaySFX(placeConstructionObjectClip);
    public void PlayRemove() => PlaySFX(removeObjectClip);
    public void PlayRotate() => PlaySFX(rotateClip);
    public void FailedTrap() => PlaySFX(failedTrap);

    public void PlaySFX(AudioClip clip)
    {
        if(clip != null)
            SFXSource.PlayOneShot(clip);
    }
    private void Start()
    {
        musicSource.clip = backGround;
        musicSource.Play();
    }
}

