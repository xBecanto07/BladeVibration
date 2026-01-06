using BladeVibrationCS;
using BladeVibrationCS.GpuPrograms;

namespace BladeVibrationControls;
public partial class ViewerForm : Form {
	readonly WindowsHolder GpuWindow;
	readonly Controler Controller;
	readonly RenderController RenderController;

	bool isUpdating = true;
	AShaderProgram lastLastProgram = null;

	public ViewerForm ( WindowsHolder window, Controler controler ) {
		InitializeComponent ();
		Controller = controler;
		GpuWindow = window;
		RenderController = window.renderController;
		LBL_StatusMsg.Text = "Status: Started";

		RB_PM_Ortho.Text = nameof ( AVoxelizer.ProjectionType.Orthographic );
		RB_PM_Persp.Text = nameof ( AVoxelizer.ProjectionType.Perspective );

		MaterialEntries = new MaterialInfo[MaterialHolder.BUFFER_SIZE];
		for ( int i = 0; i < MaterialHolder.BUFFER_SIZE; i++ ) {
			Point loc = new ( 3, 40 + i * 29 );
			MaterialEntries[i].Set ( i, loc, GB_BM_Materials );
		}
	}

	private void BTN_Pause_Click ( object sender, EventArgs e ) {
		isUpdating = !isUpdating;
		BTN_Apply.Enabled = !isUpdating; // Allow applying changes only when paused
		BTN_Apply.Visible = !isUpdating;
		LBL_StatusMsg.Text = isUpdating ? "Status: Running" : "Status: Paused";
		BTN_Pause.Text = isUpdating ? "Pause Sync" : "Resume Sync";
	}

	private void BTN_Apply_Click ( object sender, EventArgs e ) {
		LBL_StatusMsg.Text = "Status: Applied changes";
		GpuWindow.Location = GpuWindow.Location.ParseLoc ( TB_OV_WinPosX, TB_OV_WinPosY );
		GpuWindow.ModelOffset.Parse ( TB_BM_OffsetX, TB_BM_OffsetY, TB_BM_OffsetZ );
		GpuWindow.Camera.Position.Parse ( TB_BM_CamPosX, TB_BM_CamPosY, TB_BM_CamPosZ );
		GpuWindow.Camera.Rotation.Parse ( TB_BM_CamRotX, TB_BM_CamRotY );

		switch ( lastLastProgram ) {
		case BasicMeshRenderer basicMesh:
			basicMesh.MaterialHolder.Parse ( MaterialEntries );
			basicMesh.MaterialHolder.UpdateGPU ();
			basicMesh.MinBound.Parse ( TB_BM_MinCut_X, TB_BM_MinCut_Y, TB_BM_MinCut_Z );
			basicMesh.MaxBound.Parse ( TB_BM_MaxCut_X, TB_BM_MaxCut_Y, TB_BM_MaxCut_Z );
			break;
		case VoxelObject voxelizer:
			voxelizer.CamPos.Parse ( TB_VO_CamPosX, TB_VO_CamPosY, TB_VO_CamPosZ );
			voxelizer.CamDir.Parse ( TB_VO_CamRotX, TB_VO_CamRotY, TB_VO_CamRotZ );
			if ( RB_PM_Persp.Checked ) {
				voxelizer.LastProjectionPersp.Row0.Parse ( TB_VO_ProjX0, TB_VO_ProjX1 );
				voxelizer.LastProjectionPersp.Row1.Parse ( TB_VO_ProjY0, TB_VO_ProjY1 );
				voxelizer.LastProjectionPersp.Row2.Parse ( TB_VO_ProjZ0, TB_VO_ProjZ1 );
				voxelizer.CurrentProjMode = AVoxelizer.ProjectionType.Perspective;
			} else if ( RB_PM_Ortho.Checked ) {
				voxelizer.LastProjectionOrto.Row0.Parse ( TB_VO_ProjX0, TB_VO_ProjX1 );
				voxelizer.LastProjectionOrto.Row1.Parse ( TB_VO_ProjY0, TB_VO_ProjY1 );
				voxelizer.LastProjectionOrto.Row2.Parse ( TB_VO_ProjZ0, TB_VO_ProjZ1 );
				voxelizer.CurrentProjMode = AVoxelizer.ProjectionType.Orthographic;
			}
			break;
		case VoxelVisualizer voxelView:
			voxelView.ShouldTransform.Parse ( CB_VV_Transform );
			voxelView.Scale.Parse ( TB_VV_Scale );
			break;
		case PhysicsSimulator physicsSim:
			physicsSim.SwizzleIndex.Parse ( CB_PS_SwizzleIndex );
			physicsSim.SwizzleData.Parse ( CB_PS_SwizzleData );
			physicsSim.InvertAxis.Parse ( CB_PS_InvertAxis );
			physicsSim.Visualize.Parse ( TB_PS_Visualize );
			break;
		}
	}

	private void UpdateBMR ( BasicMeshRenderer bmr ) {
		TB_BM_Scale.Text = GpuWindow.scale.ToString ( "F4" );
		GpuWindow.ModelOffset.Fill ( TB_BM_OffsetX, TB_BM_OffsetY, TB_BM_OffsetZ );
		GpuWindow.Camera.Position.Fill ( TB_BM_CamPosX, TB_BM_CamPosY, TB_BM_CamPosZ );
		GpuWindow.Camera.Rotation.Fill ( TB_BM_CamRotX, TB_BM_CamRotY );
		GpuWindow.Camera.ApplyAngle ();

		bmr.MaterialHolder.Fill ( MaterialEntries );
		bmr.MinBound.Fill ( TB_BM_MinCut_X, TB_BM_MinCut_Y, TB_BM_MinCut_Z );
		bmr.MaxBound.Fill ( TB_BM_MaxCut_X, TB_BM_MaxCut_Y, TB_BM_MaxCut_Z );
	}

	private void UpdateVO ( VoxelObject vo ) {
		vo.BasePosition.Fill ( TB_VO_ModelBase_X, TB_VO_ModelBase_Y, TB_VO_ModelBase_Z );
		vo.Size.Fill ( TB_VO_ModelSize_X, TB_VO_ModelSize_Y, TB_VO_ModelSize_Z );

		TB_VO_DimsX.Text = vo.VoxX.ToString ();
		TB_VO_DimsY.Text = vo.VoxY.ToString ();
		TB_VO_DimsZ.Text = vo.VoxZ.ToString ();
		TB_VO_VoxelSize.Text = vo.VoxelSize.ToString ( "F4" );

		vo.CurrentProjMode.Select ( RB_PM_Ortho, RB_PM_Persp );
		vo.CamPos.Fill ( TB_VO_CamPosX, TB_VO_CamPosY, TB_VO_CamPosZ );
		vo.CamDir.Fill ( TB_VO_CamRotX, TB_VO_CamRotY, TB_VO_CamRotZ );

		switch ( vo.CurrentProjMode ) {
		case AVoxelizer.ProjectionType.Orthographic:
			vo.LastProjectionOrto.Row0.Fill ( TB_VO_ProjX0, TB_VO_ProjX1 );
			vo.LastProjectionOrto.Row1.Fill ( TB_VO_ProjY0, TB_VO_ProjY1 );
			vo.LastProjectionOrto.Row2.Fill ( TB_VO_ProjZ0, TB_VO_ProjZ1 );
			RB_PM_Ortho.Checked = true;
			break;
		case AVoxelizer.ProjectionType.Perspective:
			vo.LastProjectionPersp.Row0.Fill ( TB_VO_ProjX0, TB_VO_ProjX1 );
			vo.LastProjectionPersp.Row1.Fill ( TB_VO_ProjY0, TB_VO_ProjY1 );
			vo.LastProjectionPersp.Row2.Fill ( TB_VO_ProjZ0, TB_VO_ProjZ1 );
			RB_PM_Persp.Checked = true;
			break;
		}
	}

	private void UpdateTimer_Tick ( object sender, EventArgs e ) {
		if ( !isUpdating ) return;

		var gpuPos = EntryProgram.GPUWindowPosition;
		if ( !gpuPos.HasValue )
			return;

		Location = new Point ( gpuPos.Value.X, gpuPos.Value.Y );
		TB_OV_WinPosX.Text = GpuWindow.Location.X.ToString ();
		TB_OV_WinPosY.Text = GpuWindow.Location.Y.ToString ();

		AShaderProgram lastProgram = RenderController.LastRepeatableProgram;
		int newIndex = 0;
		switch ( lastProgram ) {
		case null:
			newIndex = 0;
			break;

		case BasicMeshRenderer basicMesh:
			newIndex = 1;
			UpdateBMR ( basicMesh );
			break;

		case VoxelObject voxelizer:
			newIndex = 2;
			UpdateVO ( voxelizer );
			break;
		case VoxelVisualizer voxelView:
			newIndex = 3;
			voxelView.ShouldTransform.Fill ( CB_VV_Transform );
			voxelView.Scale.Fill ( TB_VV_Scale );
			break;
		case PhysicsSimulator physicsSim:
			UpdateBMR ( physicsSim.BMR );
			physicsSim.SwizzleIndex.Fill ( CB_PS_SwizzleIndex );
			physicsSim.SwizzleData.Fill ( CB_PS_SwizzleData );
			physicsSim.InvertAxis.Fill ( CB_PS_InvertAxis );
			physicsSim.Visualize.Fill ( TB_PS_Visualize );
			newIndex = 4;
			break;
		}

		if ( lastLastProgram != lastProgram ) {
			lastLastProgram = lastProgram;
			ShaderSelector.SelectedIndex = newIndex;
		}
	}
}