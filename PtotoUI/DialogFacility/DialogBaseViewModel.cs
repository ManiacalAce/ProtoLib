using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using ProtoUI.ViewModels;

namespace ProtoUI.DialogFacility
{

	public abstract class DialogBaseViewModel : ViewModelBase
	{
		public DialogBaseViewModel(Action<object> doneHandler)
		{
			_okCmd = new RelayCommand(OkFunc);
			_cancelCmd = new RelayCommand(CancelFunc);
			_doneHandler = doneHandler;
		}
		
		public event EventHandler RequestClose;
		
		protected virtual void OkFunc(object o)
		{
			if (RequestClose != null)
				RequestClose(this, EventArgs.Empty);
		}
		
		protected virtual void CancelFunc(object o)
		{
			if (RequestClose != null)
				RequestClose(this, EventArgs.Empty);
		}
		
		protected void InvokeDoneEvent(object param)
		{
			if (_doneHandler != null)
				_doneHandler(param);
		}
		
		public ICommand OkCommand
		{
			get
			{
				return _okCmd;
			}
		}
		
		public ICommand CancelCommand
		{
			get
			{
				return _cancelCmd;
			}
		}
		
		RelayCommand _okCmd;
		RelayCommand _cancelCmd;
		Action<object> _doneHandler;
	}
}
