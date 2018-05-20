using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    class APIError: Exception
    {
        public string errormessage;
        public APIError(string error)
        {
            errormessage = error;
        }
    }
}
