
using System;
using System.Collections.Generic;
using ProtoBLL.BusinessEntities;

namespace ProtoBLL.BusinessInterfaces
{
	/// <summary>
	/// Description of IMemberManager.
	/// </summary>
	public interface IMemberManager : IEntityManagerCore<MemberBLL>,
										IAdvancedSearchCapable<MemberBLL, MemberSearchCriteria>
	{
		List<MemberBLL> GetByName(string name, int numResults);
	}
	
	public class MemberSearchCriteria
	{
	}
}
