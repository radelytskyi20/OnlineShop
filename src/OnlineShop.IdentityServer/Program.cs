// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using OnlineShop.Library.Constants;
using System;
using System.Linq;

namespace OnlineShop.IdentityServer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDocker = environment == EnvironmentNames.Docker;
            LogManager.Setup().LoadConfigurationFromFile(isDocker ? "nlog.Docker.config" : "nlog.config");
            
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Info($"Is docker environment: {isDocker}; Environment name: {environment}.");

            try
            {
                var seed = args.Contains("/seed");
                if (seed)
                {
                    args = args.Except(new[] { "/seed" }).ToArray();
                }

                var host = CreateHostBuilder(args).Build();

                if (seed)
                {
                    logger.Info("Seeding database...");
                    var config = host.Services.GetRequiredService<IConfiguration>();
                    var connectionString = config.GetConnectionString(ConnectionNames.UserConnection);
                    SeedData.EnsureSeedData(connectionString);
                    logger.Info("Done seeding database.");
                    return 0;
                }

                logger.Info("Starting host...");
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                })
                .UseNLog();
    }
}