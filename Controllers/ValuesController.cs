using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestApiBase.Data;
using RestApiBase.Models;

namespace RestApiBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : CrudControllerBase<Value,ValueDto, ValueDto, ValueDto>
    {
        public ValuesController(DataContext context, IMapper mapper) 
        : base(context, mapper){}

        protected override Task BeforeSaveChangesAsync(Value entity)
        {
            return Task.CompletedTask;
        }

        protected override List<string> GetFilterableProperties()
        {
            return new List<string>(){
                nameof(Value.Descripcion)
            };
        }

        protected override IQueryable<Value> IncludeInDetail(IQueryable<Value> query)
        {
            return query;
        }

        protected override IQueryable<Value> IncludeInList(IQueryable<Value> query)
        {
            return query;
        }

        protected override Task<bool> IsValidDtoModel(ValueDto dto, long id = 0)
        {
            return Task.FromResult(ModelState.IsValid);
        }
    }

    public class ValueDto
    {
        public string Descripcion {get; set;}
    }
}
