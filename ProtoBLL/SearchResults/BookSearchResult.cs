using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using ProtoBLL.BusinessEntities;

namespace ProtoBLL.SearchResults
{
	/// <summary>
	/// An immutable class that represents a result of a book search
	/// </summary>
	public class BookSearchResult
	{
		public BookSearchResult(int id, BookDetailsBLL.BookDescription description,
		                        BookDetailsBLL.BookPublishInfo publishInfo,
		                        string publisherName,
		                        BookDetailsBLL.BookDimensions dimensions, 
		                        List<AuthorSummary> authors,
		                        List<string> categories, List<string> subjects)
		{
			BookDetailsID = id;
			

			Title = description.Title;
			TitleLong = description.TitleLong;
			Summary = description.Summary;
			Notes = description.Notes;
			AmazonLink = description.AmazonLink;
			Language = description.Language;			
			
			ISBN13 = publishInfo.ISBN13;
			ISBN10 = publishInfo.ISBN10;			
			EditionNumber = publishInfo.EditionNumber;
			EditionName = publishInfo.EditionName;
			Printing = publishInfo.Printing;
			DatePublished = publishInfo.DatePublished;

			
			Pages = dimensions.Pages;
			Height = dimensions.Height;
			Width = dimensions.Width;
			Thickness = dimensions.Thickness;
			Weight = dimensions.Weight;
			
			if (publishInfo.PublisherID == null)
			{
				Publisher = null;
			}
			else
				Publisher = new BookSearchResult.PublisherSummary((int)publishInfo.PublisherID,
			                                                  publisherName);
			
			
			Authors = new ReadOnlyCollection<BookSearchResult.AuthorSummary>(authors);
			Categories = new ReadOnlyCollection<string>(categories);
			Subjects = new ReadOnlyCollection<string>(subjects);
		}
		
		public int BookDetailsID
		{
			get;
			private set;
		}
		
		#region Straightforward fields
		
		public string ISBN13
		{
			get;
			private set;
		}
		
		public string ISBN10
		{
			get;
			private set;
		}
		
		public string Title
		{
			get;
			private set;
		}
		
		public string TitleLong
		{
			get;
			private set;
		}
		
		
		public string Summary
		{
			get;
			private set;
		}
		
		public string Notes
		{
			get;
			private set;
		}
		
		public int? EditionNumber
		{
			get;
			private set;
		}
		
		public string EditionName
		{
			get;
			private set;
		}
		
		public int? Printing
		{
			get;
			private set;
		}
		
		public DateTime? DatePublished
		{
			get;
			private set;
		}
		
		public string CoverImage
		{
			get;
			private set;
		}
		
		public string AmazonLink
		{
			get;
			private set;
		}
		
		public string Language
		{
			get;
			private set;
		}
		

		
		
		public int? Pages
		{
			get;
			private set;
		}
		
		public double? Weight
		{
			get;
			private set;
		}
		
		public double? Height
		{
			get;
			private set;			
		}
		
		public double? Width
		{
			get;
			private set;
		}
		
		public double? Thickness
		{
			get;
			private set;
		}		
		
		
		#endregion
		
		public ReadOnlyCollection<AuthorSummary> Authors
		{
			get;
			private set;
		}
		
		public PublisherSummary Publisher
		{
			get;
			private set;
		}
		
		public ReadOnlyCollection<string> Subjects
		{
			get;
			private set;
		}
		
		public ReadOnlyCollection<string> Categories
		{
			get;
			private set;
		}
		

		
		#region Fields
		

		
		#endregion	

		
		#region Nested types (All immutable)

		public class AuthorSummary
		{
			public AuthorSummary(int id, string name)
			{
				AuthorID = id;
				Name = name;
			}
			
			public int AuthorID { get; private set; }
			public string Name { get; private set; }
		}
		
		public class PublisherSummary
		{
			public PublisherSummary(int id, string name)
			{
				PublisherID = id;
				Name = name;
			}
			
			public int PublisherID { get; private set; }
			public string Name { get; private set; }
		}
		
		#endregion
		
	}
}
