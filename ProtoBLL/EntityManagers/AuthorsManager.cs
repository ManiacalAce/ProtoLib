using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using PLEF;
using ProtoBLL.BusinessEntities;
using ProtoBLL.BusinessInterfaces;
using ProtoBLL.General;

namespace ProtoBLL.EntityManagers
{
	/// <summary>
	/// Description of AuthorsManager.
	/// </summary>
	public class AuthorsManager : IAuthorsManager
	{
		public AuthorsManager()
		{
		}
		
		#region CRUD
		
		public bool Add(AuthorBLL item, out string serverSideError)
		{
			if (item.IsValid) 
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(item, context, out serverSideError))
					{
						Author newAuth = new Author();

						//newAuth.BookDetailsSet = whoa... Okay, remember.. first authors are added, then new books and author specified there
						
						CrossLayerEntityConverter.AuthorBllToDal(context, item, newAuth);
						context.Authors.AddObject(newAuth);
						
						context.SaveChanges();
						
						return true;					
					}
				}
				
				return false;
			}
			else
			{
				serverSideError = "The Author object is invalid";
				return false;
			}
		}
		
		public bool Delete(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalAuth = (from a in context.Authors
				            where a.AuthorID == id
				            select a).SingleOrDefault();
				
				if (dalAuth != null)
				{
					//ALT: Handle author deletion in a better way
					//Delete this author's credit from all books
					var creditedBooks = from b in context.BookDetailsSet
						where b.Authors.Contains(dalAuth)
						select b;
					
					foreach (BookDetails book in creditedBooks)
						book.Authors.Remove(dalAuth);
					
					context.Authors.DeleteObject(dalAuth);
					
					
					context.SaveChanges();
					return true;
				}
			}
			
			return false;
		}
		
		public bool Update(AuthorBLL newItem, out string serverSideError)
		{
			if (newItem.IsValid)
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(newItem, context, out serverSideError, newItem.ItemID))
					{
						Author dalAuth = (from a in context.Authors
						                  where a.AuthorID == newItem.ItemID
						                  select a).Single();
						
						CrossLayerEntityConverter.AuthorBllToDal(context, newItem, dalAuth);
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
		
		public bool Update(List<AuthorBLL> items, out string serverSideError)
		{
			bool failure = false;
			serverSideError = null;
			string error = "";
			string temp = "";
			int numUpdated = 0;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				
				foreach (AuthorBLL bllAuthor in items)
				{
					if (bllAuthor.IsValid)
					{
						if (DatabaseDependantValidation(bllAuthor, context, out temp, bllAuthor.ItemID))
						{
							Author dalAuthor = (from a in context.Authors
							                       where a.AuthorID == bllAuthor.ItemID
							                       select a).SingleOrDefault();
							
							if (dalAuthor != null)
							{
								CrossLayerEntityConverter.AuthorBllToDal(context, bllAuthor, dalAuthor);
							}
							else
							{
								error += "\nNo item with ID " + bllAuthor.ItemID.ToString() +
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
						error += ("\nItem with ID: " +bllAuthor.ItemID.ToString() +
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
		
		public AuthorBLL GetByID(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				Author dalAuthor = (from a in context.Authors
				                    where a.AuthorID == id 
				                    select a).SingleOrDefault();
				
				if (dalAuthor != null)
				{
					AuthorBLL bllAuthor = new AuthorBLL();
					CrossLayerEntityConverter.AuthorDalToBll(context, bllAuthor, dalAuthor);
					return bllAuthor;
				}
				
			}
			
			return null;
		}
		
		#endregion //CRUD
		
		
		
		public List<AuthorBLL> GetByName(string name, int numResults)
		{
			List<AuthorBLL> resultList = new List<AuthorBLL>();
			
			if (name != null)
			{
				string[] nameParts = name.Split(' ');
				
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					
					//ALT: Get a better authors result using strength of result etc
					var dalAuthors = (from a in context.Authors
					                  where nameParts.Any(val => ((a.FirstName + " " + ((a.MiddleName == null)? "":a.MiddleName) +
					                                               " " + a.LastName).Contains(val)))
					                  select a).Take(numResults);
					
					foreach (Author dalAuthor in dalAuthors)
					{	
						AuthorBLL bllAuthor = new AuthorBLL();
						CrossLayerEntityConverter.AuthorDalToBll(context, bllAuthor, dalAuthor);
						resultList.Add(bllAuthor);
					}
				}
			}
			
			return resultList;
		}
		
		
		#region Browsing capability
		
		public List<AuthorBLL> GetFirstItems(int numItems)
		{
			List<AuthorBLL> retList = new List<AuthorBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalAuthors = (from a in context.Authors
				                orderby a.AuthorID
				                select a).Take(numItems);
				
				AuthorBLL bllAuthor = null;
				foreach (Author dalAuthor in dalAuthors)
				{
					bllAuthor = new AuthorBLL();
					CrossLayerEntityConverter.AuthorDalToBll(context, bllAuthor, dalAuthor);
					retList.Add(bllAuthor);
				}
			}
			
			return retList;	
		}
		
		public List<AuthorBLL> GetLastItems(int numItems)
		{
			List<AuthorBLL> retList = new List<AuthorBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalAuthors = (from a in context.Authors
				                orderby a.AuthorID descending
				                select a).Take(numItems);
				
				AuthorBLL bllAuthor = null;
				foreach (Author dalAuthor in dalAuthors)
				{
					bllAuthor = new AuthorBLL();
					CrossLayerEntityConverter.AuthorDalToBll(context, bllAuthor, dalAuthor);
					retList.Add(bllAuthor);
				}
			}
			
			retList.Reverse();
			return retList;		
		}
		
		public List<AuthorBLL> GetNextItems(int afterItemID, int numItems)
		{
			List<AuthorBLL> retList = new List<AuthorBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalAuthors = (from a in context.Authors
				                where a.AuthorID > afterItemID
				                orderby a.AuthorID
				                select a).Take(numItems);
				
				AuthorBLL bllAuthor = null;
				foreach (Author dalAuthor in dalAuthors)
				{
					bllAuthor = new AuthorBLL();
					CrossLayerEntityConverter.AuthorDalToBll(context, bllAuthor, dalAuthor);
					retList.Add(bllAuthor);
				}
			}
			
			return retList;
		}
		
		public List<AuthorBLL> GetPreviousItems(int beforeItemID, int numItems)
		{
			List<AuthorBLL> retList = new List<AuthorBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalAuthors = (from a in context.Authors
				                where a.AuthorID < beforeItemID
				                orderby a.AuthorID descending
				                select a).Take(numItems);
				
				AuthorBLL bllAuthor = null;
				foreach (Author dalAuthor in dalAuthors)
				{
					bllAuthor = new AuthorBLL();
					CrossLayerEntityConverter.AuthorDalToBll(context, bllAuthor, dalAuthor);
					retList.Add(bllAuthor);
				}
			}
			
			retList.Reverse();
			return retList;		
		}
		
		public List<AuthorBLL> GetSurroundingItems(int midItemID, int numItemsBeforeAndAfter)
		{
			List<AuthorBLL> retList = new List<AuthorBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				AuthorBLL midItem = GetByID(midItemID);
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
		
		#region IAdvancedSearchCapable members
		
		public List<AuthorBLL> ResolveSearch(AuthorSearchCriteria criteria)
		{
			throw new NotImplementedException();
		}
		
		public List<AuthorBLL> ResolveSearch(AuthorSearchCriteria criteria, int pageSize, int pageNumber, out int numResults, out int totalPages)
		{
			throw new NotImplementedException();
		}
		
		#endregion //IAdvancedSearchCapable members
		
		
		#region Private helpers
		
		private bool DatabaseDependantValidation(AuthorBLL bllAuthor, ProtoLibEntities context, 
		                                         out string error,
		                                         int idToExclude = 0)
		{
			error = null;

			return true;			
		}		
		
		
		
		#endregion //private helpers
		
		
	}
}
