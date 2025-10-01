using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.MongoDb.Entities.FormInstance
{
    public class FieldValidationError
    {
        public string FieldId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
