
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
	/// Description of LibraryBookManager.
	/// </summary>
	public class LibraryBookManager : ILibraryBookManager
	{
		public LibraryBookManager()
		{
		}
		
		#region CRUD

		public List<LibraryBookBLL> GetByName(string name, int numResults)
		{
			return null;
		}
		
		public bool Add(LibraryBookBLL item, out string serverSideError)
		{
			if (item.IsValid) 
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(item, context, out serverSideError))
					{
						LibraryBook newLibBook = new LibraryBook();
						CrossLayerEntityConverter.LibraryBookBllToDal(context, item, newLibBook);
						context.LibraryBooks.AddObject(newLibBook);
						
						context.SaveChanges();
						
						return true;					
					}
				}
				
				return false;
			}
			else
			{
				serverSideError = "The LibraryBook object is invalid";
				return false;
			}
		}
		
		public bool Delete(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var libBook = (from lb in context.LibraryBooks
				            where lb.BookInfoID == id
				            select lb).SingleOrDefault();
				
				if (libBook != null)
				{
					
					//FIXME: Don't remove transaction history on deleting book
					var trans = libBook.Transactions.ToList();
					foreach (var t in trans)
						context.Transactions.DeleteObject(t);
					
					
					context.LibraryBooks.DeleteObject(libBook);
					context.SaveChanges();
					return true;
				}
			}
			
			return false;
		}
		
		public bool Update(LibraryBookBLL newItem, out string serverSideError)
		{
			if (newItem.IsValid)
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(newItem, context, out serverSideError, newItem.ItemID))
					{
						LibraryBook dalLibBook = (from lb in context.LibraryBooks
						                       where lb.BookID == newItem.ItemID
						                       select lb).Single();
						
						CrossLayerEntityConverter.LibraryBookBllToDal(context, newItem, dalLibBook);
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
		
		public bool Update(List<LibraryBookBLL> items, out string serverSideError)
		{
			bool failure = false;
			serverSideError = null;
			string error = "";
			string temp = "";
			int numUpdated = 0;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				
				foreach (LibraryBookBLL bllLibBook in items)
				{
					if (bllLibBook.IsValid)
					{
						if (DatabaseDependantValidation(bllLibBook, context, out temp, bllLibBook.ItemID))
						{
							LibraryBook dalLibBook = (from lb in context.LibraryBooks
							                       where lb.BookID == bllLibBook.ItemID
							                       select lb).SingleOrDefault();
							
							if (dalLibBook != null)
							{
								CrossLayerEntityConverter.LibraryBookBllToDal(context, bllLibBook, dalLibBook);
							}
							else
							{
								error += "\nNo item with ID " + bllLibBook.ItemID.ToString() +
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
						error += ("\nItem with ID: " +bllLibBook.ItemID.ToString() +
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
		
		public LibraryBookBLL GetByID(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				LibraryBook libBook = (from lb in context.LibraryBooks
				                    where lb.BookID == id
				                    select lb).SingleOrDefault();
				
				if (libBook != null)
				{
					LibraryBookBLL retLibBook = new LibraryBookBLL();
					CrossLayerEntityConverter.LibraryBookDalToBll(context, retLibBook, libBook);
					return retLibBook;
				}
			}
			
			return null;
		}
		
		#endregion //CRUD
		
		#region Browsing capability
		
		public List<LibraryBookBLL> GetFirstItems(int numItems)
		{
			List<LibraryBookBLL> retList = new List<LibraryBookBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalLibBooks = (from lb in context.LibraryBooks
				                orderby lb.BookID
				                select lb).Take(numItems);
				
				LibraryBookBLL bllLibBook = null;
				foreach (LibraryBook dalLibBook in dalLibBooks)
				{
					bllLibBook = new LibraryBookBLL();
					CrossLayerEntityConverter.LibraryBookDalToBll(context, bllLibBook, dalLibBook);
					retList.Add(bllLibBook);
				}
			}
			
			return retList;
		}
		
		public List<LibraryBookBLL> GetLastItems(int numItems)
		{
			List<LibraryBookBLL> retList = new List<LibraryBookBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalLibBooks = (from lb in context.LibraryBooks
				                orderby lb.BookID descending
				                select lb).Take(numItems);
				
				LibraryBookBLL bllLibBook = null;
				foreach (LibraryBook dalLibBook in dalLibBooks)
				{
					bllLibBook = new LibraryBookBLL();
					CrossLayerEntityConverter.LibraryBookDalToBll(context, bllLibBook, dalLibBook);
					retList.Add(bllLibBook);
				}
			}
			
			retList.Reverse();
			return retList;	
		}
		
		public List<LibraryBookBLL> GetNextItems(int afterItemID, int numItems)
		{
			List<LibraryBookBLL> retList = new List<LibraryBookBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalLibBooks = (from lb in context.LibraryBooks
				                where lb.BookID > afterItemID
				                orderby lb.BookID
				                select lb).Take(numItems);
				
				LibraryBookBLL bllLibBook = null;
				foreach (LibraryBook dalLibBook in dalLibBooks)
				{
					bllLibBook = new LibraryBookBLL();
					CrossLayerEntityConverter.LibraryBookDalToBll(context, bllLibBook, dalLibBook);
					retList.Add(bllLibBook);
				}
			}
			
			return retList;
		}
		
		public List<LibraryBookBLL> GetPreviousItems(int beforeItemID, int numItems)
		{
			List<LibraryBookBLL> retList = new List<LibraryBookBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalLibBooks = (from lb in context.LibraryBooks
				                where lb.BookID < beforeItemID
				                orderby lb.BookID descending
				                select lb).Take(numItems);
				
				LibraryBookBLL bllLibBook = null;
				foreach (LibraryBook dalLibBook in dalLibBooks)
				{
					bllLibBook = new LibraryBookBLL();
					CrossLayerEntityConverter.LibraryBookDalToBll(context, bllLibBook, dalLibBook);
					retList.Add(bllLibBook);
				}
			}
			
			retList.Reverse();
			return retList;		
		}
		
		public List<LibraryBookBLL> GetSurroundingItems(int midItemID, int numItemsBeforeAndAfter)
		{
			List<LibraryBookBLL> retList = new List<LibraryBookBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				LibraryBookBLL midItem = GetByID(midItemID);
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
	
		
		
		#region Private helpers
		
		private bool DatabaseDependantValidation(LibraryBookBLL bllLibBook, ProtoLibEntities context, 
		                                         out string error,
		                                         int idToExclude = 0)
		{
			error = null;
			
			
			return true;			
		}			
		
		#endregion //Private helpers
	}
}
