using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;

namespace ProtoUI.DialogFacility
{
	/// <summary>
	/// Description of DialogHoster.
	/// </summary>
	public class DialogHoster : INotifyPropertyChanged
	{
		public DialogHoster()
		{
			_isParentEnabled = true;
		}
		
		public void ShowDialog(DialogBaseViewModel dialog)
		{
			ActiveDialog = dialog;
			ActiveDialog.RequestClose +=  CloseDialog;
		}
		
		private void CloseDialog(object o, EventArgs e)
		{
			ActiveDialog.RequestClose -= CloseDialog;
			ActiveDialog = null;
		}
		
		public bool IsParentEnabled
		{
			get
			{
				return _isParentEnabled;
			}
			
			set
			{
				if (_isParentEnabled == value)
					return;
				
				_isParentEnabled = value;
				OnPropertyChanged("IsParentEnabled");
			}
		}
				
		public DialogBaseViewModel ActiveDialog
		{
			get
			{
				return _activeDialog;
			}
			private set
			{
				if (_activeDialog == value)
					return;
				
				_activeDialog = value;
				OnPropertyChanged("ActiveDialog");
			}
		}
		
		DialogBaseViewModel _activeDialog;
		bool _isParentEnabled;
		
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

		
	}
}
