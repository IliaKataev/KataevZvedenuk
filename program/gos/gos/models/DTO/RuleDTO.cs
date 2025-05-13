using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.models.DTO
{
    public class RuleDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string Value { get; set; }
        public string ComparisonOperator { get; set; }  // Пример: "=", ">", "<"
        public int NeededTypeId { get; set; }  // ID типа параметра, который должен быть проверен
    }

}
