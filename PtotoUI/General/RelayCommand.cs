using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;
using System.Windows.Input;

namespace ProtoUI
{
	/// <summary>
	/// Description of RelayCommand.
	/// </summary>
	public class RelayCommand : ICommand
	{
		#region Constructors
		public RelayCommand(Action<object> execute)
			:this(execute, null)
		{
		}
		
		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");
			
			_execute = execute;
			_canExecute = canExecute;
		}
		#endregion
		
		#region Fields
		readonly Action<object> _execute;
		readonly Predicate<object> _canExecute;
		#endregion  //Fields
		
		#region ICommand implementation
		public event EventHandler CanExecuteChanged
		{
			add {CommandManager.RequerySuggested += value;}
			remove {CommandManager.RequerySuggested -= value;}
		}
		
		public void Execute(object parameter)
		{
			_execute(parameter);
		}
		
		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}
		#endregion
	}
}
