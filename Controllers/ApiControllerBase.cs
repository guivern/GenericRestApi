using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        protected readonly bool isSoftDelete;
        protected readonly bool isAudit;

        public ApiControllerBase(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = _context.Set<TEntity>();
            isSoftDelete = typeof(TEntity).IsSubclassOf(typeof(SoftDeleteEntityBase));
            isAudit = typeof(TEntity).IsSubclassOf(typeof(AuditEntityBase));
        }

        [HttpGet]
        public virtual async Task<IActionResult> List()
        {
            // prepara query
            var query = _dbSet.AsQueryable();
            if (isSoftDelete)
            {
                query = query.Where(e => (e as SoftDeleteEntityBase).Activo);
            }
            // refina query
            query = RefineListQuery(query);
            // ejecuta query
            var result = await query.ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Detail(long id)
        {
            // prepara query
            var query = _dbSet.AsQueryable();
            if (isSoftDelete)
            {
                query = query.Where(e => (e as SoftDeleteEntityBase).Activo);
            }
            // refina query
            query = RefineDetailQuery(query);
            // ejecuta query
            var result = await query.SingleOrDefaultAsync(r => r.Id == id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TDto dto)
        {
            if (await IsValidDto(dto))
            {
                TEntity entity = _mapper.Map<TEntity>(dto);
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                return CreatedAtAction("Detail", new { id = entity.Id }, entity);
            }

            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(long id, TDto dto)
        {
            if (await IsValidDto(dto, id))
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null) return NotFound();

                entity = _mapper.Map<TDto, TEntity>(dto, entity);

                if (isAudit)
                {
                    (entity as AuditEntityBase).FechaEdicion = DateTime.Now;
                }
                // FindAsync() carga la entidad en memoria y el contexto le
                // da seguimiento por lo que no es necesario usar Update()
                await _context.SaveChangesAsync();
                return NoContent();
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null) return NotFound();

            if (isSoftDelete)
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

        // metodo para realizar validaciones
        protected virtual async Task<bool> IsValidDto(TDto dto, long id = 0)
        {
            return ModelState.IsValid;
        }

        // metodo para refinar las consultas del list
        // realizar Includes, Where, etc.
        protected virtual IQueryable<TEntity> RefineListQuery(IQueryable<TEntity> query)
        {
            return query;
        }

        // metodo para refinar las consultas del detail
        // realizar Includes, Where, etc.
        protected virtual IQueryable<TEntity> RefineDetailQuery(IQueryable<TEntity> query)
        {
            return query;
        }
    }
}