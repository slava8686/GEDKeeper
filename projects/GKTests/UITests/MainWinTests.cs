﻿/*
 *  "GEDKeeper", the personal genealogical database editor.
 *  Copyright (C) 2009-2016 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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

#if !__MonoCS__

using System;
using System.Windows.Forms;
using GKCommon.GEDCOM;
using GKCore;
using GKCore.Interfaces;
using GKCore.Options;
using GKTests.Service;
using GKUI;
using GKUI.Charts;
using GKUI.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace GKTests.UITests
{
    /// <summary>
    /// Tests for the main application window. Dependent calls of other windows
    /// and dialogs that are heavily dependent on the main window.
    /// </summary>
    [TestFixture]
    public class MainWinTests : CustomWindowTest
    {
        private MainWin fMainWin;
        private IBaseWindow fCurBase;
        private int fIndex;

        public override void Setup()
        {
            base.Setup();
            fMainWin = new MainWin();
            fMainWin.Show();
        }

        [STAThread]
        [Test]
        public void Test_Common()
        {
            // Stage 1: call to AboutDlg, closing in AboutDlg_Handler
            Wait();
            ExpectModal("AboutDlg", "AboutDlg_Handler");
            ClickToolStripMenuItem("miAbout", fMainWin);


            // Stage 2.1: GetCurrentFile()
            Assert.IsNull(fMainWin.GetCurrentFile(), "Stage 2.1");

            // Stage 2.2: create an empty base
            Wait();
            ClickToolStripButton("tbFileNew", fMainWin);

            // Stage 2.3: GetCurrentFile()
            fCurBase = fMainWin.GetCurrentFile();
            Assert.IsNotNull(fCurBase, "Stage 2.3");

            // Stage 2.4: fill context for sample data
            TestStubs.FillContext(fCurBase.Context);
            fCurBase.UpdateView();

            // Stage 2.5: select first individual record in base
            Wait();
            fCurBase.SelectRecordByXRef("I1");
            Assert.AreEqual("I1", ((BaseWin) fCurBase).GetSelectedPerson().XRef);

            // Stage 3: call to FilePropertiesDlg
            Wait();
            ModalFormHandler = FilePropertiesDlg_btnCancel_Handler; // FilePropertiesDlg.Cancel
            ClickToolStripMenuItem("miFileProperties", fMainWin);
            Wait();
            ModalFormHandler = FilePropertiesDlg_btnAccept_Handler; // FilePropertiesDlg.Accept
            ClickToolStripMenuItem("miFileProperties", fMainWin);


            // Stage 4: call to OptionsDlg
            Wait();
            ModalFormHandler = OptionsDlg_btnCancel_Handler; // OptionsDlg.Cancel
            ClickToolStripMenuItem("miOptions", fMainWin);
            Wait();
            ModalFormHandler = OptionsDlg_btnAccept_Handler; // OptionsDlg.Accept
            ClickToolStripMenuItem("miOptions", fMainWin);


            // Stage 5: calls to the different Editors
            Wait();
            for (GEDCOMRecordType rt = GEDCOMRecordType.rtIndividual; rt <= GEDCOMRecordType.rtLocation; rt++) {
                fCurBase.ShowRecordsTab(rt);

                Wait();
                ModalFormHandler = EditorDlg_btnCancel_Handler;
                ClickToolStripButton("tbRecordAdd", fMainWin);

                Wait();
                ModalFormHandler = EditorDlg_btnAccept_Handler;
                ClickToolStripButton("tbRecordAdd", fMainWin);

                Wait();
                ModalFormHandler = EditorDlg_btnCancel_Handler;
                ClickToolStripButton("tbRecordEdit", fMainWin);

                Wait();
                ModalFormHandler = EditorDlg_btnAccept_Handler;
                ClickToolStripButton("tbRecordEdit", fMainWin);
            }


            // Stage 22: call to QuickFind
            Wait();
            ClickToolStripMenuItem("miSearch", fMainWin);


            // Stage 23: call to PersonsFilterDlg
            Wait();
            ModalFormHandler = PersonsFilterDlg_btnCancel_Handler; // PersonsFilterDlg.Cancel
            ClickToolStripMenuItem("miFilter", fMainWin);
            Wait();
            ModalFormHandler = PersonsFilterDlg_btnAccept_Handler; // PersonsFilterDlg.Accept
            ClickToolStripMenuItem("miFilter", fMainWin);


            // Stage 24: call to TreeToolsWin
            Wait();
            ModalFormHandler = TreeToolsWin_Handler;
            ClickToolStripMenuItem("miTreeTools", fMainWin);


            // Stage 25: call to CircleChartWin (required the base, selected person)
            Wait();
            ClickToolStripMenuItem("miAncestorsCircle", fMainWin);
            Assert.IsTrue(fMainWin.ActiveMdiChild is CircleChartWin, "Stage 25");
            CircleChartWin_Tests(fMainWin.ActiveMdiChild as CircleChartWin);

            // Stage 26: call to CircleChartWin (required the base, selected person)
            Wait();
            ClickToolStripMenuItem("miDescendantsCircle", fMainWin);
            Assert.IsTrue(fMainWin.ActiveMdiChild is CircleChartWin, "Stage 26");
            CircleChartWin_Tests(fMainWin.ActiveMdiChild as CircleChartWin);


            // Stage 27: call to TreeChartWin (required the base, selected person)
            Wait();
            ClickToolStripMenuItem("miTreeAncestors", fMainWin);
            Assert.IsTrue(fMainWin.ActiveMdiChild is TreeChartWin, "Stage 27");
            TreeChartWin_Tests(fMainWin.ActiveMdiChild as TreeChartWin, TreeChartBox.ChartKind.ckAncestors);


            // Stage 28: call to TreeChartWin (required the base, selected person)
            Wait();
            ClickToolStripMenuItem("miTreeDescendants", fMainWin);
            Assert.IsTrue(fMainWin.ActiveMdiChild is TreeChartWin, "Stage 28");
            TreeChartWin_Tests(fMainWin.ActiveMdiChild as TreeChartWin, TreeChartBox.ChartKind.ckDescendants);


            // Stage 29: call to TreeChartWin (required the base, selected person)
            Wait();
            ClickToolStripMenuItem("miTreeBoth", fMainWin);
            Assert.IsTrue(fMainWin.ActiveMdiChild is TreeChartWin, "Stage 29");
            TreeChartWin_Tests(fMainWin.ActiveMdiChild as TreeChartWin, TreeChartBox.ChartKind.ckBoth);


            // Stage 30: call to StatsWin (required the base)
            Wait();
            ClickToolStripButton("tbStats", fMainWin);
            Assert.IsTrue(fMainWin.ActiveMdiChild is StatisticsWin, "Stage 30");
            StatsWin_Tests(fMainWin.ActiveMdiChild as StatisticsWin);
            fMainWin.ActiveMdiChild.Close();


            // Stage 31: call to SlideshowWin (required the base)
            Wait();
            ClickToolStripMenuItem("miSlideshow", fMainWin);
            Assert.IsTrue(fMainWin.ActiveMdiChild is SlideshowWin, "Stage 31");
            fMainWin.ActiveMdiChild.Close();


            // Stage 32: call to ScriptEditWin (required the base)
            Wait();
            ModalFormHandler = ScriptEditWin_Handler;
            ClickToolStripMenuItem("miScripts", fMainWin);
            //Assert.IsTrue(fMainWin.ActiveMdiChild is SlideshowWin, "Stage 32");


            // Stage 33: call to OrganizerWin
            Wait();
            ModalFormHandler = OrganizerWin_Handler;
            ClickToolStripMenuItem("miOrganizer", fMainWin);


            // Stage 34: call to RelationshipCalculatorDlg
            Wait();
            ModalFormHandler = RelationshipCalculatorDlg_Handler;
            ClickToolStripMenuItem("miRelationshipCalculator", fMainWin);


            // Stage 35: call to MapsViewerWin (required the base)
            Wait();
            ClickToolStripMenuItem("miMap", fMainWin);
            Assert.IsTrue(fMainWin.ActiveMdiChild is MapsViewerWin, "Stage 35");
            fMainWin.ActiveMdiChild.Close();


            // Stage 50: close Base
            Wait();
            ClickToolStripMenuItem("miFileClose", fMainWin);


            // Stage 51: call to LanguageSelectDlg
            Wait();
            ModalFormHandler = LanguageSelectDlg_Handler;
            fMainWin.LoadLanguage(0);


            // Stage 52: exit
            Wait();
            ClickToolStripMenuItem("miExit", fMainWin);
        }

        #region AboutDlg handler

        public void AboutDlg_Handler()
        {
            ClickButton("btnClose", "AboutDlg");
        }

        #endregion

        #region FilePropertiesDlg handlers

        public void FilePropertiesDlg_btnCancel_Handler(string name, IntPtr ptr, Form form)
        {
            ClickButton("btnCancel", form);
        }

        public void FilePropertiesDlg_btnAccept_Handler(string name, IntPtr ptr, Form form)
        {
            Assert.AreEqual(fCurBase, ((IBaseEditor)form).Base);

            var txtName = new TextBoxTester("txtName");
            txtName.Enter("sample text");

            ClickButton("btnAccept", form);

            GEDCOMSubmitterRecord submitter = fCurBase.Context.Tree.Header.Submitter.Value as GEDCOMSubmitterRecord;
            Assert.AreEqual("sample text", submitter.Name.StringValue);
        }

        #endregion

        #region OptionsDlg handlers

        public void OptionsDlg_btnCancel_Handler(string name, IntPtr ptr, Form form)
        {
            ClickButton("btnCancel", form);
        }

        public void OptionsDlg_btnAccept_Handler(string name, IntPtr ptr, Form form)
        {
            Assert.AreEqual(GlobalOptions.Instance, ((OptionsDlg)form).Options);

            /*var txtName = new TextBoxTester("txtName");
            txtName.Enter("sample text");
            Assert.AreEqual("sample text", txtName.Text);*/

            ClickButton("btnAccept", form);

            //GEDCOMSubmitterRecord submitter = fCurWin.Context.Tree.Header.Submitter.Value as GEDCOMSubmitterRecord;
            //Assert.AreEqual("sample text", submitter.Name.StringValue);
        }

        #endregion

        #region PersonsFilterDlg handlers

        public void PersonsFilterDlg_btnCancel_Handler(string name, IntPtr ptr, Form form)
        {
            ClickButton("btnCancel", form);
        }

        public void PersonsFilterDlg_btnAccept_Handler(string name, IntPtr ptr, Form form)
        {
            Assert.AreEqual(fCurBase, ((CommonFilterDlg)form).Base);

            /*var txtName = new TextBoxTester("txtName");
            txtName.Enter("sample text");
            Assert.AreEqual("sample text", txtName.Text);*/

            ClickButton("btnAccept", form);

            //GEDCOMSubmitterRecord submitter = fCurWin.Context.Tree.Header.Submitter.Value as GEDCOMSubmitterRecord;
            //Assert.AreEqual("sample text", submitter.Name.StringValue);
        }

        #endregion

        #region ScriptEditWin handlers

        public void ScriptEditWin_Handler(string name, IntPtr ptr, Form form)
        {
            form.Close();
        }

        #endregion

        #region TreeToolsWin handlers

        public void TreeToolsWin_Handler(string name, IntPtr ptr, Form form)
        {
            var tabs = new TabControlTester("tabsTools", form);

            for (int i = 0; i < tabs.Properties.TabCount; i++) {
                Wait();
                tabs.SelectTab(i);
            }

            form.Close();
        }

        #endregion

        #region OrganizerWin handlers

        public void OrganizerWin_Handler(string name, IntPtr ptr, Form form)
        {
            form.Close();
        }

        #endregion

        #region RelationshipCalculatorDlg handlers

        public void RelationshipCalculatorDlg_Handler(string name, IntPtr ptr, Form form)
        {
            Assert.IsTrue(fCurBase.Context.Tree.RecordsCount > 1);
            GEDCOMIndividualRecord iRec1 = fCurBase.Context.Tree.XRefIndex_Find("I1") as GEDCOMIndividualRecord;
            Assert.IsNotNull(iRec1);
            GEDCOMIndividualRecord iRec2 = fCurBase.Context.Tree.XRefIndex_Find("I2") as GEDCOMIndividualRecord;
            Assert.IsNotNull(iRec2);

            fIndex = 0;
            ModalFormHandler = RCD_RecordSelectDlg_Select_Handler; // required
            ClickButton("btnRec1Select", form);
            fIndex = 1;
            ModalFormHandler = RCD_RecordSelectDlg_Select_Handler; // required
            ClickButton("btnRec2Select", form);

            var txtResult = new TextBoxTester("txtResult", form);
            Assert.AreEqual("Ivanova Maria Petrovna is wife of Ivanov Ivanа Ivanovichа", txtResult.Text); // :D

            ClickButton("btnClose", form);
        }

        private void RCD_RecordSelectDlg_Select_Handler(string name, IntPtr ptr, Form form)
        {
            var listRecords = new GKRecordsViewTester("fListRecords", form);
            listRecords.Properties.SelectItem(fIndex);
            ClickButton("btnSelect", form);
        }

        #endregion

        #region LanguageSelectDlg handlers

        public void LanguageSelectDlg_Handler(string name, IntPtr ptr, Form form)
        {
            ClickButton("btnCancel", form);
        }

        #endregion

        #region EditorDlg handlers

        private static bool FamilyEditDlg_FirstCall = true;
        private static bool GroupEditDlg_FirstCall = true;
        private static bool PersonEditDlg_FirstCall = true;
        private static bool ResearchEditDlg_FirstCall = true;

        public void EditorDlg_btnCancel_Handler(string name, IntPtr ptr, Form form)
        {
            ClickButton("btnCancel", form);
        }

        public void EditorDlg_btnAccept_Handler(string name, IntPtr ptr, Form form)
        {
            //Assert.AreEqual(fCurWin, ((IBaseEditor)form).Base);

            if (FamilyEditDlg_FirstCall && form is FamilyEditDlg) {
                FamilyEditDlg_Handler(form as FamilyEditDlg);
                FamilyEditDlg_FirstCall = false;
            }

            if (GroupEditDlg_FirstCall && form is GroupEditDlg) {
                GroupEditDlg_Handler(form as GroupEditDlg);
                GroupEditDlg_FirstCall = false;
            }

            if (PersonEditDlg_FirstCall && form is PersonEditDlg) {
                PersonEditDlg_Handler(form as PersonEditDlg);
                PersonEditDlg_FirstCall = false;
            }

            if (ResearchEditDlg_FirstCall && form is ResearchEditDlg) {
                ResearchEditDlg_Handler(form as ResearchEditDlg);
                ResearchEditDlg_FirstCall = false;
            }

            ClickButton("btnAccept", form);
        }

        private void FamilyEditDlg_Handler(FamilyEditDlg dlg)
        {
            GEDCOMFamilyRecord familyRecord = dlg.Family;
            var tabs = new TabControlTester("tabsFamilyData", dlg);
            GKSheetListTester sheetTester;

            // events
            Assert.AreEqual(0, familyRecord.Events.Count);
            Wait();
            tabs.SelectTab(1);
            ModalFormHandler = EventEditDlg_Select_Handler;
            ClickToolStripButton("fEventsList_ToolBar_btnAdd", dlg);
            Assert.AreEqual(1, familyRecord.Events.Count);

            sheetTester = new GKSheetListTester("fEventsList", dlg);
            sheetTester.Properties.SelectItem(0);
            ModalFormHandler = EventEditDlg_Select_Handler;
            ClickToolStripButton("fEventsList_ToolBar_btnEdit", dlg);
            Assert.AreEqual(1, familyRecord.Events.Count);

            ModalFormHandler = MessageBox_YesHandler;
            sheetTester.Properties.SelectItem(0);
            ClickToolStripButton("fEventsList_ToolBar_btnDelete", dlg);
            Assert.AreEqual(0, familyRecord.Events.Count);

            // notes
            Assert.AreEqual(0, familyRecord.Notes.Count);
            Wait();
            tabs.SelectTab(2);
            ModalFormHandler = RecordSelectDlg_Select_Handler;
            ClickToolStripButton("fNotesList_ToolBar_btnAdd", dlg);
            Assert.AreEqual(1, familyRecord.Notes.Count);

            sheetTester = new GKSheetListTester("fNotesList");
            sheetTester.Properties.SelectItem(0);
            ClickToolStripButton("fNotesList_ToolBar_btnEdit", dlg);
            Assert.AreEqual(1, familyRecord.Notes.Count);

            ModalFormHandler = MessageBox_YesHandler;
            sheetTester.Properties.SelectItem(0);
            ClickToolStripButton("fNotesList_ToolBar_btnDelete", dlg);
            Assert.AreEqual(0, familyRecord.Notes.Count);

            // media
            Assert.AreEqual(0, familyRecord.MultimediaLinks.Count);
            Wait();
            tabs.SelectTab(3);
            ModalFormHandler = RecordSelectDlg_Select_Handler;
            ClickToolStripButton("fMediaList_ToolBar_btnAdd", dlg);
            Assert.AreEqual(1, familyRecord.MultimediaLinks.Count);

            sheetTester = new GKSheetListTester("fMediaList");
            sheetTester.Properties.SelectItem(0);
            ClickToolStripButton("fMediaList_ToolBar_btnEdit", dlg);
            Assert.AreEqual(1, familyRecord.MultimediaLinks.Count);

            ModalFormHandler = MessageBox_YesHandler;
            sheetTester.Properties.SelectItem(0);
            ClickToolStripButton("fMediaList_ToolBar_btnDelete", dlg);
            Assert.AreEqual(0, familyRecord.MultimediaLinks.Count);

            // sources
            Assert.AreEqual(0, familyRecord.SourceCitations.Count);
            Wait();
            tabs.SelectTab(4);
            ModalFormHandler = SourceCitEditDlg_Handler;
            ClickToolStripButton("fSourcesList_ToolBar_btnAdd", dlg);
            Assert.AreEqual(1, familyRecord.SourceCitations.Count);

            sheetTester = new GKSheetListTester("fSourcesList");
            sheetTester.Properties.SelectItem(0);
            ClickToolStripButton("fSourcesList_ToolBar_btnEdit", dlg);
            Assert.AreEqual(1, familyRecord.SourceCitations.Count);

            ModalFormHandler = MessageBox_YesHandler;
            sheetTester.Properties.SelectItem(0);
            ClickToolStripButton("fSourcesList_ToolBar_btnDelete", dlg);
            Assert.AreEqual(0, familyRecord.SourceCitations.Count);
        }

        private void PersonEditDlg_Handler(PersonEditDlg dlg)
        {
            GEDCOMIndividualRecord indiRecord = dlg.Person;
            GKSheetListTester sheetTester = new GKSheetListTester("fGroupsList", dlg);

            // groups
            Assert.AreEqual(0, indiRecord.Groups.Count);
            Wait();
            RSD_SubHandler = GroupAdd_Mini_Handler;
            ModalFormHandler = RecordSelectDlg_Create_Handler;
            ClickToolStripButton("fGroupsList_ToolBar_btnAdd", dlg);
            Assert.AreEqual(1, indiRecord.Groups.Count);
            Assert.AreEqual("sample group", ((GEDCOMGroupRecord)indiRecord.Groups[0].Value).GroupName);

            //ModalFormHandler = MessageBox_YesHandler;
            //sheetTester.Properties.SelectItem(0);
            //ClickToolStripButton("fGroupsList_ToolBar_btnDelete", dlg);
            //Assert.AreEqual(0, indiRecord.Groups.Count);
        }

        private void ResearchEditDlg_Handler(ResearchEditDlg dlg)
        {
            GEDCOMResearchRecord resRecord = dlg.Research;
            GKSheetListTester sheetTester = new GKSheetListTester("fGroupsList", dlg);

            // groups
            Assert.AreEqual(0, resRecord.Groups.Count);
            Wait();
            RSD_SubHandler = GroupAdd_Mini_Handler;
            ModalFormHandler = RecordSelectDlg_Create_Handler;
            ClickToolStripButton("fGroupsList_ToolBar_btnAdd", dlg);
            Assert.AreEqual(1, resRecord.Groups.Count);
            Assert.AreEqual("sample group", ((GEDCOMGroupRecord)resRecord.Groups[0].Value).GroupName);

            //ModalFormHandler = MessageBox_YesHandler;
            //sheetTester.Properties.SelectItem(0);
            //ClickToolStripButton("fGroupsList_ToolBar_btnDelete", dlg);
            //Assert.AreEqual(0, resRecord.Groups.Count);
        }

        private void GroupEditDlg_Handler(GroupEditDlg dlg)
        {
            GEDCOMGroupRecord groupRecord = dlg.Group;
            GKSheetListTester sheetTester = new GKSheetListTester("fMembersList", dlg);

            // members
            Assert.AreEqual(0, groupRecord.Members.Count);
            Wait();
            ModalFormHandler = RecordSelectDlg_Select_Handler;
            ClickToolStripButton("fMembersList_ToolBar_btnAdd", dlg);
            Assert.AreEqual(1, groupRecord.Members.Count);

            ModalFormHandler = MessageBox_YesHandler;
            sheetTester.Properties.SelectItem(0);
            ClickToolStripButton("fMembersList_ToolBar_btnDelete", dlg);
            Assert.AreEqual(0, groupRecord.Members.Count);
        }

        private void MessageBox_YesHandler(string name, IntPtr ptr, Form form)
        {
            MessageBoxTester messageBox = new MessageBoxTester(ptr);
            messageBox.SendCommand(MessageBoxTester.Command.Yes);
        }

        private void EventEditDlg_Select_Handler(string name, IntPtr ptr, Form form)
        {
            EventEditDlg eventDlg = (EventEditDlg) form;
            Assert.AreEqual(fCurBase, eventDlg.Base);
            Assert.IsNotNull(eventDlg.Event);

            var cmbEventType = new ComboBoxTester("cmbEventType", form);
            cmbEventType.Select(1); // Birth

            var txtEventPlace = new TextBoxTester("txtEventPlace", form);
            txtEventPlace.Enter("test place");

            var cmbEventDateType = new ComboBoxTester("cmbEventDateType", form);
            cmbEventDateType.Select(3); // Between

            var txtEventDate1 = new MaskedTextBoxTester("txtEventDate1", form);
            txtEventDate1.Enter("01.01.1900");

            var txtEventDate2 = new MaskedTextBoxTester("txtEventDate2", form);
            txtEventDate2.Enter("10.01.1900");

            var cmbDate1Calendar = new ComboBoxTester("cmbDate1Calendar", form);
            cmbDate1Calendar.Select(1); // Julian

            var cmbDate2Calendar = new ComboBoxTester("cmbDate2Calendar", form);
            cmbDate2Calendar.Select(1); // Julian

            var txtEventCause = new TextBoxTester("txtEventCause", form);
            txtEventCause.Enter("test cause");

            var txtEventOrg = new TextBoxTester("txtEventOrg", form);
            txtEventOrg.Enter("test agency");

            ModalFormHandler = AddressEditDlg_btnCancel_Handler;
            ClickButton("btnAddress", form);

            var tsBtn = new ButtonTester("btnAccept", form);
            tsBtn.FireEvent("Click");

            // this don't working here
            //Assert.AreEqual("BIRT", eventDlg.Event.Name);
            //Assert.AreEqual("test place", eventDlg.Event.Detail.Place.StringValue);
            //Assert.IsInstanceOf(typeof(GEDCOMDateExact), eventDlg.Event.Detail.Date.Value);
            //Assert.AreEqual("01 JAN 1900", eventDlg.Event.Detail.Date.Value.StringValue);
            //Assert.AreEqual("test cause", eventDlg.Event.Detail.Cause);
            //Assert.AreEqual("test agency", eventDlg.Event.Detail.Agency);
        }

        public void AddressEditDlg_btnCancel_Handler(string name, IntPtr ptr, Form form)
        {
            ClickButton("btnCancel", form);
        }

        #region RecordSelectDlg handlers

        public void GroupAdd_Mini_Handler(string name, IntPtr ptr, Form form)
        {
            var edName = new TextBoxTester("edName", form);
            edName.Enter("sample group");

            var tsBtn = new ButtonTester("btnAccept", form);
            tsBtn.FireEvent("Click");
        }

        private ModalFormHandler RSD_SubHandler;

        public void RecordSelectDlg_Create_Handler(string name, IntPtr ptr, Form form)
        {
            ModalFormHandler = RSD_SubHandler;
            var tsBtn = new ButtonTester("btnCreate", form);
            tsBtn.FireEvent("Click");
        }

        public void RecordSelectDlg_Select_Handler(string name, IntPtr ptr, Form form)
        {
            var listRecords = new GKRecordsViewTester("fListRecords", form);
            listRecords.Properties.SelectItem(0);

            var tsBtn = new ButtonTester("btnSelect", form);
            tsBtn.FireEvent("Click");
        }

        #endregion

        public void SourceCitEditDlg_Handler(string name, IntPtr ptr, Form form)
        {
            var cmbSource = new ComboBoxTester("cmbSource", form);
            cmbSource.Select(0);

            var tsBtn = new ButtonTester("btnAccept", form);
            tsBtn.FireEvent("Click");
        }

        #endregion

        private void CircleChartWin_Tests(CircleChartWin frm)
        {
            Assert.AreEqual(fCurBase, frm.Base);
            //Assert.AreEqual(kind, frm.ChartKind);
            frm.UpdateView();
            frm.Close();
        }

        private void TreeChartWin_Tests(TreeChartWin frm, TreeChartBox.ChartKind kind)
        {
            Assert.AreEqual(fCurBase, frm.Base);
            Assert.AreEqual(kind, frm.ChartKind);
            frm.UpdateView();
            frm.Close();
        }

        private void StatsWin_Tests(StatisticsWin frm)
        {
            var cbType = new ToolStripComboBoxTester("cbType", frm);

            for (int i = 0; i < cbType.Properties.Items.Count; i++) {
                Wait();
                cbType.Select(i);
            }

            //frm.Close();
        }

        #region Useful stuff

        /*[Test]
        public void Test_EnterDataAndApply()
        {
            var txtNote = new TextBoxTester("txtNote");
            txtNote.Enter("sample text");

            var btnAccept = new ButtonTester("btnAccept");
            btnAccept.Click();

            Assert.AreEqual("sample text\r\n", fNoteRecord.Note.Text);
        }*/

        /*[Test]
        public void TestData()
        {
            // CheckBoxTester uncheckBoxTester = new CheckBoxTester("aPanelName.checkBoxName", "MyFormName");
            // RadioButtonTester radioTester = new RadioButtonTester("mainFormControlName.panelName.radioButtonName",  "MyFormName");

            var txtInput = new TextBoxTester("txtInput") {["Text"] = "2+2"};
            var txtOutput = new TextBoxTester("txtOutput");
            Assert.AreEqual("2+2", txtInput.Text);
            
            var btnRes = new ButtonTester("btnRes");
            btnRes.Click();
            Assert.AreEqual("4", txtOutput.Text);

            var okButton = new ButtonTester("btnOK");
            okButton.Click();
            Assert.IsTrue(form.DialogResult == DialogResult.OK);
        }*/
        
        /*public void TestFormNoDataHandler()
        {
            var messageBoxTester = new MessageBoxTester("Message");
            if (messageBoxTester != null)
            {
                messageBoxTester.ClickOk();
            }
        }*/

        #endregion
    }
}

#endif