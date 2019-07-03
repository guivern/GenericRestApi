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
    public class ValuesController : ApiControllerBase<Value,ValueDto>
    {
        public ValuesController(DataContext context, IMapper mapper) 
        : base(context, mapper){}
    }

    public class ValueDto
    {
        public string Descripcion {get; set;}
    }
}
