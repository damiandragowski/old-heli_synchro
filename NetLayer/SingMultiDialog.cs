using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NetLayer
{
	/// <summary>
	/// Summary description for SingMultiDialog.
	/// </summary>
	public class SingMultiDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		public  int retValue=0;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SingMultiDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SingMultiDialog));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(640, 480);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseHover += new System.EventHandler(this.pictureBox1_MouseHover);
			this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
			// 
			// SingMultiDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SingMultiDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SingMultiDialog";
			this.TopMost = true;
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SingMultiDialog_MouseUp);
			this.ResumeLayout(false);

		}
		#endregion

		private void SingMultiDialog_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.X > 45 && e.X < 174 && e.Y > 298 && e.Y < 317 ) 
			{
				retValue = 1;
				Close();
			}
			if ( e.X > 45 && e.X < 161 && e.Y > 333 && e.Y < 353 ) 
			{
				retValue = 2;
				Close();
			}
			if ( e.X > 45 && e.X < 209 && e.Y > 370 && e.Y < 387 ) 
			{
				retValue = 0;
				Close();
			}
		}

		private void pictureBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.X > 45 && e.X < 174 && e.Y > 298 && e.Y < 317 ) 
			{
				retValue = 1;
				Close();
			}
			if ( e.X > 45 && e.X < 161 && e.Y > 333 && e.Y < 353 ) 
			{
				retValue = 2;
				Close();
			}
			if ( e.X > 45 && e.X < 209 && e.Y > 370 && e.Y < 387 ) 
			{
				retValue = 0;
				Close();
			}		
		}

		private void pictureBox1_MouseHover(object sender, System.EventArgs e)
		{
		{
			Point essesman = Cursor.Position;
			if ( essesman.X > 45 && essesman.X < 174 && essesman.Y > 298 && essesman.Y < 317 ) 
			{
				this.Cursor = Cursors.Hand;
			}
			if ( essesman.X > 45 && essesman.X < 161 && essesman.Y > 333 && essesman.Y < 353 ) 
			{
				this.Cursor = Cursors.Hand;
			}
			if ( essesman.X > 45 && essesman.X < 209 && essesman.Y > 370 && essesman.Y < 387 ) 
			{
				this.Cursor = Cursors.Hand;
			}
			this.Cursor = Cursors.Arrow;
		}
		}
	}

}
