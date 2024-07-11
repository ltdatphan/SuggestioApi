using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Dtos.CuratedList;

namespace SuggestioApi.Helpers
{
    public class ListQueryObject
    {
        public bool WithItems { get; set; } = false;
    }
}