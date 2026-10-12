using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TrafficPenaltyManagement.Application.Interfaces
{
	public interface IGenericRepository<T> where T : class
	{
		Task<T?> GetByIdAsync(int id);
		Task<List<T>> GetAllAsync();
		Task AddAsync(T entity);
		void Update(T entity);
		Task DeleteAsync(int id);
		Task<bool> ExistAsync(Expression<Func<T, bool>> expression);
	}
}
