﻿/*
 *  "GEDKeeper", the personal genealogical database editor.
 *  Copyright (C) 2009-2017 by Sergey V. Zhdanovskih.
 *
 *  This file is part of "GEDKeeper".
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using Eto.Forms;
using GKCommon;
using GKCore;
using GKCore.Options;
using GKCore.UIContracts;
using GKUI.Components;

namespace GKUI.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LanguageSelectDlg : ModalDialog, ILanguageSelectDlg
    {
        public int SelectedLanguage { get; set; }

        public LanguageSelectDlg()
        {
            InitializeComponent();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                GKComboItem item = lstLanguages.Items[lstLanguages.SelectedIndex] as GKComboItem;
                if (item != null) {
                    SelectedLanguage = (int)item.Tag;
                }

                DialogResult = DialogResult.Ok;
            }
            catch (Exception ex)
            {
                Logger.LogWrite("LanguageSelectDlg.btnAccept_Click(): " + ex.Message);
                DialogResult = DialogResult.None;
            }
        }

        private void LanguageSelectDlg_Load(object sender, EventArgs e)
        {
            lstLanguages.Items.Clear();
            lstLanguages.Items.Add(new GKComboItem(LangMan.LS_DEF_NAME, LangMan.LS_DEF_CODE));
            foreach (LangRecord lngRec in GlobalOptions.Instance.Languages) {
                lstLanguages.Items.Add(new GKComboItem(lngRec.Name, lngRec.Code));
            }
            UIHelper.SelectComboItem(lstLanguages, LangMan.LS_DEF_CODE, true);
        }
    }
}
