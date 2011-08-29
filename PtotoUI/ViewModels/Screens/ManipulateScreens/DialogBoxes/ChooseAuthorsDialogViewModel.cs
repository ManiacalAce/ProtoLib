using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using ProtoBLL;
using ProtoBLL.BusinessEntities;
using ProtoBLL.BusinessInterfaces;
using ProtoUI.DialogFacility;

namespace ProtoUI.ViewModels.Screens.ManipulateScreens.DialogBoxes
{
	/// <summary>
	/// Description of ChooseAuthorDialog.
	/// </summary>
	public class ChooseAuthorsDialogViewModel : DialogBaseViewModel
	{
		public ChooseAuthorsDialogViewModel(Action<object> doneHandler, IAuthorsManager authMgr)
			:base(doneHandler)
		{
			_authManager = authMgr;
			
			SelectedAuthorsList = new List<AuthorSummary>();
		}
		
		#region Bindable fields
		
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
				
				RefreshList();
				
				base.OnPropertyChanged("FilterText");
			}
		}
		
		public List<AuthorSummary> FilterList
		{
			get
			{
				return _filterList;
			}
			
			private set
			{
				if (_filterList == value)
					return;
				
				_filterList = value;
				base.OnPropertyChanged("FilterList");
			}
		}
		
		public List<AuthorSummary> SelectedAuthorsList
		{
			get
			{
				return _selectedAuthorsList;
			}
			
			private set
			{
				if (_selectedAuthorsList == value)
					return;
				
				_selectedAuthorsList = value;
				base.OnPropertyChanged("SelectedAuthorsList");
			}
		}
		
		public int CurrentCandidateAuthorIndex
		{
			get
			{
				return _currentCandidateAuthor;
			}
			set
			{
				if (_currentCandidateAuthor == value)
					return;
				
				_currentCandidateAuthor = value;
				base.OnPropertyChanged("CurrentCandidateAuthorIndex");
			}
		}
		
		public int CurrentSelectedAuthorIndex
		{
			get
			{
				return _currentSelectedAuthor;
			}
			set
			{
				if (_currentSelectedAuthor == value)
					return;
				
				_currentSelectedAuthor = value;
				base.OnPropertyChanged("CurrentSelectedAuthorIndex");
			}
		}
		
		
		public string EnteredAuthorId
		{
			get
			{
				return _enteredAuthorId;
			}
			set
			{
				if (_enteredAuthorId == value)
					return;
				
				_enteredAuthorId = value;
				base.OnPropertyChanged("EnteredAuthorId");
			}
		}
		
		
		#endregion //Bindable fields
		
		#region Commands
		
		public ICommand SelectAuthorCommand
		{
			get
			{
				if (_selectAuthorCmd == null)
					_selectAuthorCmd = new RelayCommand((o) =>
					                                    {
					                                    	AuthorSummary a = (from auth in SelectedAuthorsList
					                                    		where auth.ID == FilterList[CurrentCandidateAuthorIndex].ID
					                                    		select auth).SingleOrDefault();
					                                    	
					                                    	if (a == null)
					                                    	{
					                                    		SelectedAuthorsList.Add(FilterList[CurrentCandidateAuthorIndex]);
					                                    		
					                                    		//HACK: Creating a new list just to update view
					                                    		List<AuthorSummary> nl = new List<AuthorSummary>(SelectedAuthorsList);
					                                    		SelectedAuthorsList = nl;
					                                    	}
					                                    }
					                                   );
				
				return _selectAuthorCmd;
			}
		}
		
		public ICommand AddAuthorByIdCommand
		{
			get
			{
				if (_addAuthorByIdCmd == null)
					_addAuthorByIdCmd = new RelayCommand((o) =>
					                                     {
					                                     	int id = 0;
					                                     	if (int.TryParse(EnteredAuthorId, out id))
					                                     	{
					                                     		AuthorBLL a = _authManager.GetByID(id);
					                                     		if (a != null && a.ItemID != 0)
					                                     		{
					                                     			AuthorSummary asumm = new AuthorSummary(a);
					                                     			var dupe = (from auth in SelectedAuthorsList where
					                                     			            auth.ID == asumm.ID select auth).FirstOrDefault();
					                                     			if (dupe == null)
					                                     			{
					                                     				SelectedAuthorsList.Add(asumm);
					                                     				List<AuthorSummary> nl = new List<AuthorSummary>(SelectedAuthorsList);
					                                     				SelectedAuthorsList = nl;
					                                     			}
					                                     		}
					                                     		
					                                     	}
					                                     }
					                                    );
				return _addAuthorByIdCmd;
			}
		}
		
		public ICommand UnSelectAuthorCommand
		{
			get
			{
				if (_unSelectAuthorCmd == null)
					_unSelectAuthorCmd = new RelayCommand((o) =>
					                                      {
					                                      	AuthorSummary a = (from auth in SelectedAuthorsList
					                                      	                   where auth.ID == SelectedAuthorsList[CurrentSelectedAuthorIndex].ID
					                                      	                   select auth).FirstOrDefault();
					                                      	
					                                      	if (a != null )
					                                      	{
					                                      		SelectedAuthorsList.Remove(a);
					                                      		List<AuthorSummary> nl = new List<AuthorSummary>(SelectedAuthorsList);
					                                      		SelectedAuthorsList = nl;
					                                      	}
					                                      }
					                                     );
				return _unSelectAuthorCmd;
			}
		}
		
		RelayCommand _unSelectAuthorCmd;
		
		
		
		#endregion //Commands
		
		
		protected override void OkFunc(object o)
		{
			//List<int> finalList = new List<int>(from sa in _selectedAuthorsList select sa.ID);
			base.InvokeDoneEvent(SelectedAuthorsList);
            base.OkFunc(SelectedAuthorsList);
		}
		
		#region Private helpers
		
		private void RefreshList()
		{
			if (!string.IsNullOrWhiteSpace(FilterText))
			{
				List<AuthorBLL> l = _authManager.GetByName(FilterText, 50);
				FilterList = new List<AuthorSummary>();
				
				foreach (AuthorBLL a in l)
					FilterList.Add(new AuthorSummary(a));
				
			}
		}		
		
		#endregion //Private helpers
		
		#region Fields
		
		List<AuthorSummary> _selectedAuthorsList;
		int _currentCandidateAuthor;
		int _currentSelectedAuthor;
		
		
		string _filterText;
		List<AuthorSummary> _filterList;
		string _enteredAuthorId;
		
		
		IAuthorsManager _authManager;
		
//		RelayCommand _filterCmd;
		RelayCommand _selectAuthorCmd;
		RelayCommand _addAuthorByIdCmd;
		
		
		
		#endregion //Fields
		
		#region Nested types

        public class AuthorSummary
        {
            public AuthorSummary(AuthorBLL author)
            {
                ID = author.ItemID;

                Name = author.FirstName + " " + author.MiddleName +
                    " " + author.LastName;

                if (author.DateOfBirth.HasValue)
                {
                    TimeSpan ts = DateTime.Now - (DateTime)author.DateOfBirth;
                    Age = ts.Days / 365;
                }

                Nationality = author.Nationality;

            }

            public int ID
            {
                get;
                private set;
            }

            public string Name
            {
                get;
                private set;
            }

            public int? Age
            {
                get;
                private set;
            }

            public string Nationality
            {
                get;
                private set;
            }


        }
		
		#endregion //Nested types
	}
}
