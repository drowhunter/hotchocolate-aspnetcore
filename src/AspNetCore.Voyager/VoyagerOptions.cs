using System;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore.Voyager
{
    public class VoyagerOptions
    {
        private PathString _path = "/voyager";
        private PathString _queryPath = "/";
       
        public PathString Path
        {
            get => _path;
            set
            {
                if (!value.HasValue)
                {
                    throw new ArgumentException(
                        "The path cannot be empty.");
                }

                _path = value;
            }
        }

        public PathString QueryPath
        {
            get => _queryPath;
            set
            {
                if (!value.HasValue)
                {
                    throw new ArgumentException(
                        "The query-path cannot be empty.");
                }

                _queryPath = value;
                
            }
        }

       
    }
}
