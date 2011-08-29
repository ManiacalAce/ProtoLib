
using System;
using System.Collections.Generic;
using ProtoBLL.BusinessEntities;

namespace ProtoBLL.BusinessInterfaces
{
	/// <summary>
	/// Description of ITransactionManager.
	/// </summary>
	public interface ITransactionManager
	{
		bool IssueBook(int libBookID, int memberID, out string serverSideError);
		bool ReturnBook(int libBookID, int memberID, int finePerDay, int borrowLimit, out string serverSideError);
		
		List<TransactionBLL> GetMemberTransactions(int memberID);
	}
}
