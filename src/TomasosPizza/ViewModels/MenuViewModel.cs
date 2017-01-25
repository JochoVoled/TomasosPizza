using System.Collections.Generic;
using TomasosPizza.Models;

namespace TomasosPizza.ViewModels
{
    public class MenuViewModel
    {
        public List<Matratt> Menu { get; set; } = new List<Matratt>();
        public List<BestallningMatratt> Order { get; set; } = new List<BestallningMatratt>();
        public Kund Kund { get; set; }
    }
}
