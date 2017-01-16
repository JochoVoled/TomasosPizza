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

        [Required(ErrorMessage = "Ett namn krävs (krävs till beställning)")]
        [StringLength(100, ErrorMessage = "Ditt namn får inte vara längre än 100 tecken")]
        [RegularExpression(@"^[a-zåäöA-ZÅÄÖ\s]+$",ErrorMessage = "Namnet kan bara bestå av bokstäver A-Ö samt blanksteg")]
        public string Namn { get; set; }

        [Required(ErrorMessage = "En postort krävs (är förslag till beställningsadress)")]
        [StringLength(50, ErrorMessage = "Gatuadressen får inte vara längre än 50 tecken")]
        public string Gatuadress { get; set; }

        [Required(ErrorMessage = "Ett postnummer krävs (är förslag till beställningsadress)")]
        [RegularExpression(@"^\d{5}$",ErrorMessage = "Postnumret måste bestå av fem siffror, utan blanksteg")]
        public string Postnr { get; set; }

        [Required(ErrorMessage = "En postort krävs (är förslag till beställningsadress)")]
        public string Postort { get; set; }

        [EmailAddress(ErrorMessage = "Din epostadress motsvarar inte förväntad formattering")]
        [StringLength(50, ErrorMessage = "Lösenord får inte vara längre än 20 tecken")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Ditt lösenord motsvarar inte förväntad formattering (i utveckling)")]
        [StringLength(50, ErrorMessage = "Telefonnumret får inte vara längre än 50 tecken")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Ett användarnamn måste uppges")]
        [StringLength(20, ErrorMessage = "Användarnamn får inte vara längre än 20 tecken")]
        public string AnvandarNamn { get; set; }

        [Required(ErrorMessage = "Ett lösenord måste anges")]
        [StringLength(20,ErrorMessage = "Lösenord får inte vara längre än 20 tecken")]
        public string Losenord { get; set; }

        public virtual ICollection<Bestallning> Bestallning { get; set; }
    }
}
