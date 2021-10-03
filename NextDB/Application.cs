using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NextDB.Services;
using NextDB.Settings;

namespace NextDB
{
    public static class Application
    {
        public static ServiceProvider ServiceProvider;
        public static Configuration Configuration;

        public static unsafe void BuildApplication(delegate*unmanaged<string, string, string, int> extensionCallback)
        {
            var services = new ServiceCollection();
            services.AddSingleton(new LoggerService("NextDB", true));
            services.AddSingleton(new CallbackService(extensionCallback));
            ServiceProvider = services.BuildServiceProvider();
        }

        public static bool ReadConfiguration()
        {
            var nextDbDirectory = Path.Combine(Environment.CurrentDirectory, Constants.ModName);
            var configurationFile = Path.Combine(nextDbDirectory, Constants.ConfigurationFile);
            
            using var scope = ServiceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<LoggerService>();
            
            logger.LogInformation($"Reading configuration file at location: {nextDbDirectory}");
            if (!File.Exists(configurationFile))
            {
                logger.LogWarning($"Configuration File does not exist! Creating demo one...");
                var demoConfiguration = new Configuration
                {
                    MaxRetries = 3,
                    Username = "admin",
                    Password = "ultrasecretpassword",
                    Port = 3070,
                    ServerAddress = "localhost",
                    SqlQueries = new List<SqlQuery>
                    {
                        new SqlQuery
                        {
                            QueryPosInputs =
                            {
                                new QueryPosOperations
                                {
                                    ElementPos = 1,
                                    Operation = Operation.None
                                },
                                new QueryPosOperations
                                {
                                    ElementPos = 2,
                                    Operation = Operation.Time
                                }
                            },
                            QueryPosOutputs =
                            {
                                new QueryPosOperations
                                {
                                    ElementPos = 1,
                                    Operation = Operation.None
                                }
                            },
                            Statement = "SELECT playerId FROM players WHERE playerUid = $1 AND lastTimePlayed < $2"
                        }
                    }
                };

                try
                {
                    var json = JsonSerializer.Serialize(demoConfiguration);
                    File.WriteAllText(configurationFile, json);
                    logger.LogInformation(
                        "Example configuration file has been created! This file will be loaded this time. Make sure to edit it to your needs!");
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to create example json file!");
                }
            }

            try
            {
                var json = File.ReadAllText(configurationFile);
                Configuration = JsonSerializer.Deserialize<Configuration>(json) ??
                                new Configuration();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to read configuration file! You can find more information here: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}