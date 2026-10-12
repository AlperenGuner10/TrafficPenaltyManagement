using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Application.Interfaces;
using TrafficPenaltyManagement.Infrastructure.Persistence;

namespace TrafficPenaltyManagement.Infrastructure.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T: class
	{
		private readonly ApplicationDbContext _context;
		private readonly DbSet<T> _dbSet;

		public GenericRepository(ApplicationDbContext context)
		{
			_context=context;
			_dbSet=_context.Set<T>();
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _dbSet.FindAsync(id);

			if (entity is not null)
			{
				_dbSet.Remove(entity);
			}
			
		}

		public async Task<List<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<T?> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public void Update(T entity)
		{
			 _dbSet.Update(entity);
		}

		public async Task<bool> ExistAsync(Expression<Func<T, bool>> expression)
		{
			return await _dbSet.AnyAsync(expression);
		}
	}
}
