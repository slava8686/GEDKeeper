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

using System;
using System.IO;
using System.Windows.Forms;

using GKCommon.GEDCOM;
using GKCore;
using GKCore.Interfaces;
using GKCore.Types;
using GKUI.Sheets;

namespace GKUI.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TfmMediaEdit : Form, IBaseEditor
    {
        private bool fIsNew;
        private GEDCOMMultimediaRecord fMediaRec;

        private readonly IBaseWindow fBase;
        private readonly GKNotesSheet fNotesList;
        private readonly GKSourcesSheet fSourcesList;

        public GEDCOMMultimediaRecord MediaRec
        {
            get { return this.fMediaRec; }
            set { this.SetMediaRec(value); }
        }

        public IBaseWindow Base
        {
            get { return this.fBase; }
        }

        private bool AcceptChanges()
        {
            GEDCOMFileReferenceWithTitle fileRef = this.fMediaRec.FileReferences[0];

            if (this.fIsNew)
            {
                MediaStoreType gst = (MediaStoreType)this.cbStoreType.SelectedIndex;
                if ((gst == MediaStoreType.mstArchive || gst == MediaStoreType.mstStorage) && !this.fBase.Context.CheckBasePath())
                {
                    return false;
                }

                bool result = this.fBase.Context.MediaSave(fileRef, this.edFile.Text, gst);

                if (!result) {
                    return false;
                }
            }

            fileRef.MediaType = (GEDCOMMediaType)this.cbMediaType.SelectedIndex;
            fileRef.Title = this.edName.Text;

            this.ControlsRefresh();
            this.fBase.ChangeRecord(this.fMediaRec);

            return true;
        }

        private void ControlsRefresh()
        {
            GEDCOMFileReferenceWithTitle fileRef = this.fMediaRec.FileReferences[0];

            this.fIsNew = (fileRef.StringValue == "");

            this.edName.Text = fileRef.Title;
            this.cbMediaType.SelectedIndex = (int)fileRef.MediaType;
            this.edFile.Text = fileRef.StringValue;

            if (this.fIsNew) {
                this.StoreTypesRefresh(true, MediaStoreType.mstReference);
            } else {
                string dummy = "";
                MediaStoreType gst = this.fBase.Context.GetStoreType(fileRef, ref dummy);
                this.StoreTypesRefresh((gst == MediaStoreType.mstArchive), gst);
            }

            this.btnFileSelect.Enabled = this.fIsNew;
            this.cbStoreType.Enabled = this.fIsNew;

            this.fNotesList.DataList = this.fMediaRec.Notes.GetEnumerator();
            this.fSourcesList.DataList = this.fMediaRec.SourceCitations.GetEnumerator();
        }

        private void SetMediaRec(GEDCOMMultimediaRecord value)
        {
            this.fMediaRec = value;
            try
            {
                this.ControlsRefresh();
                this.ActiveControl = this.edName;
            }
            catch (Exception ex)
            {
                this.fBase.Host.LogWrite("TfmMediaEdit.SetMediaRec(): " + ex.Message);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                base.DialogResult = this.AcceptChanges() ? DialogResult.OK : DialogResult.None;
            }
            catch (Exception ex)
            {
                this.fBase.Host.LogWrite("TfmMediaEdit.btnAccept_Click(): " + ex.Message);
                base.DialogResult = DialogResult.None;
            }
        }

        private void btnFileSelect_Click(object sender, EventArgs e)
        {
            if (this.OpenDialog1.ShowDialog() == DialogResult.OK)
            {
                string file = this.OpenDialog1.FileName;

                this.edFile.Text = file;
                GEDCOMMultimediaFormat fileFmt = GEDCOMFileReference.RecognizeFormat(file);

                FileInfo info = new FileInfo(file);
                double fileSize = (((double)info.Length / 1024) / 1024); // mb
                bool canArc = (CheckFormatArchived(fileFmt) && fileSize <= 2);

                this.StoreTypesRefresh(canArc, MediaStoreType.mstReference);
                this.cbStoreType.Enabled = true;
            }
        }

        private static bool CheckFormatArchived(GEDCOMMultimediaFormat format)
        {
            switch (format)
            {
                case GEDCOMMultimediaFormat.mfWAV:
                case GEDCOMMultimediaFormat.mfAVI:
                case GEDCOMMultimediaFormat.mfMPG:
                    return false;
                default:
                    return true;
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            this.AcceptChanges();
            this.fBase.ShowMedia(this.fMediaRec, true);
        }

        private void edName_TextChanged(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} \"{1}\"", LangMan.LS(LSID.LSID_RPMultimedia), this.edName.Text);
        }

        private void StoreTypesRefresh(bool allowArc, MediaStoreType select)
        {
            this.cbStoreType.Items.Clear();
            this.cbStoreType.Items.Add(LangMan.LS(GKData.GKStoreTypes[(int)MediaStoreType.mstReference].Name));
            this.cbStoreType.Items.Add(LangMan.LS(GKData.GKStoreTypes[(int)MediaStoreType.mstStorage].Name));
            if (allowArc) {
                this.cbStoreType.Items.Add(LangMan.LS(GKData.GKStoreTypes[(int)MediaStoreType.mstArchive].Name));
            }
            this.cbStoreType.SelectedIndex = (int)select;
        }

        public TfmMediaEdit(IBaseWindow aBase)
        {
            this.InitializeComponent();
            this.fBase = aBase;

            for (GEDCOMMediaType mt = GEDCOMMediaType.mtNone; mt <= GEDCOMMediaType.mtLast; mt++)
            {
                this.cbMediaType.Items.Add(LangMan.LS(GKData.MediaTypes[(int)mt]));
            }

            this.fNotesList = new GKNotesSheet(this, this.SheetNotes);
            this.fSourcesList = new GKSourcesSheet(this, this.SheetSources);

            // SetLang()
            this.btnAccept.Text = LangMan.LS(LSID.LSID_DlgAccept);
            this.btnCancel.Text = LangMan.LS(LSID.LSID_DlgCancel);
            this.SheetCommon.Text = LangMan.LS(LSID.LSID_Common);
            this.SheetNotes.Text = LangMan.LS(LSID.LSID_RPNotes);
            this.SheetSources.Text = LangMan.LS(LSID.LSID_RPSources);
            this.Label1.Text = LangMan.LS(LSID.LSID_Title);
            this.Label2.Text = LangMan.LS(LSID.LSID_Type);
            this.Label4.Text = LangMan.LS(LSID.LSID_StoreType);
            this.Label3.Text = LangMan.LS(LSID.LSID_File);
            this.btnView.Text = LangMan.LS(LSID.LSID_View) + @"...";
        }
    }
}
