/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ProtoBLL;

namespace ProtoUI.DialogFacility.GroupSelectionDialog
{
	/// <summary>
	/// Description of GroupSelectionDialogViewModel.
	/// </summary>
	public class GroupSelectionDialogViewModel<TEntity, TManager> : DialogBaseViewModel where TEntity : new()
	{
		public GroupSelectionDialogViewModel(Action<object> doneHandler, 
		                                     FilterFunction filter)
			:base(doneHandler)
		{
			_filterFunc += what;
		}
		
		public delegate List<string> FilterFunction(TEntity entity, TManager manager,
		                                            string text);
		
		private List<string> what(TEntity e, TManager m, string s)
		{
			TEntity x = new TEntity();
			
			return new List<string>();
		}
		

		public string FilterText
		{
			get
			{
				return _filterText;
			}
			
			set
			{
				if (_filterText == value)
					return;
				
				_filterText = value;
				
				RefreshList(value);
				
				base.OnPropertyChanged("FilterText");
			}
		}
		
		public List<string> FilterList
		{
			get
			{
				return _filterList;
			}
			
			private set
			{
				_filterList = value;
				base.OnPropertyChanged("FilterList");
			}
		}
		
		
		#region Filter command
		
		public ICommand FilterCommand
		{
			get
			{
				if (_filterCmd == null)
					_filterCmd = new RelayCommand(RefreshList);
				
				return _filterCmd;
			}
		}
		
		private void RefreshList(object o)
		{
			if (_filterFunc != null);
				_filterList = _filterFunc(
		}
		
		#endregion //Filter command
		
		#region Private helpers
		
		
		
		#endregion //Private helpers
		
		#region Fields
		
		string _filterText;
		List<string> _filterList;
		
		FilterFunction _filterFunc;
		
		
		RelayCommand _filterCmd;
		
		#endregion //Fields		
	}
}
*/