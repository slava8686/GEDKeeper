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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GKCore;
using GKCore.Interfaces;

namespace GKUI.Components
{
    public class ImageView : UserControl, ILocalization
    {
        private ImageBox imageBox;
        private ToolStrip toolStrip;
        private ToolStripComboBox cbZoomLevels;
        private ToolStripButton btnSizeToFit;
        private ToolStripButton btnZoomIn;
        private ToolStripButton btnZoomOut;


        public List<NamedRegion> NamedRegions
        {
            get { return imageBox.NamedRegions; }
        }

        public bool ShowNamedRegionTips
        {
            get { return imageBox.ShowNamedRegionTips; }
            set { imageBox.ShowNamedRegionTips = value; }
        }

        public bool ShowToolbar
        {
            get { return toolStrip.Visible; }
            set { toolStrip.Visible = value; }
        }

        public ImageBoxSelectionMode SelectionMode
        {
            get { return imageBox.SelectionMode; }
            set { imageBox.SelectionMode = value; }
        }

        public RectangleF SelectionRegion
        {
            get { return imageBox.SelectionRegion; }
            set { imageBox.SelectionRegion = value; }
        }


        public ImageView()
        {
            InitializeComponent();

            FillZoomLevels();
            imageBox.ImageBorderStyle = ImageBoxBorderStyle.FixedSingleGlowShadow;
            imageBox.ImageBorderColor = Color.AliceBlue;
            imageBox.SelectionMode = ImageBoxSelectionMode.Zoom;
            imageBox.AllowZoom = true;
        }

        public void SetLang()
        {
            btnSizeToFit.Text = LangMan.LS(LSID.LSID_SizeToFit);
            btnZoomIn.Text = LangMan.LS(LSID.LSID_ZoomIn);
            btnZoomOut.Text = LangMan.LS(LSID.LSID_ZoomOut);
        }

        #region Design

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                // dummy
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            toolStrip = new ToolStrip();
            btnSizeToFit = new ToolStripButton();
            btnZoomIn = new ToolStripButton();
            btnZoomOut = new ToolStripButton();
            cbZoomLevels = new ToolStripComboBox();
            imageBox = new ImageBox();
            toolStrip.SuspendLayout();
            SuspendLayout();

            toolStrip.Items.AddRange(new ToolStripItem[] {
                                         btnSizeToFit,
                                         btnZoomIn,
                                         btnZoomOut,
                                         new ToolStripSeparator(),
                                         cbZoomLevels,
                                         new ToolStripSeparator()});

            btnSizeToFit.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnSizeToFit.Image = ExtResources.iSizeToFit;
            btnSizeToFit.ImageTransparentColor = Color.Magenta;
            btnSizeToFit.Name = "btnSizeToFit";
            btnSizeToFit.Click += btnSizeToFit_Click;

            btnZoomIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnZoomIn.Image = ExtResources.iZoomIn;
            btnZoomIn.ImageTransparentColor = Color.Magenta;
            btnZoomIn.Name = "btnZoomIn";
            btnZoomIn.Click += btnZoomIn_Click;

            btnZoomOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnZoomOut.Image = ExtResources.iZoomOut;
            btnZoomOut.ImageTransparentColor = Color.Magenta;
            btnZoomOut.Name = "btnZoomOut";
            btnZoomOut.Click += btnZoomOut_Click;

            cbZoomLevels.DropDownStyle = ComboBoxStyle.DropDownList;
            cbZoomLevels.Name = "zoomLevelsToolStripComboBox";
            cbZoomLevels.Size = new Size(140, 28);
            cbZoomLevels.SelectedIndexChanged += zoomLevelsToolStripComboBox_SelectedIndexChanged;

            imageBox.BackColor = SystemColors.ControlDark;
            imageBox.Dock = DockStyle.Fill;
            imageBox.ZoomChanged += imageBox_ZoomChanged;

            Controls.Add(imageBox);
            Controls.Add(toolStrip);
            Size = new Size(944, 389);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public void OpenImage(Image image)
        {
            if (image != null) {
                imageBox.BeginUpdate();
                imageBox.Image = image;
                imageBox.ZoomToFit();
                imageBox.EndUpdate();

                UpdateZoomLevels();
            }
        }

        private void FillZoomLevels()
        {
            cbZoomLevels.Items.Clear();

            foreach (int zoom in imageBox.ZoomLevels)
                cbZoomLevels.Items.Add(string.Format("{0}%", zoom));
        }

        private void UpdateZoomLevels()
        {
            cbZoomLevels.Text = string.Format("{0}%", imageBox.Zoom);
        }

        private void btnSizeToFit_Click(object sender, EventArgs e)
        {
            imageBox.ZoomToFit();
            UpdateZoomLevels();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            imageBox.ZoomIn();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            imageBox.ZoomOut();
        }

        private void imageBox_ZoomChanged(object sender, EventArgs e)
        {
            UpdateZoomLevels();
        }

        private void zoomLevelsToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int zoom = Convert.ToInt32(cbZoomLevels.Text.Substring(0, cbZoomLevels.Text.Length - 1));
            imageBox.Zoom = zoom;
        }

        protected override void Select(bool directed, bool forward)
        {
            base.Select(directed, forward);
            imageBox.Select();
        }
    }
}
