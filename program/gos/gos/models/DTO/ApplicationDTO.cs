using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.models.DTO
{
    public class ApplicationDTO
    {
        public int ServiceId { get; set; }
        public int UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ClosureDate { get; set; }
        public DateTime Deadline {  get; set; }
        public ApplicationStatus Status {  get; set; }
        
        public string? Result { get; set; }
        public int ApplicationId { get; set; }


    }
}
