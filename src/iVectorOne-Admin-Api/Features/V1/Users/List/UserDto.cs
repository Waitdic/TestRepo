﻿namespace iVectorOne_Admin_Api.Features.V1.Users.List
{
    public record UserDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = "";

    }
}