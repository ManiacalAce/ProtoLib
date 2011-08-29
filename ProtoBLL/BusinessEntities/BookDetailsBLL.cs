using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;
using ProtoBLL.BusinessInterfaces;

namespace ProtoBLL.BusinessEntities
{
	/// <summary>
	/// Description of BookDetailsBLL.
	/// </summary>
	public class BookDetailsBLL : IKeyedItem, IDataErrorInfo
	{
		public BookDetailsBLL()
		{
			_description = new BookDetailsBLL.BookDescription();
			_publishInfo = new BookDetailsBLL.BookPublishInfo();
			_dimensions = new BookDimensions();
			
			AuthorIDs = new List<int>();
			
		}
		
		#region Public properties
		
		public int ItemID
		{
			get;
			internal set;
		}
		
		
		public BookDescription Description
		{
			get
			{
				return _description;
			}
		}
		

		public BookPublishInfo PublishInfo
		{
			get
			{				
				return _publishInfo;
			}
		}
		
		public BookDimensions Dimensions
		{
			get
			{
				return _dimensions;
			}

		}
		
	
		
		public List<int> AuthorIDs
		{
			get;
			set;
		}
		

		
		public List<int> SubjectIDs
		{
			get
			{
				if (_subjectIDs == null)
					_subjectIDs = new List<int>();
				
				return _subjectIDs;
			}
		}
		
		public List<int> CategoryIDs
		{
			get
			{
				if (_categoryIDs == null)
					_categoryIDs = new List<int>();
				
				return _categoryIDs;
			}
		}
		
		#endregion //Public propertiess

		
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
				//return null;
				
				if (!this.IsValid)
					return string.Format("BookInfoBLL item(ID:{0}) is in an invalid state", ItemID.ToString());
				else
					return null;
			}
		}
		
		#endregion		
		
		
		#region Validation
		
		public bool IsValid
		{
			get
			{
				foreach (string property in ValidatedProperties)
					if (GetValidationError(property) != null)
						return false;
				
				return true;
			}
		}
		
		static readonly string[] ValidatedProperties = 
		{
			"Description.Title",
			"Description.TitleLong",
			"Description.Summary",
			"Description.Notes",
			"Description.Language",
			"Description.AmazonLink",
			
			"PublishInfo.DatePublished",
			"PublishInfo.ISBN13",
			"PublishInfo.ISBN10",
			"PublishInfo.EditionNumber",
			"PublishInfo.EditionName",
			"PublishInfo.Printing",
			
			"Dimensions.Pages",
			"Dimensions.Height",
			"Dimensions.Width",
			"Dimensions.Thickness",
			"Dimensions.Weight",
			
			"AuthorIDs"
		};
		
		string GetValidationError(string propertyName)
		{
			
			
			string error = null;
			
			//eg. propertyName == "Description.Title"
			//splitName[0] == Description
			//splitName[1] == Title
			string[] splitName = propertyName.Split('.');			
			
			
			if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
				return null;
			
			
			switch (splitName[0]) 
			{

				case "Description":
					error = (_description as IDataErrorInfo)[splitName[1]];
					break;
					
				case "PublishInfo":
					error = (_publishInfo as IDataErrorInfo)[splitName[1]];
					break;
					
				case "Dimensions":
					error = (_dimensions as IDataErrorInfo)[splitName[1]];
					break;
					
			}
			
			return error;
		}
		
		#endregion
		
		
		#region Fields
		
		BookDescription _description;
		BookPublishInfo _publishInfo;
		BookDimensions _dimensions;

		
		List<int> _categoryIDs;
		List<int> _subjectIDs;
		
		#endregion
		
		
		#region Nested types
		
		public class BookDimensions : IDataErrorInfo
		{
			public int? Pages
			{
				get;
				set;
			}
			
			public double? Weight
			{
				get;
				set;
			}
			
			public double? Height
			{
				get;
				set;			
			}
			
			public double? Width
			{
				get;
				set;
			}
			
			public double? Thickness
			{
				get;
				set;
			}		
			
			#region IDataErrorInfo members
			
	        string IDataErrorInfo.this[string propertyName]
	        {
	            get { return this.GetValidationError(propertyName); }
	        }	
			
			string IDataErrorInfo.Error 
			{
				get 
				{
					return null;
				}
			}
			
			#endregion
			
			#region Validation
			
	        public bool IsValid
	        {
	        	
	            get
	            {
	            	
	                foreach (string property in ValidatedProperties)
	                    if (GetValidationError(property) != null)
	                        return false;
	
	                return true;
	            }
	        }
	
	        static readonly string[] ValidatedProperties = 
	        { 
	            "Pages", 
	            "Weight", 
	            "Height",
	            "Width",
	            "Thickness"
	        };
	
	        string GetValidationError(string propertyName)
	        {
	            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
	                return null;
	
	            string error = null;	

	            switch (propertyName)
	            {
	            	case "Pages":
	            		error = this.ValidateDimension((int?)Pages, "Pages");
	            		break;
	            		
	            	case "Weight":
	            		error = this.ValidateDimension(Weight, "Weight");
	            		break;
	            		
	            	case "Height":
	            		error = this.ValidateDimension(Height, "Height");
	            		break;
	            		
	            	case "Width":
	            		error = this.ValidateDimension(Width, "Width");
	            		break;
	            		
	            	case "Thickness":
	            		error = this.ValidateDimension(Thickness, "Thickness");
	            		break;
	            }
	            
	            return error;
	        }
	        

	        
	        string ValidateDimension(double? dim, string propName)
	        {
	        	if (dim != null && IsNegative(dim))
	        		return propName + " must be a positive number";
	        	
	        	return null;
	        }
	        
	        bool IsNegative(double? n)
	        {
	        	if (n != null)
	        		return n < 0 ? true : false;
	        	else
	        		return false;
	        }
	        
			#endregion
			
		}		
		
		public class BookDescription : IDataErrorInfo
		{
			public string Title
			{
				get;
				set;
			}
			
			public string TitleLong
			{
				get;
				set;
			}
			
			
			public string Summary
			{
				get;
				set;
			}
			
			public string Notes
			{
				get;
				set;
			}	
			
			public string CoverImage
			{
				get;
				set;
			}
	
			public string Language
			{
				get;
				set;
			}			
			
			public string AmazonLink
			{
				get;
				set;
			}		


			#region IDataErrorInfo members
			
	        string IDataErrorInfo.this[string propertyName]
	        {
	            get { return this.GetValidationError(propertyName); }
	        }	
			
			string IDataErrorInfo.Error 
			{
				get 
				{
					return null;
				}
			}
			
			#endregion
			
			#region Validation
			
	        public bool IsValid
	        {
	            get
	            {
	                foreach (string property in ValidatedProperties)
	                    if (GetValidationError(property) != null)
	                        return false;
	
	                return true;
	            }
	        }
	
	        static readonly string[] ValidatedProperties = 
	        { 
	            "Title", 
	            "TitleLong", 
	            "Summary",
	            "Notes",
	            "Language",
	            "AmazonLink",
	            "CoverImage"
	        };
	
	        string GetValidationError(string propertyName)
	        {
	            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
	                return null;
	
	            string error = null;	

	            switch (propertyName)
	            {
	            	case "Title": 
	            		error = this.ValidateTitle();
	            		break;
	            		
	            	case "TitleLong":
	            		error = this.ValidateStringLength("Long title", TitleLong, 1000);
	            		break;
	            		
	            	case "Language":
	            		error = this.ValidateStringLength("Language",Language, 300);
	            		break;
	            		
	            	case "AmazonLink":
	            		error = this.ValidateStringLength("The amazon link", AmazonLink,
	            		                                  500);
	            		break;
	            		
	            	case "CoverImage":
	            		//TODO: Do something for cover image?
	            		break;
	            }
	            
	            return error;
	        }
	        

	        
	        string ValidateTitle()
	        {
	        	if (string.IsNullOrWhiteSpace(Title))
	        		return "The title can't be empty";
	        	else 
	        		return ValidateStringLength("Title", Title, 500);
	        	
	        }
	        
	        string ValidateStringLength(string field, string value, int maxLength)
	        {
	        	if (value != null && value.Length > maxLength)
	        		return string.Format("{0} can't have more than {1} characters!",
	        		                     field, maxLength.ToString());
	        	
	        	return null;
	        }
	        
			#endregion			

			#region Fields
			/*
			string _title;
			string _titleLong;
			string _summary;
			string _notes;			
			string _coverImage;
			string _language;
			string _amazonLink;
			*/
			#endregion
		}
		
		public class BookPublishInfo : IDataErrorInfo
		{
			
			public string ISBN13
			{
				get;
				set;
			}
			
			public string ISBN10
			{
				get;
				set;
			}				
			
			public int? PublisherID
			{
				get;
				set;
			}			
			
			public int? EditionNumber
			{
				get;
				set;
			}
			
			public string EditionName
			{
				get;
				set;
			}
			
			public int? Printing
			{
				get;
				set;
			}
			
			public DateTime? DatePublished
			{
				get;
				set;
			}
			
			#region IDataErrorInfo members
			
	        string IDataErrorInfo.this[string propertyName]
	        {
	            get { return this.GetValidationError(propertyName); }
	        }	
			
			string IDataErrorInfo.Error 
			{
				get 
				{
					return null;
				}
			}
			
			#endregion
			
			#region Validation
			
	        public bool IsValid
	        {
	            get
	            {
	                foreach (string property in ValidatedProperties)
	                    if (GetValidationError(property) != null)
	                        return false;
	
	                return true;
	            }
	        }
	
	        static readonly string[] ValidatedProperties = 
	        { 
	            "ISBN13", 
	            "ISBN10", 
	            "PublisherID",
	            "EditionNumber",
	            "EditionName",
	            "Printing",
	            "DatePublished"
	        };
	
	        string GetValidationError(string propertyName)
	        {
	            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
	                return null;
	
	            string error = null;	

	            switch (propertyName)
	            {
	            	case "ISBN13":
	            		error = this.ValidateIsbn13();
	            		break;
	            		
	            	case "ISBN10":
	            		error = this.ValidateIsbn10();
	            		break;
	            		
	            	case "PublisherID":
	            		error = this.ValidatePublisherID();
	            		break;
	            		
	            	case "EditionNumber":
	            		error = this.ValidateEditionNumber();
	            		break;
	            		
	            	case "EditionName":
	            		error = this.ValidateEditionName();
	            		break;
	            		
	            	case "Printing":
	            		error = this.ValidatePrinting();
	            		break;
	            		
	            	case "DatePublished":
	            		error = this.ValidateDatePublished();
	            		break;
	            }
	            
	            return error;
	        }
	        

	        
	        string ValidateIsbn13()
	        {
	        	if (string.IsNullOrWhiteSpace(ISBN13) && string.IsNullOrWhiteSpace(ISBN10))
	        		return null;
	        	else if (!string.IsNullOrWhiteSpace(ISBN13) && String.IsNullOrWhiteSpace(ISBN10))
	        		return null;
	        	else if(String.IsNullOrWhiteSpace(ISBN13) && !String.IsNullOrWhiteSpace(ISBN10))
	        		return "ISBN13 can't be empty if ISBN10 is present!";
	        	else
	        		return ValidateStringLength("ISBN13", ISBN13, 15);
	        	
	        }
	        
	        string ValidateIsbn10()
	        {
	        	if (string.IsNullOrWhiteSpace(ISBN13) && string.IsNullOrWhiteSpace(ISBN10))
	        		return null;
	        	else if (!string.IsNullOrWhiteSpace(ISBN13) && String.IsNullOrWhiteSpace(ISBN10))
	        		return null;
	        	else if(String.IsNullOrWhiteSpace(ISBN13) && !String.IsNullOrWhiteSpace(ISBN10))
	        		return "ISBN13 can't be empty if ISBN10 is present!";
	        	else
	        		return ValidateStringLength("ISBN10", ISBN10, 15);
	        }
	        
	        string ValidatePublisherID()
	        {
	        	return null;
	        }
	        
	        string ValidateEditionNumber()
	        {
	        	if (EditionNumber != null && IsNegative(EditionNumber))
	        		return "Edition number can't be negative!";
	        	
	        	return null;
	        }
	        
	        string ValidateEditionName()
	        {
	        	return ValidateStringLength("Edition name", EditionName, 500);
	        }
	        
	        string ValidatePrinting()
	        {
	        	if (Printing != null && IsNegative(Printing))
	        		return "Printing number can't be negative!";
	        	
	        	return null;
	        }
	        
	        string ValidateDatePublished()
	        {
	        	if (DatePublished != null && DateTime.Compare((DateTime)DatePublished,
	        	                                              DateTime.Now) > 0)
	        		return "The published date can't be in the future!";
	        	
	        	return null;
	        }
	        
	        string ValidateStringLength(string field, string value, int maxLength)
	        {
	        	if (value != null && value.Length > maxLength)
	        		return string.Format("{0} can't have more than {1} characters!",
	        		                     field, maxLength.ToString());
	        	
	        	return null;
	        }
	        
	        bool IsNegative(double? n)
	        {
	        	if (n != null)
	        		return n < 0 ? true : false;
	        	else
	        		return false;
	        }
	        
			#endregion						
			
			#region Fields
			/*
			string _isbn13;
			string _isbn10;
			string _editionName;	
			int? _publisherID;		
			int? _printing;
			int? _editionNumber;		
			DateTime? _datePublished;
			*/
			#endregion
		}
		
		#endregion
		
		
	}	
	

}
