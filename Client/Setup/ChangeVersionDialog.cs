//-----------------------------------------------------------------------
// <copyright>
// Copyright (C) Ruslan Yakushev for the PHP Manager for IIS project.
//
// This file is subject to the terms and conditions of the Microsoft Public License (MS-PL).
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL for more details.
// </copyright>
//-----------------------------------------------------------------------

//#define VSDesigner

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Web.Management.Client.Win32;
using Web.Management.PHP.Config;

namespace Web.Management.PHP.Setup
{

    internal sealed class ChangeVersionDialog :
#if VSDesigner
        Form
#else
        TaskForm
#endif
    {
        private PHPModule _module;

        private ManagementPanel _contentPanel;
        private Label _selectVersionLabel;
        private ComboBox _versionComboBox;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ChangeVersionDialog(PHPModule module) : base(module)
        {
            _module = module;
            InitializeComponent();
            InitializeUI();
        }

        protected override bool CanAccept
        {
            get
            {
                return (_versionComboBox.Items.Count > 0);
            }
        }

        protected override bool CanShowHelp
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._selectVersionLabel = new System.Windows.Forms.Label();
            this._versionComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _selectVersionLabel
            // 
            this._selectVersionLabel.AutoSize = true;
            this._selectVersionLabel.Location = new System.Drawing.Point(0, 13);
            this._selectVersionLabel.Name = "_selectVersionLabel";
            this._selectVersionLabel.Size = new System.Drawing.Size(102, 13);
            this._selectVersionLabel.TabIndex = 0;
            this._selectVersionLabel.Text = Resources.ChangeVersionDialogSelectVersion;
            // 
            // _versionComboBox
            // 
            this._versionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._versionComboBox.FormattingEnabled = true;
            this._versionComboBox.Location = new System.Drawing.Point(3, 30);
            this._versionComboBox.Name = "_versionComboBox";
            this._versionComboBox.Size = new System.Drawing.Size(326, 21);
            this._versionComboBox.TabIndex = 1;
            // 
            // ChangeVersionDialog
            // 
            this.ClientSize = new System.Drawing.Size(364, 142);
            this.Controls.Add(this._versionComboBox);
            this.Controls.Add(this._selectVersionLabel);
            this.Name = "ChangeVersionDialog";
            this.ResumeLayout(false);
#if VSDesigner
            this.PerformLayout();
#endif
        }

        private void InitializeUI()
        {
            _contentPanel = new ManagementPanel();
            _contentPanel.SuspendLayout();

            this._contentPanel.Location = new System.Drawing.Point(0, 0);
            this._contentPanel.Dock = DockStyle.Fill;
            this._contentPanel.Controls.Add(_selectVersionLabel);
            this._contentPanel.Controls.Add(_versionComboBox);

            this._contentPanel.ResumeLayout(false);
            this._contentPanel.PerformLayout();

            this.Text = Resources.ChangeVersionDialogTitle;

            SetContent(_contentPanel);
            UpdateTaskForm();
        }

        protected override void OnAccept()
        {
            PHPVersion selectedItem  = (PHPVersion)_versionComboBox.SelectedItem;

            try
            {
                _module.Proxy.SelectPHPVersion(selectedItem.HandlerName);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex, Resources.ResourceManager);
            }
            Close();
        }

        private void OnGetVersionsDoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _module.Proxy.GetAllPHPVersions();
        }

        private void OnGetVersionsDoWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _versionComboBox.BeginUpdate();
            _versionComboBox.SuspendLayout();

            try
            {
                RemoteObjectCollection<PHPVersion> phpVersions = e.Result as RemoteObjectCollection<PHPVersion>;
                foreach (PHPVersion phpVersion in phpVersions)
                {
                    phpVersion.Version = String.Format("{0} ({1})", phpVersion.Version, phpVersion.ScriptProcessor);
                    _versionComboBox.Items.Add(phpVersion);
                }
                _versionComboBox.DisplayMember = "Version";
                _versionComboBox.SelectedIndex = 0;
                if (_versionComboBox.Items.Count > 0)
                {
                    UpdateTaskForm();
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex, Resources.ResourceManager);
            }
            finally
            {
                _versionComboBox.ResumeLayout();
                _versionComboBox.EndUpdate();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            StartAsyncTask(OnGetVersionsDoWork, OnGetVersionsDoWorkCompleted);
        }

        protected override void ShowHelp()
        {
            Helper.Browse(Globals.ChangeVersionOnlineHelp);
        }
    }
}
