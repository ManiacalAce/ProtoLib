using System;
using System.ComponentModel;
using System.Diagnostics;
using ProtoBLL.BusinessInterfaces;

namespace ProtoBLL.BusinessEntities
{
	/// <summary>
	/// Description of ContactBLL.
	/// </summary>

	public class ContactBLL : IKeyedItem, IDataErrorInfo
	{
		public ContactBLL()
		{
		
		}
		
		#region Public properties
		
		public int ItemID
		{
			get;
			internal set;
		}

		public string AddressLine1
		{
			get;
			set;
		}		
		public string AddressLine2
		{
			get;
			set;
		}		
		public string AddressLine3
		{
			get;
			set;
		}		
		public string Pin
		{
			get;
			set;
		}		
		public string Phone1
		{
			get;
			set;
		}		
		public string Phone2
		{
			get;
			set;
		}		
		public string Email
		{
			get;
			set;
		}
		public string Website
		{
			get;
			set;
		}		
		public string City
		{
			get;
			set;
		}		
		public string StateOrProvince
		{
			get;
			set;
		}		
		public string Country
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
					return string.Format("ContactBLL item(ID:{0}) is in an invalid state", ItemID.ToString());
				else
					return null;
			}
		}	
		
		#endregion //IDataErrorInfo members
		
		#region Validation
		
		static readonly string[] ValidatedProperties = 
		{
			"AddressLine1",
			"AddressLine2",
			"AddressLine3",
			"Pin",
			"Phone1",
			"Phone2",
			"Website",
			"Email",
			"City",
			"StateOrProvince",
			"Country"
			
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


			case "AddressLine1":
				error = this.ValidateAddressLine1();
				break;


			case "AddressLine2":
				error = this.ValidateAddressLine2();
				break;


			case "AddressLine3":
				error = this.ValidateAddressLine3();
				break;


			case "Pin":
				error = this.ValidatePin();
				break;


			case "Phone1":
				error = this.ValidatePhone1();
				break;


			case "Phone2":
				error = this.ValidatePhone2();
				break;


			case "Website":
				error = this.ValidateWebsite();
				break;

			case "Email":
				error = this.ValidateEmail();
				break;

			case "City":
				error = this.ValidateCity();
				break;


			case "StateOrProvince":
				error = this.ValidateStateOrProvince();
				break;


			case "Country":
				error = this.ValidateCountry();
				break;


                default:
                    Debug.Fail("Unexpected property being validated on Customer: " + propertyName);
                    break;					
			}
			
			return error;			
		}
		
		#region Validation helper methods
		

		private string ValidateAddressLine1()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateAddressLine2()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateAddressLine3()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidatePin()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidatePhone1()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidatePhone2()
		{
			string err = null;
			
			
			return err;
		}
		
		private string ValidateEmail()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateWebsite()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateCity()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateStateOrProvince()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateCountry()
		{
			string err = null;
			
			
			return err;
		}
		

		
		#endregion //Validation helper methods
		
		#endregion //Validation
	}
	

}
