// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using OnlineShop.Library.UserManagementService.Models;
using OnlineShop.Library.Data;

namespace OnlineShop.IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var yaroslav = userMgr.FindByNameAsync("yaroslav").Result;
                    if (yaroslav == null)
                    {
                        yaroslav = new ApplicationUser
                        {
                            UserName = "yaroslav",
                            FirstName = "Yaroslav",
                            LastName = "Radelytskyi",
                            Email = "yaroslav.radelytskyi@gmail.com",
                            EmailConfirmed = true,
                            DefaultAddress = new Library.Common.Models.Address
                            {
                                City = "Kharkiv",
                                Country = "Ukraine",
                                PostalCode = "00-350",
                                AddressLine1 = "Heroiv Truda 21",
                                AddressLine2 = "34"
                            },
                            DeliveryAddress = new Library.Common.Models.Address
                            {
                                City = "Kharkiv",
                                Country = "Ukraine",
                                PostalCode = "00-350",
                                AddressLine1 = "MyStreet 34"
                            }
                        };
                        var result = userMgr.CreateAsync(yaroslav, "Pass_123").Result;


                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(yaroslav, new Claim[]{
                                new Claim(JwtClaimTypes.Name, "Yaroslav Radelytskyi"),
                                new Claim(JwtClaimTypes.GivenName, "Yaroslav"),
                                new Claim(JwtClaimTypes.FamilyName, "Radelytskyi"),
                                new Claim(JwtClaimTypes.WebSite, "https://github.com/youngradik"),
                            }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("Yaroslav has been created");
                    }
                    else
                    {
                        Log.Debug("Yaroslav already exists");

                        if (yaroslav.DefaultAddress == null)
                        {
                            yaroslav.DefaultAddress = new Library.Common.Models.Address()
                            {
                                City = "Kharkiv",
                                Country = "Ukraine",
                                PostalCode = "00-350",
                                AddressLine1 = "Heroiv Truda 21",
                                AddressLine2 = "34"
                            };
                        }

                        if (yaroslav.DeliveryAddress == null)
                        {
                            yaroslav.DeliveryAddress = new Library.Common.Models.Address()
                            {
                                City = "Kharkiv",
                                Country = "Ukraine",
                                PostalCode = "00-350",
                                AddressLine1 = "MyStreet 34"
                            };
                        }

                        var result = userMgr.UpdateAsync(yaroslav).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("Yaroslav has been updated");
                    }
                }
            }
        }
    }
}
