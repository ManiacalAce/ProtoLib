using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProtoBLL.BusinessInterfaces
{
	/// <summary>
	/// Core functionality provided by an entity manager in the BLL
	/// </summary>
	public interface IEntityManagerCore<TEntity> where TEntity : IKeyedItem
	{
		bool Add(TEntity item, out string serverSideError);
		
		bool Delete(int id);
				
//		bool Update(int id, TEntity newItem, out string serverSideError);
		bool Update(TEntity newItem, out string serverSideError); //ID of TEntity is read-only, so can't change
		bool Update(List<TEntity> items, out string serverSideError);
		
		TEntity GetByID(int id);
		
		List<TEntity> GetFirstItems(int numItems);
		List<TEntity> GetLastItems(int numItems);
		List<TEntity> GetNextItems(int afterItemID, int numItems);
		List<TEntity> GetPreviousItems(int beforeItemID, int numItems);
		List<TEntity> GetSurroundingItems(int midItemID, int numItemsBeforeAndAfter);
		
		
		
	}
}
