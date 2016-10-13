using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiniSharp
{
    [Serializable]
    public class Output
    {
        [Serializable]
        public class Result
        {
            public string message;
        }

        public List<Result> errors;
        public List<Result> warnings;
        public List<Result> messages;

        public bool success;
        public bool skipped;

        public Output()
        {
            errors = new List<Result>();
            warnings = new List<Result>();
            messages = new List<Result>();
        }
    }
}
