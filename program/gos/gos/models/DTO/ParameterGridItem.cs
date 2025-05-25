using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.models.DTO
{
    public class ParameterGridItem
    {
        public int Id { get; set; }
        public int TypeId { get; set; } // Для ComboBox
        public string TypeName { get; set; } = string.Empty; // Для отображения
        public string Value { get; set; } = string.Empty;
    }
}
