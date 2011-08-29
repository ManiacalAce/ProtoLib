using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using PLEF;
using ProtoBLL.BusinessInterfaces;
using ProtoBLL.BusinessEntities;
using ProtoBLL.General;

namespace ProtoBLL.EntityManagers
{
	/// <summary>
	/// Description of StaffAccountManager.
	/// </summary>
	public class StaffAccountManager : IStaffAccountManager
	{
		public StaffAccountManager()
		{
		}
		
		
		public StaffAccountBLL GetUserByLoginDetails(string username, string pass)
		{
			StaffAccountBLL bllSA = null;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				StaffAccount dalSA = (from sa in context.StaffAccounts
				             where sa.UserName == username && sa.Password == pass
				             select sa).FirstOrDefault();
				
				if (dalSA != null)
				{
					bllSA = new StaffAccountBLL();
					CrossLayerEntityConverter.StaffAccountDalToBll(context, bllSA, dalSA);
				}
			}
			
			return bllSA;
		}
		
		#region CRUD
		
		public bool Add(StaffAccountBLL item, out string serverSideError)
		{
			serverSideError = null;
			if (item.IsValid)
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(item, context, out serverSideError))
					{
						StaffAccount dalSA = new StaffAccount();
						dalSA.AccountPrefs = new AccountPrefs();

						CrossLayerEntityConverter.StaffAccountBllToDal(context, item, dalSA);

						context.StaffAccounts.AddObject(dalSA);
						
						context.SaveChanges();
						return true;
					}
				}
			}
			
			return false;
		}
		
		public bool Delete(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				StaffAccount dalSA = (from sa in context.StaffAccounts.Include("AccountPrefs")
				                      where sa.AccountID == id
				                      select sa).SingleOrDefault();
				
				if (dalSA != null)
				{
					context.AccountPrefsSet.DeleteObject(dalSA.AccountPrefs);
					context.StaffAccounts.DeleteObject(dalSA);
					
					context.SaveChanges();
					
					return true;
				}
			}
			
			return false;
		}
		
		public bool Update(StaffAccountBLL newItem, out string serverSideError)
		{
			if (newItem.IsValid)
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(newItem, context, out serverSideError, newItem.ItemID))
					{
						StaffAccount dalSA = (from sa in context.StaffAccounts
						                       where sa.AccountID == newItem.ItemID
						                       select sa).Single();
						
						CrossLayerEntityConverter.StaffAccountBllToDal(context, newItem, dalSA);
						context.SaveChanges();
						
						return true;
					}
					else
						return false;
				}
			}
			
			serverSideError = "Item is in an invalid state!";
			return false;
		}
		
		public bool Update(List<StaffAccountBLL> items, out string serverSideError)
		{
			bool failure = false;
			serverSideError = null;
			string error = "";
			string temp = "";
			int numUpdated = 0;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				
				foreach (StaffAccountBLL bllSA in items)
				{
					if (bllSA.IsValid)
					{
						if (DatabaseDependantValidation(bllSA, context, out temp, bllSA.ItemID))
						{
							StaffAccount dalSA = (from sa in context.StaffAccounts
							                       where sa.AccountID == bllSA.ItemID
							                       select sa).SingleOrDefault();
							
							if (dalSA != null)
							{
								CrossLayerEntityConverter.StaffAccountBllToDal(context, bllSA, dalSA);
							}
							else
							{
								error += "\nNo item with ID " + bllSA.ItemID.ToString() +
									" exists.";
								failure = true;
							}
							
						}
						else
						{
							error += ("\n" + temp);
							failure = true;
						}
					}
					else
					{
						error += ("\nItem with ID: " + bllSA.ItemID.ToString() +
						          " is in an invalid state.");
						failure = true;
					}
				}
			
			
				numUpdated = context.SaveChanges();
			
			}
			
			if (failure)
			{
				serverSideError = "There were unsuccessful updates. " +
					numUpdated.ToString() + " items were updated.\n" + error;
				return false;
			}
			
			return true;
		}
		
		public StaffAccountBLL GetByID(int id)
		{
			StaffAccountBLL bllSA = null;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				StaffAccount dalSA = (from sa in context.StaffAccounts
				                         where sa.AccountID == id
				                         select sa).SingleOrDefault();
				
				if (dalSA != null)
				{
					bllSA = new StaffAccountBLL();
					CrossLayerEntityConverter.StaffAccountDalToBll(context,
					                                               bllSA, dalSA);
				}
			}
			
			return bllSA;
		}
		#endregion //CRUD
		
		#region Browsing capability
		
		public List<StaffAccountBLL> GetFirstItems(int numItems)
		{
			List<StaffAccountBLL> retList = new List<StaffAccountBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalSAs = (from sa in context.StaffAccounts
				                orderby sa.AccountID
				                select sa).Take(numItems);
				
				StaffAccountBLL bllSA = null;
				foreach (StaffAccount dalSA in dalSAs)
				{
					bllSA = new StaffAccountBLL();
					CrossLayerEntityConverter.StaffAccountDalToBll(context, bllSA, dalSA);
					retList.Add(bllSA);
				}
			}
			
			return retList;	
		}
		
		public List<StaffAccountBLL> GetLastItems(int numItems)
		{
			List<StaffAccountBLL> retList = new List<StaffAccountBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalSAs = (from sa in context.StaffAccounts
				                orderby sa.AccountID descending
				                select sa).Take(numItems);
				
				StaffAccountBLL bllSA = null;
				foreach (StaffAccount dalSA in dalSAs)
				{
					bllSA = new StaffAccountBLL();
					CrossLayerEntityConverter.StaffAccountDalToBll(context, bllSA, dalSA);
					retList.Add(bllSA);
				}
			}
			
			retList.Reverse();
			return retList;	
		}
		
		public List<StaffAccountBLL> GetNextItems(int afterItemID, int numItems)
		{
			List<StaffAccountBLL> retList = new List<StaffAccountBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalSAs = (from sa in context.StaffAccounts
				              where sa.AccountID > afterItemID
				                orderby sa.AccountID
				                select sa).Take(numItems);
				
				StaffAccountBLL bllSA = null;
				foreach (StaffAccount dalSA in dalSAs)
				{
					bllSA = new StaffAccountBLL();
					CrossLayerEntityConverter.StaffAccountDalToBll(context, bllSA, dalSA);
					retList.Add(bllSA);
				}
			}
			
			return retList;	
		}
		
		public List<StaffAccountBLL> GetPreviousItems(int beforeItemID, int numItems)
		{
			List<StaffAccountBLL> retList = new List<StaffAccountBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalSAs = (from sa in context.StaffAccounts
				              where sa.AccountID < beforeItemID
				                orderby sa.AccountID descending
				                select sa).Take(numItems);
				
				StaffAccountBLL bllSA = null;
				foreach (StaffAccount dalSA in dalSAs)
				{
					bllSA = new StaffAccountBLL();
					CrossLayerEntityConverter.StaffAccountDalToBll(context, bllSA, dalSA);
					retList.Add(bllSA);
				}
			}
			
			retList.Reverse();
			return retList;	
		}
		
		public List<StaffAccountBLL> GetSurroundingItems(int midItemID, int numItemsBeforeAndAfter)
		{
			List<StaffAccountBLL> retList = new List<StaffAccountBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				StaffAccountBLL midItem = GetByID(midItemID);
				if (midItem != null)
				{
					retList.AddRange(GetPreviousItems(midItemID, numItemsBeforeAndAfter));
					retList.Add(midItem);
					retList.AddRange(GetNextItems(midItemID, numItemsBeforeAndAfter));
				}
			}
			
			return retList;
		}
		
		#endregion Browsing capability
		
		
		#region Private helpers
		
		bool DatabaseDependantValidation(StaffAccountBLL bllSA, ProtoLibEntities context,
		                                 out string error, int idToExclude = 0)
		{
			error = null;
			
			//Check username existance
			if (UserNameExists(context, bllSA.UserName, bllSA.ItemID))
			{
				error = string.Format("A user already exists with the user-name '{0}'",
				                      bllSA.UserName);
				return false;
			}
			
			if (ValidMemberId(context, bllSA.MemberID) == false)
			{
				error = string.Format("No member with ID: {0} exists!", bllSA.MemberID.ToString());
				return false;
			}
			
			if (MemberIdAlreadyTaken(context, bllSA.MemberID, bllSA.ItemID))
			{
				error = string.Format("An account already exists for the member with ID: {0}", bllSA.MemberID.ToString());
				return false;
			}
			
			return true;
		}
		
		bool UserNameExists(ProtoLibEntities context, string uname, int idToExclude = 0)
		{
			StaffAccount dalSA = (from sa in context.StaffAccounts
			             where sa.UserName == uname && sa.AccountID != idToExclude 
			             select sa).SingleOrDefault();
			
			if (dalSA != null)
				return true;
			
			return false;
		}
		
		bool ValidMemberId(ProtoLibEntities context, int memID)
		{
			Member mem = (from m in context.Members
			              where m.MemberID == memID
			              select m).SingleOrDefault();
			
			if (mem != null)
				return true;
			
			return false;
		}
		
		bool MemberIdAlreadyTaken(ProtoLibEntities context, int memID, int idToExclude = 0)
		{
			StaffAccount dalSA = (from sa in context.StaffAccounts
			                      where sa.MemberID == memID && sa.MemberID != idToExclude
			                      select sa).SingleOrDefault();
			
			if (dalSA != null)
				return true;
			
			return false;
		}
		
		#endregion //Private helpers
		
		#region Fields
		
		#endregion //Fields
	}
}
