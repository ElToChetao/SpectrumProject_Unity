// Copyright (C) 2015-2020 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace Gamevanilla
{
    // This class is responsible for opening the equipment popup.
    public class EquipmentPopupOpener : PopupOpener
    {
        public Sprite iconSprite;
        public string iconText;

        public override void OpenPopup()
        {
            base.OpenPopup();
            m_popup.GetComponent<EquipmentPopup>().SetInfo(iconSprite, iconText);
        }
    }
}
