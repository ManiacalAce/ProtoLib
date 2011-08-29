using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using ProtoBLL.BusinessEntities;

namespace ProtoBLL.BusinessInterfaces
{
	/// <summary>
	/// Description of IBookDetailsManager.
	/// </summary>
	public interface IBookDetailsManager : IEntityManagerCore<BookDetailsBLL>, 
										IAdvancedSearchCapable<BookDetailsBLL, BookDetailsSearchCriteria>
	{
		
	}
	
	//HACK:Remove this dummy SearchCriteria class and implement it properly
	public class BookDetailsSearchCriteria
	{
		Func<string, bool> TitleFilter {get; set;}
	}
}
