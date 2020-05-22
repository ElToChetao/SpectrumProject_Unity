// Copyright (C) 2015-2020 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpecialButton : Button
{
    private bool pointerWasUp;

    private AudioSource audioSource;
    private ButtonSounds buttonSounds;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        buttonSounds = GetComponent<ButtonSounds>();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource != null)
        {
            audioSource.clip = buttonSounds.pressedSound;
            audioSource.Play();
        }

        base.OnPointerClick(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        pointerWasUp = true;
        base.OnPointerUp(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (pointerWasUp)
        {
            pointerWasUp = false;
            base.OnPointerEnter(eventData);
        }
        else
        {
            if (audioSource != null)
            {
                audioSource.clip = buttonSounds.rolloverSound;
                audioSource.Play();
            }

            base.OnPointerEnter(eventData);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        pointerWasUp = false;
        base.OnPointerExit(eventData);
    }
}
