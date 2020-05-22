// Copyright (C) 2015-2020 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamevanilla
{
    // This class is responsible for managing the equipment popup.
    public class EquipmentPopup : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI text;

        public void SetInfo(Sprite iconSprite, string iconText)
        {
            image.sprite = iconSprite;
            text.text = iconText;
        }
    }
}
