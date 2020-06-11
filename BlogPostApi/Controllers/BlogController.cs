using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BlogPostApi.Controllers
{
    // Ruta del controlador
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        // Propiedad de la base de datos
        public AppDb Db { get; }
        // Constructor de la clase, que toma una instancia como parametro
        public BlogController(AppDb db) 
        {
            Db = db;
        }
        // GET api/blog
        [HttpGet]
        public async Task<IActionResult> GetLatest()
        {
            // Abrir una conexion
            await Db.Connection.OpenAsync();
            // Crear una instancia de la clase BlogPostQuery
            var query = new BlogPostQuery(Db);
            // Guardar el resultado de la consulta
            var result = await query.LatestPostAsync();
            // Retornar un ok (200) y el resultado
            return new OkObjectResult(result);
        }
        // GET api/blog/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            // Abrir una conexion
            await Db.Connection.OpenAsync();
            // Crear una instancai de la clase blogpostquery
            var query = new BlogPostQuery(Db);
            // Guardar el resultado edel query, se envia el parametro obtenido del url
            var result = await query.FindOneAsync(id);
            // Si returna nullo, retornan NotFound (404)
            if (result is null)
                return new NotFoundResult();
            // Retornar OK y el resultado
            return new OkObjectResult(result);
        }
        // POST api/blog
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]BlogPost body) 
        {
            // Abrir una conexion
            await Db.Connection.OpenAsync();
            // Body es de tipo blogpost, por lo cual tiene una propiedad Db
            body.Db = Db;
            // Insertar de forma asincrona el post, usando al body.
            await body.InsertAsync();
            // Retornar 201 y el item creado
            return new OkObjectResult(body);
        }
        // PUT api/blog/5
        [HttpPut("{ID}")]
        public async Task<IActionResult> PutOne(int id, [FromBody]BlogPost body) 
        {
            // Abrir una conexion
            await Db.Connection.OpenAsync();
            // Crear un query
            var query = new BlogPostQuery(Db);
            // Buscar un post, con el id proporcionado
            var result = await query.FindOneAsync(id);
            // Si existe un resultado, retornar 404
            if (result is null)
                return new NotFoundResult();
            // Actualizar las propiedades del post con el contenido en el cuerpo JSON
            result.Title = body.Title;
            result.Content = body.Content;
            // Actualizar el blog
            await result.UpdateAsync();
            // Retornar Ok y el blog cambiado
            return new OkObjectResult(result);
        }
        // DELETE api/blog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOne(int id) {
            // Abrir una conexion
            await Db.Connection.OpenAsync();
            // Crear un query
            var query = new BlogPostQuery(Db);
            // Buscar un post con el id proporcionado
            var result = await query.FindOneAsync(id);
            // Si no existe un resultado, retornar 404
            if (result is null)
                return new NotFoundResult();
            // Borrar el post
            await result.DeleteAsync();
            // Retornar Ok
            return new OkResult();
        }
        // DELETE api/blog
        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            // Abrir conexion
            await Db.Connection.OpenAsync();
            // Crear conexion
            var query = new BlogPostQuery(Db);
            // Borrar todo
            await query.DeleteAllAsync();
            // Retornar Ok
            return new OkResult();
        }

    }
}
