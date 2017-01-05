using System;
using System.Collections.Generic;

namespace TomasosPizza.Models
{
    public partial class BestallningMatratt
    {
        public int MatrattId { get; set; }
        public int BestallningId { get; set; }
        public int Antal { get; set; }

        public virtual Bestallning Bestallning { get; set; }
        public virtual Matratt Matratt { get; set; }
    }
}
