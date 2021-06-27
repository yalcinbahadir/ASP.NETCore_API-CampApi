using AutoMapper;
using CampApi.Entities;
using CampApi.Models;
using CampApi.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampApi.Controllers
{
    [Route("api/Camps")]
    //[Route("api/v{version:apiVersion}/Camps")]
    [ApiController]
    [ApiVersion("2.0")]
    public class Camps2Controller : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public Camps2Controller(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }


        [HttpGet]   
        public async Task<IActionResult> GetCamps(bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks); //Camps []
                //var model = _mapper.Map<CampModel[]>(results);      //CampModel[]
                var model = MapModel(results);//custum method
                if (!model.Any())
                {
                    return NotFound();
                }

                var response = new
                {
                    Count = results.Count(),
                    Results = results
                };
                return Ok(response);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database. " + ex.Message);
            }

        }





        [HttpGet("{moniker}")]//extention of the route : ...api/camps/moniker
        public async Task<ActionResult<CampModel>> GetCamp(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker); //Camp
                if (result == null)
                {
                    return NotFound();
                }
                //var model = _mapper.Map<CampModel>(result); //CampModel
                var model = new CampModel(result);
                return Ok(model);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database. " + ex.Message);
            }

        }

        [HttpGet("search")]//extention of the route
        // .../api/camps/search?thedate=2018-10-18&includeTalks=true
        public async Task<ActionResult<CampModel>> SearchByDate(DateTime theDate, bool includeTalks)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!results.Any())
                {
                    return NotFound();
                }
                var model = MapModel(results);
                return Ok(model);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database. " + ex.Message);
            }

        }
        //[FromBody] CampModel camp : if you don't decorate the controller with [ApiController] you should use [FromBody]
        [HttpPost]
        public async Task<ActionResult<Camp>> Post(CampModel model)
        {
            try
            {
                var link = _linkGenerator.GetPathByAction("GetCamp", "Camps", new { Moniker = model.Moniker });
                if (string.IsNullOrEmpty(link))
                {
                    return BadRequest("Can not use moniker.");
                }
                var existing = await _repository.GetCampAsync(model.Moniker);
                if (existing != null)
                {
                    ModelState.AddModelError("Moniker", "Moniker must be unique");
                    return BadRequest(ModelState);
                }

                var camp = MapModel(model);
                var result = await _repository.AddCamp(camp);
                if (result)
                {
                    return CreatedAtAction(nameof(GetCamp), new { Moniker = camp.Moniker }, camp);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating resource . " + ex.Message);
            }

        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<Camp>> Update(string moniker, CampModel model)
        {
            try
            {
                if (moniker !=model.Moniker)
                {
                    return BadRequest("Moniker mismatch.");
                }
                var existing = await _repository.GetCampAsync(moniker);
                if (existing == null)
                {                 
                    return NotFound("Resource not found");
                }

                var updatedCamp = MapModifications(existing,model);
                if (await _repository.SaveChangesAsync())
                {
                    return Ok(updatedCamp);
                }

                return BadRequest("Failed to delete resource.");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating resource . " + ex.Message);
            }

        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {

                var campToDelete = await _repository.GetCampAsync(moniker);
                if (campToDelete == null)
                {
                    return NotFound("Resource not found");
                }

                if (await _repository.Delete(campToDelete))
                {
                    return Ok();
                }
              

                return BadRequest("Failed to delete resource.");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating resource . " + ex.Message);
            }

        }


        [HttpGet("doSth")]//extention of the route ...api/camps/doSth?withThis=this
        public async Task<ActionResult<CampModel>> Test(string withThis)
        {
            try
            {
                var results = await _repository.GetAllCampsAsync();
                if (!results.Any())
                {
                    return NotFound();
                }
                var model = MapModel(results);
                return Ok(model);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database. " + ex.Message);
            }

        }

        #region HelperMethods
        private CampModel[] MapModel(ICollection<Camp> camps)
        {
            ICollection<CampModel> models = new List<CampModel>();
            CampModel campModel;
            foreach (var camp in camps)
            {
                campModel = new CampModel(camp);
                models.Add(campModel);
            }

            return models.ToArray<CampModel>();
        }

        private Camp MapModel(CampModel model)
        {
            var camp = new Camp
            {
                Name = model.Name,
                Moniker = model.Moniker,
                EventDate = model.EventDate,
                Length = model.Length,
                Location = new Location
                { 
                    VenueName=model.Venue, Address1=model.LocationAddress1,
                    Address2 = model.LocationAddress2,
                    Address3 = model.LocationAddress3, 
                    CityTown=model.LocationCityTown, 
                    Country=model.LocationCountry, 
                    PostalCode=model.LocationPostalCode, 
                    StateProvince=model.LocationStateProvince
                }

            };

            return camp;
        }

        private Camp MapModifications(Camp camp, CampModel model)
        {
            camp.Name = model.Name;
            camp.Moniker = model.Moniker;
            camp.EventDate = model.EventDate;
            camp.Length = model.Length;
                camp.Location = new Location
                {
                    VenueName = model.Venue,
                    Address1 = model.LocationAddress1,
                    Address2 = model.LocationAddress2,
                    Address3 = model.LocationAddress3,
                    CityTown = model.LocationCityTown,
                    Country = model.LocationCountry,
                    PostalCode = model.LocationPostalCode,
                    StateProvince = model.LocationStateProvince
                };

 

            return camp;
        }
        #endregion
    }
}
