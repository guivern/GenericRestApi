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
            // incluye las entidades relacionadas
            query = IncludeNestedEntitiesInList(query);
            // ejecuta query
            var result = await query.OrderByDescending(e => e.Id).ToListAsync();
            
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
            // incluye las entidades relacionadas
            query = IncludeNestedEntitiesInDetail(query);
            // ejecuta query
            var result = await query.SingleOrDefaultAsync(r => r.Id == id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TDto dto)
        {
            if (!await IsValidDto(dto)) return BadRequest(ModelState);

            TEntity entity = _mapper.Map<TEntity>(dto);

            await _dbSet.AddAsync(entity);

            CustomMapping(ref entity, dto);
            BeforeSaveChanges(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Detail", new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(long id, TDto dto)
        {
            if (!await EntityExits(id)) return NotFound();
            if (!await IsValidDto(dto, id)) return BadRequest(ModelState);

            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return NotFound();

            entity = _mapper.Map<TDto, TEntity>(dto, entity);

            if (isAudit)
            {
                (entity as AuditEntityBase).FechaEdicion = DateTime.Now;
            }

            // FindAsync() carga la entidad en memoria y el contexto le
            // da seguimiento por lo que no es necesario usar Update()

            CustomMapping(ref entity, dto);
            BeforeSaveChanges(entity);
            await _context.SaveChangesAsync();

            return NoContent();
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

        private async Task<bool> EntityExits(long id)
        {
            return await _dbSet.AnyAsync(u => u.Id == id);
        }

        // metodo para realizar validaciones
        protected virtual Task<bool> IsValidDto(TDto dto, long id = 0)
        {
            return Task.Run(() => ModelState.IsValid);
        }

        // para incluir entidades relacionadas en la lista
        protected virtual IQueryable<TEntity> IncludeNestedEntitiesInList(IQueryable<TEntity> query)
        {
            return query;
        }

        // para incluir entidades relacionadas en el detalle
        protected virtual IQueryable<TEntity> IncludeNestedEntitiesInDetail(IQueryable<TEntity> query)
        {
            return query;
        }

        // para agregar mas cambios al contexto antes de guardar,
        // siguiendo el patron Unit of Work. 
        protected virtual async void BeforeSaveChanges(TEntity entity)
        {
            await Task.Run(() => {});
        }

        // para mapear atributos que no se mapean con Automapper 
        protected virtual void CustomMapping(ref TEntity entity, TDto dto)
        {}
    }
}