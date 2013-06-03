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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Gurux.Device.Editor;
using Gurux.Device;
using System.Globalization;
using Gurux.Common;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// A IEC62056 specific custom wizard page. The page is used with the GXWizardDlg class.
	/// </summary>
    internal class IEC62056TypeWizardDlg : System.Windows.Forms.Form, IGXWizardPage
	{
		private System.Windows.Forms.Label typeLbl;
		GXIEC62056Table m_Table = null;
		private System.ComponentModel.Container m_Components = null;

		/// <summary>
		/// Initializes a new instance of the DlmsTypeWizardDlg class.
		/// </summary>
		public IEC62056TypeWizardDlg(GXTable table)
		{
			InitializeComponent();

			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
			UpdateResources();
			m_Table = (GXIEC62056Table)table;
		}

		private void UpdateResources()
		{
			this.typeLbl.Text = Gurux.IEC62056_21.AddIn.Properties.Resources.TypeTxt;
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
			// IEC62056TypeWizardDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(464, 397);
			this.Controls.Add(this.typeLbl);
			this.Name = "IEC62056TypeWizardDlg";
			this.Text = "IEC62056TypeWizardDlg";
			this.ResumeLayout(false);

		}
		#endregion	

        #region IGXWizardPage Members

        bool IGXWizardPage.IsShown()
        {
            return true;
        }

        void IGXWizardPage.Next()
        {           
        }

        void IGXWizardPage.Back()
        {         
        }

        void IGXWizardPage.Finish()
        {
            throw new NotImplementedException();
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
