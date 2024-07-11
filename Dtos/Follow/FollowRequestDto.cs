using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestioApi.Dtos.Follow
{
    public class FollowRequestDto
    {
        public string CurrentUserId { get; set; }
        public string TargetUserId { get; set; }
    }
}