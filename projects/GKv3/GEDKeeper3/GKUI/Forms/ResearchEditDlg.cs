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
using GKCommon.GEDCOM;
using GKCore;
using GKCore.Interfaces;
using GKCore.Lists;
using GKCore.Types;
using GKCore.UIContracts;
using GKUI.Components;

namespace GKUI.Forms
{
    public partial class ResearchEditDlg : EditorDialog, IResearchEditDlg
    {
        private readonly GKSheetList fTasksList;
        private readonly GKSheetList fCommunicationsList;
        private readonly GKSheetList fGroupsList;
        private readonly GKSheetList fNotesList;

        private GEDCOMResearchRecord fResearch;

        public GEDCOMResearchRecord Research
        {
            get { return fResearch; }
            set { SetResearch(value); }
        }

        private void SetResearch(GEDCOMResearchRecord value)
        {
            fResearch = value;
            try {
                if (fResearch == null) {
                    txtName.Text = "";
                    cmbPriority.SelectedIndex = -1;
                    cmbStatus.SelectedIndex = -1;
                    txtStartDate.Text = "";
                    txtStopDate.Text = "";
                    nudPercent.Value = 0;
                } else {
                    txtName.Text = fResearch.ResearchName;
                    cmbPriority.SelectedIndex = (int)fResearch.Priority;
                    cmbStatus.SelectedIndex = (int)fResearch.Status;
                    txtStartDate.Text = fResearch.StartDate.GetDisplayString(DateFormat.dfDD_MM_YYYY);
                    txtStopDate.Text = fResearch.StopDate.GetDisplayString(DateFormat.dfDD_MM_YYYY);
                    nudPercent.Value = fResearch.Percent;
                }

                fNotesList.ListModel.DataOwner = fResearch;
                fTasksList.ListModel.DataOwner = fResearch;
                fCommunicationsList.ListModel.DataOwner = fResearch;
                fGroupsList.ListModel.DataOwner = fResearch;
            } catch (Exception ex) {
                Logger.LogWrite("ResearchEditDlg.SetResearch(): " + ex.Message);
            }
        }

        public ResearchEditDlg()
        {
            InitializeComponent();

            for (GKResearchPriority rp = GKResearchPriority.rpNone; rp <= GKResearchPriority.rpTop; rp++) {
                cmbPriority.Items.Add(LangMan.LS(GKData.PriorityNames[(int)rp]));
            }

            for (GKResearchStatus rs = GKResearchStatus.rsDefined; rs <= GKResearchStatus.rsWithdrawn; rs++) {
                cmbStatus.Items.Add(LangMan.LS(GKData.StatusNames[(int)rs]));
            }

            fTasksList = new GKSheetList(pageTasks);
            fTasksList.OnModify += ListTasksModify;

            fCommunicationsList = new GKSheetList(pageCommunications);
            fCommunicationsList.OnModify += ListCommunicationsModify;

            fGroupsList = new GKSheetList(pageGroups);
            fGroupsList.OnModify += ListGroupsModify;

            fNotesList = new GKSheetList(pageNotes);

            SetLang();
        }

        public void SetLang()
        {
            btnAccept.Text = LangMan.LS(LSID.LSID_DlgAccept);
            btnCancel.Text = LangMan.LS(LSID.LSID_DlgCancel);
            Title = LangMan.LS(LSID.LSID_WinResearchEdit);
            pageTasks.Text = LangMan.LS(LSID.LSID_RPTasks);
            pageCommunications.Text = LangMan.LS(LSID.LSID_RPCommunications);
            pageGroups.Text = LangMan.LS(LSID.LSID_RPGroups);
            pageNotes.Text = LangMan.LS(LSID.LSID_RPNotes);
            lblName.Text = LangMan.LS(LSID.LSID_Title);
            lblPriority.Text = LangMan.LS(LSID.LSID_Priority);
            lblStatus.Text = LangMan.LS(LSID.LSID_Status);
            lblPercent.Text = LangMan.LS(LSID.LSID_Percent);
            lblStartDate.Text = LangMan.LS(LSID.LSID_StartDate);
            lblStopDate.Text = LangMan.LS(LSID.LSID_StopDate);
        }

        private void ListTasksModify(object sender, ModifyEventArgs eArgs)
        {
            GEDCOMTaskRecord task = eArgs.ItemData as GEDCOMTaskRecord;
            if (eArgs.Action == RecordAction.raJump && task != null) {
                AcceptChanges();
                fBase.SelectRecordByXRef(task.XRef);
                Close();
            }
        }

        private void ListCommunicationsModify(object sender, ModifyEventArgs eArgs)
        {
            GEDCOMCommunicationRecord comm = eArgs.ItemData as GEDCOMCommunicationRecord;
            if (eArgs.Action == RecordAction.raJump && comm != null) {
                AcceptChanges();
                fBase.SelectRecordByXRef(comm.XRef);
                Close();
            }
        }

        private void ListGroupsModify(object sender, ModifyEventArgs eArgs)
        {
            GEDCOMGroupRecord group = eArgs.ItemData as GEDCOMGroupRecord;
            if (eArgs.Action == RecordAction.raJump && group != null) {
                AcceptChanges();
                fBase.SelectRecordByXRef(group.XRef);
                Close();
            }
        }

        private void AcceptChanges()
        {
            fResearch.ResearchName = txtName.Text;
            fResearch.Priority = (GKResearchPriority)cmbPriority.SelectedIndex;
            fResearch.Status = (GKResearchStatus)cmbStatus.SelectedIndex;
            fResearch.StartDate.Assign(GEDCOMDate.CreateByFormattedStr(txtStartDate.Text, true));
            fResearch.StopDate.Assign(GEDCOMDate.CreateByFormattedStr(txtStopDate.Text, true));
            fResearch.Percent = (int)nudPercent.Value;

            CommitChanges();

            fBase.NotifyRecord(fResearch, RecordAction.raEdit);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try {
                AcceptChanges();
                DialogResult = DialogResult.Ok;
            } catch (Exception ex) {
                Logger.LogWrite("ResearchEditDlg.btnAccept_Click(): " + ex.Message);
                DialogResult = DialogResult.None;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try {
                RollbackChanges();
                CancelClickHandler(sender, e);
            } catch (Exception ex) {
                Logger.LogWrite("ResearchEditDlg.btnCancel_Click(): " + ex.Message);
            }
        }

        public override void InitDialog(IBaseWindow baseWin)
        {
            base.InitDialog(baseWin);

            fTasksList.ListModel = new ResTasksSublistModel(fBase, fLocalUndoman);
            fCommunicationsList.ListModel = new ResCommunicationsSublistModel(fBase, fLocalUndoman);
            fGroupsList.ListModel = new ResGroupsSublistModel(fBase, fLocalUndoman);
            fNotesList.ListModel = new NoteLinksListModel(fBase, fLocalUndoman);
        }
    }
}
