using System.Collections.Generic;
using TomasosPizza.Models;

namespace TomasosPizza.ViewModels
{
    public class ModalViewModel
    {
        public Matratt Matratt { get; set; }
        public List<Produkt> Produkter { get; set; }
    }
}
