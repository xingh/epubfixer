﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ePubFixer
{
    public partial class frmCover : Form
    {
        #region Fields & Constructor
        Image Cover;
        Image DefaultImage;
        string DefaultURL;
        internal event EventHandler<CoverChangedArgs> CoverChanged;
        private string SourceMessage;

        public frmCover(Image CoverFile)
        {
            InitializeComponent();
            this.Icon = Utils.GetIcon();
            WindowSave.RestoreWindows(Properties.Settings.Default.frmCover, this);
            DefaultImage = CoverFile == null ? null : CoverFile;
            DefaultURL = CoverFile == null ? "" : "DefaultImageStream";
            this.Text += " - " + Variables.BookName;
            ChangeImage(DefaultURL);
            SetTooltips();
        }

        private void SetTooltips()
        {
            toolTip.SetToolTip(btnFromFolder, "Uses the cover.jpg file that is in the same directory has the Book.\n" +
                                                "Useful when fetching new covers with Calibre and you just want to\n" +
                                                "update the cover without having to Convert the file again.");
            toolTip.SetToolTip(btnReset, "Changes back to the default Cover from the book");
            toolTip.SetToolTip(btnFile, "Chooses a image file to update the Cover");
            toolTip.SetToolTip(btnSave, "Will Update the image file and make sure that the cover is stretched to fit.\n" +
                                        "Also adds the cover to the guide if it was missing");
        }
        #endregion

        #region Form Events
        private void frmCover_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.frmCover = WindowSave.SaveWindow(this);
            Properties.Settings.Default.Save();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                ChangeImage(openDialog.FileName);
                SourceMessage = " Using Source " + openDialog.FileName;
            }
        }

        private void btnFromFolder_Click(object sender, EventArgs e)
        {
            string CoverFromFolder = string.Empty;
            string[] PossibleNames = { "cover.jpg", "cover.jpeg" };
            foreach (var item in PossibleNames)
            {
                string url = Path.Combine(Path.GetDirectoryName(Variables.Filename), item);
                if (File.Exists(url))
                {
                    CoverFromFolder = url;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(CoverFromFolder))
            {
                ChangeImage(CoverFromFolder);
                SourceMessage = " Using File cover.jpg in folder " + Path.GetDirectoryName(Variables.Filename);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ChangeImage(DefaultURL);
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            OnCoverChanged(new CoverChangedArgs(Cover, cbPreserveRatio.Checked));
        }
        #endregion

        private void ChangeImage(string URL)
        {
            if (!string.IsNullOrEmpty(URL))
            {
                Cover = URL == DefaultURL ? DefaultImage : Image.FromFile(URL);
                pbCover.Image = Cover;
                lblHeigth.Text = Cover.Height.ToString();
                lblWidth.Text = Cover.Width.ToString();
            }
        }

        private void OnCoverChanged(CoverChangedArgs e)
        {
            if (CoverChanged != null)
            {
                CoverChanged(this, e);
                lblStatus.Text = e.ChangedCoverFile ? e.Message + SourceMessage : e.Message;

                //Update the new DefaultImage
                DefaultImage = e.Cover;
            }
        }



    }
}