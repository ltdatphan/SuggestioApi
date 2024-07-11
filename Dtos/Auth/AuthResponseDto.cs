using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuggestioApi.Dtos.Account
{
    public class AuthResponseDto
    {
        public string? FirstName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? ProfileImgUrl { get; set; }
    }
}