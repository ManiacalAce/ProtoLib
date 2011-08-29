using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Input;

namespace ProtoUI.ViewModels
{
	/// <summary>
	/// abstract view model class that represents something that may be 'closed'
	/// </summary>
	public abstract class CloseableViewModel : ViewModelBase
	{
		public CloseableViewModel()
		{
			
		}
		
		public ICommand CloseCommand
		{
			get
			{
				if (_closeCommand == null)
					_closeCommand = new RelayCommand((param) => this.OnRequestClose());
				
				return _closeCommand;
			}
		}
		
		public event EventHandler RequestClose;
		
		void OnRequestClose()
		{
			EventHandler handler = this.RequestClose;
			if (handler != null)
				RequestClose(this, EventArgs.Empty);
		}
		
		
		RelayCommand _closeCommand;
	}
}
