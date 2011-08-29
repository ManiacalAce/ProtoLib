using System;
using System.ComponentModel;
using System.Diagnostics;
using ProtoBLL.BusinessInterfaces;
using ProtoBLL.General;

namespace ProtoBLL.BusinessEntities
{
	/// <summary>
	/// Description of TransactionBLL.
	/// </summary>

	public class TransactionBLL : IKeyedItem, IDataErrorInfo
	{
		public TransactionBLL()
		{
			
		}
		
		#region Public properties
		
		public int ItemID
		{
			get;
			internal set;
		}

		public int BookID
		{
			get;
			set;
		}		
		public int MemberID
		{
			get;
			set;
		}		
		public DateTime CheckedOutOn
		{
			get;
			set;
		}		
		public DateTime? ReturnedOn
		{
			get;
			set;
		}		
		public double Fine
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
					return string.Format("TransactionBLL item(ID:{0}) is in an invalid state", ItemID.ToString());
				else
					return null;
			}
		}	
		
		#endregion //IDataErrorInfo members
		
		#region Validation
		
		static readonly string[] ValidatedProperties = 
		{
			"BookID",
			"MemberID",
			"CheckedOutOn",
			"ReturnedOn",
			"Fine"
			
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


			case "BookID":
				error = this.ValidateBookID();
				break;


			case "MemberID":
				error = this.ValidateMemberID();
				break;


			case "CheckedOutOn":
				error = this.ValidateCheckedOutOn();
				break;


			case "ReturnedOn":
				error = this.ValidateReturnedOn();
				break;


			case "Fine":
				error = this.ValidateFine();
				break;


                default:
                    Debug.Fail("Unexpected property being validated on Customer: " + propertyName);
                    break;					
			}
			
			return error;			
		}
		
		#region Validation helper methods
		

		private string ValidateBookID()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateMemberID()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateCheckedOutOn()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateReturnedOn()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateFine()
		{
			string err = null;
			
			
			return err;
		}
		

		
		#endregion //Validation helper methods
		
		#endregion //Validation
	}
	

}
