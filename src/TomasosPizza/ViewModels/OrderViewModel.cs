using System.ComponentModel.DataAnnotations;

namespace TomasosPizza.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "En postort krävs (är förslag till beställningsadress)")]
        [StringLength(50, ErrorMessage = "Gatuadressen får inte vara längre än 50 tecken")]
        public string Gatuadress { get; set; }

        [Required(ErrorMessage = "Ett postnummer krävs (är förslag till beställningsadress)")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Postnumret måste bestå av fem siffror, utan blanksteg")]
        public string Postnr { get; set; }

        [Required(ErrorMessage = "En postort krävs (är förslag till beställningsadress)")]
        public string Postort { get; set; }
    }
}
