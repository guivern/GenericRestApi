using System;
using System.Collections.Generic;
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
    public abstract class CrudControllerBase<TEntity, TDto, TListDto, TDetailDto> : ControllerBase where TEntity : EntityBase
    {
        protected readonly DataContext _context;
        protected readonly IMapper _mapper;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly bool _isSoftDelete;
        protected readonly bool _isAudit;

        public CrudControllerBase(DataContext context, IMapper mapper)
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
            [FromQuery] List<string> orderBy,
            [FromQuery] int pageSize = Constants.DEFAULT_PAGE_SIZE,
            [FromQuery] int pageNumber = Constants.DEFAULT_PAGE_NUMBER)
        {
            var query = _dbSet.AsQueryable();

            if (_isSoftDelete)
            {
                query = ExpressionHelper<TEntity>.GenerateSoftDeleteQuery(query);
            }

            query = IncludeInList(query);
            query = Filter(query, filter);
            query = OrderBy(query, orderBy);

            var entities = await PagedList<TEntity>.CreateAsync(query, pageNumber, pageSize);
            var dtos = _mapper.Map<TListDto[]>(entities);
            
            Response.AddPagination(entities.PageNumber, entities.PageSize, entities.TotalPages, entities.TotalCount);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Detail(long id)
        {
            var entity = await GetEntity(id);
            if (entity == null) return NotFound();

            var dto = _mapper.Map<TDetailDto>(entity);

            return Ok(dto);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TDto dto)
        {
            if (!await IsValidDtoModel(dto)) return BadRequest(ModelState);

            var entity = _mapper.Map<TEntity>(dto);
            await _dbSet.AddAsync(entity);
            await BeforeSaveChangesAsync(entity);
            await _context.SaveChangesAsync();
            var dtoDetail = _mapper.Map<TDetailDto>(entity);
            
            return CreatedAtAction("Detail", new { id = entity.Id }, dtoDetail);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(long id, TDto dto)
        {
            if (!await IsValidDtoModel(dto, id)) return BadRequest(ModelState);

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

        protected virtual async Task<TEntity> GetEntity(long id)
        {
            var query = _dbSet.AsQueryable();

            if (_isSoftDelete)
            {
                query = ExpressionHelper<TEntity>.GenerateSoftDeleteQuery(query);
            }

            query = IncludeInDetail(query);

            return await query.SingleOrDefaultAsync(e => e.Id == id);
        }

        protected virtual IQueryable<TEntity> Filter(IQueryable<TEntity> query, string value)
        {
            try
            {
                var filterProps = GetFilterableProperties();
                var filteredQuery = ExpressionHelper<TEntity>.GenerateSearchQuery(filterProps, query, value);
                return filteredQuery;
            }
            catch (Exception)
            {
                return query;
            }
        }

        protected virtual IQueryable<TEntity> OrderBy(IQueryable<TEntity> query, List<string> orderProperties)
        {
            if (orderProperties.Count() == 0)
                orderProperties.Add(Constants.DEFAULT_ODERING);

            var orderedQuery = ExpressionHelper<TEntity>.GenerateOrderByQuery(query, orderProperties);

            return orderedQuery;
        }

        protected abstract Task<bool> IsValidDtoModel(TDto dto, long id = 0);
        protected abstract IQueryable<TEntity> IncludeInList(IQueryable<TEntity> query);
        protected abstract IQueryable<TEntity> IncludeInDetail(IQueryable<TEntity> query);
        protected abstract List<string> GetFilterableProperties();
        protected abstract Task BeforeSaveChangesAsync(TEntity entity);
    }
}