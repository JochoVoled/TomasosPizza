using System.Collections.Generic;
using TomasosPizza.IdentityModels;
using TomasosPizza.Models;

namespace TomasosPizza.ViewModels
{
    public class AdminViewModel
    {
        public List<Matratt> Matratter { get; set; }
        public List<Produkt> Ingredienser { get; set; }
        public List<Bestallning> Bestallningar { get; set; }
        public List<MatrattTyp> Typer { get; set; }
        public List<IdentityKund> IdentityKunder { get; set; }
    }
}
