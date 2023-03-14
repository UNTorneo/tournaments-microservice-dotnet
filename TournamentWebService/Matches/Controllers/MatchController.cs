using System;
using System.Net;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TournamentWebService.Matches.Models;
using TournamentWebService.Matches.Services;

namespace TournamentWebService.Matches.Controllers
{
    [Controller]
    [Route("api/[Controller]")]
    public class MatchController : BaseController
    {
        private readonly MatchMongoDBService _matchMongoDBService;
        public MatchController(MatchMongoDBService matchMongoDBService)
        {
            _matchMongoDBService = matchMongoDBService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMatches()
        {
            try
            {
                List<Match> matches = await _matchMongoDBService.GetAllAsync();
                if (matches.Count == 0) { return CustomResult("No se encontraron partidos", HttpStatusCode.BadRequest); }
                return CustomResult(matches);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMatch([FromBody] Match match)
        {
            try
            {
                await _matchMongoDBService.CreateAsync(match);
                return CreatedAtAction(nameof(GetAllMatches), new { id = match.Id }, match);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(string id, [FromBody] Match match)
        {
            try
            {
                Match matchAux = await _matchMongoDBService.GetOneAsync(id);
                if (matchAux == null) { return CustomResult("Partido no encontrado", HttpStatusCode.BadRequest); }
                await _matchMongoDBService.UpdateAsync(id, match);
                return CustomResult("Partido actualizado");
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(string id)
        {
            try
            {
                Match matchAux = await _matchMongoDBService.GetOneAsync(id);
                if (matchAux == null) { return CustomResult("Partido no encontrado", HttpStatusCode.BadRequest); }
                await _matchMongoDBService.DeleteAsync(id);
                return CustomResult("Partido eliminado");
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatch(string id)
        {
            try
            {
                Match match = await _matchMongoDBService.GetOneAsync(id);
                if (match == null) { return CustomResult("Partido no encontrado", HttpStatusCode.BadRequest); }
                return CustomResult(match);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }
    }
}
