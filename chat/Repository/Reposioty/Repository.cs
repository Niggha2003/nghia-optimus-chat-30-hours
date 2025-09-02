using AutoMapper;
using Chat.Contexts;
using Chat.Hubs;
using Chat.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public Repository(AppDbContext context, IMapper mapper, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _mapper = mapper;
            _hubContext = hubContext;
        }

        // Phương thức ánh xạ từ Entity sang DTO
        public TDest MapObject<TDest>(object source)
        {
            return _mapper.Map<TDest>(source);
        }

        // Phương thức ánh xạ từ danh sách Entity sang danh sách DTO
        public IEnumerable<TDest> MapList<TDest>(IEnumerable<object> source)
        {
            return _mapper.Map<IEnumerable<TDest>>(source);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return _dbSet == null? null : await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
