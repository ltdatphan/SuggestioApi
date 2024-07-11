using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestioApi.Helpers.CustomReturns
{
    public class UserRelationship
    {
        public bool IsFollowingCurrentUser = false;
        public bool IsFollowedByCurrentUser = false;
        public UserRelationship(bool IsFollowingCurrentUser = false, bool IsFollowedByCurrentUser = false)
        {
            this.IsFollowingCurrentUser = IsFollowingCurrentUser;
            this.IsFollowedByCurrentUser = IsFollowedByCurrentUser;
        }
    }
}