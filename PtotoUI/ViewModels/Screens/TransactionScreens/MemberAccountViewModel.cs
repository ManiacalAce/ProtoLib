
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ProtoBLL;
using ProtoBLL.BusinessEntities;

namespace ProtoUI.ViewModels.Screens.TransactionScreens
{
	/// <summary>
	/// Description of MemberAccountViewModel.
	/// </summary>
	public class MemberAccountViewModel : ScreenBaseViewModel
	{
		public MemberAccountViewModel(ProtoBridge bridge, StaffAccountBLL currUser)
			:base(LibraryScreens.TRANSACTIONS, bridge, currUser)
		{
			int chosenId = (int)Application.Current.Properties["EnteredMemId"];
			_chosenMem = bridge.MemberMgr.GetByID(chosenId);
			
			if (_chosenMem == null)
			{
				Console.Beep();
			}
			else
			{
				MemberID = _chosenMem.ItemID.ToString();
				MemberName = _chosenMem.FirstName + " " + _chosenMem.MiddleName + " " + _chosenMem.LastName;
				TimeSpan ts = DateTime.Now - _chosenMem.JoinDate;
				Age = (ts.Days/365).ToString();
				Address = _chosenMem.Contact.AddressLine1 + " " + _chosenMem.Contact.AddressLine2 + " " + _chosenMem.Contact.AddressLine3
					+ " " + _chosenMem.Contact.City + " " + _chosenMem.Contact.Pin + " " + _chosenMem.Contact.StateOrProvince + " " + _chosenMem.Contact.Country;
				Gender = _chosenMem.Gender;
				JoinDate = _chosenMem.JoinDate.ToString();
				
				
				List<TransactionBLL> transs = bridge.TransactionMgr.GetMemberTransactions(_chosenMem.ItemID);
				
				string err;
				bridge.TransactionMgr.IssueBook(120, chosenId, out err);
				if (err != null)
					MessageBox.Show(err);
				
				foreach (TransactionBLL t in transs)
				{
					if (t.ReturnedOn == null)
						TransHistory.Add(new TransactionDetails(bridge, t, true));
					else
						CurrentTrans.Add(new TransactionDetails(bridge, t, false));
				}
				
				
			}
		}
		
		
		#region Bindable props
		
		public string MemberName
		{
			get;
			private set;
		}
		
		public string Age
		{
			get;
			private set;
		}
		
		public string MemberID
		{
			get;
			private set;
		}
		
		public string Address
		{
			get;
			private set;
		}
		
		public string Gender
		{
			get;
			private set;
		}
		
		public string JoinDate
		{
			get;
			private set;
		}
		
		#endregion //Bindable props
		
		public ObservableCollection<TransactionDetails> CurrentTrans
		{
			get;
			set;
		}
		
		public ObservableCollection<TransactionDetails> TransHistory
		{
			get;
			set;
		}
		
		#region Fields
		
		MemberBLL _chosenMem;
		
		#endregion
		
		
		#region Nested types
		
		public class TransactionDetails
		{
			public TransactionDetails(ProtoBridge bridge, TransactionBLL trans, bool historyMode = false)
			{
				if (trans != null)
				{
					BookID = trans.BookID.ToString();
					
					LibraryBookBLL libBook = bridge.LibraryBookMgr.GetByID(trans.BookID);
					BookDetailsBLL bookInfo = bridge.BookDetailsMgr.GetByID(libBook.BookDetailsId);
					
					Title = bookInfo.Description.Title;
					List<AuthorBLL> auths = new List<AuthorBLL>();
					foreach(int aid in bookInfo.AuthorIDs)
						auths.Add(bridge.AuthorsMgr.GetByID(aid));
					
					foreach(AuthorBLL a in auths)
						Authors += (", " + a.FirstName + " " + a.LastName);
					
					Authors = Authors.Substring(1);
					
					IssueDate = trans.CheckedOutOn.ToLongDateString();
					
					if (trans.ReturnedOn == null)
						DueDate = null;
					else
						DueDate = ((DateTime)trans.ReturnedOn).ToLongDateString();
					
					int borrowLimit = (int)Application.Current.Properties["BorrowLimit"];
					int fineAmt = 2;
					
					if (historyMode == false)
					{
						if (trans.ReturnedOn == null)
							Status = "Checked out";
						else
						{
							TimeSpan ts = DateTime.Now - trans.CheckedOutOn;
							
							if (ts.Days > borrowLimit)
							{
								Status = "Overdue by " + ts.Days.ToString() + " days";
							}
							
						}
					}
					else
					{
						TimeSpan ts = (DateTime)trans.ReturnedOn - trans.CheckedOutOn;
						if (ts.Days > borrowLimit)
						{
							Status = ts.Days.ToString() + " days overdue. Fined Rs. " + (fineAmt * ts.Days).ToString() + ".";
							
						}
						else
						{
							Status = "Returned on time";
						}
					}
				}
			}
			
			public string BookID
			{
				get;
				set;
			}
			
			public string Title
			{
				get;
				set;
			}
			
			public string Authors
			{
				get;
				set;
			}
			
//			public string Publisher
//			{
//				get;
//				set;
//			}
			
			public string IssueDate
			{
				get;
				set;
			}
			
			public string DueDate
			{
				get;
				set;
			}
			
			public string Status
			{
				get;
				set;
			}
		}
		
		#endregion
	}
}
