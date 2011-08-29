using System;
using System.ComponentModel;
using System.Diagnostics;
using ProtoBLL.BusinessInterfaces;

namespace ProtoBLL.BusinessEntities
{
	/// <summary>
	/// Description of MemberBLL.
	/// </summary>

	public class MemberBLL : IKeyedItem, IDataErrorInfo
	{
		public MemberBLL()
		{
			Contact = new ContactBLL();
		}
		
		#region Public properties
		
		public int ItemID
		{
			get;
			internal set;
		}

		public string FirstName
		{
			get;
			set;
		}		
		public string MiddleName
		{
			get;
			set;
		}		
		public string LastName
		{
			get;
			set;
		}		
		public DateTime? DateOfBirth
		{
			get;
			set;
		}		
		public string Gender
		{
			get;
			set;
		}		
		public DateTime JoinDate
		{
			get;
			set;
		}		
		public string Portrait
		{
			get;
			set;
		}		
		
		
		public ContactBLL Contact
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
					return string.Format("MemberBLL item(ID:{0}) is in an invalid state", ItemID.ToString());
				else
					return null;
			}
		}	
		
		#endregion //IDataErrorInfo members
		
		#region Validation
		
		static readonly string[] ValidatedProperties = 
		{
			"FirstName",
			"MiddleName",
			"LastName",
			"DateOfBirth",
			"Gender",
			"JoinDate",
			"Portrait",
			
			"Contact"
			
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


			case "FirstName":
				error = this.ValidateFirstName();
				break;


			case "MiddleName":
				error = this.ValidateMiddleName();
				break;


			case "LastName":
				error = this.ValidateLastName();
				break;


			case "DateOfBirth":
				error = this.ValidateDateOfBirth();
				break;


			case "Gender":
				error = this.ValidateGender();
				break;


			case "JoinDate":
				error = this.ValidateJoinDate();
				break;


			case "Portrait":
				error = this.ValidatePortrait();
				break;
				
			case "Contact":
				break;


                default:
                    Debug.Fail("Unexpected property being validated on Customer: " + propertyName);
                    break;					
			}
			
			return error;			
		}
		
		#region Validation helper methods
		

		private string ValidateFirstName()
		{
			if (string.IsNullOrWhiteSpace(FirstName))
				return "The member's first name can't be empty!";
			
			
			return null;
		}
		

		private string ValidateMiddleName()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateLastName()
		{
			if (string.IsNullOrWhiteSpace(FirstName))
				return "The member's last name can't be empty!";
			
			return null;
		}
		

		private string ValidateDateOfBirth()
		{
			if (DateOfBirth != null && DateTime.Compare((DateTime)DateOfBirth,
	        	                                              DateTime.Now) > 0)
	        		return "The date of birth can't be in the future!";
	        	
	        return null;
		}
		

		private string ValidateGender()
		{
			if (!(Gender.ToLower() == "male" || Gender.ToLower() == "female"))
				return "The member's gender must be either male or female.";
			
			return null;
		}
		

		private string ValidateJoinDate()
		{
			if (DateTime.Compare((DateTime)JoinDate,
	        	                                              DateTime.Now) > 0)
	        		return "The date of joining can't be in the future!";
	        	
	        return null;
		}
		

		private string ValidatePortrait()
		{
			string err = null;
			
			
			return err;
		}
		

		
		#endregion //Validation helper methods
		
		#endregion //Validation
	}
	

}
