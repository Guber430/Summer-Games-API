using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SummerGames.Models
{
    [ModelMetadataType(typeof(SportMetaData))]
    public class SportDTO
    {
        public int ID { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public Byte[]? RowVersion { get; set; }
        public ICollection<AthleteDTO>? Athletes { get; set; }
    }
}
