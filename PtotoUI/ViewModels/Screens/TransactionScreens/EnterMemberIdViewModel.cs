
using System;
using System.Windows;
using System.Windows.Input;
using ProtoBLL;
using ProtoBLL.BusinessEntities;

namespace ProtoUI.ViewModels.Screens.TransactionScreens
{
	/// <summary>
	/// Description of EnterMemberIdViewModel.
	/// </summary>
	public class EnterMemberIdViewModel : ScreenBaseViewModel
	{
		public EnterMemberIdViewModel(ProtoBridge bridge, StaffAccountBLL currUser)
			:base(LibraryScreens.TRANSACTIONS_ENTERID, bridge, currUser)
		{
			BadInput += (o, e) => {};
		}
		
		public string IdText
		{
			get
			{
				return _idText;
			}
			
			set
			{
				if (_idText == value)
					return;
				
				_idText = value;
				base.OnPropertyChanged("IdText");
			}
		}
		
		public ICommand GoCommand
		{
			get
			{
				if (_goCmd == null)
					_goCmd = new RelayCommand(AcceptId);
				
				return _goCmd;
			}
		}
		
		public event EventHandler BadInput;
		
		private void AcceptId(object o)
		{
			int id = 0;
			if (int.TryParse(IdText, out id))
			{
				MemberBLL member = _Bridge.MemberMgr.GetByID(id);
				if (member != null)
				{
					Application.Current.Properties["EnteredMemId"] = id;
					FireScreenTransitionEvent(LibraryScreens.TRANSACTIONS);
				}
				else
					if (BadInput != null)
						BadInput(this, EventArgs.Empty);
				
			}
			else
				if (BadInput != null)
					BadInput(this, EventArgs.Empty);
			
		}
		
		
		string _idText;
		
		RelayCommand _goCmd;
	}
}
