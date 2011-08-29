using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ProtoBLL;
using ProtoBLL.BusinessEntities;

namespace ProtoUI.ViewModels
{
	public class MainWinViewModel : CloseableViewModel
	{		
		
		public MainWinViewModel(ProtoBridge bridge)
		{
			_bridge = bridge;
			
			//Create default workspace:
			this.CreateNewWorkspace();
		}
		
		#region New workspace creation
		
		public ICommand CreateNewWorkspaceCommand
		{
			get
			{
				if (_newWorkspaceCmd == null)
				{
					_newWorkspaceCmd = new RelayCommand((param) => this.CreateNewWorkspace());
				}
				return _newWorkspaceCmd;				
			}
		}
		
		void CreateNewWorkspace()
		{
			WorkspaceViewModel wvm = new WorkspaceViewModel(_bridge, _currentUser);
//			wvm.LoggedOut += PerformMassLogout; //added in OnWorkspacesChanged
			
			this.Workspaces.Add(wvm);
			this.SetActiveWorkspace(wvm);
		}
		
		void PerformMassLogout(object o)
		{
			_currentUser = null;
			Workspaces.Clear(); //HACK: ?? Clean up event handlers? OnWkspcChngd not being called
			
			CreateNewWorkspace();
		}
		
		void PerformLogin(object user)
		{
			if (user is StaffAccountBLL)
			{
				_currentUser = (StaffAccountBLL)user;
			}
		}
		
		void SetActiveWorkspace(WorkspaceViewModel wvm)
		{
			ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
			if (collectionView != null)
				collectionView.MoveCurrentTo(wvm); 
		}
		
		#endregion
		
		
		
		public ObservableCollection<WorkspaceViewModel> Workspaces
		{
			get
			{
				if (_workspaces == null)
				{
					_workspaces = new ObservableCollection<WorkspaceViewModel>();
					_workspaces.CollectionChanged += OnWorkspacesChanged;
				}
				
				return _workspaces;
			}
		}
		
		//Why not just add OnWorkspaceReqClose in CreateNewWorkspace()...and ReadonlyList<T>? To enable cleanup (removal of handlers)?
		void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null && e.NewItems.Count != 0)
				foreach (WorkspaceViewModel w in e.NewItems)
				{
					w.RequestClose += OnWorkspaceRequestClose;			
					w.LoggedOut += PerformMassLogout;
					w.LoggedIn += PerformLogin;
				}
			
			if (e.OldItems != null && e.OldItems.Count != 0)
				foreach (WorkspaceViewModel w in e.OldItems)
				{
					w.RequestClose -= OnWorkspaceRequestClose;			
					w.LoggedOut -= PerformMassLogout;
					w.LoggedIn -= PerformLogin;
//							Console.Beep();
					
				}
		}
		
		void OnWorkspaceRequestClose(object sender, EventArgs e)
		{
			WorkspaceViewModel workspace = sender as WorkspaceViewModel;
			if (workspace != null)
				this.Workspaces.Remove(workspace);
		}
		
		#region Fields
		
		ProtoBridge _bridge;
		ObservableCollection<WorkspaceViewModel> _workspaces; //In the demo, this was ObsvColl<CloseableVM
		RelayCommand _newWorkspaceCmd;
		
		StaffAccountBLL _currentUser;
		
		#endregion
	}
}