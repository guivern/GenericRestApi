using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiBase.Annotations;
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
        protected readonly List<PropertyInfo> filterProps;

        public ApiControllerBase(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = _context.Set<TEntity>();
            isSoftDelete = typeof(TEntity).IsSubclassOf(typeof(SoftDeleteEntityBase));
            isAudit = typeof(TEntity).IsSubclassOf(typeof(AuditEntityBase));
            filterProps = GetFilterProps();
        }

        [HttpGet]
        public virtual async Task<IActionResult> List([FromQuery] string filter)
        {
            // prepara query
            var query = _dbSet.AsQueryable();

            if (isSoftDelete)
            {
                query = SoftDeleteFilter(query);
            }

            // incluye las entidades relacionadas y filtra
            query = Filter(IncludeNestedEntitiesInList(query), filter);
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
                query = SoftDeleteFilter(query);
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

        /// <summary>
        /// Filtra la consulta de acuerdo a las propiedades filtrables de la entidad
        /// </summary>
        /// <param name="query">La consulta a filtrar</param>
        /// <param name="value">El valor del filtro</param>
        /// <returns></returns>
        private IQueryable<TEntity> Filter(IQueryable<TEntity> query, string value)
        {
            // https://stackoverflow.com/questions/34192488/build-an-expression-tree-with-multiple-parameters
            // https://stackoverflow.com/questions/57209466/generic-search-with-expression-trees-gives-system-nullreferenceexception-for-nul
            if (string.IsNullOrEmpty(value) || this.filterProps.Count == 0) return query;

            ConstantExpression constant = Expression.Constant(value.ToLower());
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
            MemberExpression[] members = new MemberExpression[filterProps.Count()];
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
            //MethodInfo method = typeof(string).GetMethod("StartsWith", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);

            for (int i = 0; i < filterProps.Count(); i++)
            {
                var attr = (SearchFilter[])filterProps[i].GetCustomAttributes(typeof(SearchFilter), false);
                var nestedProp = attr[0].nestedProp;

                if (nestedProp != null)
                {   // si el filtro es una propiedad de una entidad anidada
                    members[i] = GetNestedMemberExpression(parameter, nestedProp);
                }
                else
                {
                    members[i] = Expression.Property(parameter, filterProps[i]);
                }
            }

            Expression predicate = null;
            foreach (var member in members)
            {
                //e => e.Member != null
                BinaryExpression notNullExp = Expression.NotEqual(member, Expression.Constant(null));
                //e => e.Member.ToLower() 
                MethodCallExpression toLowerExp = Expression.Call(member, toLowerMethod);
                //e => e.Member.Contains(value)
                MethodCallExpression containsExp = Expression.Call(toLowerExp, containsMethod, constant);
                //e => e.Member != null && e.Member.Contains(value)
                BinaryExpression filterExpression = Expression.AndAlso(notNullExp, containsExp);

                predicate = predicate == null ? (Expression)filterExpression : Expression.OrElse(predicate, filterExpression);
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(predicate, parameter);

            return query.Where(lambda);
        }

        private MemberExpression GetNestedMemberExpression(ParameterExpression parameter, string propertyName)
        {
            Expression expression = parameter;
            foreach (var member in propertyName.Split('.'))
            {
                expression = Expression.PropertyOrField(expression, member);
            }
            return (MemberExpression)expression;
        }

        private IQueryable<TEntity> SoftDeleteFilter(IQueryable<TEntity> query)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var trueCons = Expression.Constant(true);
            var property = Expression.Property(parameter, "Activo");
            // e => e.Activo == true
            var activeExp = Expression.Equal(property, trueCons);

            query = query.Where(Expression.Lambda<Func<TEntity, bool>>(activeExp, parameter));

            return query;
        }

        // Obtiene los atributos filtrables de la entidad
        private List<PropertyInfo> GetFilterProps()
        {
            var t = typeof(TEntity);
            var props = t.GetProperties();
            var filterProps = new List<PropertyInfo>();

            foreach (var prop in props)
            {
                var attr = (SearchFilter[])prop.GetCustomAttributes(typeof(SearchFilter), false);
                if (attr.Length > 0)
                {
                    filterProps.Add(prop);
                }
            }

            return filterProps;
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
            await Task.Run(() => { });
        }

        // para mapear atributos que no se mapean con Automapper 
        protected virtual void CustomMapping(ref TEntity entity, TDto dto)
        { }
    }
}