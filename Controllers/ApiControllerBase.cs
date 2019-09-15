using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GenericRestApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiBase.Data;
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
        private readonly ExpressionTreeHelper<TEntity> _expressionHelper;

        public ApiControllerBase(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = _context.Set<TEntity>();
            _isSoftDelete = typeof(TEntity).IsSubclassOf(typeof(SoftDeleteEntityBase));
            _isAudit = typeof(TEntity).IsSubclassOf(typeof(AuditEntityBase));
            _expressionHelper = new ExpressionTreeHelper<TEntity>();
        }

        [HttpGet]
        public virtual async Task<IActionResult> List([FromQuery] string filter)
        {
            var entities = await GetEntities(filter, true);
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Detail(long id)
        {
            var entity = await GetEntity(id, true);
            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TDto dto)
        {
            if (!await IsValidDto(dto)) return BadRequest(ModelState);

            var entity = await CreateEntity(dto);
            BeforeSaveChanges(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Detail", new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(long id, TDto dto)
        {
            if (!await IsValidDto(dto, id)) return BadRequest(ModelState);

            var entity = await GetEntity(id, false);
            if (entity == null) return NotFound();

            UpdateEntity(ref entity, dto);
            BeforeSaveChanges(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var entity = await GetEntity(id, false);
            if (entity == null) return NotFound();

            DeleteEntity(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected virtual async Task<List<TEntity>> GetEntities(string filter, bool includeNestedEntities)
        {
            var query = _dbSet.AsQueryable();

            if (_isSoftDelete)
            {
                var activeFilter = _expressionHelper.CreateActiveExpression(query);
                query = query.Where(activeFilter);
            }

            if(includeNestedEntities) query = IncludeInList(query);
            query = Filter(query, filter);

            return await query.OrderByDescending(e => e.Id).ToListAsync();
        }

        protected virtual async Task<TEntity> GetEntity(long id, bool includeNestedEntities)
        {
            var query = _dbSet.AsQueryable();

            if (_isSoftDelete)
            {
                var activeFilter = _expressionHelper.CreateActiveExpression(query);
                query = query.Where(activeFilter);
            }

            if(includeNestedEntities) query = IncludeInDetail(query);

            return await query.SingleOrDefaultAsync(e => e.Id == id);
        }

        protected virtual async Task<TEntity> CreateEntity(TDto dto)
        {
            TEntity entity = _mapper.Map<TEntity>(dto);
            await _dbSet.AddAsync(entity);

            return entity;
        }

        protected virtual void UpdateEntity(ref TEntity entity, TDto dto)
        {
            entity = _mapper.Map<TDto, TEntity>(dto, entity);

            if (_isAudit)
            {
                (entity as AuditEntityBase).FechaEdicion = DateTime.Now;
            }
        }

        protected virtual void DeleteEntity(TEntity entity)
        {
            if (_isSoftDelete)
            {
                (entity as SoftDeleteEntityBase).Activo = false;
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        protected virtual IQueryable<TEntity> Filter(IQueryable<TEntity> query, string value)
        {
            try
            {
                var predicate = _expressionHelper.CreateSearchExpression(query, value);
                return query.Where(predicate);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return query;
            }
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
        protected virtual async void BeforeSaveChanges(TEntity entity)
        {
            await Task.Run(() => { });
        }
    }
}