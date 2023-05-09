using System;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TournamentWebService.Matches.Models;
using TournamentWebService.Matches.Services;
using TournamentWebService.Teams.Models;
using TournamentWebService.Teams.Services;
using TournamentWebService.Tournaments.Models;
using TournamentWebService.Tournaments.Services;
using TournamentWebService.Core.Publisher;
using Newtonsoft.Json;

namespace TournamentWebService.Matches.Controllers
{
    [Controller]
    [Route("api/[Controller]")]
    public class MatchController : BaseController
    {
        private readonly MatchMongoDBService _matchMongoDBService;
        private readonly TournamentMongoDBService _tournamentMongoDBService;
        private readonly TeamMongoDBService _teamMongoDBService;
        public MatchController(MatchMongoDBService matchMongoDBService, TournamentMongoDBService tournamentMongoDBService, TeamMongoDBService teamMongoDBService)
        {
            _matchMongoDBService = matchMongoDBService;
            _tournamentMongoDBService = tournamentMongoDBService;
            _teamMongoDBService = teamMongoDBService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMatches()
        {
            try
            {
                List<Match> matches = await _matchMongoDBService.GetAllAsync();
                if (matches.Count == 0) return BadRequest(new { error = "No se encontraron partidos" });
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMatch([FromBody] Match match)
        {
            try
            {
                if (match.tournamentId == null || match.homeTeam == null || match.visitingTeam == null || match.date == null || match.status == null)
                {
                    return BadRequest(new {error = "La petición no tiene la información suficiente para crear un partido"});
                }
                if (match.status != null && !MatchDataValidation.matchStatus.Contains(match.status))
                {
                    return BadRequest(new { error = "El estado del partido no es valido" });
                }
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(match.tournamentId);
                if (tournament == null) return BadRequest(new { error = "El torneo enviado no existe" });
                Team homeTeam = await _teamMongoDBService.GetOneAsync(match.homeTeam);
                if (homeTeam == null) return BadRequest(new { error = "El equipo local enviado no existe" });
                Team visitingTeam = await _teamMongoDBService.GetOneAsync(match.visitingTeam);
                if (visitingTeam == null) return BadRequest(new { error = "El equipo visitante enviado no existe" });
                await _matchMongoDBService.CreateAsync(match);
                return CreatedAtAction(nameof(GetAllMatches), new { id = match.Id }, new { message = "Partido creado", match });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(string id, [FromBody] Match match)
        {
            try
            {
                Match matchAux = await _matchMongoDBService.GetOneAsync(id);
                if (matchAux == null) return BadRequest(new { error = "Partido no encontrado" });
                if (match.status != null && !MatchDataValidation.matchStatus.Contains(match.status))
                {
                    return BadRequest(new { error = "El estado del partido no es valido" });
                }
                if (match.status == MatchDataValidation.matchStatus[(int) MatchStatusIndex.Playing] || match.status == MatchDataValidation.matchStatus[(int)MatchStatusIndex.Finished])
                {
                    return BadRequest(new { error = "Este no es el método indicado para iniciar o finalizar partidos" });
                }
                if(match.homeTeamScore == 0 && match.visitingTeamScore == 0)
                {
                    match.homeTeamScore = matchAux.homeTeamScore;
                    match.visitingTeamScore = matchAux.visitingTeamScore;
                }
                await _matchMongoDBService.UpdateAsync(id, match);
                return Ok(new { message = "Partido actualizado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(string id)
        {
            try
            {
                Match matchAux = await _matchMongoDBService.GetOneAsync(id);
                if (matchAux == null) return BadRequest(new { error = "Partido no encontrado" });
                await _matchMongoDBService.DeleteAsync(id);
                return Ok(new { message = "Partido eliminado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatch(string id)
        {
            try
            {
                Match match = await _matchMongoDBService.GetOneAsync(id);
                if (match == null) return BadRequest(new { error = "Partido no encontrado" });
                return Ok(match);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("tournament-matches/{id}")]
        public async Task<IActionResult> GetTournamentMatches(string id)
        {
            try
            {
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournament == null) return BadRequest(new { error = "Torneo no encontrado" });
                List<Match> matches = await _matchMongoDBService.GetMatchesByTournamentAsync(id);
                if (matches == null) return BadRequest(new { error = "Partidos no encontrados para el torneo" });
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("team-matches/{id}")]
        public async Task<IActionResult> GetTeamMatches(string id)
        {
            try
            {
                Team team = await _teamMongoDBService.GetOneAsync(id);
                if (team == null) return BadRequest(new { error = "Equipo no encontrado" });
                List<Match> matches = await _matchMongoDBService.GetMatchesByTeamAsync(id);
                if (matches == null) return BadRequest(new { error = "Partidos no encontrados para el equipo" });
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("start-match/{id}")]
        public async Task<IActionResult> StartMatch(string id)
        {
            try {
                Match match = await _matchMongoDBService.GetOneAsync(id);
                if (!(match.status == MatchDataValidation.matchStatus[(int)MatchStatusIndex.Confirmed]))
                    return BadRequest(new { error = "El estado actual del partido es incorrecto, no es posible iniciarlo" });
                match.status = MatchDataValidation.matchStatus[(int)MatchStatusIndex.Playing];
                await _matchMongoDBService.UpdateAsync(id, match);
                StartChatMessage startChatMessage = new()
                {
                    DATA = new StartChatData(match.Id, "private")
                };
                string message = JsonConvert.SerializeObject(startChatMessage);
                Console.WriteLine(message);
                Publisher.publishMessage(Constants.mqHost, Constants.matchesQueue, message);
                return Ok(new { message = "Partido iniciado" });
            } 
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("end-match/{id}")]
        public async Task<IActionResult> EndMatch(string id)
        {
            try
            {
                Match match = await _matchMongoDBService.GetOneAsync(id);
                if (!(match.status == MatchDataValidation.matchStatus[(int)MatchStatusIndex.Playing]))
                    return BadRequest(new { error = "El estado actual del partido es incorrecto, no es posible finalizarlo" });
                match.status = MatchDataValidation.matchStatus[(int)MatchStatusIndex.Finished];
                await _matchMongoDBService.UpdateAsync(id, match);
                EndChatMessage endChatMessage = new()
                {
                    DATA = new EndChatData(match.Id)
                };
                string message = JsonConvert.SerializeObject(endChatMessage);
                Console.WriteLine(message);
                Publisher.publishMessage(Constants.mqHost, Constants.matchesQueue, message);
                return Ok(new { message = "Partido terminado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
