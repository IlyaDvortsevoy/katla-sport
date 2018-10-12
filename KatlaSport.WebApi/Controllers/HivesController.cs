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
    [RoutePrefix("api/hives")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CustomExceptionFilter]
    [SwaggerResponseRemoveDefaults]
    public class HivesController : ApiController
    {
        private readonly IHiveService _hiveService;
        private readonly IHiveSectionService _hiveSectionService;

        public HivesController(IHiveService hiveService, IHiveSectionService hiveSectionService)
        {
            _hiveService = hiveService ?? throw new ArgumentNullException(nameof(hiveService));
            _hiveSectionService = hiveSectionService ?? throw new ArgumentNullException(nameof(hiveSectionService));
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hives.", Type = typeof(HiveListItem[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHivesTaskAsync()
        {
            var hives = await _hiveService.GetHivesAsync();
            return Ok(hives);
        }

        [HttpGet]
        [Route("{hiveId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a hive.", Type = typeof(Hive))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveTaskAsync(int hiveId)
        {
            var hive = await _hiveService.GetHiveAsync(hiveId);
            return Ok(hive);
        }

        [HttpGet]
        [Route("{hiveId:int:min(1)}/sections")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hive sections for specified hive.", Type = typeof(HiveSectionListItem))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSectionsTaskAsync(int hiveId)
        {
            var hive = await _hiveSectionService.GetHiveSectionsAsync(hiveId);
            return Ok(hive);
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Creates a new hive.", Type = typeof(Hive))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad request. The hive can't be created.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Conflict on the server. The hive can't be created.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error. The hive can't be created.")]
        public async Task<IHttpActionResult> AddHive([FromBody] UpdateHiveRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hive = await _hiveService.CreateHiveAsync(createRequest);
            var location = string.Format("/api/hives/{0}", hive.Id);
            return Created<Hive>(location, hive);
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Updates an existed hive.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad request. The hive can't be updated.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Conflict on the server. The hive can't be updated.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The requested hive doesn't exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error. The hive can't be updated.")]
        public async Task<IHttpActionResult> UpdateHive([FromUri] int id, [FromBody] UpdateHiveRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _hiveService.UpdateHiveAsync(id, updateRequest);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Deletes an existed hive.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad request. The hive can't be deleted.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Conflict on the server. The hive can't be deleted.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The requested hive doesn't exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error. The hive can't be deleted.")]
        public async Task<IHttpActionResult> DeleteHive([FromUri] int id)
        {
            if (id < 1)
            {
                return BadRequest("id");
            }

            await _hiveService.DeleteHiveAsync(id);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [HttpPut]
        [Route("{hiveId:int:min(1)}/status/{deletedStatus:bool}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Sets deleted status for an existed hive.")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> SetStatusTaskAsync([FromUri] int hiveId, [FromUri] bool deletedStatus)
        {
            await _hiveService.SetStatusAsync(hiveId, deletedStatus);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}
