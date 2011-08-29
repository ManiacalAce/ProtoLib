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
	/// Manages BookDetails objects
	/// </summary>
	public class BookDetailsManager : IBookDetailsManager
	{
		public BookDetailsManager()
		{
		}
		
		#region CRUD
		
		public bool Add(BookDetailsBLL item, out string serverSideError)
		{
			//HACK: in manip bar impl, we try to insert a new book before working on it. Better approach possible.
			if (item.Description.Title == null)
				item.Description.Title = "New book";
			
			if (item.IsValid) 
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(item,context, out serverSideError))
					{
						//Good to go.
						BookDetails newBook = new BookDetails();
						newBook.BookDimensions = new BookDimensions();
						CrossLayerEntityConverter.BookDetailsBllToDal(context, item, newBook);
						context.BookDetailsSet.AddObject(newBook);
						
						context.SaveChanges();
						
						return true;					
					}
				}
				
				return false;
			}
			else
			{
				//FIXME: return IDataErrorInfo validation
				serverSideError = "The BookDetails object is invalid";
				return false;
			}
		}
		
		public bool Delete(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var book = (from b in context.BookDetailsSet.Include("BookDimensions") 
				            where b.BookDetailsID == id
				            select b).SingleOrDefault();
				
				if (book != null)
				{
					context.BookDimensions.DeleteObject(book.BookDimensions);
					
					var libBooks = book.LibraryBooks.ToList();
					foreach (var lb in libBooks)
						context.DeleteObject(lb);
					
					var cats = book.Categories.ToList();
					foreach (var c in cats)
						c.BookDetailsSet.Remove(book);
					
					
					foreach (Subject s in book.Subjects)
						s.BookDetailsSet.Remove(book);
					
					Publisher p = book.Publisher;
					p.BookDetailsSet.Remove(book);
					
					var auths = book.Authors.ToList();
					foreach (var a in auths)
						a.BookDetailsSet.Remove(book);
					
					
					context.BookDetailsSet.DeleteObject(book);
					context.SaveChanges();
					return true;
				}
			}
			
			return false;
		}
				
		public bool Update(BookDetailsBLL newItem, out string serverSideError)
		{
			if (newItem.IsValid)
			{
				using (ProtoLibEntities context = new ProtoLibEntities())
				{
					if (DatabaseDependantValidation(newItem, context, out serverSideError, newItem.ItemID))
					{
						BookDetails dalBook = (from b in context.BookDetailsSet
						                       where b.BookDetailsID == newItem.ItemID
						                       select b).Single();
						
						CrossLayerEntityConverter.BookDetailsBllToDal(context, newItem, dalBook);
						context.SaveChanges();
						
						//FIXME: Remove assert!
						Debug.Assert(serverSideError == null);
						return true;
					}
					else
						return false;
				}
			}
			
			//FIXME: Return IDataErrorInfo validation
			serverSideError = "Item is in an invalid state!";
			return false;
		}
		
		public bool Update(List<BookDetailsBLL> items, out string serverSideError)
		{
			bool failure = false;
			serverSideError = null;
			string error = "";
			string temp = "";
			int numUpdated = 0;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				
				foreach (BookDetailsBLL bllBook in items)
				{
					if (bllBook.IsValid)
					{
						if (DatabaseDependantValidation(bllBook, context, out temp, bllBook.ItemID))
						{
							BookDetails dalBook = (from b in context.BookDetailsSet
							                       where b.BookDetailsID == bllBook.ItemID
							                       select b).SingleOrDefault();
							
							if (dalBook != null)
							{
								CrossLayerEntityConverter.BookDetailsBllToDal(context, bllBook, dalBook);
							}
							else
							{
								error += "\nNo item with ID " + bllBook.ItemID.ToString() +
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
						error += ("\nItem with ID: " +bllBook.ItemID.ToString() +
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
		
		public BookDetailsBLL GetByID(int id)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				BookDetails book = (from b in context.BookDetailsSet
				                    where b.BookDetailsID == id
				                    select b).SingleOrDefault();
				
				if (book != null)
				{
					BookDetailsBLL retBook = new BookDetailsBLL();
					CrossLayerEntityConverter.BookDetailsDalToBll(context, retBook, book);
					return retBook;
				}
			}
			
			return null;
		}
		
		#endregion
		
		#region Browsing capability
		
		public List<BookDetailsBLL> GetFirstItems(int numItems)
		{
			List<BookDetailsBLL> retList = new List<BookDetailsBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalBooks = (from b in context.BookDetailsSet
				                orderby b.BookDetailsID
				                select b).Take(numItems);
				
				BookDetailsBLL bllBook = null;
				foreach (BookDetails dalBook in dalBooks)
				{
					bllBook = new BookDetailsBLL();
					CrossLayerEntityConverter.BookDetailsDalToBll(context, bllBook, dalBook);
					retList.Add(bllBook);
				}
			}
			
			return retList;			
		}
		
		public List<BookDetailsBLL> GetLastItems(int numItems)
		{
			List<BookDetailsBLL> retList = new List<BookDetailsBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalBooks = (from b in context.BookDetailsSet
				                orderby b.BookDetailsID descending
				                select b).Take(numItems);
				
				BookDetailsBLL bllBook = null;
				foreach (BookDetails dalBook in dalBooks)
				{
					bllBook = new BookDetailsBLL();
					CrossLayerEntityConverter.BookDetailsDalToBll(context, bllBook, dalBook);
					retList.Add(bllBook);
				}
			}
			
			retList.Reverse();
			return retList;				
		}
		
		public List<BookDetailsBLL> GetNextItems(int afterItemID, int numItems)
		{
			List<BookDetailsBLL> retList = new List<BookDetailsBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalBooks = (from b in context.BookDetailsSet
				                where b.BookDetailsID > afterItemID
				                orderby b.BookDetailsID
				                select b).Take(numItems);
				
				BookDetailsBLL bllBook = null;
				foreach (BookDetails dalBook in dalBooks)
				{
					bllBook = new BookDetailsBLL();
					CrossLayerEntityConverter.BookDetailsDalToBll(context, bllBook, dalBook);
					retList.Add(bllBook);
				}
			}
			
			return retList;
		}
		
		public List<BookDetailsBLL> GetPreviousItems(int beforeItemID, int numItems)
		{
			List<BookDetailsBLL> retList = new List<BookDetailsBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				var dalBooks = (from b in context.BookDetailsSet
				                where b.BookDetailsID < beforeItemID
				                orderby b.BookDetailsID descending
				                select b).Take(numItems);
				
				BookDetailsBLL bllBook = null;
				foreach (BookDetails dalBook in dalBooks)
				{
					bllBook = new BookDetailsBLL();
					CrossLayerEntityConverter.BookDetailsDalToBll(context, bllBook, dalBook);
					retList.Add(bllBook);
				}
			}
			
			retList.Reverse();
			return retList;			
		}
		
		public List<BookDetailsBLL> GetSurroundingItems(int midItemID, int numItemsBeforeAndAfter)
		{
			List<BookDetailsBLL> retList = new List<BookDetailsBLL>();
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				BookDetailsBLL midItem = GetByID(midItemID);
				if (midItem != null)
				{
					retList.AddRange(GetPreviousItems(midItemID, numItemsBeforeAndAfter));
					retList.Add(midItem);
					retList.AddRange(GetNextItems(midItemID, numItemsBeforeAndAfter));
				}
			}
			
			return retList;			
		}
		
		#endregion
		
		#region IAdvacedSearchCapable members
		
		public List<BookDetailsBLL> ResolveSearch(BookDetailsSearchCriteria criteria)
		{
			throw new NotImplementedException();
		}
		
		public List<BookDetailsBLL> ResolveSearch(BookDetailsSearchCriteria criteria,
		                                          int pageSize, int pageNumber,
		                                          out int numResults, out int totalPages)
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		
		#region Private helpers
		
		#region Validation
		
		private bool DatabaseDependantValidation(BookDetailsBLL bllBook, ProtoLibEntities context, 
		                                         out string error,
		                                         int isbnCheckIdToExclude = 0)
		{
			//Redundant validations... but they're needed if the presentation
			//layer does not ensure validity of these fields. (Mine does).
			//Sage say: validation be done in all layers.
			
			if (ValidateIsbn(bllBook.PublishInfo.ISBN13, bllBook.PublishInfo.ISBN10,
			                 context, out error, isbnCheckIdToExclude) == false)
				return false;
			
			foreach (int authorID in bllBook.AuthorIDs)
				if (ValidateAuthorId(authorID, context) == false)
				{
					error = "Non-existant author ID entered";
					return false;
				}
			
			foreach (int catID in bllBook.CategoryIDs)
				if (ValidateCategoryId(catID, context) == false)
				{
					error = "Non-existant category ID entered";
					return false;
				}
			
			foreach (int subID in bllBook.SubjectIDs)
				if (ValidateSubjectId(subID, context) == false)
				{
					error = "Non-existant subject ID entered";
					return false;
				}		

			return true;			
		}
		
		private bool ValidateIsbn(string isbn13, string isbn10, ProtoLibEntities context,
		                               out string error, int idToExclude = 0)
		{
			var v1 = (from b in context.BookDetailsSet where
			           b.ISBN13 == isbn13 && b.BookDetailsID != idToExclude
			           select b).SingleOrDefault();
			
			var v2 = (from b in context.BookDetailsSet where
			           b.ISBN10 == isbn10 && b.BookDetailsID != idToExclude
			           select b).SingleOrDefault();
			
			
			if (v1 != null)
			{
				error = "The ISBN13 number already belongs to " +
					"an item with ID: " + v1.BookDetailsID.ToString() + 
					" titled: " + v1.Title;
				return false;
			}
			if (v2 != null)
			{
				error = "The ISBN10 number already belongs to " +
					"an item with ID: " + v2.BookDetailsID.ToString() + 
					" titled: " + v2.Title;
				return false;
			}			
			
			error = null;
			return true;
		}
		
		private bool ValidateAuthorId(int id, ProtoLibEntities context)
		{
			Author author = (from a in context.Authors
			         where a.AuthorID == id
			         select a).SingleOrDefault();
			
			if (author == null)
				return false;
			
			return true;
		}
		
		private bool ValidateCategoryId(int id, ProtoLibEntities context)
		{
			Category category = (from c in context.Categories
			         where c.CategoryID == id
			         select c).SingleOrDefault();
			
			if (category == null)
				return false;
			
			return true;			
		}
		
		private bool ValidateSubjectId(int id, ProtoLibEntities context)
		{
			Subject subject = (from s in context.Subjects
			         where s.SubjectID == id
			         select s).SingleOrDefault();
			
			if (subject == null)
				return false;
			
			return true;			
		}
		
		private bool ValidatePublisherId(int id, ProtoLibEntities context)
		{
			Publisher publisher = (from p in context.Publishers
			         				where p.PublisherID == id
			         				select p).SingleOrDefault();
			
			if (publisher == null)
				return false;
			
			return true;			
		}
		#endregion
		
		#endregion
		
		#region Fields
		
		
		#endregion
		

	}
}
