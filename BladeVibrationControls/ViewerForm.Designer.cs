namespace BladeVibrationControls {
	partial class ViewerForm {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose ( bool disposing ) {
			if ( disposing && (components != null) ) {
				components.Dispose ();
			}
			base.Dispose ( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent () {
			components = new System.ComponentModel.Container ();
			UpdateTimer = new System.Windows.Forms.Timer ( components );
			ShaderSelector = new TabControl ();
			tabPage1 = new TabPage ();
			TB_OV_WinPosY = new TextBox ();
			TB_OV_WinPosX = new TextBox ();
			label7 = new Label ();
			tabPage2 = new TabPage ();
			TB_BM_MaxCut_Z = new TextBox ();
			TB_BM_MinCut_Z = new TextBox ();
			TB_BM_MaxCut_Y = new TextBox ();
			TB_BM_MinCut_Y = new TextBox ();
			TB_BM_MaxCut_X = new TextBox ();
			label19 = new Label ();
			TB_BM_MinCut_X = new TextBox ();
			label15 = new Label ();
			GB_BM_Materials = new GroupBox ();
			label18 = new Label ();
			label17 = new Label ();
			label16 = new Label ();
			TB_BM_CamRotY = new TextBox ();
			TB_BM_CamRotX = new TextBox ();
			label4 = new Label ();
			TB_BM_CamPosZ = new TextBox ();
			TB_BM_CamPosY = new TextBox ();
			TB_BM_CamPosX = new TextBox ();
			label3 = new Label ();
			label2 = new Label ();
			TB_BM_Scale = new TextBox ();
			TB_BM_OffsetZ = new TextBox ();
			TB_BM_OffsetY = new TextBox ();
			TB_BM_OffsetX = new TextBox ();
			label1 = new Label ();
			tabPage3 = new TabPage ();
			TB_VO_CamRotZ = new TextBox ();
			TB_VO_CamRotY = new TextBox ();
			TB_VO_CamRotX = new TextBox ();
			label13 = new Label ();
			TB_VO_CamPosZ = new TextBox ();
			TB_VO_CamPosY = new TextBox ();
			TB_VO_CamPosX = new TextBox ();
			label14 = new Label ();
			GB_ProjMode = new GroupBox ();
			RB_PM_Persp = new RadioButton ();
			RB_PM_Ortho = new RadioButton ();
			TB_VO_ProjZ1 = new TextBox ();
			TB_VO_ProjZ0 = new TextBox ();
			label10 = new Label ();
			TB_VO_ProjY1 = new TextBox ();
			TB_VO_ProjY0 = new TextBox ();
			label9 = new Label ();
			TB_VO_ProjX1 = new TextBox ();
			TB_VO_ProjX0 = new TextBox ();
			label8 = new Label ();
			TB_VO_ModelSize_Z = new TextBox ();
			TB_VO_ModelSize_Y = new TextBox ();
			TB_VO_ModelSize_X = new TextBox ();
			label6 = new Label ();
			TB_VO_DimsZ = new TextBox ();
			TB_VO_ModelBase_Z = new TextBox ();
			TB_VO_DimsY = new TextBox ();
			TB_VO_ModelBase_Y = new TextBox ();
			TB_VO_VoxelSize = new TextBox ();
			label12 = new Label ();
			TB_VO_DimsX = new TextBox ();
			label11 = new Label ();
			TB_VO_ModelBase_X = new TextBox ();
			label5 = new Label ();
			tabPage4 = new TabPage ();
			BTN_Pause = new Button ();
			BTN_Apply = new Button ();
			LBL_StatusMsg = new Label ();
			TB_VV_Scale = new TextBox ();
			label20 = new Label ();
			label21 = new Label ();
			CB_VV_Transform = new CheckBox ();
			ShaderSelector.SuspendLayout ();
			tabPage1.SuspendLayout ();
			tabPage2.SuspendLayout ();
			GB_BM_Materials.SuspendLayout ();
			tabPage3.SuspendLayout ();
			GB_ProjMode.SuspendLayout ();
			tabPage4.SuspendLayout ();
			SuspendLayout ();
			// 
			// UpdateTimer
			// 
			UpdateTimer.Enabled = true;
			UpdateTimer.Interval = 20;
			UpdateTimer.Tick += UpdateTimer_Tick;
			// 
			// ShaderSelector
			// 
			ShaderSelector.Controls.Add ( tabPage1 );
			ShaderSelector.Controls.Add ( tabPage2 );
			ShaderSelector.Controls.Add ( tabPage3 );
			ShaderSelector.Controls.Add ( tabPage4 );
			ShaderSelector.Dock = DockStyle.Top;
			ShaderSelector.Location = new Point ( 0, 0 );
			ShaderSelector.Name = "ShaderSelector";
			ShaderSelector.SelectedIndex = 0;
			ShaderSelector.Size = new Size ( 414, 809 );
			ShaderSelector.TabIndex = 0;
			// 
			// tabPage1
			// 
			tabPage1.Controls.Add ( TB_OV_WinPosY );
			tabPage1.Controls.Add ( TB_OV_WinPosX );
			tabPage1.Controls.Add ( label7 );
			tabPage1.Location = new Point ( 4, 24 );
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding ( 3 );
			tabPage1.Size = new Size ( 406, 781 );
			tabPage1.TabIndex = 0;
			tabPage1.Text = "Overview";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// TB_OV_WinPosY
			// 
			TB_OV_WinPosY.Location = new Point ( 222, 6 );
			TB_OV_WinPosY.Name = "TB_OV_WinPosY";
			TB_OV_WinPosY.Size = new Size ( 82, 23 );
			TB_OV_WinPosY.TabIndex = 14;
			// 
			// TB_OV_WinPosX
			// 
			TB_OV_WinPosX.Location = new Point ( 134, 6 );
			TB_OV_WinPosX.Name = "TB_OV_WinPosX";
			TB_OV_WinPosX.Size = new Size ( 82, 23 );
			TB_OV_WinPosX.TabIndex = 13;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new Point ( 3, 9 );
			label7.Name = "label7";
			label7.Size = new Size ( 132, 15 );
			label7.TabIndex = 1;
			label7.Text = "Game window position:";
			label7.TextAlign = ContentAlignment.TopRight;
			// 
			// tabPage2
			// 
			tabPage2.Controls.Add ( TB_BM_MaxCut_Z );
			tabPage2.Controls.Add ( TB_BM_MinCut_Z );
			tabPage2.Controls.Add ( TB_BM_MaxCut_Y );
			tabPage2.Controls.Add ( TB_BM_MinCut_Y );
			tabPage2.Controls.Add ( TB_BM_MaxCut_X );
			tabPage2.Controls.Add ( label19 );
			tabPage2.Controls.Add ( TB_BM_MinCut_X );
			tabPage2.Controls.Add ( label15 );
			tabPage2.Controls.Add ( GB_BM_Materials );
			tabPage2.Controls.Add ( TB_BM_CamRotY );
			tabPage2.Controls.Add ( TB_BM_CamRotX );
			tabPage2.Controls.Add ( label4 );
			tabPage2.Controls.Add ( TB_BM_CamPosZ );
			tabPage2.Controls.Add ( TB_BM_CamPosY );
			tabPage2.Controls.Add ( TB_BM_CamPosX );
			tabPage2.Controls.Add ( label3 );
			tabPage2.Controls.Add ( label2 );
			tabPage2.Controls.Add ( TB_BM_Scale );
			tabPage2.Controls.Add ( TB_BM_OffsetZ );
			tabPage2.Controls.Add ( TB_BM_OffsetY );
			tabPage2.Controls.Add ( TB_BM_OffsetX );
			tabPage2.Controls.Add ( label1 );
			tabPage2.Location = new Point ( 4, 24 );
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding ( 3 );
			tabPage2.Size = new Size ( 406, 781 );
			tabPage2.TabIndex = 1;
			tabPage2.Text = "BasicMesh";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// TB_BM_MaxCut_Z
			// 
			TB_BM_MaxCut_Z.Location = new Point ( 310, 194 );
			TB_BM_MaxCut_Z.Name = "TB_BM_MaxCut_Z";
			TB_BM_MaxCut_Z.Size = new Size ( 82, 23 );
			TB_BM_MaxCut_Z.TabIndex = 17;
			// 
			// TB_BM_MinCut_Z
			// 
			TB_BM_MinCut_Z.Location = new Point ( 310, 165 );
			TB_BM_MinCut_Z.Name = "TB_BM_MinCut_Z";
			TB_BM_MinCut_Z.Size = new Size ( 82, 23 );
			TB_BM_MinCut_Z.TabIndex = 17;
			// 
			// TB_BM_MaxCut_Y
			// 
			TB_BM_MaxCut_Y.Location = new Point ( 222, 194 );
			TB_BM_MaxCut_Y.Name = "TB_BM_MaxCut_Y";
			TB_BM_MaxCut_Y.Size = new Size ( 82, 23 );
			TB_BM_MaxCut_Y.TabIndex = 16;
			// 
			// TB_BM_MinCut_Y
			// 
			TB_BM_MinCut_Y.Location = new Point ( 222, 165 );
			TB_BM_MinCut_Y.Name = "TB_BM_MinCut_Y";
			TB_BM_MinCut_Y.Size = new Size ( 82, 23 );
			TB_BM_MinCut_Y.TabIndex = 16;
			// 
			// TB_BM_MaxCut_X
			// 
			TB_BM_MaxCut_X.Location = new Point ( 134, 194 );
			TB_BM_MaxCut_X.Name = "TB_BM_MaxCut_X";
			TB_BM_MaxCut_X.Size = new Size ( 82, 23 );
			TB_BM_MaxCut_X.TabIndex = 15;
			// 
			// label19
			// 
			label19.AutoSize = true;
			label19.Location = new Point ( 28, 197 );
			label19.Name = "label19";
			label19.Size = new Size ( 101, 15 );
			label19.TabIndex = 14;
			label19.Text = "Maximum Cutoff:";
			label19.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_BM_MinCut_X
			// 
			TB_BM_MinCut_X.Location = new Point ( 134, 165 );
			TB_BM_MinCut_X.Name = "TB_BM_MinCut_X";
			TB_BM_MinCut_X.Size = new Size ( 82, 23 );
			TB_BM_MinCut_X.TabIndex = 15;
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.Location = new Point ( 28, 168 );
			label15.Name = "label15";
			label15.Size = new Size ( 100, 15 );
			label15.TabIndex = 14;
			label15.Text = "Minimum Cutoff:";
			label15.TextAlign = ContentAlignment.TopRight;
			// 
			// GB_BM_Materials
			// 
			GB_BM_Materials.Controls.Add ( label18 );
			GB_BM_Materials.Controls.Add ( label17 );
			GB_BM_Materials.Controls.Add ( label16 );
			GB_BM_Materials.Dock = DockStyle.Bottom;
			GB_BM_Materials.Location = new Point ( 3, 399 );
			GB_BM_Materials.Name = "GB_BM_Materials";
			GB_BM_Materials.Size = new Size ( 400, 379 );
			GB_BM_Materials.TabIndex = 13;
			GB_BM_Materials.TabStop = false;
			GB_BM_Materials.Text = "Materials";
			// 
			// label18
			// 
			label18.AutoSize = true;
			label18.Location = new Point ( 248, 19 );
			label18.Name = "label18";
			label18.Size = new Size ( 44, 15 );
			label18.TabIndex = 2;
			label18.Text = "Diffuse";
			// 
			// label17
			// 
			label17.AutoSize = true;
			label17.Location = new Point ( 157, 19 );
			label17.Name = "label17";
			label17.Size = new Size ( 56, 15 );
			label17.TabIndex = 2;
			label17.Text = "Shininess";
			// 
			// label16
			// 
			label16.AutoSize = true;
			label16.Location = new Point ( 58, 19 );
			label16.Name = "label16";
			label16.Size = new Size ( 78, 15 );
			label16.TabIndex = 2;
			label16.Text = "Render mode";
			// 
			// TB_BM_CamRotY
			// 
			TB_BM_CamRotY.Location = new Point ( 222, 117 );
			TB_BM_CamRotY.Name = "TB_BM_CamRotY";
			TB_BM_CamRotY.Size = new Size ( 82, 23 );
			TB_BM_CamRotY.TabIndex = 12;
			// 
			// TB_BM_CamRotX
			// 
			TB_BM_CamRotX.Location = new Point ( 134, 117 );
			TB_BM_CamRotX.Name = "TB_BM_CamRotX";
			TB_BM_CamRotX.Size = new Size ( 82, 23 );
			TB_BM_CamRotX.TabIndex = 11;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point ( 32, 120 );
			label4.Name = "label4";
			label4.Size = new Size ( 96, 15 );
			label4.TabIndex = 10;
			label4.Text = "Camera rotation:";
			label4.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_BM_CamPosZ
			// 
			TB_BM_CamPosZ.Location = new Point ( 310, 88 );
			TB_BM_CamPosZ.Name = "TB_BM_CamPosZ";
			TB_BM_CamPosZ.Size = new Size ( 82, 23 );
			TB_BM_CamPosZ.TabIndex = 9;
			// 
			// TB_BM_CamPosY
			// 
			TB_BM_CamPosY.Location = new Point ( 222, 88 );
			TB_BM_CamPosY.Name = "TB_BM_CamPosY";
			TB_BM_CamPosY.Size = new Size ( 82, 23 );
			TB_BM_CamPosY.TabIndex = 8;
			// 
			// TB_BM_CamPosX
			// 
			TB_BM_CamPosX.Location = new Point ( 134, 88 );
			TB_BM_CamPosX.Name = "TB_BM_CamPosX";
			TB_BM_CamPosX.Size = new Size ( 82, 23 );
			TB_BM_CamPosX.TabIndex = 7;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point ( 31, 91 );
			label3.Name = "label3";
			label3.Size = new Size ( 97, 15 );
			label3.TabIndex = 6;
			label3.Text = "Camera position:";
			label3.TextAlign = ContentAlignment.TopRight;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point ( 91, 49 );
			label2.Name = "label2";
			label2.Size = new Size ( 37, 15 );
			label2.TabIndex = 5;
			label2.Text = "Scale:";
			label2.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_BM_Scale
			// 
			TB_BM_Scale.Location = new Point ( 134, 46 );
			TB_BM_Scale.Name = "TB_BM_Scale";
			TB_BM_Scale.Size = new Size ( 82, 23 );
			TB_BM_Scale.TabIndex = 4;
			// 
			// TB_BM_OffsetZ
			// 
			TB_BM_OffsetZ.Location = new Point ( 310, 6 );
			TB_BM_OffsetZ.Name = "TB_BM_OffsetZ";
			TB_BM_OffsetZ.Size = new Size ( 82, 23 );
			TB_BM_OffsetZ.TabIndex = 3;
			// 
			// TB_BM_OffsetY
			// 
			TB_BM_OffsetY.Location = new Point ( 222, 6 );
			TB_BM_OffsetY.Name = "TB_BM_OffsetY";
			TB_BM_OffsetY.Size = new Size ( 82, 23 );
			TB_BM_OffsetY.TabIndex = 2;
			// 
			// TB_BM_OffsetX
			// 
			TB_BM_OffsetX.Location = new Point ( 134, 6 );
			TB_BM_OffsetX.Name = "TB_BM_OffsetX";
			TB_BM_OffsetX.Size = new Size ( 82, 23 );
			TB_BM_OffsetX.TabIndex = 1;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point ( 86, 9 );
			label1.Name = "label1";
			label1.Size = new Size ( 42, 15 );
			label1.TabIndex = 0;
			label1.Text = "Offset:";
			label1.TextAlign = ContentAlignment.TopRight;
			// 
			// tabPage3
			// 
			tabPage3.Controls.Add ( TB_VO_CamRotZ );
			tabPage3.Controls.Add ( TB_VO_CamRotY );
			tabPage3.Controls.Add ( TB_VO_CamRotX );
			tabPage3.Controls.Add ( label13 );
			tabPage3.Controls.Add ( TB_VO_CamPosZ );
			tabPage3.Controls.Add ( TB_VO_CamPosY );
			tabPage3.Controls.Add ( TB_VO_CamPosX );
			tabPage3.Controls.Add ( label14 );
			tabPage3.Controls.Add ( GB_ProjMode );
			tabPage3.Controls.Add ( TB_VO_ProjZ1 );
			tabPage3.Controls.Add ( TB_VO_ProjZ0 );
			tabPage3.Controls.Add ( label10 );
			tabPage3.Controls.Add ( TB_VO_ProjY1 );
			tabPage3.Controls.Add ( TB_VO_ProjY0 );
			tabPage3.Controls.Add ( label9 );
			tabPage3.Controls.Add ( TB_VO_ProjX1 );
			tabPage3.Controls.Add ( TB_VO_ProjX0 );
			tabPage3.Controls.Add ( label8 );
			tabPage3.Controls.Add ( TB_VO_ModelSize_Z );
			tabPage3.Controls.Add ( TB_VO_ModelSize_Y );
			tabPage3.Controls.Add ( TB_VO_ModelSize_X );
			tabPage3.Controls.Add ( label6 );
			tabPage3.Controls.Add ( TB_VO_DimsZ );
			tabPage3.Controls.Add ( TB_VO_ModelBase_Z );
			tabPage3.Controls.Add ( TB_VO_DimsY );
			tabPage3.Controls.Add ( TB_VO_ModelBase_Y );
			tabPage3.Controls.Add ( TB_VO_VoxelSize );
			tabPage3.Controls.Add ( label12 );
			tabPage3.Controls.Add ( TB_VO_DimsX );
			tabPage3.Controls.Add ( label11 );
			tabPage3.Controls.Add ( TB_VO_ModelBase_X );
			tabPage3.Controls.Add ( label5 );
			tabPage3.Location = new Point ( 4, 24 );
			tabPage3.Name = "tabPage3";
			tabPage3.Size = new Size ( 406, 781 );
			tabPage3.TabIndex = 2;
			tabPage3.Text = "Voxelizer";
			tabPage3.UseVisualStyleBackColor = true;
			// 
			// TB_VO_CamRotZ
			// 
			TB_VO_CamRotZ.Location = new Point ( 310, 207 );
			TB_VO_CamRotZ.Name = "TB_VO_CamRotZ";
			TB_VO_CamRotZ.Size = new Size ( 82, 23 );
			TB_VO_CamRotZ.TabIndex = 28;
			// 
			// TB_VO_CamRotY
			// 
			TB_VO_CamRotY.Location = new Point ( 222, 207 );
			TB_VO_CamRotY.Name = "TB_VO_CamRotY";
			TB_VO_CamRotY.Size = new Size ( 82, 23 );
			TB_VO_CamRotY.TabIndex = 28;
			// 
			// TB_VO_CamRotX
			// 
			TB_VO_CamRotX.Location = new Point ( 134, 207 );
			TB_VO_CamRotX.Name = "TB_VO_CamRotX";
			TB_VO_CamRotX.Size = new Size ( 82, 23 );
			TB_VO_CamRotX.TabIndex = 27;
			// 
			// label13
			// 
			label13.AutoSize = true;
			label13.Location = new Point ( 32, 210 );
			label13.Name = "label13";
			label13.Size = new Size ( 96, 15 );
			label13.TabIndex = 26;
			label13.Text = "Camera rotation:";
			label13.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_VO_CamPosZ
			// 
			TB_VO_CamPosZ.Location = new Point ( 310, 178 );
			TB_VO_CamPosZ.Name = "TB_VO_CamPosZ";
			TB_VO_CamPosZ.Size = new Size ( 82, 23 );
			TB_VO_CamPosZ.TabIndex = 25;
			// 
			// TB_VO_CamPosY
			// 
			TB_VO_CamPosY.Location = new Point ( 222, 178 );
			TB_VO_CamPosY.Name = "TB_VO_CamPosY";
			TB_VO_CamPosY.Size = new Size ( 82, 23 );
			TB_VO_CamPosY.TabIndex = 24;
			// 
			// TB_VO_CamPosX
			// 
			TB_VO_CamPosX.Location = new Point ( 134, 178 );
			TB_VO_CamPosX.Name = "TB_VO_CamPosX";
			TB_VO_CamPosX.Size = new Size ( 82, 23 );
			TB_VO_CamPosX.TabIndex = 23;
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Location = new Point ( 31, 181 );
			label14.Name = "label14";
			label14.Size = new Size ( 97, 15 );
			label14.TabIndex = 22;
			label14.Text = "Camera position:";
			label14.TextAlign = ContentAlignment.TopRight;
			// 
			// GB_ProjMode
			// 
			GB_ProjMode.Controls.Add ( RB_PM_Persp );
			GB_ProjMode.Controls.Add ( RB_PM_Ortho );
			GB_ProjMode.Location = new Point ( 3, 330 );
			GB_ProjMode.Name = "GB_ProjMode";
			GB_ProjMode.Size = new Size ( 400, 56 );
			GB_ProjMode.TabIndex = 21;
			GB_ProjMode.TabStop = false;
			GB_ProjMode.Text = "Projection Matrix Mode";
			// 
			// RB_PM_Persp
			// 
			RB_PM_Persp.AutoSize = true;
			RB_PM_Persp.Location = new Point ( 108, 22 );
			RB_PM_Persp.Name = "RB_PM_Persp";
			RB_PM_Persp.Size = new Size ( 85, 19 );
			RB_PM_Persp.TabIndex = 22;
			RB_PM_Persp.TabStop = true;
			RB_PM_Persp.Text = "Perspective";
			RB_PM_Persp.UseVisualStyleBackColor = true;
			// 
			// RB_PM_Ortho
			// 
			RB_PM_Ortho.AutoSize = true;
			RB_PM_Ortho.Location = new Point ( 6, 22 );
			RB_PM_Ortho.Name = "RB_PM_Ortho";
			RB_PM_Ortho.Size = new Size ( 96, 19 );
			RB_PM_Ortho.TabIndex = 22;
			RB_PM_Ortho.TabStop = true;
			RB_PM_Ortho.Text = "Orthographic";
			RB_PM_Ortho.UseVisualStyleBackColor = true;
			// 
			// TB_VO_ProjZ1
			// 
			TB_VO_ProjZ1.Location = new Point ( 222, 137 );
			TB_VO_ProjZ1.Name = "TB_VO_ProjZ1";
			TB_VO_ProjZ1.Size = new Size ( 82, 23 );
			TB_VO_ProjZ1.TabIndex = 20;
			// 
			// TB_VO_ProjZ0
			// 
			TB_VO_ProjZ0.Location = new Point ( 134, 137 );
			TB_VO_ProjZ0.Name = "TB_VO_ProjZ0";
			TB_VO_ProjZ0.Size = new Size ( 82, 23 );
			TB_VO_ProjZ0.TabIndex = 19;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new Point ( 54, 140 );
			label10.Name = "label10";
			label10.Size = new Size ( 74, 15 );
			label10.TabIndex = 18;
			label10.Text = "Projection Z:";
			label10.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_VO_ProjY1
			// 
			TB_VO_ProjY1.Location = new Point ( 222, 108 );
			TB_VO_ProjY1.Name = "TB_VO_ProjY1";
			TB_VO_ProjY1.Size = new Size ( 82, 23 );
			TB_VO_ProjY1.TabIndex = 17;
			// 
			// TB_VO_ProjY0
			// 
			TB_VO_ProjY0.Location = new Point ( 134, 108 );
			TB_VO_ProjY0.Name = "TB_VO_ProjY0";
			TB_VO_ProjY0.Size = new Size ( 82, 23 );
			TB_VO_ProjY0.TabIndex = 16;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new Point ( 54, 111 );
			label9.Name = "label9";
			label9.Size = new Size ( 74, 15 );
			label9.TabIndex = 15;
			label9.Text = "Projection Y:";
			label9.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_VO_ProjX1
			// 
			TB_VO_ProjX1.Location = new Point ( 222, 79 );
			TB_VO_ProjX1.Name = "TB_VO_ProjX1";
			TB_VO_ProjX1.Size = new Size ( 82, 23 );
			TB_VO_ProjX1.TabIndex = 14;
			// 
			// TB_VO_ProjX0
			// 
			TB_VO_ProjX0.Location = new Point ( 134, 79 );
			TB_VO_ProjX0.Name = "TB_VO_ProjX0";
			TB_VO_ProjX0.Size = new Size ( 82, 23 );
			TB_VO_ProjX0.TabIndex = 13;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new Point ( 54, 82 );
			label8.Name = "label8";
			label8.Size = new Size ( 74, 15 );
			label8.TabIndex = 12;
			label8.Text = "Projection X:";
			label8.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_VO_ModelSize_Z
			// 
			TB_VO_ModelSize_Z.Location = new Point ( 310, 35 );
			TB_VO_ModelSize_Z.Name = "TB_VO_ModelSize_Z";
			TB_VO_ModelSize_Z.Size = new Size ( 82, 23 );
			TB_VO_ModelSize_Z.TabIndex = 11;
			// 
			// TB_VO_ModelSize_Y
			// 
			TB_VO_ModelSize_Y.Location = new Point ( 222, 35 );
			TB_VO_ModelSize_Y.Name = "TB_VO_ModelSize_Y";
			TB_VO_ModelSize_Y.Size = new Size ( 82, 23 );
			TB_VO_ModelSize_Y.TabIndex = 10;
			// 
			// TB_VO_ModelSize_X
			// 
			TB_VO_ModelSize_X.Location = new Point ( 134, 35 );
			TB_VO_ModelSize_X.Name = "TB_VO_ModelSize_X";
			TB_VO_ModelSize_X.Size = new Size ( 82, 23 );
			TB_VO_ModelSize_X.TabIndex = 9;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point ( 57, 38 );
			label6.Name = "label6";
			label6.Size = new Size ( 67, 15 );
			label6.TabIndex = 8;
			label6.Text = "Model Size:";
			label6.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_VO_DimsZ
			// 
			TB_VO_DimsZ.Location = new Point ( 310, 256 );
			TB_VO_DimsZ.Name = "TB_VO_DimsZ";
			TB_VO_DimsZ.Size = new Size ( 82, 23 );
			TB_VO_DimsZ.TabIndex = 7;
			// 
			// TB_VO_ModelBase_Z
			// 
			TB_VO_ModelBase_Z.Location = new Point ( 310, 6 );
			TB_VO_ModelBase_Z.Name = "TB_VO_ModelBase_Z";
			TB_VO_ModelBase_Z.Size = new Size ( 82, 23 );
			TB_VO_ModelBase_Z.TabIndex = 7;
			// 
			// TB_VO_DimsY
			// 
			TB_VO_DimsY.Location = new Point ( 222, 256 );
			TB_VO_DimsY.Name = "TB_VO_DimsY";
			TB_VO_DimsY.Size = new Size ( 82, 23 );
			TB_VO_DimsY.TabIndex = 6;
			// 
			// TB_VO_ModelBase_Y
			// 
			TB_VO_ModelBase_Y.Location = new Point ( 222, 6 );
			TB_VO_ModelBase_Y.Name = "TB_VO_ModelBase_Y";
			TB_VO_ModelBase_Y.Size = new Size ( 82, 23 );
			TB_VO_ModelBase_Y.TabIndex = 6;
			// 
			// TB_VO_VoxelSize
			// 
			TB_VO_VoxelSize.Location = new Point ( 134, 285 );
			TB_VO_VoxelSize.Name = "TB_VO_VoxelSize";
			TB_VO_VoxelSize.Size = new Size ( 82, 23 );
			TB_VO_VoxelSize.TabIndex = 5;
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.Location = new Point ( 68, 288 );
			label12.Name = "label12";
			label12.Size = new Size ( 60, 15 );
			label12.TabIndex = 4;
			label12.Text = "Voxel Size:";
			label12.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_VO_DimsX
			// 
			TB_VO_DimsX.Location = new Point ( 134, 256 );
			TB_VO_DimsX.Name = "TB_VO_DimsX";
			TB_VO_DimsX.Size = new Size ( 82, 23 );
			TB_VO_DimsX.TabIndex = 5;
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Location = new Point ( 19, 259 );
			label11.Name = "label11";
			label11.Size = new Size ( 109, 15 );
			label11.TabIndex = 4;
			label11.Text = "Model Dimensions:";
			label11.TextAlign = ContentAlignment.TopRight;
			// 
			// TB_VO_ModelBase_X
			// 
			TB_VO_ModelBase_X.Location = new Point ( 134, 6 );
			TB_VO_ModelBase_X.Name = "TB_VO_ModelBase_X";
			TB_VO_ModelBase_X.Size = new Size ( 82, 23 );
			TB_VO_ModelBase_X.TabIndex = 5;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point ( 57, 9 );
			label5.Name = "label5";
			label5.Size = new Size ( 71, 15 );
			label5.TabIndex = 4;
			label5.Text = "Model Base:";
			label5.TextAlign = ContentAlignment.TopRight;
			// 
			// tabPage4
			// 
			tabPage4.Controls.Add ( CB_VV_Transform );
			tabPage4.Controls.Add ( label21 );
			tabPage4.Controls.Add ( TB_VV_Scale );
			tabPage4.Controls.Add ( label20 );
			tabPage4.Location = new Point ( 4, 24 );
			tabPage4.Name = "tabPage4";
			tabPage4.Size = new Size ( 406, 781 );
			tabPage4.TabIndex = 3;
			tabPage4.Text = "VoxelVievew";
			tabPage4.UseVisualStyleBackColor = true;
			// 
			// BTN_Pause
			// 
			BTN_Pause.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			BTN_Pause.Location = new Point ( 327, 826 );
			BTN_Pause.Name = "BTN_Pause";
			BTN_Pause.Size = new Size ( 75, 23 );
			BTN_Pause.TabIndex = 1;
			BTN_Pause.Text = "Pause Sync";
			BTN_Pause.UseVisualStyleBackColor = true;
			BTN_Pause.Click += BTN_Pause_Click;
			// 
			// BTN_Apply
			// 
			BTN_Apply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			BTN_Apply.Location = new Point ( 246, 826 );
			BTN_Apply.Name = "BTN_Apply";
			BTN_Apply.Size = new Size ( 75, 23 );
			BTN_Apply.TabIndex = 1;
			BTN_Apply.Text = "Apply";
			BTN_Apply.UseVisualStyleBackColor = true;
			BTN_Apply.Visible = false;
			BTN_Apply.Click += BTN_Apply_Click;
			// 
			// LBL_StatusMsg
			// 
			LBL_StatusMsg.AutoSize = true;
			LBL_StatusMsg.Location = new Point ( 12, 830 );
			LBL_StatusMsg.Name = "LBL_StatusMsg";
			LBL_StatusMsg.Size = new Size ( 88, 15 );
			LBL_StatusMsg.TabIndex = 2;
			LBL_StatusMsg.Text = "Status Message";
			// 
			// TB_VV_Scale
			// 
			TB_VV_Scale.Location = new Point ( 134, 6 );
			TB_VV_Scale.Name = "TB_VV_Scale";
			TB_VV_Scale.Size = new Size ( 82, 23 );
			TB_VV_Scale.TabIndex = 3;
			// 
			// label20
			// 
			label20.AutoSize = true;
			label20.Location = new Point ( 91, 9 );
			label20.Name = "label20";
			label20.Size = new Size ( 37, 15 );
			label20.TabIndex = 2;
			label20.Text = "Scale:";
			label20.TextAlign = ContentAlignment.TopRight;
			// 
			// label21
			// 
			label21.AutoSize = true;
			label21.Location = new Point ( 84, 52 );
			label21.Name = "label21";
			label21.Size = new Size ( 44, 15 );
			label21.TabIndex = 4;
			label21.Text = "Should";
			label21.TextAlign = ContentAlignment.TopRight;
			// 
			// CB_VV_Transform
			// 
			CB_VV_Transform.AutoSize = true;
			CB_VV_Transform.Location = new Point ( 134, 51 );
			CB_VV_Transform.Name = "CB_VV_Transform";
			CB_VV_Transform.Size = new Size ( 80, 19 );
			CB_VV_Transform.TabIndex = 5;
			CB_VV_Transform.Text = "Transform";
			CB_VV_Transform.UseVisualStyleBackColor = true;
			// 
			// ViewerForm
			// 
			AutoScaleDimensions = new SizeF ( 7F, 15F );
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size ( 414, 861 );
			Controls.Add ( LBL_StatusMsg );
			Controls.Add ( BTN_Apply );
			Controls.Add ( BTN_Pause );
			Controls.Add ( ShaderSelector );
			Name = "ViewerForm";
			Text = "Blade Vibration Viewer";
			ShaderSelector.ResumeLayout ( false );
			tabPage1.ResumeLayout ( false );
			tabPage1.PerformLayout ();
			tabPage2.ResumeLayout ( false );
			tabPage2.PerformLayout ();
			GB_BM_Materials.ResumeLayout ( false );
			GB_BM_Materials.PerformLayout ();
			tabPage3.ResumeLayout ( false );
			tabPage3.PerformLayout ();
			GB_ProjMode.ResumeLayout ( false );
			GB_ProjMode.PerformLayout ();
			tabPage4.ResumeLayout ( false );
			tabPage4.PerformLayout ();
			ResumeLayout ( false );
			PerformLayout ();
		}

		#endregion

		private System.Windows.Forms.Timer UpdateTimer;
		private TabControl ShaderSelector;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private TabPage tabPage3;
		private TabPage tabPage4;
		private TextBox TB_BM_Scale;
		private TextBox TB_BM_OffsetZ;
		private TextBox TB_BM_OffsetY;
		private TextBox TB_BM_OffsetX;
		private Label label1;
		private Label label2;
		private TextBox TB_BM_CamRotY;
		private TextBox TB_BM_CamRotX;
		private Label label4;
		private TextBox TB_BM_CamPosZ;
		private TextBox TB_BM_CamPosY;
		private TextBox TB_BM_CamPosX;
		private Label label3;
		private TextBox TB_VO_ModelSize_Z;
		private TextBox TB_VO_ModelSize_Y;
		private TextBox TB_VO_ModelSize_X;
		private Label label6;
		private TextBox TB_VO_ModelBase_Z;
		private TextBox TB_VO_ModelBase_Y;
		private TextBox TB_VO_ModelBase_X;
		private Label label5;
		private TextBox TB_OV_WinPosY;
		private TextBox TB_OV_WinPosX;
		private Label label7;
		private TextBox TB_VO_ProjZ1;
		private TextBox TB_VO_ProjZ0;
		private Label label10;
		private TextBox TB_VO_ProjY1;
		private TextBox TB_VO_ProjY0;
		private Label label9;
		private TextBox TB_VO_ProjX1;
		private TextBox TB_VO_ProjX0;
		private Label label8;
		private TextBox TB_VO_DimsY;
		private TextBox TB_VO_VoxelSize;
		private Label label12;
		private TextBox TB_VO_DimsX;
		private Label label11;
		private TextBox TB_VO_DimsZ;
		private Button BTN_Pause;
		private Button BTN_Apply;
		private Label LBL_StatusMsg;
		private GroupBox GB_ProjMode;
		private RadioButton RB_PM_Ortho;
		private RadioButton RB_PM_Persp;
		private TextBox TB_VO_CamRotY;
		private TextBox TB_VO_CamRotX;
		private Label label13;
		private TextBox TB_VO_CamPosZ;
		private TextBox TB_VO_CamPosY;
		private TextBox TB_VO_CamPosX;
		private Label label14;
		private TextBox TB_VO_CamRotZ;
		private GroupBox GB_BM_Materials;
		private Label label18;
		private Label label17;
		private Label label16;
		private MaterialInfo[] MaterialEntries;
		private TextBox TB_BM_MaxCut_Z;
		private TextBox TB_BM_MinCut_Z;
		private TextBox TB_BM_MaxCut_Y;
		private TextBox TB_BM_MinCut_Y;
		private TextBox TB_BM_MaxCut_X;
		private Label label19;
		private TextBox TB_BM_MinCut_X;
		private Label label15;
		private TextBox textBox2;
		private Label label21;
		private TextBox TB_VV_Scale;
		private Label label20;
		private CheckBox CB_VV_Transform;
	}

	public struct MaterialInfo {
		public Label MatLabel;
		public ComboBox RenderMode;
		public TextBox Shininess;
		public TextBox Diffuse;

		public void Set ( int id, Point pos, Control owner ) {
			MatLabel = new () {
				AutoSize = true,
				Location = new Point ( pos.X, pos.Y + 3 ),
				Name = "label" + (19 + id).ToString (),
				Size = new Size ( 47, 15 ),
				TabIndex = 0,
				Text = "Mat #" + id.ToString () + ":",
			};
			Shininess = new () {
				Location = new Point ( pos.X + 139, pos.Y ),
				Name = "TB_BM_Mat" + id.ToString () + "_Shine",
				Size = new Size ( 80, 23 ),
				TabIndex = 0,
			};
			Diffuse = new () {
				Location = new Point ( pos.X + 225, pos.Y ),
				Name = "TB_BM_Mat" + id.ToString () + "_Diffuse",
				Size = new Size ( 80, 23 ),
				TabIndex = 0,
			};
			RenderMode = new () {
				FormattingEnabled = true,
				Items = { "Simple", "Faces", "Phong", "Objects" },
				Location = new Point ( pos.X + 53, pos.Y ),
				Name = "CB_BM_Mat" + id.ToString () + "_Render",
				Size = new Size ( 80, 23 ),
				TabIndex = 0,
			};
			owner.Controls.Add ( MatLabel );
			owner.Controls.Add ( Shininess );
			owner.Controls.Add ( Diffuse );
			owner.Controls.Add ( RenderMode );
		}
	}
}