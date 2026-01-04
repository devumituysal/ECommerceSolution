using App.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Seeds
{
    public static class UserSeed
    {
        public static List<UserEntity> Data => new()
        {
            new UserEntity() { Id = 1, FirstName = "admin", LastName = "admin", Email = "admin@gmail.com", Enabled = true, IsBanned = false, RoleId = 1, Password = "AQAAAAIAAYagAAAAEH0gFVqsVLuAPXM/7gJ6gFdjfL3SYwdVAnCtoeodowDdKYFbnudwySNiePyQ66N7nA==", CreatedAt = DateTime.UtcNow },

            new UserEntity() { Id = 2, FirstName = "seller1", LastName = "seller1", Email = "seller1@gmail.com", Enabled = true, IsBanned = false, RoleId = 2, Password = "AQAAAAIAAYagAAAAEH0gFVqsVLuAPXM/7gJ6gFdjfL3SYwdVAnCtoeodowDdKYFbnudwySNiePyQ66N7nA==", CreatedAt = DateTime.UtcNow },

            new UserEntity() { Id = 3, FirstName = "seller2", LastName = "seller2", Email = "seller2@gmail.com", Enabled = true, IsBanned = false, RoleId = 2, Password = "AQAAAAIAAYagAAAAEH0gFVqsVLuAPXM/7gJ6gFdjfL3SYwdVAnCtoeodowDdKYFbnudwySNiePyQ66N7nA==", CreatedAt = DateTime.UtcNow },

            new UserEntity() { Id = 4, FirstName = "seller3", LastName = "seller3", Email = "seller3@gmail.com", Enabled = true, IsBanned = false, RoleId = 2, Password = "AQAAAAIAAYagAAAAEH0gFVqsVLuAPXM/7gJ6gFdjfL3SYwdVAnCtoeodowDdKYFbnudwySNiePyQ66N7nA==", CreatedAt = DateTime.UtcNow },

            new UserEntity() { Id = 5, FirstName = "seller4", LastName = "seller4", Email = "seller4@gmail.com", Enabled = true, IsBanned = false, RoleId = 2, Password = "AQAAAAIAAYagAAAAEH0gFVqsVLuAPXM/7gJ6gFdjfL3SYwdVAnCtoeodowDdKYFbnudwySNiePyQ66N7nA==", CreatedAt = DateTime.UtcNow },

            new UserEntity() { Id = 6, FirstName = "seller5", LastName = "seller5", Email = "seller5@gmail.com", Enabled = true, IsBanned = false, RoleId = 2, Password = "AQAAAAIAAYagAAAAEH0gFVqsVLuAPXM/7gJ6gFdjfL3SYwdVAnCtoeodowDdKYFbnudwySNiePyQ66N7nA==", CreatedAt = DateTime.UtcNow }
        };
    }
}
