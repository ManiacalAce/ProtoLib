using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using ProtoBLL;
using ProtoBLL.BusinessEntities;
using ProtoBLL.EntityManagers;
using ProtoUI.General.XamlDeficiencyHacks;
using ProtoUI.ViewModels.General;
using ProtoUI.DialogFacility;
using ProtoUI.ViewModels.Screens.ManipulateScreens.DialogBoxes;
using ProtoUI.Views.Screens.ManipulateScreens.DialogBoxes;

namespace ProtoUI.ViewModels.Screens.ManipulateScreens
{
	/// <summary>
	/// Description of ManipBookDetailsViewModel.
	/// </summary>
	public class ManipBookDetailsViewModel : ScreenBaseViewModel, IDataErrorInfo
	{
		public ManipBookDetailsViewModel(ProtoBridge bridge, StaffAccountBLL currUser)
			:base(LibraryScreens.MANIPULATE_RECORDS, bridge, currUser)
		{			
			_internalSet = false;
			
			_dialogHoster = new DialogHoster();
//			_chooseAuthDiagBox = new ChooseAuthorsDialogViewModel

            _selectedAuthors = new List<ChooseAuthorsDialogViewModel.AuthorSummary>();

			
			ManipulateBar = new ManipulateBarViewModel<BookDetailsBLL>(_Bridge.BookDetailsMgr, OnCurrentItemChanged);
			OnCurrentItemChanged();

		}
		
		private void OnCurrentItemChanged()
		{
			//_internalSet is set to true when the View's fields are updated from
			//automatically from within the class (without user intervention).
			//When it is false, the 'set' method of each field will dirty the
			//current record so the database may be updated later (on calling Save).
			_internalSet = true;
			
			ItemID = ManipulateBar.CurrentItem.ItemID.ToString();
			
			Title = ManipulateBar.CurrentItem.Description.Title;
			TitleLong = ManipulateBar.CurrentItem.Description.TitleLong;
			Summary = ManipulateBar.CurrentItem.Description.Summary;
			Notes = ManipulateBar.CurrentItem.Description.Notes;
			Language = ManipulateBar.CurrentItem.Description.Language;
			AmazonLink = ManipulateBar.CurrentItem.Description.AmazonLink;
			
			ISBN13 = ManipulateBar.CurrentItem.PublishInfo.ISBN13;
			ISBN10 = ManipulateBar.CurrentItem.PublishInfo.ISBN10;
			EditionName = ManipulateBar.CurrentItem.PublishInfo.EditionName;
			EditionNumber = ManipulateBar.CurrentItem.PublishInfo.EditionNumber.HasValue?ManipulateBar.CurrentItem.PublishInfo.EditionNumber.ToString():null;
			Printing = ManipulateBar.CurrentItem.PublishInfo.Printing.HasValue?ManipulateBar.CurrentItem.PublishInfo.Printing.ToString():null;
			DatePublished = ManipulateBar.CurrentItem.PublishInfo.DatePublished.HasValue?ManipulateBar.CurrentItem.PublishInfo.DatePublished.ToString():null;
			
			Pages = ManipulateBar.CurrentItem.Dimensions.Pages.HasValue?ManipulateBar.CurrentItem.Dimensions.Pages.ToString():null;
			Height = ManipulateBar.CurrentItem.Dimensions.Height.HasValue?ManipulateBar.CurrentItem.Dimensions.Height.ToString():null;
			Width = ManipulateBar.CurrentItem.Dimensions.Width.HasValue?ManipulateBar.CurrentItem.Dimensions.Width.ToString():null;
			Thickness = ManipulateBar.CurrentItem.Dimensions.Thickness.HasValue?ManipulateBar.CurrentItem.Dimensions.Thickness.ToString():null;
			Weight = ManipulateBar.CurrentItem.Dimensions.Weight.HasValue?ManipulateBar.CurrentItem.Dimensions.Weight.ToString():null;
		
			List<ChooseAuthorsDialogViewModel.AuthorSummary> lst = new List<ChooseAuthorsDialogViewModel.AuthorSummary>();
			foreach (int id in ManipulateBar.CurrentItem.AuthorIDs)
			{
				AuthorBLL a = _Bridge.AuthorsMgr.GetByID(id);
				if (a != null)
					lst.Add(new ChooseAuthorsDialogViewModel.AuthorSummary(a));
			}
			SelectedAuthors = lst;
		
			_internalSet = false;
		}
		
		
		
		#region Choose author stuff
		
		public ICommand ChooseAuthorsCommand
		{
			get
			{
				if (_chooseAuthorsCmd == null)
					_chooseAuthorsCmd = new RelayCommand(ShowAuthorDialog);
				
				return _chooseAuthorsCmd;
			}
		}
		
		private void ShowAuthorDialog(object o)
		{
            _chooseAuthDiagBox = new ChooseAuthorsDialogViewModel((oo) => { SelectedAuthors = (List<ChooseAuthorsDialogViewModel.AuthorSummary>)oo; }, _Bridge.AuthorsMgr);
			DialogHost.ShowDialog(_chooseAuthDiagBox);
		}
		
		#endregion // Choose author stuff
		
		public DialogHoster DialogHost
		{
			get
			{
				if (_dialogHoster == null)
					_dialogHoster = new DialogHoster();
				
				return _dialogHoster;
			}
		}
		
		
		//ALT: Have a different class that exposes NextCmd, PrevCmd etc, and THAT
		//class can have an instance exposed publically to VM, while a simpler class
		//with CurrentItem is maintained privately. CurrentItem, then, won't be
		//publically exposed
		public ManipulateBarViewModel<BookDetailsBLL> ManipulateBar
		{
			get {return _manipBar;}
			private set
			{
				_manipBar = value;
				base.OnPropertyChanged("ManipulateBar");
			}
		}

        public List<ChooseAuthorsDialogViewModel.AuthorSummary> SelectedAuthors
        {
            get
            {
                return _selectedAuthors;
            }

            set
            {
                if (_selectedAuthors == value)
                    return;

                _selectedAuthors = value;
                DirtyRecordIfPossible();
                
                base.OnPropertyChanged("SelectedAuthors");
            }
        }

		
		#region BookDetails text fields
		
		public string ItemID
		{
			get
			{
				return _manipBar.CurrentItem.ItemID.ToString();
			}
			private set
			{
				if (_id == value)
					return;
				
				_id = value;
				
				base.OnPropertyChanged("ItemID");
			}
		}

		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				if (_title == value)
					return;			
		
				_title = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Title");
			}
		}
		
		public string TitleLong
		{
			get
			{
				return _titleLong;
			}
			set
			{
				if (_titleLong == value)
					return;
		
				_titleLong = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("TitleLong");
			}
		}
		
		public string Summary
		{
			get
			{
				return _summary;
			}
			set
			{
				if (_summary == value)
					return;
		
				_summary = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Summary");
			}
		}
		
		public string Notes
		{
			get
			{
				return _notes;
			}
			set
			{
				if (_notes == value)
					return;
		
				_notes = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Notes");
			}
		}
		
		public string AmazonLink
		{
			get
			{
				return _amazonLink;
			}
			set
			{
				if (_amazonLink == value)
					return;
		
				_amazonLink = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("AmazonLink");
			}
		}
		
		public string ISBN13
		{
			get
			{
				return _isbn13;
			}
			set
			{
				if (_isbn13 == value)
					return;
		
				_isbn13 = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("ISBN13");
			}
		}
		
		public string ISBN10
		{
			get
			{
				return _isbn10;
			}
			set
			{
				if (_isbn10 == value)
					return;
		
				_isbn10 = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("ISBN10");
			}
		}
		
		public string EditionNumber
		{
			get
			{
				return _editionNumber;
			}
			set
			{
				if (_editionNumber == value)
					return;
		
				_editionNumber = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("EditionNumber");
			}
		}
		
		public string EditionName
		{
			get
			{
				return _editionName;
			}
			set
			{
				if (_editionName == value)
					return;
		
				_editionName = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("EditionName");
			}
		}
		
		public string Printing
		{
			get
			{
				return _printing;
			}
			set
			{
				if (_printing == value)
					return;
		
				_printing = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Printing");
			}
		}
		
		public string Language
		{
			get
			{
				return _language;
			}
			set
			{
				if (_language == value)
					return;
		
				_language = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Language");
			}
		}
		
		public string DatePublished
		{
			get
			{
				return _datePublished;
			}
			set
			{
				if (_datePublished == value)
					return;
		
				_datePublished = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("DatePublished");
			}
		}
		
		public string Pages
		{
			get
			{
				return _pages;
			}
			set
			{
				if (_pages == value)
					return;
		
				_pages = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Pages");
			}
		}
		
		public string Height
		{
			get
			{
				return _height;
			}
			set
			{
				if (_height == value)
					return;
		
				_height = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Height");
			}
		}
		
		public string Width
		{
			get
			{
				return _width;
			}
			set
			{
				if (_width == value)
					return;
		
				_width = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Width");
			}
		}
		
		public string Thickness
		{
			get
			{
				return _thickness;
			}
			set
			{
				if (_thickness == value)
					return;
		
				_thickness = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Thickness");
			}
		}
		
		public string Weight
		{
			get
			{
				return _weight;
			}
			set
			{
				if (_weight == value)
					return;
		
				_weight = value;
				
				DirtyRecordIfPossible();
				
		
				base.OnPropertyChanged("Weight");
			}
		}
		

		
		#endregion What is
		
		#region Private helpers
		
		void DirtyRecordIfPossible()
		{
			if (_internalSet == false)
				_manipBar.DirtyCurrentRecord();
		}
		
		#endregion
		
		
		#region IDataErrorInfo members
		
		string IDataErrorInfo.this[string propertyName]
		{
			get 
			{
				return this.GetValidationError(propertyName);
			}
		}
		
		string IDataErrorInfo.Error 
		{
			get 
			{
				string err = (ManipulateBar.CurrentItem as IDataErrorInfo).Error;
				
				CommandManager.InvalidateRequerySuggested();
				
				return err;
			}
		}

		#endregion
		

		#region ViewModel Validtion
		

		//Slight weirdness here. On initial load, redundant work is done.
		//Load values from DB and store them in VM properties (OnCurrentItemChanged()).
		//Then, the View, via the binding system, will call this method during
		//validation. Here, the VM properties are checked initially for string-based 
		//correctness. Then this.ManipulateBar.CurrentItem.<propHolder>.<prop> = value;
		//is done -- a re-entry of value into the BLL object.
		
		//Uh oh... this might prevent me from easily implementing a Save feature.
		//The redundant entry will unconditionally dirty the record...
		//Thinking about removing the BLL-obj assignment.
		public string GetValidationError(string propertyName)
		{
			string err = null;
			
			switch(propertyName)
			{
		
				case "Title":
					string title;
					err = this.ValidateTitle(out title);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Description.Title = title;
					break;
					
		
				case "TitleLong":
					string titlelong;
					err = this.ValidateTitleLong(out titlelong);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Description.TitleLong = titlelong;
					break;
					
		
				case "Summary":
					string summary;
					err = this.ValidateSummary(out summary);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Description.Summary = summary;
					break;
					
		
				case "Notes":
					string notes;
					err = this.ValidateNotes(out notes);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Description.Notes = notes;
					break;
					
		
				case "AmazonLink":
					string amazonlink;
					err = this.ValidateAmazonLink(out amazonlink);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Description.AmazonLink = amazonlink;
					break;
					
		
				case "ISBN13":
					string isbn13;
					err = this.ValidateISBN13(out isbn13);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.PublishInfo.ISBN13 = isbn13;
					break;
					
		
				case "ISBN10":
					string isbn10;
					err = this.ValidateISBN10(out isbn10);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.PublishInfo.ISBN10 = isbn10;
					break;
					
		
				case "EditionNumber":
					int ? editionnumber;
					err = this.ValidateEditionNumber(out editionnumber);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.PublishInfo.EditionNumber = editionnumber;
					break;
					
		
				case "EditionName":
					string editionname;
					err = this.ValidateEditionName(out editionname);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.PublishInfo.EditionName = editionname;
					break;
					
		
				case "Printing":
					int? printing;
					err = this.ValidatePrinting(out printing);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.PublishInfo.Printing = printing;
					break;
					
		
				case "Language":
					string language;
					err = this.ValidateLanguage(out language);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Description.Language = language;
					break;
					
		
				case "DatePublished":
					DateTime? datepublished;
					err = this.ValidateDatePublished(out datepublished);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.PublishInfo.DatePublished = datepublished;
					break;
					
		
				case "Pages":
					int? pages;
					err = this.ValidatePages(out pages);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Dimensions.Pages = pages;
					break;
					
		
				case "Height":
					double? height;
					err = this.ValidateHeight(out height);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Dimensions.Height = height;
					break;
					
		
				case "Width":
					double? width;
					err = this.ValidateWidth(out width);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Dimensions.Width = width;
					break;
					
		
				case "Thickness":
					double? thickness;
					err = this.ValidateThickness(out thickness);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Dimensions.Thickness = thickness;
					break;
					
		
				case "Weight":
					double? weight;
					err = this.ValidateWeight(out weight);
					if (!string.IsNullOrEmpty(err))
					    return err;
					
					this.ManipulateBar.CurrentItem.Dimensions.Weight = weight;
					break;
					
					
				case "SelectedAuthors":
					this.ManipulateBar.CurrentItem.AuthorIDs = (from aSumm in SelectedAuthors select aSumm.ID).ToList();
					break;
					
			}
			
			return err = (this.ManipulateBar.CurrentItem as IDataErrorInfo)[_propertyNameResolver[propertyName]];
				
		}


		#region Validation helper methods
		
		private string ValidateTitle(out string title)
		{			
			string err = null;
			if (_title == null)
			{
				err = "The title can't be empty!";
				title = null;
			}
			else
				title = _title;
			
			return err;
		}
		
		private string ValidateTitleLong(out string titlelong)
		{
			string err = null;
			//NO VM-SIDE VALIDATION!! If there was any, such as checking
			//length of string, do that here. If fails, return some weird value
			//to enter into CurrentItem as a placeholder.
			titlelong = _titleLong;
			
			return err;
		}
		
		private string ValidateSummary(out string summary)
		{
			string err = null;
			summary = _summary;
			
			return err;
		}
		
		private string ValidateNotes(out string notes)
		{
			string err = null;
			notes = _notes;
			
			return err;
		}
		
		private string ValidateAmazonLink(out string amazonlink)
		{
			string err = null;
			amazonlink = _amazonLink;
			
			return err;
		}
		
		private string ValidateISBN13(out string isbn13)
		{
			string err = null;
			isbn13 = _isbn13;
			
			return err;
		}
		
		private string ValidateISBN10(out string isbn10)
		{
			string err = null;
			isbn10 = _isbn10;
			
			return err;
		}
		
		private string ValidateEditionNumber(out int? editionnumber)
		{
			string err = null;
			
			if (_editionNumber == null)
			{
				editionnumber = null;
				return err;
			}
			else
			{
				int temp;
				if (int.TryParse(_editionNumber, out temp))
					editionnumber = temp;
				else
				{
					err = "Edition number must be a whole number";
					editionnumber = -1;
				}
			}
			
			return err;
		}
		
		private string ValidateEditionName(out string editionname)
		{
			string err = null;
			editionname = _editionName;
			
			return err;
		}
		
		private string ValidatePrinting(out int? printing)
		{
			string err = null;
			
			if (_printing == null)
			{
				printing = null;
				return err;
			}
			else
			{
				int temp;
				if (int.TryParse(_printing, out temp))
					printing = temp;
				else
				{
					err = "Edition number must be a whole number";
					printing = -1;
				}
			}
			
			return err;
		}
		
		private string ValidateLanguage(out string language)
		{
			string err = null;
			language = _language;
			
			return err;
		}
		
		private string ValidateDatePublished(out DateTime? datepublished)
		{
			string err = null;
			if (_datePublished == null)
			{
				datepublished = null;
				return err;
			}
			else
			{
				DateTime temp;
				if (DateTime.TryParse(_datePublished, out temp))
					datepublished = temp;
				else
				{
					err = "Please enter a valid date";
					datepublished = null;
				}
			}		
			
			return err;
		}
		
		private string ValidatePages(out int? pages)
		{
			string err = null;
			
			if (_pages == null)
			{
				pages = null;
				return err;
			}
			else
			{
				int temp;
				if (int.TryParse(_pages, out temp))
					pages = temp;
				else
				{
					err = "The number of pages must be a whole number";
					pages = -1;
				}
			}		
			return err;
		}
		
		private string ValidateHeight(out double? height)
		{
			string err = null;
			
			if (_height == null)
			{
				height = null;
				return err;
			}
			else
			{
				double temp;
				if (double.TryParse(_height, out temp))
					height = temp;
				else
				{
					err = "The height must be a valid decimal number.";
					height = -1;
				}
			}			
			
			return err;
		}
		
		private string ValidateWidth(out double? width)
		{
			string err = null;
			
			if (_width == null)
			{
				width = null;
				return err;
			}
			else
			{
				double temp;
				if (double.TryParse(_width, out temp))
					width = temp;
				else
				{
					err = "The width must be a valid decimal number.";
					width = -1;
				}
			}			
			
			return err;
		}
		
		private string ValidateThickness(out double? thickness)
		{
			string err = null;
			
			if (_thickness == null)
			{
				thickness = null;
				return err;
			}
			else
			{
				double temp;
				if (double.TryParse(_thickness, out temp))
					thickness = temp;
				else
				{
					err = "The thickness must be a valid decimal number.";
					thickness = -1;
				}
			}			
			
			return err;
		}
		
		private string ValidateWeight(out double? weight)
		{
			string err = null;
			
			if (_weight == null)
			{
				weight = null;
				return err;
			}
			else
			{
				double temp;
				if (double.TryParse(_weight, out temp))
					weight = temp;
				else
				{
					err = "Edition number must be a whole number";
					weight = -1;
				}
			}			
			
			return err;
		}

		#endregion //Validation helper methods
		
		#endregion
		
		#region Fields
		
		#region Static fields
		static Dictionary<string, string> _propertyNameResolver = 
			new Dictionary<string, string>()
		{
			{"Title", "Description.Title"},
			{"TitleLong", "Description.TitleLong"},
			{"Summary", "Description.Summary"},
			{"Notes", "Description.Notes"},
			{"Language", "Description.Language"},
			{"AmazonLink", "Description.AmazonLink"},
			
			{"DatePublished", "PublishInfo.DatePublished"},
			{"ISBN13", "PublishInfo.ISBN13"},
			{"ISBN10", "PublishInfo.ISBN10"},
			{"EditionNumber", "PublishInfo.EditionNumber"},
			{"EditionName", "PublishInfo.EditionName"},
			{"Printing", "PublishInfo.Printing"},
			
			{"Pages", "Dimensions.Pages"},
			{"Height", "Dimensions.Height"},
			{"Width", "Dimensions.Width"},
			{"Thickness", "Dimensions.Thickness"},
			{"Weight", "Dimensions.Weight"},
			{"SelectedAuthors", "AuthorIDs"}
		};
		#endregion //Static fields
		
		bool _internalSet;
		
		ManipulateBarViewModel<BookDetailsBLL> _manipBar;
		
		DialogHoster _dialogHoster;
		ChooseAuthorsDialogViewModel _chooseAuthDiagBox;
		
		RelayCommand _chooseAuthorsCmd;
        List<ChooseAuthorsDialogViewModel.AuthorSummary> _selectedAuthors;
		
		
		string _id;
		
		string _title;
		string _titleLong;
		string _summary;
		string _notes;
		string _amazonLink;
		string _language;
		
		string _isbn13;
		string _isbn10;
		string _editionNumber;
		string _editionName;
		string _printing;
		string _datePublished;
		
		string _pages;
		string _height;
		string _width;
		string _thickness;
		string _weight;
		
		#endregion
	
	}
}
