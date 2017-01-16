using System.ComponentModel.DataAnnotations;

namespace TomasosPizza.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ett användarnamn måste uppges")]
        [StringLength(20, ErrorMessage = "Användarnamn får inte vara längre än 20 tecken")]
        public string AnvandarNamn { get; set; }
        
        [Required(ErrorMessage = "Ett lösenord måste anges")]
        [StringLength(20, ErrorMessage = "Lösenord får inte vara längre än 20 tecken")]
        public string Losenord { get; set; }
    }
}
