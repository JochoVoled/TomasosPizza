using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TomasosPizza.Models
{
    public partial class Kund
    {
        public Kund()
        {
            Bestallning = new HashSet<Bestallning>();
        }
        [Key]
        public int KundId { get; set; }
        [RegularExpression(@"^[a-zåäöA-ZÖÅÖ\s]+$",ErrorMessage = "Namnet kan bara bestå av bokstäver A-Ö samt blanksteg")]
        public string Namn { get; set; }
        public string Gatuadress { get; set; }
        [RegularExpression(@"^\d{5}$",ErrorMessage = "Postnumret måste bestå av fem siffror, utan blanksteg")]
        public string Postnr { get; set; }
        public string Postort { get; set; }
        [EmailAddress(ErrorMessage = "Din epostadress motsvarar inte förväntad formattering")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Ditt lösenord motsvarar inte förväntad formattering (i utveckling)")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Ett användarnamn måste uppges")]
        public string AnvandarNamn { get; set; }

        [Required(ErrorMessage = "Ett lösenord måste anges")]
        [StringLength(30,MinimumLength = 8,ErrorMessage = "Lösenord ska vara mellan 8 och 30 tecken")]
        public string Losenord { get; set; }

        public virtual ICollection<Bestallning> Bestallning { get; set; }
    }
}
