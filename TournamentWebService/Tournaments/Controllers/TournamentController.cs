using System;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TournamentWebService.Tournaments.Models;
using TournamentWebService.Tournaments.Services;
using TournamentWebService.Core.Publisher;
using TournamentWebService.Teams.Models;
using TournamentWebService.Teams.Services;
using Newtonsoft.Json;
using TournamentWebService.Core;

namespace TournamentWebService.Tournaments.Controllers
{
    [Controller]
    [Route("api/[Controller]")]
    public class TournamentController : BaseController
    {
        private readonly TournamentMongoDBService _tournamentMongoDBService;
        private readonly TeamMongoDBService _teamMongoDBService;
        public TournamentController(TournamentMongoDBService tournamentMongoDBService, TeamMongoDBService teamMongoDBService)
        {
            _tournamentMongoDBService = tournamentMongoDBService;
            _teamMongoDBService = teamMongoDBService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTournaments() {
            try
            {
                Console.WriteLine("GetAllTournaments");
                List<Tournament> tournaments = await _tournamentMongoDBService.GetAllAsync();
                if (tournaments.Count == 0) return BadRequest(new { error = "No se encontraron torneos" });
                return Ok(tournaments);
            }catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] Tournament tournament) {
            try
            {
                if (!TournamentDataValidation.tournamentAcces.Contains(tournament.access))
                {
                    return BadRequest(new { error = "El tipo de acceso del torneo no es valido" });
                }
                if (!TournamentDataValidation.tournamentStatus.Contains(tournament.status))
                {
                    return BadRequest(new { error = "El estado del torneo no es valido" });
                }
                await _tournamentMongoDBService.CreateAsync(tournament);
                return CreatedAtAction(nameof(GetAllTournaments), new { id = tournament.Id }, new { message = "Torneo creado", tournament });
            }catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTournament(string id, [FromBody] Tournament tournament) {
            try
            {
                Tournament tournamentAux = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournamentAux == null) return BadRequest(new { error = "Torneo no encontrado" });

                if (tournament.access != null && !TournamentDataValidation.tournamentAcces.Contains(tournament.access))
                {
                    return BadRequest(new { error = "El tipo de acceso del torneo no es valido" });
                }
                if (tournament.status != null && !TournamentDataValidation.tournamentStatus.Contains(tournament.status))
                {
                    return BadRequest(new { error = "El estado del torneo no es valido" });
                }
                if (tournament.status == TournamentDataValidation.tournamentStatus[(int)TournamentStatusIndex.InProgress] || tournament.status == TournamentDataValidation.tournamentStatus[(int)TournamentStatusIndex.Finished])
                {
                    return BadRequest(new { error = "Este no es el método indicado para iniciar o finalizar torneos" });
                }
                await _tournamentMongoDBService.UpdateAsync(id, tournament);
                return Ok(new { message = "Torneo actualizado" });
            }catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(string id) {
            try
            {
                Tournament tournamentAux = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournamentAux == null) return BadRequest(new { error = "Torneo no encontrado" });
                await _tournamentMongoDBService.DeleteAsync(id);
                return Ok(new { message = "Torneo eliminado" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournament(string id)
        {
            try
            {
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournament == null) return BadRequest(new { error = "Torneo no encontrado" });


                HttpClient client = new();
                using HttpResponseMessage sportsResponse = await client.GetAsync($"{UrlConstants.sportsMS}/sport/{tournament.sportId}");
                if ((int) sportsResponse.StatusCode > 300)
                {
                    return BadRequest(new { error = "Hubo un error encontrando los datos del deporte de este torneo" });
                }
                sportsResponse.EnsureSuccessStatusCode();
                string? sportsResponseBody = await sportsResponse.Content.ReadAsStringAsync();
                Sport? sport;
                if (!String.IsNullOrEmpty(sportsResponseBody))
                    sport = JsonConvert.DeserializeObject<Sport>(sportsResponseBody);
                else
                    sport = null;


                using HttpResponseMessage? modesResponse = await client.GetAsync($"{UrlConstants.sportsMS}/mode/{tournament.modeId}/mode");
                if ((int)sportsResponse.StatusCode > 300)
                {
                    return BadRequest(new { error = "Hubo un error encontrando del modo de este torneo" });
                }
                modesResponse.EnsureSuccessStatusCode();
                string? modesResponseBody = await modesResponse.Content.ReadAsStringAsync();
                Mode? mode;
                if (!String.IsNullOrEmpty(modesResponseBody))
                {
                    mode = JsonConvert.DeserializeObject<Mode>(modesResponseBody);
                }
                else
                {
                    mode = null;
                }


                Clan? clan;
                if (tournament.clanId != null)
                {
                    using HttpResponseMessage clansResponse = await client.GetAsync($"{UrlConstants.clansMS}/clans/{tournament.modeId}");
                    if ((int)sportsResponse.StatusCode > 300)
                    {
                        return BadRequest(new { error = "Hubo un error encontrando el clan de este torneo" });
                    }
                    modesResponse.EnsureSuccessStatusCode();
                    string clansResponseBody = await modesResponse.Content.ReadAsStringAsync();

                    if (!String.IsNullOrEmpty(clansResponseBody))
                        clan = JsonConvert.DeserializeObject<Clan>(modesResponseBody);
                    else
                        clan = null;
                }
                else
                    clan = null;


                Venue? venue;
                if (tournament.venueId != null)
                {
                    using HttpResponseMessage venueResponse = await client.GetAsync($"{UrlConstants.tournamentVenuesMS}/venue?id={tournament.venueId}");
                    if ((int)sportsResponse.StatusCode > 300)
                    {
                        return BadRequest(new { error = "Hubo un error encontrando el lugar de este torneo" });
                    }
                    venueResponse.EnsureSuccessStatusCode();
                    string venueResponseBody = await venueResponse.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(venueResponseBody))
                        venue = JsonConvert.DeserializeObject<Venue>(venueResponseBody);
                    else
                        venue = null;

                }
                else
                    venue = null;
                TournamentPopulated res = new()
                {
                    Id = tournament.Id,
                    name = tournament.name,
                    teams = tournament.teams,
                    sportId = sport,
                    modeId = mode,
                    clanId = clan,
                    venueId = venue,
                    venueName = tournament.venueName,
                    access = tournament.access,
                    status = tournament.status,
                    createdAt = tournament.createdAt,
                    updatedAt = tournament.updatedAt
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("in-progress")]
        public async Task<IActionResult> GetActiveTournaments(string id)
        {
            try
            {
                List<Tournament> activeTournaments = await _tournamentMongoDBService.GetActiveTournamentsAsync();
                Console.WriteLine(activeTournaments);
                if (activeTournaments.Count() == 0) return BadRequest(new { error = "No hay torneos activos" });
                return Ok(activeTournaments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("by-status")]
        public async Task<IActionResult> GetTournamentsByStatus([FromQuery(Name = "status")] string status)
        {
            try
            {
                if (status != null && !TournamentDataValidation.tournamentStatus.Contains(status))
                {
                    return BadRequest(new { error = "El estado del torneo no es válido" });
                }
                List<Tournament> tournaments = await _tournamentMongoDBService.GetTournamentsByStatusAsync(status);
                if (tournaments.Count() == 0) return BadRequest(new { error = "No hay torneos con este estado" });
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch]
        [Route("start-tournament/{id}")]
        public async Task<IActionResult> StartTournament(string id)
        {
            try
            {
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournament == null) return BadRequest(new { error = "Torneo no encontrado" });
                if (tournament.status != TournamentDataValidation.tournamentStatus[(int)TournamentStatusIndex.Confirmed])
                    return BadRequest(new { error = "El estado actual del torneo es incorrecto, no es posible iniciarlo" });
                tournament.status = TournamentDataValidation.tournamentStatus[(int)TournamentStatusIndex.InProgress];
                List<string> players = new();
                foreach (var team in tournament.teams)
                {
                    Team teamAux = await _teamMongoDBService.GetOneAsync(team);
                    if (teamAux == null) return BadRequest(new { error = "No se encontraron los equipos de este partido" });
                    players.AddRange(teamAux.members);
                }
                int[] playersInt = Array.ConvertAll(players.ToArray(), int.Parse);
                StartTournamentMessage startTournamentMessage = new()
                {
                    DATA = new StartTournamentData(tournament.Id, playersInt)
                };
                string message = JsonConvert.SerializeObject(startTournamentMessage);
                Publisher.publishMessage(Constants.mqHost, Constants.tournamentsQueue, message);
                await _tournamentMongoDBService.UpdateAsync(id, tournament);
                return Ok(new { message = "Torneo iniciado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch]
        [Route("end-tournament/{id}")]
        public async Task<IActionResult> EndTournament(string id)
        {
            try
            {
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournament == null) return BadRequest(new { error = "Torneo no encontrado" });
                if (tournament.status != TournamentDataValidation.tournamentStatus[(int)TournamentStatusIndex.InProgress])
                    return BadRequest(new { error = "El estado actual del torneo es incorrecto, no es posible finalizarlo" });
                tournament.status = TournamentDataValidation.tournamentStatus[(int)TournamentStatusIndex.Finished];
                EndTournamentMessage endTournamentMessage = new()
                {
                    DATA = new EndTournamentData(tournament.Id)
                };
                string message = JsonConvert.SerializeObject(endTournamentMessage);
                Publisher.publishMessage(Constants.mqHost, Constants.tournamentsQueue, message);
                await _tournamentMongoDBService.UpdateAsync(id, tournament);
                return Ok(new { message = "Torneo finalizado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
