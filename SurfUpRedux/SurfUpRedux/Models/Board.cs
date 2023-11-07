
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SurfUpRedux.Models
{
    public class Board
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Display(Name = "Længde")]
        [DisplayFormat(DataFormatString = "{0:G29}", ApplyFormatInEditMode = true)]
        public decimal Length { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Display(Name = "Bredde")]
        [DisplayFormat(DataFormatString = "{0:G29}", ApplyFormatInEditMode = true)]
        public decimal Width { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Display(Name = "Tykkelse")]
        [DisplayFormat(DataFormatString = "{0:G29}", ApplyFormatInEditMode = true)]
        public decimal Thickness { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Display(Name = "Volumen")]
        [DisplayFormat(DataFormatString = "{0:G29}", ApplyFormatInEditMode = true)]
        public decimal Volume { get; set; }

        [Required]
        public string Type { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        [Display(Name = "Pris")]
        [DisplayFormat(DataFormatString = "{0:G29}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        [Display(Name = "Udstyr")]
        public string? Equipment { get; set; }

        [Display(Name = "Link til billede")]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int? BookingId { get; set; }

        public Booking? Booking { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}

