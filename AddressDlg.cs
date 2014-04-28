//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL: svn://utopia/projects/GXDeviceEditor/Development/AddIns/DLMS/AddressDlg.cs $
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
using System.Globalization;
using System.IO;
using System.Xml;
using Gurux.Device.Editor;
using Gurux.Device;
using Gurux.Common;
using System.Text;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// An IEC62056 specific custom wizard page. The page is used with the GXWizardDlg class.
	/// </summary>
	internal class AddressDlg : System.Windows.Forms.Form, IGXWizardPage
    {
		private System.Windows.Forms.Label nameLbl;
		GXProperty m_Property = null;		
		private Label ReadModeLbl;
		private Label WriteModeLbl;
		private ComboBox ReadModeDdl;
		private ComboBox WriteModeDdl;
		private ComboBox AddressCb;
		private TextBox addressTb;
		private System.ComponentModel.Container m_Components = null;				

        public AddressDlg(GXProperty property)
		{
			InitializeComponent();			
            m_Property = property;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            //Update resources..            
            nameLbl.Text = "OBIS " + Gurux.IEC62056_21.AddIn.Properties.Resources.AddressTxt;            
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
            this.nameLbl = new System.Windows.Forms.Label();
            this.ReadModeLbl = new System.Windows.Forms.Label();
            this.WriteModeLbl = new System.Windows.Forms.Label();
            this.ReadModeDdl = new System.Windows.Forms.ComboBox();
            this.WriteModeDdl = new System.Windows.Forms.ComboBox();
            this.AddressCb = new System.Windows.Forms.ComboBox();
            this.addressTb = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // nameLbl
            // 
            this.nameLbl.Location = new System.Drawing.Point(22, 12);
            this.nameLbl.Name = "nameLbl";
            this.nameLbl.Size = new System.Drawing.Size(90, 16);
            this.nameLbl.TabIndex = 0;
            this.nameLbl.Text = "OBIS Address:";
            // 
            // ReadModeLbl
            // 
            this.ReadModeLbl.Location = new System.Drawing.Point(22, 41);
            this.ReadModeLbl.Name = "ReadModeLbl";
            this.ReadModeLbl.Size = new System.Drawing.Size(90, 16);
            this.ReadModeLbl.TabIndex = 6;
            this.ReadModeLbl.Text = "Read Mode:";
            // 
            // WriteModeLbl
            // 
            this.WriteModeLbl.Location = new System.Drawing.Point(22, 68);
            this.WriteModeLbl.Name = "WriteModeLbl";
            this.WriteModeLbl.Size = new System.Drawing.Size(90, 16);
            this.WriteModeLbl.TabIndex = 8;
            this.WriteModeLbl.Text = "Write Mode:";
            // 
            // ReadModeDdl
            // 
            this.ReadModeDdl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ReadModeDdl.FormattingEnabled = true;
            this.ReadModeDdl.Items.AddRange(new object[] {
            "R1",
            "R2",
            "R3",
            "R4",
            "R5"});
            this.ReadModeDdl.Location = new System.Drawing.Point(119, 38);
            this.ReadModeDdl.Name = "ReadModeDdl";
            this.ReadModeDdl.Size = new System.Drawing.Size(167, 21);
            this.ReadModeDdl.TabIndex = 9;
            // 
            // WriteModeDdl
            // 
            this.WriteModeDdl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WriteModeDdl.FormattingEnabled = true;
            this.WriteModeDdl.Items.AddRange(new object[] {
            "W1",
            "W2",
            "W3",
            "W4",
            "W5"});
            this.WriteModeDdl.Location = new System.Drawing.Point(118, 65);
            this.WriteModeDdl.Name = "WriteModeDdl";
            this.WriteModeDdl.Size = new System.Drawing.Size(167, 21);
            this.WriteModeDdl.TabIndex = 10;
            // 
            // AddressCb
            // 
            this.AddressCb.FormattingEnabled = true;
            this.AddressCb.Location = new System.Drawing.Point(119, 9);
            this.AddressCb.Name = "AddressCb";
            this.AddressCb.Size = new System.Drawing.Size(166, 21);
            this.AddressCb.TabIndex = 11;
            // 
            // addressTb
            // 
            this.addressTb.Location = new System.Drawing.Point(118, 9);
            this.addressTb.Name = "addressTb";
            this.addressTb.Size = new System.Drawing.Size(167, 20);
            this.addressTb.TabIndex = 12;
            // 
            // AddressDlg
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(322, 208);
            this.Controls.Add(this.addressTb);
            this.Controls.Add(this.AddressCb);
            this.Controls.Add(this.WriteModeDdl);
            this.Controls.Add(this.ReadModeDdl);
            this.Controls.Add(this.WriteModeLbl);
            this.Controls.Add(this.ReadModeLbl);
            this.Controls.Add(this.nameLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddressDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddressDlg";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        #region IGXWizardPage Members

        bool IGXWizardPage.IsShown()
        {
            return true;
        }

        void IGXWizardPage.Next()
        {
            if (AddressCb.Text.Trim().Length == 0)
            {
                throw new Exception("OBIS code is invalid.");
            }            
        }

        void IGXWizardPage.Back()
        {
            
        }

        void IGXWizardPage.Finish()
        {
            if (m_Property is GXIEC62056Property)
            {                
                ((GXIEC62056Property)m_Property).Data = AddressCb.Text;
                ((GXIEC62056Property)m_Property).WriteMode = Convert.ToInt32((WriteModeDdl.SelectedIndex + 1).ToString());
                ((GXIEC62056Property)m_Property).ReadMode = Convert.ToInt32((ReadModeDdl.SelectedIndex + 1).ToString());
            }
            else if (m_Property is GXIEC62056Property)
            {
                string address = addressTb.Text;
                address = address.Replace("(", ".");
                address = address.Replace(")", ".");
                address = address.Replace("-", ".");
                address = address.Replace(":", ".");
                ((GXIEC62056Property)m_Property).Data = address;
            }           
        }

        void IGXWizardPage.Cancel()
        {
            
        }

        void IGXWizardPage.Initialize()
        {
            /* Mikko
            if (m_Property is GXIEC62056Property)
			{
                GXIEC62056TableProperty prop = m_Property  as GXIEC62056TableProperty;
				AddressCb.Visible = WriteModeLbl.Visible = WriteModeDdl.Visible = ReadModeLbl.Visible = ReadModeDdl.Visible = false;
				addressTb.Text = prop.Data;                
			}
            else 
             * */
            if (m_Property is GXIEC62056Property)
            {
                GXIEC62056Property prop = m_Property as GXIEC62056Property;
                addressTb.Visible = false;                
                ReadModeDdl.SelectedIndex = Convert.ToInt32(prop.ReadMode) - 1;
                if (ReadModeDdl.SelectedIndex == -1)
                {
                    ReadModeDdl.SelectedIndex = 1;
                }
                WriteModeDdl.SelectedIndex = Convert.ToInt32(prop.WriteMode) -1;
                if (WriteModeDdl.SelectedIndex == -1)
                {
                    WriteModeDdl.SelectedIndex = 1;
                }                
                if (AddressCb.SelectedIndex == -1)
                {
                    AddressCb.Text = prop.Data;
                }
            }
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
                return "IEC62056";
            }
        }

        string IGXWizardPage.Description
        {
            get
            {
                return string.Empty;
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
