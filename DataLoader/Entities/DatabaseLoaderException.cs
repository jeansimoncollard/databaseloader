using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLoader.Entities
{
    class DatabaseLoaderException : Exception
    {
        public DatabaseLoaderException()
        {
        }

        public DatabaseLoaderException(string message)
        : base(message)
    {
        }

        public DatabaseLoaderException(string message, Exception inner)
        : base(message, inner)
    {
        }
    }
}
