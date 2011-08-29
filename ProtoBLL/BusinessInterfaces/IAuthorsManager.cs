using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using ProtoBLL.BusinessEntities;

namespace ProtoBLL.BusinessInterfaces
{
	/// <summary>
	/// Description of Interface1.
	/// </summary>
	public interface IAuthorsManager : IEntityManagerCore<AuthorBLL>, 
										IAdvancedSearchCapable<AuthorBLL, AuthorSearchCriteria>
	{
		List<AuthorBLL> GetByName(string name, int numResults);
	}
										
	public class AuthorSearchCriteria
	{
		
	}
}
