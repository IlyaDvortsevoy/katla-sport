using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using KatlaSport.Services.Models.HiveManagement;
using KatlaSport.WebApi.CustomFilters;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;

namespace KatlaSport.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/sections")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CustomExceptionFilter]
    [SwaggerResponseRemoveDefaults]
    public class HiveSectionsController : ApiController
    {
        private readonly IHiveSectionService _hiveSectionService;

        public HiveSectionsController(IHiveSectionService hiveSectionService)
        {
            _hiveSectionService = hiveSectionService ?? throw new ArgumentNullException(nameof(hiveSectionService));
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hive sections.", Type = typeof(HiveSectionListItem[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSections()
        {
            var hives = await _hiveSectionService.GetHiveSectionsAsync();
            return Ok(hives);
        }

        [HttpGet]
        [Route("{hiveSectionId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a hive section.", Type = typeof(HiveSection))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSection(int hiveSectionId)
        {
            var hive = await _hiveSectionService.GetHiveSectionAsync(hiveSectionId);
            return Ok(hive);
        }

        [HttpPut]
        [Route("{hiveSectionId:int:min(1)}/status/{deletedStatus:bool}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Sets deleted status for an existed hive section.")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> SetStatus([FromUri] int hiveSectionId, [FromUri] bool deletedStatus)
        {
            await _hiveSectionService.SetStatusAsync(hiveSectionId, deletedStatus);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Creates a new hive section.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad request. The hive section can't be created.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Conflict on the server. The hive section can't be created.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error. The hive section can't be created.")]
        public async Task<IHttpActionResult> AddHiveSection([FromBody] UpdateHiveSectionRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var section = await _hiveSectionService.CreateHiveSectionAsync(createRequest);
            var location = string.Format("/api/sections/{0}", section.Id);
            return Created<HiveSection>(location, section);
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Updates an existed hive section.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad request. The hive section can't be updated.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Conflict on the server. The hive section can't be updated.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The requested hive section doesn't exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error. The hive section can't be updated.")]
        public async Task<IHttpActionResult> UpdateHiveSection([FromUri] int id, [FromBody] UpdateHiveSectionRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _hiveSectionService.UpdateHiveSectionAsync(id, updateRequest);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Deletes an existed hive section.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad request. The hive section can't be deleted.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Conflict on the server. The hive section can't be deleted.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The requested hive section doesn't exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error. The hive section can't be deleted.")]
        public async Task<IHttpActionResult> DeleteHiveSection([FromUri] int id)
        {
            await _hiveSectionService.SetStatusAsync(id, true);
            await _hiveSectionService.DeleteHiveSectionAsync(id);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}
