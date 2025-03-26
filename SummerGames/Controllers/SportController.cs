using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SummerGames.Data;
using SummerGames.Models;

namespace SummerGames.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportController : ControllerBase
    {
        private readonly SummerGamesContext _context;

        public SportController(SummerGamesContext context)
        {
            _context = context;
        }

        // GET: api/Sport
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SportDTO>>> GetSports()
        {
            var sportDTOs = await _context.Sports
                .Select(s => new SportDTO
                {
                    ID = s.ID,
                    Code = s.Code,
                    Name = s.Name,
                    RowVersion = s.RowVersion
                })
                .ToListAsync();

            if (sportDTOs.Count() > 0)
            {
                return sportDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Sport records found in the database." });
            }
        }

        // GET: api/Sport/inc - Include the Athletes Collection
        [HttpGet("inc")]
        public async Task<ActionResult<IEnumerable<SportDTO>>> GetSportsInc()
        {
            var sportDTOs = await _context.Sports
                .Include(s => s.Athletes)
                .Select(s => new SportDTO
                {
                    ID = s.ID,
                    Code = s.Code,
                    Name = s.Name,
                    RowVersion = s.RowVersion,
                    Athletes = s.Athletes.Select(sAthletes => new AthleteDTO
                    {
                        ID = sAthletes.ID,
                        FirstName = sAthletes.FirstName,
                        MiddleName = sAthletes.MiddleName,
                        LastName = sAthletes.LastName,
                        AthleteCode = sAthletes.AthleteCode,
                        DOB = sAthletes.DOB,
                        Height = sAthletes.Height,
                        Weight = sAthletes.Weight,
                        Gender = sAthletes.Gender,
                        Affiliation = sAthletes.Affiliation
                    }).ToList()
                })
                .ToListAsync();

            if (sportDTOs.Count() > 0)
            {
                return sportDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Sport records found in the database." });
            }
        }

        // GET: api/Sport/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SportDTO>> GetSport(int id)
        {
            var sportDTO = await _context.Sports
                .Select(s => new SportDTO
                {
                    ID = s.ID,
                    Code = s.Code,
                    Name = s.Name,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(s => s.ID == id);

            if (sportDTO == null)
            {
                return NotFound(new { message = "Error: No Sport records found in the database." });
            }

            return sportDTO;
        }

        // PUT: api/Sport/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSport(int id, SportDTO sportDTO)
        {
            if (id != sportDTO.ID)
            {
                return BadRequest(new { message = "Error: Incorrect ID for Sport."});
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Get the record you want to update
            var sportToUpdate = await _context.Sports.FindAsync(id);

            //Check that you got it
            if (sportToUpdate == null)
            {
                return NotFound(new { message = "Error: Sport record not found." });
            }

            //Wow, we have a chance to check for concurrency even before bothering
            //the database!  Of course, it will get checked again in the database just in case
            //it changes after we pulled the record.  
            //Note using SequenceEqual becuase it is an array after all.
            if (sportDTO.RowVersion != null)
            {
                if (!sportToUpdate.RowVersion.SequenceEqual(sportDTO.RowVersion))
                {
                    return Conflict(new { message = "Concurrecny Error: Sport has been changed by another user.  Back out and try editing the record again." });
                }
            }

            //Update the prpoerties of the entity obect form the DTO object

            //sportToUpdate = sportDTO;     //Fix with MappingGenerator

            sportToUpdate.ID = sportDTO.ID;
            sportToUpdate.Code = sportDTO.Code;
            sportToUpdate.Name = sportDTO.Name;
            sportToUpdate.RowVersion = sportDTO.RowVersion;

            //Put the original RowVersion value in the OriginalValue collection for the entity
            _context.Entry(sportToUpdate).Property("RowVersion").OriginalValue = sportDTO.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SportDTOExists(id))
                {
                    return Conflict(new { message = "Concurrency Error: Sport has been Removed." });
                }
                else
                {
                    return Conflict(new { message = "Concurrency Error: Sport has been updated by another user.  Back out and try editing the record again." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
            }
        }

        // POST: api/Sport
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SportDTO>> PostSport(SportDTO sportDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Sport sport = new Sport
            {
                Code = sportDTO.Code,
                Name = sportDTO.Name
            };

            try
            {
                _context.Sports.Add(sport);
                await _context.SaveChangesAsync();
                //Assign Database Generated values back into the DTO
                sportDTO.ID = sport.ID;
                sportDTO.RowVersion = sportDTO.RowVersion;
                return CreatedAtAction(nameof(GetSport), new { id = sport.ID }, sportDTO);
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    return BadRequest(new { message = "Unable to save: Duplicate Code." });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        // DELETE: api/Sport/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSport(int id)
        {
            var sport = await _context.Sports.FindAsync(id);
            if (sport == null)
            {
                return NotFound(new { message = "Delete Error: Sport has already been removed." });
            }
            try
            {
                _context.Sports.Remove(sport);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    return BadRequest(new { message = "Delete Error: Remember, you cannot delete a Sport that has athletes assigned." });
                }
                else
                {
                    return BadRequest(new { message = "Delete Error: Unable to delete Sport. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        private bool SportDTOExists(int id)
        {
            return _context.SportDTO.Any(e => e.ID == id);
        }
    }
}
