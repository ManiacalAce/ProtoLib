using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProtoBLL.BusinessInterfaces
{
	/// <summary>
	/// Description of IAdvancedSearchCapable.
	/// </summary>
	public interface IAdvancedSearchCapable<TEntity, TSearchCriteria>
	{
		List<TEntity> ResolveSearch(TSearchCriteria criteria);
		List<TEntity> ResolveSearch(TSearchCriteria criteria, int pageSize, int pageNumber,
		                            out int numResults, out int totalPages);
	}
}
