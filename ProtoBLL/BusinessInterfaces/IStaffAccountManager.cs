using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using ProtoBLL.BusinessEntities;

namespace ProtoBLL.BusinessInterfaces
{
	/// <summary>
	/// Description of IStaffAccountManager.
	/// </summary>
	public interface IStaffAccountManager : IEntityManagerCore<StaffAccountBLL>
	{
		StaffAccountBLL GetUserByLoginDetails(string username, string pass);
	}
}
