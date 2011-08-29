
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using ProtoBLL.BusinessInterfaces;
using ProtoBLL.BusinessEntities;
using PLEF;

using ProtoBLL.General;

namespace ProtoBLL.EntityManagers
{
	/// <summary>
	/// Description of MemberManager.
	/// </summary>
	public class MemberManager : IMemberManager
	{
		public MemberManager()
		{
		}
		
		#region CRUD

		public List<MemberBLL> GetByName(string name, int numResults)
		{
			List<MemberBLL> resultList = new List<MemberBLL>();
			
			if (name != null)
			{
				string[] nameParts = name.Split(' ');
				
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					
					var dalMembers = (from m in context.Members
					                  where nameParts.Any(val => ((m.FirstName + " " + ((m.MiddleName == null)? "":m.MiddleName) +
					                                               " " + m.LastName).Contains(val)))
					                  select m).Take(numResults);
					
					foreach (Member dalMember in dalMembers)
					{	
						MemberBLL bllMember = new MemberBLL();
						CrossLayerEntityConverter.MemberDalToBll(context, bllMember, dalMember);
						resultList.Add(bllMember);
					}
				}
			}
			
			return resultList;
		}
		
		public bool Add(MemberBLL item, out string serverSideError)
		{
			if (item.IsValid) 
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(item, context, out serverSideError))
					{
						Member newMember = new Member();
						newMember.Contact = new Contact();
//						newMember.Contact.City = new City(); // possible reuse handled in converter.
						newMember.JoinDate = DateTime.Now;
						CrossLayerEntityConverter.MemberBllToDal(context, item, newMember);
						context.Members.AddObject(newMember);
						
						context.SaveChanges();
						
						return true;					
					}
				}
				
				return false;
			}
			else
			{
				serverSideError = "The Member object is invalid";
				return false;
			}
		}
		
		public bool Delete(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var member = (from m in context.Members
				            where m.MemberID == id
				            select m).SingleOrDefault();
				
				if (member != null)
				{
					
					//FIXME: Don't remove transaction history on deleting member
					var trans = member.Transactions.ToList();
					foreach (var t in trans)
						context.Transactions.DeleteObject(t);
					
					Contact c = member.Contact;
					context.DeleteObject(c);
					
					var accs = member.StaffAccounts.ToList();
					foreach (var sa in accs)
						context.StaffAccounts.DeleteObject(sa);
					
					
					context.Members.DeleteObject(member);
					context.SaveChanges();
					return true;
				}
			}
			
			return false;
		}
		
		public bool Update(MemberBLL newItem, out string serverSideError)
		{
			if (newItem.IsValid)
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(newItem, context, out serverSideError, newItem.ItemID))
					{
						Member dalMember = (from m in context.Members
						                       where m.MemberID == newItem.ItemID
						                       select m).Single();
						
						CrossLayerEntityConverter.MemberBllToDal(context, newItem, dalMember);
						context.SaveChanges();
						
						Debug.Assert(serverSideError == null);
						return true;
					}
					else
						return false;
				}
			}
			
			serverSideError = "Item is in an invalid state!";
			return false;
		}
		
		public bool Update(List<MemberBLL> items, out string serverSideError)
		{
			bool failure = false;
			serverSideError = null;
			string error = "";
			string temp = "";
			int numUpdated = 0;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				
				foreach (MemberBLL bllMember in items)
				{
					if (bllMember.IsValid)
					{
						if (DatabaseDependantValidation(bllMember, context, out temp, bllMember.ItemID))
						{
							Member dalMember = (from m in context.Members
							                       where m.MemberID == bllMember.ItemID
							                       select m).SingleOrDefault();
							
							if (dalMember != null)
							{
								CrossLayerEntityConverter.MemberBllToDal(context, bllMember, dalMember);
							}
							else
							{
								error += "\nNo item with ID " + bllMember.ItemID.ToString() +
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
						error += ("\nItem with ID: " +bllMember.ItemID.ToString() +
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
			
			Debug.Assert(serverSideError == null);
			return true;
		}
		
		public MemberBLL GetByID(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				Member member = (from m in context.Members
				                    where m.MemberID == id
				                    select m).SingleOrDefault();
				
				if (member != null)
				{
					MemberBLL retMem = new MemberBLL();
					CrossLayerEntityConverter.MemberDalToBll(context, retMem, member);
					return retMem;
				}
			}
			
			return null;
		}
		
		#endregion //CRUD
		
		#region Browsing capability
		
		public List<MemberBLL> GetFirstItems(int numItems)
		{
			List<MemberBLL> retList = new List<MemberBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalMembers = (from m in context.Members
				                orderby m.MemberID
				                select m).Take(numItems);
				
				MemberBLL bllMember = null;
				foreach (Member dalMember in dalMembers)
				{
					bllMember = new MemberBLL();
					CrossLayerEntityConverter.MemberDalToBll(context, bllMember, dalMember);
					retList.Add(bllMember);
				}
			}
			
			return retList;
		}
		
		public List<MemberBLL> GetLastItems(int numItems)
		{
			List<MemberBLL> retList = new List<MemberBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalMembers = (from m in context.Members
				                orderby m.MemberID descending
				                select m).Take(numItems);
				
				MemberBLL bllMember = null;
				foreach (Member dalMember in dalMembers)
				{
					bllMember = new MemberBLL();
					CrossLayerEntityConverter.MemberDalToBll(context, bllMember, dalMember);
					retList.Add(bllMember);
				}
			}
			
			retList.Reverse();
			return retList;	
		}
		
		public List<MemberBLL> GetNextItems(int afterItemID, int numItems)
		{
			List<MemberBLL> retList = new List<MemberBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalMembers = (from m in context.Members
				                where m.MemberID > afterItemID
				                orderby m.MemberID
				                select m).Take(numItems);
				
				MemberBLL bllMember = null;
				foreach (Member dalMember in dalMembers)
				{
					bllMember = new MemberBLL();
					CrossLayerEntityConverter.MemberDalToBll(context, bllMember, dalMember);
					retList.Add(bllMember);
				}
			}
			
			return retList;
		}
		
		public List<MemberBLL> GetPreviousItems(int beforeItemID, int numItems)
		{
			List<MemberBLL> retList = new List<MemberBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalMembers = (from m in context.Members
				                where m.MemberID < beforeItemID
				                orderby m.MemberID descending
				                select m).Take(numItems);
				
				MemberBLL bllMember = null;
				foreach (Member dalMember in dalMembers)
				{
					bllMember = new MemberBLL();
					CrossLayerEntityConverter.MemberDalToBll(context, bllMember, dalMember);
					retList.Add(bllMember);
				}
			}
			
			retList.Reverse();
			return retList;		
		}
		
		public List<MemberBLL> GetSurroundingItems(int midItemID, int numItemsBeforeAndAfter)
		{
			List<MemberBLL> retList = new List<MemberBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				MemberBLL midItem = GetByID(midItemID);
				if (midItem != null)
				{
					retList.AddRange(GetPreviousItems(midItemID, numItemsBeforeAndAfter));
					retList.Add(midItem);
					retList.AddRange(GetNextItems(midItemID, numItemsBeforeAndAfter));
				}
			}
			
			return retList;
		}
		
		#endregion //Browsing capability
		
		#region IAdvancedSearchCapable members

		public List<MemberBLL> ResolveSearch(MemberSearchCriteria criteria)
		{
			throw new NotImplementedException();
		}
		
		public List<MemberBLL> ResolveSearch(MemberSearchCriteria criteria, int pageSize, int pageNumber, out int numResults, out int totalPages)
		{
			throw new NotImplementedException();
		}
		
		#endregion //IAdvancedSearchCapable members
		
		
		#region Private helpers
		
		private bool DatabaseDependantValidation(MemberBLL bllMember, ProtoLibEntities context, 
		                                         out string error,
		                                         int idToExclude = 0)
		{
			error = null;
			
			
			return true;			
		}			
		
		#endregion //Private helpers
	}
}
