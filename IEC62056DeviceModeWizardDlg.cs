//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL: svn://utopia/projects/GXDeviceEditor/Development/AddIns/DLMS/DlmsTypeWizardDlg.cs $
//
// Version:         $Revision: 870 $,
//                  $Date: 2009-09-29 17:21:48 +0300 (ti, 29 syys 2009) $
//                  $Author: airija $
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------


using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Gurux.Device.Editor;
using Gurux.Device;
using System.Globalization;
using System.Collections.Generic;
using Gurux.Common;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// An IEC62056 specific custom wizard page. The page is used with the GXWizardDlg class.
	/// </summary>
	internal class IEC62056DeviceModeWizardDlg : System.Windows.Forms.Form, IGXWizardPage
	{
		private System.Windows.Forms.Label typeLbl;
		GXIEC62056Device m_Device = null;
		private System.ComponentModel.Container m_Components = null;
		private object m_Target = null;
		private ComboBox typeCb;
		private object m_TargetParent = null;

		/// <summary>
		/// Initializes a new instance of the IEC62056DeviceModeWizardDlg class.
		/// </summary>
		public IEC62056DeviceModeWizardDlg(GXDevice device)
		{
			InitializeComponent();
            typeCb.Items.Add(Protocol.ModeA);
            typeCb.Items.Add(Protocol.ModeC);
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
			UpdateResources();
			m_Device = (GXIEC62056Device)device;
			this.TopLevel = false;
			this.FormBorderStyle = FormBorderStyle.None;
		}

		private void UpdateResources()
		{
			this.typeLbl.Text = Gurux.IEC62056_21.AddIn.Properties.Resources.DeviceModeTxt;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_Components != null)
				{
					m_Components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
            this.typeLbl = new System.Windows.Forms.Label();
            this.typeCb = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // typeLbl
            // 
            this.typeLbl.Location = new System.Drawing.Point(12, 25);
            this.typeLbl.Name = "typeLbl";
            this.typeLbl.Size = new System.Drawing.Size(104, 16);
            this.typeLbl.TabIndex = 1;
            this.typeLbl.Text = "TypeLbl";
            // 
            // typeCb
            // 
            this.typeCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeCb.FormattingEnabled = true;
            this.typeCb.Location = new System.Drawing.Point(97, 22);
            this.typeCb.Name = "typeCb";
            this.typeCb.Size = new System.Drawing.Size(290, 21);
            this.typeCb.TabIndex = 4;
            // 
            // IEC62056DeviceModeWizardDlg
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(464, 397);
            this.Controls.Add(this.typeCb);
            this.Controls.Add(this.typeLbl);
            this.Name = "IEC62056DeviceModeWizardDlg";
            this.Text = "DlmsTypeWizardDlg";
            this.ResumeLayout(false);

		}
		#endregion
		

		#region IGXWizardPage Members


		public object Target
		{
			get
			{
				return m_Target;
			}
			set
			{
				m_Target = value;
			}
		}

		public object TargetParent
		{
			get
			{
				return m_TargetParent;
			}
			set
			{
				m_TargetParent = value;
			}
		}

		#endregion

        #region IGXWizardPage Members

        bool IGXWizardPage.IsShown()
        {
            return true;
        }

        void IGXWizardPage.Next()
        {
            if (typeCb.SelectedIndex == -1)
            {
                throw new Exception(Gurux.IEC62056_21.AddIn.Properties.Resources.DeviceModeSelectErrTxt);
            }
        }

        void IGXWizardPage.Back()
        {
        }

        void IGXWizardPage.Finish()
        {
            if (m_Device != null)
            {                
                if (typeCb.SelectedIndex == -1)
                {
                    throw new Exception("Invalid protocol mode.");             
                }
                m_Device.Mode = (Protocol)typeCb.SelectedItem;
            }
        }

        void IGXWizardPage.Cancel()
        {
        }

        void IGXWizardPage.Initialize()
        {
        }

        GXWizardButtons IGXWizardPage.EnabledButtons
        {
            get
            {
                return GXWizardButtons.All;
            }
        }

        string IGXWizardPage.Caption
        {
            get
            {
                return "";
            }
        }

        string IGXWizardPage.Description
        {
            get
            {
                return "";
            }
        }

        object IGXWizardPage.Target
        {
            get;
            set;
        }

        #endregion
    }
}
