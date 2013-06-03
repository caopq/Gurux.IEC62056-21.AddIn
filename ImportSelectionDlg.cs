using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gurux.Device.Editor;
using Gurux.Common;
using System.IO;
using System.Xml;

namespace Gurux.IEC62056_21.AddIn
{
	public partial class ImportSelectionDlg : Form, IGXWizardPage
	{
		public ImportSelectionDlg()
		{
			InitializeComponent();
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
		}

        public string DeviceSerialNumber
        {
            get;
            set;
        }

		#region IGXWizardPage Members

		bool IGXWizardPage.IsShown()
		{
			return true;
		}

		void IGXWizardPage.Next()
		{
			DeviceSerialNumber = this.SerialNumberTB.Text;
		}

		void IGXWizardPage.Back()
		{
		}

		void IGXWizardPage.Finish()
		{
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
