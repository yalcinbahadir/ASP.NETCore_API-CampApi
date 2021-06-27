using CampApi.Entities;
using CampApi.Models;
using CampApi.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampApi.Controllers
{
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private ICampRepository _repository;

        public TalksController(ICampRepository repository)
        {
            _repository = repository;
        }

        [HttpGet] //.../api/camps/{moniker}/talks //https://localhost:44331/api/camps/atl2018/talks
        public async Task<ActionResult<TalkModel[]>> GetTalks(string moniker)
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker,true);
                var result = MapTalkModel(talks);
                if (!result.Any())
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retieving data. " + ex.Message);
            }

        }

        [HttpGet("{id:int}")] //.../api/camps/{moniker}/talks/1 //https://localhost:44331/api/camps/atl2018/talks/1
       
        public async Task<ActionResult<TalkModel>> GetTalk(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker,id, true);
                if (talk==null)
                {
                    return NotFound();
                }
                return Ok(MapTalkModel(talk));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retieving data. " + ex.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> PostTalk(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null)
                {
                    return BadRequest("Camp not exists.");
                }

                if (model.Speaker == null)
                {
                    return BadRequest("Speaker ID is required.");
                }

                var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);

                if (speaker == null)
                {
                    return BadRequest("Speaker not exists.");
                }

                var talk = new Talk() { Abstract = model.Abstract, Level = model.Level, Title = model.Title, Camp = camp, Speaker=speaker};
                var isCreated=await _repository.AddTalk(talk);
                if (isCreated)
                {
                    return CreatedAtAction(nameof(GetTalk), new { moniker, Id = talk.TalkId }, MapTalkModel(talk));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed creating resource. " + ex.Message);
            }



        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> UpdateTalk(string moniker, int id, TalkModel model)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker,id,true);
                if (talk==null)
                {
                    return NotFound("Talk not exists.");
                }

                talk.Title =model.Title;
                talk.Abstract = model.Abstract;
                talk.Level = model.Level;

                if (model.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null)
                    {
                        talk.Speaker = speaker;
                    }
                }

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(MapTalkModel(talk));
                }

                return BadRequest("ailed updating resource.");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed updating resource. " + ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTalk(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null)
                {
                    return NotFound("Talk not exists.");
                }

               var result=await _repository.DeleteTalk(talk);

                if (result)
                {
                    return Ok();
                }


                return BadRequest("Failed deleting resource.");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed updating resource. " + ex.Message);
            }
        }






        private TalkModel[] MapTalkModel(Talk[] talks)
        {
            var result=talks.Select(t => new TalkModel
            {
                TalkId=t.TalkId,
                Abstract = t.Abstract,
                Level = t.Level,
                Title = t.Title,
                //Speaker = new SpeakerModel
                //{
                //FirstName = t.Speaker.FirstName,
                //LastName = t.Speaker.LastName,
                //MiddleName = t.Speaker.MiddleName,
                //BlogUrl = t.Speaker.BlogUrl,
                //Company = t.Speaker.Company,
                //CompanyUrl = t.Speaker.CompanyUrl,
                //Twitter = t.Speaker.Twitter,
                //GitHub = t.Speaker.GitHub
                //}
            });
            return result.ToArray();
        }

        private TalkModel MapTalkModel(Talk talk)
        {          
            return new TalkModel(talk);
        }

    }
}
