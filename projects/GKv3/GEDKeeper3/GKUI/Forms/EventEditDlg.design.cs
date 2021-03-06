﻿using Eto.Drawing;
using Eto.Forms;
using GKUI.Components;

namespace GKUI.Forms
{
    partial class EventEditDlg
    {
        private Button btnAccept;
        private Button btnCancel;
        private TabControl tabsData;
        private TabPage pageNotes;
        private TabPage pageMultimedia;
        private TabPage pageSources;
        private Button btnAddress;
        private TabPage pageCommon;
        private Label lblEvent;
        private Label lblPlace;
        private Label lblDate;
        private Label lblCause;
        private Label lblOrg;
        private Label lblAttrValue;
        private ComboBox cmbEventType;
        private TextBox txtEventName;
        private TextBox txtEventPlace;
        private ComboBox cmbEventDateType;
        private MaskedTextBox txtEventDate1;
        private MaskedTextBox txtEventDate2;
        private TextBox txtEventCause;
        private TextBox txtEventOrg;
        private ComboBox txtAttribute;
        private Button btnPlaceAdd;
        private Button btnPlaceDelete;
        private ComboBox cmbDate1Calendar;
        private ComboBox cmbDate2Calendar;
        private CheckBox btnBC1;
        private CheckBox btnBC2;

        private void InitializeComponent()
        {
            SuspendLayout();

            btnBC1 = new CheckBox();
            btnBC1.Text = "BC";

            btnBC2 = new CheckBox();
            btnBC2.Text = "BC";

            lblEvent = new Label();
            lblEvent.Text = "lblEvent";

            lblPlace = new Label();
            lblPlace.Text = "lblPlace";

            lblDate = new Label();
            lblDate.Text = "lblDate";

            lblCause = new Label();
            lblCause.Text = "lblCause";

            lblOrg = new Label();
            lblOrg.Text = "lblOrg";

            lblAttrValue = new Label();
            lblAttrValue.Text = "lblAttrValue";

            btnPlaceAdd = new Button();
            btnPlaceAdd.Enabled = false;
            btnPlaceAdd.Size = new Size(26, 26);
            btnPlaceAdd.Click += btnPlaceAdd_Click;
            btnPlaceAdd.Image = Bitmap.FromResource("Resources.btn_rec_new.gif");

            btnPlaceDelete = new Button();
            btnPlaceDelete.Enabled = false;
            btnPlaceDelete.Size = new Size(26, 26);
            btnPlaceDelete.Click += btnPlaceDelete_Click;
            btnPlaceDelete.Image = Bitmap.FromResource("Resources.btn_rec_delete.gif");

            cmbEventType = new ComboBox();
            cmbEventType.ReadOnly = true;
            cmbEventType.SelectedIndexChanged += EditEventType_SelectedIndexChanged;

            txtEventName = new TextBox();

            txtEventPlace = new TextBox();
            txtEventPlace.KeyDown += EditEventPlace_KeyDown;

            cmbEventDateType = new ComboBox();
            cmbEventDateType.ReadOnly = true;
            cmbEventDateType.SelectedIndexChanged += EditEventDateType_SelectedIndexChanged;

            txtEventDate1 = new MaskedTextBox();
            txtEventDate1.Provider = new FixedMaskedTextProvider("00/00/0000");
            //txtEventDate1.AllowDrop = true;
            //txtEventDate1.DragDrop += new DragEventHandler(EditEventDate1_DragDrop);
            //txtEventDate1.DragOver += new DragEventHandler(EditEventDate1_DragOver);

            txtEventDate2 = new MaskedTextBox();
            txtEventDate2.Provider = new FixedMaskedTextProvider("00/00/0000");
            //txtEventDate2.AllowDrop = true;
            //txtEventDate2.DragDrop += new DragEventHandler(EditEventDate1_DragDrop);
            //txtEventDate2.DragOver += new DragEventHandler(EditEventDate1_DragOver);

            txtEventCause = new TextBox();

            txtEventOrg = new TextBox();

            txtAttribute = new ComboBox();

            cmbDate1Calendar = new ComboBox();
            cmbDate1Calendar.ReadOnly = true;

            cmbDate2Calendar = new ComboBox();
            cmbDate2Calendar.ReadOnly = true;

            //

            var datesPanel = new TableLayout {
                Padding = new Padding(0),
                Spacing = new Size(10, 10),
                Rows = {
                    new TableRow {
                        Cells = { cmbEventDateType, txtEventDate1, txtEventDate2 }
                    },
                    new TableRow {
                        Cells = { null,
                            TableLayout.Horizontal(10, cmbDate1Calendar, btnBC1),
                            TableLayout.Horizontal(10, cmbDate2Calendar, btnBC2) }
                    },
                }
            };

            pageCommon = new TabPage();
            pageCommon.Text = "pageCommon";
            pageCommon.Content = new DefTableLayout {
                Rows = {
                    new TableRow {
                        Cells = { lblEvent, TableLayout.Horizontal(10, cmbEventType, txtEventName) }
                    },
                    new TableRow {
                        Cells = { lblAttrValue, txtAttribute }
                    },
                    new TableRow {
                        Cells = { lblPlace, TableLayout.Horizontal(10, new TableCell(txtEventPlace, true), btnPlaceAdd, btnPlaceDelete) }
                    },
                    new TableRow {
                        Cells = { lblDate, datesPanel }
                    },
                    new TableRow {
                        Cells = { lblCause, txtEventCause }
                    },
                    new TableRow {
                        Cells = { lblOrg, txtEventOrg }
                    }
                }
            };

            pageNotes = new TabPage();
            pageNotes.Text = "pageNotes";

            pageMultimedia = new TabPage();
            pageMultimedia.Text = "pageMultimedia";

            pageSources = new TabPage();
            pageSources.Text = "pageSources";

            tabsData = new TabControl();
            tabsData.Pages.Add(pageCommon);
            tabsData.Pages.Add(pageNotes);
            tabsData.Pages.Add(pageMultimedia);
            tabsData.Pages.Add(pageSources);

            btnAccept = new Button();
            btnAccept.ImagePosition = ButtonImagePosition.Left;
            btnAccept.Size = new Size(130, 26);
            btnAccept.Text = "btnAccept";
            btnAccept.Click += btnAccept_Click;
            btnAccept.Image = Bitmap.FromResource("Resources.btn_accept.gif");

            btnCancel = new Button();
            btnCancel.ImagePosition = ButtonImagePosition.Left;
            btnCancel.Size = new Size(130, 26);
            btnCancel.Text = "btnCancel";
            btnCancel.Click += btnCancel_Click;
            btnCancel.Image = Bitmap.FromResource("Resources.btn_cancel.gif");

            btnAddress = new Button();
            btnAddress.Size = new Size(130, 26);
            btnAddress.Text = "btnAddress";
            btnAddress.Click += btnAddress_Click;

            Content = new DefTableLayout {
                Rows = {
                    new TableRow {
                        ScaleHeight = true,
                        Cells = { tabsData }
                    },
                    UIHelper.MakeDialogFooter(btnAddress, null, btnAccept, btnCancel)
                }
            };

            DefaultButton = btnAccept;
            AbortButton = btnCancel;
            Title = "EventEditDlg";

            SetPredefProperties(620, 490);
            ResumeLayout();
        }
    }
}
