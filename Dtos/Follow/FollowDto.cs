using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestioApi.Dtos.Follow
{
    public class FollowDto
    {
        public int Id { get; set; }
        public string CurrentUserId { get; set; }
        public string TargetUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}