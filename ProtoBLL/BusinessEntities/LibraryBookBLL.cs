using System;
using System.ComponentModel;
using System.Diagnostics;
using ProtoBLL.BusinessInterfaces;
using ProtoBLL.General;


namespace ProtoBLL.BusinessEntities
{
	/// <summary>
	/// Description of LibraryBookBLL.
	/// </summary>

	public class LibraryBookBLL : IKeyedItem, IDataErrorInfo
	{
		public LibraryBookBLL()
		{
			Status = BookStatusBLL.ON_SHELF;
			
		}
		
		#region Public properties
		
		public int ItemID
		{
			get;
			internal set;
		}

		public int? Price
		{
			get;
			set;
		}		
		public string ObtainedFrom
		{
			get;
			set;
		}		
		
		public BookStatusBLL Status
		{
			get;
			set;
		}
		
		public int BookDetailsId
		{
			get;
			set;
		}
		
		#endregion //Public properties
		
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
				if (!this.IsValid)
					return string.Format("LibraryBookBLL item(ID:{0}) is in an invalid state", ItemID.ToString());
				else
					return null;
			}
		}	
		
		#endregion //IDataErrorInfo members
		
		#region Validation
		
		static readonly string[] ValidatedProperties = 
		{
			"Price",
			"ObtainedFrom"
			
		};
		
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
		
		string GetValidationError(string propertyName)
		{
			if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
				return null;
			
			string error = null;
			
			
			switch (propertyName)
			{


			case "Price":
				error = this.ValidatePrice();
				break;


			case "ObtainedFrom":
				error = this.ValidateObtainedFrom();
				break;


                default:
                    Debug.Fail("Unexpected property being validated on Customer: " + propertyName);
                    break;					
			}
			
			return error;			
		}
		
		#region Validation helper methods
		

		private string ValidatePrice()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateObtainedFrom()
		{
			string err = null;
			
			
			return err;
		}
		

		
		#endregion //Validation helper methods
		
		#endregion //Validation
	}
	

}
