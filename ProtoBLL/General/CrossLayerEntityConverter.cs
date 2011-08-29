using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using PLEF;
using ProtoBLL.BusinessEntities;


namespace ProtoBLL.General
{
	/// <summary>
	/// Utility class for converting between DAL and BLL entities.
	/// Mostly performs blind conversion. Database dependant validation
	/// should be performed in the appropriate manager class.
	/// </summary>
	internal static class CrossLayerEntityConverter
	{
		static CrossLayerEntityConverter()
		{
		}
		
		#region BookDetails
		/// <summary>
		///Doesn't support automatically adding NEW related entities... eg. authors,
		///categories, etc.
		/// </summary>
		public static void BookDetailsBllToDal(ProtoLibEntities context, BookDetailsBLL bllBook, BookDetails dalBook)
		{
			dalBook.ISBN13 = bllBook.PublishInfo.ISBN13;
			dalBook.ISBN10 = bllBook.PublishInfo.ISBN10;
			dalBook.EditionNumber = bllBook.PublishInfo.EditionNumber;
			dalBook.EditionName = bllBook.PublishInfo.EditionName;
			dalBook.Printing = bllBook.PublishInfo.Printing;
			dalBook.DatePublished = bllBook.PublishInfo.DatePublished;
			
			dalBook.Title = bllBook.Description.Title;
			dalBook.TitleLong = bllBook.Description.TitleLong;
			dalBook.Summary = bllBook.Description.Summary;
			dalBook.Notes = bllBook.Description.Notes;
			dalBook.Language = bllBook.Description.Language;
			dalBook.AmazonLink = bllBook.Description.AmazonLink;
			
			dalBook.BookDimensions.Pages = bllBook.Dimensions.Pages;
			dalBook.BookDimensions.Height = bllBook.Dimensions.Height;
			dalBook.BookDimensions.Width = bllBook.Dimensions.Width;
			dalBook.BookDimensions.Thickness = bllBook.Dimensions.Thickness;
			dalBook.BookDimensions.Weight = bllBook.Dimensions.Weight;
			
			//HACK: Use LINQ intersects
			dalBook.Authors.Clear();
			foreach (int authorID in bllBook.AuthorIDs)
				dalBook.Authors.Add(context.Authors.First(a => a.AuthorID == authorID));
			
			dalBook.Categories.Clear();
			foreach (int categoryID in bllBook.CategoryIDs)
				dalBook.Categories.Add(context.Categories.First(c => c.CategoryID == categoryID));
			
			dalBook.Subjects.Clear();
			foreach (int subjectID in bllBook.SubjectIDs)
				dalBook.Subjects.Add(context.Subjects.First(s => s.SubjectID == subjectID));
			
			//dalBook.PublisherID = bllBook.PublishInfo.PublisherID;
			if (bllBook.PublishInfo.PublisherID != null)
				dalBook.Publisher = context.Publishers.First(p => p.PublisherID == (int)bllBook.PublishInfo.PublisherID);
			
			
		}
		
		public static void BookDetailsDalToBll(ProtoLibEntities context, BookDetailsBLL bllBook, BookDetails dalBook)
		{
			bllBook.ItemID = dalBook.BookDetailsID;
			
			bllBook.PublishInfo.ISBN13 = dalBook.ISBN13;
			bllBook.PublishInfo.ISBN10 = dalBook.ISBN10;
			bllBook.PublishInfo.EditionNumber = dalBook.EditionNumber;
			bllBook.PublishInfo.EditionName = dalBook.EditionName;
			bllBook.PublishInfo.Printing = dalBook.Printing;
			bllBook.PublishInfo.DatePublished = dalBook.DatePublished;
			
			bllBook.Description.Title = dalBook.Title;
			bllBook.Description.TitleLong = dalBook.TitleLong;
			bllBook.Description.Summary = dalBook.Summary;
			bllBook.Description.Notes = dalBook.Notes;
			bllBook.Description.Language = dalBook.Language;
			bllBook.Description.AmazonLink = dalBook.AmazonLink;
			
			bllBook.Dimensions.Pages = dalBook.BookDimensions.Pages;
			bllBook.Dimensions.Height = dalBook.BookDimensions.Height;
			bllBook.Dimensions.Width = dalBook.BookDimensions.Width; 
			bllBook.Dimensions.Thickness = dalBook.BookDimensions.Thickness;
			bllBook.Dimensions.Weight = dalBook.BookDimensions.Weight;
			
			bllBook.AuthorIDs.Clear();
			foreach (Author a in dalBook.Authors)
				bllBook.AuthorIDs.Add(a.AuthorID);
			
			bllBook.CategoryIDs.Clear();
			foreach (Category c in dalBook.Categories)
				bllBook.CategoryIDs.Add(c.CategoryID);
			
			bllBook.SubjectIDs.Clear();
			foreach (Subject s in dalBook.Subjects)
				bllBook.SubjectIDs.Add(s.SubjectID);
			
			bllBook.PublishInfo.PublisherID = bllBook.PublishInfo.PublisherID;
			
		}		
		
		#endregion //BookDetails
		
		
		#region StaffAccount
		
		public static void StaffAccountBllToDal(ProtoLibEntities context,
		                                        StaffAccountBLL bllSA, StaffAccount dalSA)
		{
			
			dalSA.UserName = bllSA.UserName;
			dalSA.Password = bllSA.Password;
			dalSA.ClearanceLevel = bllSA.ClearanceLevel;
			
            
			dalSA.Member = context.Members.SingleOrDefault(m => m.MemberID == bllSA.MemberID);
			
			dalSA.AccountPrefs.CacheSize = bllSA.Preferences.CacheSize;
			dalSA.AccountPrefs.SearchResultsPageSize = bllSA.Preferences.SearchResultsPageSize;
			dalSA.AccountPrefs.FullScreenMode = bllSA.Preferences.FullScreenMode;
		}
		
		public static void StaffAccountDalToBll(ProtoLibEntities context,
		                                        StaffAccountBLL bllSA, StaffAccount dalSA)
		{
			bllSA.ItemID = dalSA.AccountID;
			bllSA.UserName = dalSA.UserName;
			bllSA.Password = dalSA.Password;
			bllSA.ClearanceLevel = dalSA.ClearanceLevel;
			
			bllSA.MemberID = dalSA.MemberID;
			
			bllSA.Preferences.CacheSize = dalSA.AccountPrefs.CacheSize;
			bllSA.Preferences.SearchResultsPageSize = dalSA.AccountPrefs.SearchResultsPageSize;
			bllSA.Preferences.FullScreenMode = dalSA.AccountPrefs.FullScreenMode;
		}
		
		#endregion //StaffAccount
		
		
		#region Author
		
		public static void AuthorBllToDal(ProtoLibEntities context, AuthorBLL bllAuthor, Author dalAuthor)
		{
			dalAuthor.FirstName = bllAuthor.FirstName;
			dalAuthor.MiddleName = bllAuthor.MiddleName;
			dalAuthor.LastName = bllAuthor.LastName;
			dalAuthor.DateOfBirth = bllAuthor.DateOfBirth;
			dalAuthor.DateOfDeath = bllAuthor.DateOfDeath;
			dalAuthor.Nationality = bllAuthor.Nationality;
			
			//FIXME: copy image binary data to BLL object
			dalAuthor.AuthorImage = bllAuthor.AuthorImage;

			foreach (int bookID in bllAuthor.BookDetailsIDs)
				dalAuthor.BookDetailsSet.Add(context.BookDetailsSet.Single(b => b.BookDetailsID == bookID));
			
		}
		
		public static void AuthorDalToBll(ProtoLibEntities context, AuthorBLL bllAuthor, Author dalAuthor)
		{
			bllAuthor.ItemID = dalAuthor.AuthorID;
			
			bllAuthor.FirstName = dalAuthor.FirstName;
			bllAuthor.MiddleName = dalAuthor.MiddleName;
			bllAuthor.LastName = dalAuthor.LastName;
			bllAuthor.DateOfBirth = dalAuthor.DateOfBirth;
			bllAuthor.DateOfDeath = dalAuthor.DateOfDeath;
			bllAuthor.Nationality = dalAuthor.Nationality;
			
			bllAuthor.AuthorImage = dalAuthor.AuthorImage;
			
			foreach (BookDetails b in dalAuthor.BookDetailsSet)
				bllAuthor.BookDetailsIDs.Add(b.BookDetailsID);
		}
		
		#endregion //Author
		
		#region Member
		
		public static void MemberBllToDal(ProtoLibEntities context, MemberBLL bllMember, Member dalMember)
		{
			dalMember.FirstName = bllMember.FirstName;
			dalMember.MiddleName = bllMember.MiddleName;
			dalMember.LastName = bllMember.LastName;
			dalMember.DateOfBirth = bllMember.DateOfBirth;
			dalMember.JoinDate = bllMember.JoinDate;
			dalMember.Gender = bllMember.Gender;
			
			//FIXME: copy image binary data to BLL object
			dalMember.Portrait = bllMember.Portrait;

			dalMember.Contact.AddLine1 = bllMember.Contact.AddressLine1;
			dalMember.Contact.AddLine2 = bllMember.Contact.AddressLine2;
			dalMember.Contact.AddLine3 = bllMember.Contact.AddressLine3;
			dalMember.Contact.Phone1 = bllMember.Contact.Phone1;
			dalMember.Contact.Phone2 = bllMember.Contact.Phone2;
			dalMember.Contact.Email = bllMember.Contact.Email;
			dalMember.Contact.Website = bllMember.Contact.Website;
			dalMember.Contact.Pin = bllMember.Contact.Pin;
			
			City city = (from c in context.Cities where c.City1 == bllMember.Contact.City select c).FirstOrDefault();
			
			if (city == null)
			{
				dalMember.Contact.City = new City();
				dalMember.Contact.City.City1 = bllMember.Contact.City;
				dalMember.Contact.City.StateOrProvince = bllMember.Contact.StateOrProvince;
				dalMember.Contact.City.Country = bllMember.Contact.Country;	
			}
			else
			{
				dalMember.Contact.City = city;
			}
			
			
			
			
		}
		
		public static void MemberDalToBll(ProtoLibEntities context, MemberBLL bllMember, Member dalMember)
		{
			bllMember.ItemID = dalMember.MemberID;
			
			bllMember.FirstName = dalMember.FirstName;
			bllMember.MiddleName = dalMember.MiddleName;
			bllMember.LastName = dalMember.LastName;
			bllMember.DateOfBirth = dalMember.DateOfBirth;
			bllMember.JoinDate = dalMember.JoinDate;
			bllMember.Gender = dalMember.Gender;
			
			//FIXME: copy image binary data to BLL object
			bllMember.Portrait = dalMember.Portrait;

			bllMember.Contact.AddressLine1 = dalMember.Contact.AddLine1;
			bllMember.Contact.AddressLine2 = dalMember.Contact.AddLine2;
			bllMember.Contact.AddressLine3 = dalMember.Contact.AddLine3;
			bllMember.Contact.Phone1 = dalMember.Contact.Phone1;
			bllMember.Contact.Phone2 = dalMember.Contact.Phone2;
			bllMember.Contact.Email = dalMember.Contact.Email;
			bllMember.Contact.Website = dalMember.Contact.Website;
			bllMember.Contact.Pin = dalMember.Contact.Pin;
			bllMember.Contact.City = dalMember.Contact.City.City1;
			bllMember.Contact.StateOrProvince = dalMember.Contact.City.StateOrProvince;
			bllMember.Contact.Country = dalMember.Contact.City.Country;
		}		
		
		#endregion //Member
				
		#region LibraryBook
		
		public static void LibraryBookBllToDal(ProtoLibEntities context, LibraryBookBLL bllLibBook, LibraryBook dalLibBook)
		{
			dalLibBook.Price = bllLibBook.Price;
			dalLibBook.ObtainedFrom = bllLibBook.ObtainedFrom;
			dalLibBook.BookDetails = (from book in context.BookDetailsSet where book.BookDetailsID == bllLibBook.BookDetailsId select book).SingleOrDefault();
			dalLibBook.BookStatus = (from st in context.BookStatus1 where st.StatusID == (int)bllLibBook.Status select st).Single();
			
			//TODO: Implement branch stuff
			dalLibBook.Branch = (from br in context.Branches where br.BranchID == 100 select br).First();
			
			
		}
		
		public static void LibraryBookDalToBll(ProtoLibEntities context, LibraryBookBLL bllLibBook, LibraryBook dalLibBook)
		{
			bllLibBook.ItemID = dalLibBook.BookID;
			
			bllLibBook.Price = dalLibBook.Price;
			bllLibBook.ObtainedFrom = dalLibBook.ObtainedFrom;
			bllLibBook.BookDetailsId = dalLibBook.BookInfoID;
			bllLibBook.Status = (BookStatusBLL)(dalLibBook.StatusID);
			
		}		
		
		#endregion //Member
		
		#region Transaction
		
		public static void TransactionBllToDal(ProtoLibEntities context, TransactionBLL bllTrans, Transaction dalTrans)
		{
			dalTrans.LibraryBook = (from lb in context.LibraryBooks where lb.BookID == bllTrans.BookID select lb).SingleOrDefault();
			dalTrans.Member = (from mem in context.Members where mem.MemberID == bllTrans.MemberID select mem).SingleOrDefault();
			
			dalTrans.CheckedOutOn = bllTrans.CheckedOutOn;
			dalTrans.ReturnedOn = bllTrans.ReturnedOn;
			
			dalTrans.Fine = bllTrans.Fine;
			
		}
		
		public static void TransactionDalToBll(ProtoLibEntities context, TransactionBLL bllTrans, Transaction dalTrans)
		{
			bllTrans.ItemID = dalTrans.TransactionID;
			
			bllTrans.BookID = dalTrans.BookID;
			bllTrans.MemberID = dalTrans.MemberID;
			bllTrans.CheckedOutOn = dalTrans.CheckedOutOn;
			bllTrans.ReturnedOn = dalTrans.ReturnedOn;
			bllTrans.Fine = dalTrans.Fine;
			
			
		}	
		
		#endregion //Transaction
		
	}
}
