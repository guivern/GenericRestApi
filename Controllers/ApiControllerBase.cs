using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GenericRestApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestApiBase.Data;
using RestApiBase.Helpers;
using RestApiBase.Models;

namespace RestApiBase.Controllers
{
    public abstract class ApiControllerBase<TEntity, TDto> : ControllerBase where TEntity : EntityBase, new() where TDto : class
    {
        protected readonly DataContext _context;
        protected readonly IMapper _mapper;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly bool _isSoftDelete;
        protected readonly bool _isAudit;

        public ApiControllerBase(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = _context.Set<TEntity>();
            _isSoftDelete = typeof(TEntity).IsSubclassOf(typeof(SoftDeleteEntityBase));
            _isAudit = typeof(TEntity).IsSubclassOf(typeof(AuditEntityBase));
        }

        [HttpGet]
        public virtual async Task<IActionResult> List(
            [FromQuery] string filter,
            [FromQuery] int? pageSize = Constants.DEFAULT_PAGE_SIZE,
            [FromQuery] int? pageNumber = Constants.DEFAULT_PAGE_NUMBER,
            [FromQuery] string order = Constants.DEFAULT_ODERING)
        {
            var entities = await GetEntities((int)pageNumber, (int)pageSize, filter, order);

            var paginationHeader = JsonConvert.SerializeObject(new
            {
                pageNumber = entities.PageNumber,
                pageSize = entities.PageSize,
                totalPages = entities.TotalPages,
                totalCount = entities.TotalCount
            });

            Response.Headers.Add("pagination", paginationHeader);

            return Ok(entities);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Detail(long id)
        {
            var entity = await GetEntity(id);
            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TDto dto)
        {
            if (!await IsValidDto(dto)) return BadRequest(ModelState);

            TEntity entity = _mapper.Map<TEntity>(dto);
            await _dbSet.AddAsync(entity);
            await BeforeSaveChangesAsync(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Detail", new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(long id, TDto dto)
        {
            if (!await IsValidDto(dto, id)) return BadRequest(ModelState);

            var entity = await GetEntity(id);
            if (entity == null) return NotFound();

            entity = _mapper.Map<TDto, TEntity>(dto, entity);

            if (_isAudit)
            {
                (entity as AuditEntityBase).UltimaModificacion = DateTime.Now;
            }

            await BeforeSaveChangesAsync(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var entity = await GetEntity(id);
            if (entity == null) return NotFound();

            if (_isSoftDelete)
            {
                (entity as SoftDeleteEntityBase).Activo = false;
            }
            else
            {
                _dbSet.Remove(entity);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected virtual async Task<PagedList<TEntity>> GetEntities(int pageNumber, int pageSize, string filter, string order)
        {
            var query = _dbSet.AsQueryable();

            if (_isSoftDelete)
            {
                var active = ExpressionHelper<TEntity>.CreateSoftDeleteExpression(query);
                query = query.Where(active);
            }

            query = IncludeInList(query);
            query = Filter(query, filter);
            query = OrderBy(query, order);

            var entities = await PagedList<TEntity>.CreateAsync(query, pageNumber, pageSize);

            return entities;
        }

        protected virtual async Task<TEntity> GetEntity(long id)
        {
            var query = _dbSet.AsQueryable();

            if (_isSoftDelete)
            {
                var active = ExpressionHelper<TEntity>.CreateSoftDeleteExpression(query);
                query = query.Where(active);
            }

            query = IncludeInDetail(query);

            return await query.SingleOrDefaultAsync(e => e.Id == id);
        }

        protected virtual IQueryable<TEntity> Filter(IQueryable<TEntity> query, string value)
        {
            try
            {
                var filter = ExpressionHelper<TEntity>.CreateSearchExpression(query, value);
                return query.Where(filter);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return query;
            }
        }

        protected virtual IQueryable<TEntity> OrderBy(IQueryable<TEntity> query, string order)
        {
            if (string.IsNullOrEmpty(order)) return query;

            var splitedOrder = order.Split(':');
            var columnName = splitedOrder[0];
            var orderType = splitedOrder.Count() > 1 ? splitedOrder[1] : "asc";
            var oderByExp = ExpressionHelper<TEntity>.CreateOrderByExpression(query, columnName, orderType);

            return query.Provider.CreateQuery<TEntity>(oderByExp);
        }

        protected virtual Task<bool> IsValidDto(TDto dto, long id = 0)
        {
            return Task.Run(() => ModelState.IsValid);
        }

        protected virtual IQueryable<TEntity> IncludeInList(IQueryable<TEntity> query)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> IncludeInDetail(IQueryable<TEntity> query)
        {
            return query;
        }

        // Unit of Work. 
        protected virtual Task BeforeSaveChangesAsync(TEntity entity)
        {
            return Task.FromResult(default(object));
        }
    }
}