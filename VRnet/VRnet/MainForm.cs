using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using Microsoft.DirectX;

namespace VRnet
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		internal System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Panel panel1;
		private GameWorld gameworld;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// nie bedzie skakac
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);

			this.Show();

			gameworld = new GameWorld(this);	
			gameworld.Initialize();
			
			Loop();



			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private void Loop()
		{
			while(Created)
			{
				gameworld.Loop();
				Application.DoEvents();
				Thread.Sleep(2);
			}
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(8, 400);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(280, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(320, 400);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "Send";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(8, 16);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(280, 368);
			this.textBox2.TabIndex = 2;
			this.textBox2.Text = "";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Location = new System.Drawing.Point(296, 16);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(280, 368);
			this.panel1.TabIndex = 3;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(584, 435);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.Name = "MainForm";
			this.Text = "PKBMCMPGDD";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			MainForm mainForm = new MainForm();
			Application.Exit();
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			if ( textBox1.Text.Length > 0 )
				gameworld.SendMyMessage(textBox1.Text);	
		}
		public void clear()
		{
			panel1.CreateGraphics().FillRectangle(Brushes.White, 0, 0, panel1.Width, panel1.Height);
		}
		public void setDot(Vector2 position)
		{
			panel1.CreateGraphics().DrawEllipse(Pens.Black, position.X, position.Y, 4, 4);
		}
	}
}
