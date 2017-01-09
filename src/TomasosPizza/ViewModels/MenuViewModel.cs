using System.Collections.Generic;
using TomasosPizza.Models;

namespace TomasosPizza.ViewModels
{
    public class MenuViewModel
    {
        public List<Matratt> Menu { get; set; } = new List<Matratt>();
        public Dictionary<Matratt, int> Order { get; set; } = new Dictionary<Matratt, int>();
    }
}
