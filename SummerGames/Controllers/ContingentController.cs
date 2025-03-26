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
    public class ContingentController : ControllerBase
    {
        private readonly SummerGamesContext _context;

        public ContingentController(SummerGamesContext context)
        {
            _context = context;
        }

        // GET: api/Contingent
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContingentDTO>>> GetContingents()
        {
            var contingentDTOs = await _context.Contingents
                .Select(c => new ContingentDTO
                {
                    ID = c.ID,
                    Code = c.Code,
                    Name = c.Name,
                    RowVersion = c.RowVersion
                })
                .ToListAsync();

            if (contingentDTOs.Count() > 0)
            {
                return contingentDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Contingent records found in the database." });
            }
        }

        // GET: api/Contingent/inc - Include the Athletes Collection
        [HttpGet("inc")]
        public async Task<ActionResult<IEnumerable<ContingentDTO>>> GetContingentsInc()
        {
            var contingentDTOs = await _context.Contingents
                .Include(s => s.Athletes)
                .Select(s => new ContingentDTO
                {
                    ID = s.ID,
                    Code = s.Code,
                    Name = s.Name,
                    RowVersion = s.RowVersion,
                    Athletes = s.Athletes.Select(cAthletes => new AthleteDTO
                    {
                        ID = cAthletes.ID,
                        FirstName = cAthletes.FirstName,
                        MiddleName = cAthletes.MiddleName,
                        LastName = cAthletes.LastName,
                        AthleteCode = cAthletes.AthleteCode,
                        DOB = cAthletes.DOB,
                        Height = cAthletes.Height,
                        Weight = cAthletes.Weight,
                        Gender = cAthletes.Gender,
                        Affiliation = cAthletes.Affiliation
                    }).ToList()
                })
                .ToListAsync();

            if (contingentDTOs.Count() > 0)
            {
                return contingentDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Contingent records found in the database." });
            }
        }

        // GET: api/Contingent/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContingentDTO>> GetContingent(int id)
        {
            var contingentDTO = await _context.Contingents
                .Select(c => new ContingentDTO
                {
                    ID = c.ID,
                    Code = c.Code,
                    Name = c.Name,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(c => c.ID == id);

            if (contingentDTO == null)
            {
                return NotFound(new { message = "Error: No Contingent records found in the database." });
            }

            return contingentDTO;
        }

        // PUT: api/Contingent/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContingent(int id, ContingentDTO contingentDTO)
        {
            if (id != contingentDTO.ID)
            {
                return BadRequest(new { message = "Error: Incorrect ID for Sport." });
            }

            //Get the record you want to update
            var contingentToUpdate = await _context.Contingents.FindAsync(id);

            //Check that you got it
            if (contingentToUpdate == null)
            {
                return NotFound(new { message = "Error: Contingent record not found." });
            }

            //Wow, we have a chance to check for concurrency even before bothering
            //the database!  Of course, it will get checked again in the database just in case
            //it changes after we pulled the record.  
            //Note using SequenceEqual becuase it is an array after all.
            if (contingentDTO.RowVersion != null)
            {
                if (!contingentToUpdate.RowVersion.SequenceEqual(contingentDTO.RowVersion))
                {
                    return Conflict(new { message = "Concurrecny Error: Contingent has been changed by another user.  Back out and try editing the record again." });
                }
            }

            //Update the properties fo the entity object fron the DTO object

            //contingentToUpdate = contingetnDTO;   //Fix with MappingGenerator

            contingentToUpdate.ID = contingentDTO.ID;
            contingentToUpdate.Code = contingentDTO.Code;
            contingentToUpdate.Name = contingentDTO.Name;
            contingentToUpdate.RowVersion = contingentDTO.RowVersion;

            //Put the original RowVersion value in the OriginalValue collection of the entity
            _context.Entry(contingentToUpdate).Property("RowVersion").OriginalValue = contingentDTO.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContingentDTOExists(id))
                {
                    return Conflict(new { message = "Concurrency Error: Contingent has been Removed." });
                }
                else
                {
                    return Conflict(new { message = "Concurrency Error: Contingent has been updated by another user.  Back out and try editing the record again." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
            }
        }

        // POST: api/Contingent
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContingentDTO>> PostContingentDTO(ContingentDTO contingentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Contingent contingent = new Contingent
            {
                Code = contingentDTO.Code,
                Name = contingentDTO.Name
            };

            try
            {
                _context.Contingents.Add(contingent);
                await _context.SaveChangesAsync();
                //Assign Database Generated values back into the DTO
                contingentDTO.ID = contingent.ID;
                contingentDTO.RowVersion = contingent.RowVersion;
                return CreatedAtAction(nameof(GetContingent), new { id = contingent.ID }, contingentDTO);
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

        // DELETE: api/Contingent/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContingent(int id)
        {
            var contingent = await _context.Contingents.FindAsync(id);
            if (contingent == null)
            {
                return NotFound(new { message = "Delete Error: Contingent has already been removed." });
            }

            try
            {
                _context.Contingents.Remove(contingent);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    return BadRequest(new { message = "Delete Error: Remember, you cannot delete a Contingent that has athletes assigned." });
                }
                else
                {
                    return BadRequest(new { message = "Delete Error: Unable to delete Contingent. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        private bool ContingentDTOExists(int id)
        {
            return _context.ContingentDTO.Any(e => e.ID == id);
        }
    }
}
