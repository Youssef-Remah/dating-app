using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var memberData = File.ReadAllText("Data/UserSeedData.json");

        var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

        if (members == null)
        {
            Console.WriteLine("No members in seed data");
            return;
        }

        foreach (var member in members)
        {
            using var hmac = new HMACSHA512();

            var appUser = new AppUser()
            {
                Id = member.Id,
                DisplayName = member.DisplayName,
                Email = member.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key,
                ImageUrl = member.ImageUrl,
                Member = new Member()
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    City = member.City,
                    Country = member.Country,
                    Created = member.Created,
                    Gender = member.Gender,
                    DateOfBirth = member.DateOfBirth,
                    Description = member.Description,
                    ImageUrl = member.ImageUrl,
                    LastActive = member.LastActive,
                }
            };

            appUser.Member.Photos.Add(new Photo()
            {
                Url = member.ImageUrl!,
                MemberId = member.Id,
            });

            context.Users.Add(appUser);
        }

        await context.SaveChangesAsync();
    }
}
