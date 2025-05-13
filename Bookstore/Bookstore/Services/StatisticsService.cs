using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Bookstore.Services
{
    public class StatisticsDto
    {
        public int BooksCount { get; set; }
        public int OffersCount { get; set; }
        public int UsersCount { get; set; }
    }

    public class StatisticsService
    {
        private readonly HubConnection _connection;

        public StatisticsService(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task StartAsync()
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
        }

        public async Task<StatisticsDto> GetStatisticsAsync()
        {
            try
            {
                await StartAsync();
                
                StatisticsDto result = null;
                
                // Set up the handler to receive the statistics
                _connection.On<StatisticsDto>("ReceiveStatistics", (statistics) => {
                    result = statistics;
                });
                
                // Request the statistics
                await _connection.InvokeAsync("GetStatistics");
                
                // Wait briefly for the response
                int attempts = 0;
                while (result == null && attempts < 10)
                {
                    await Task.Delay(100);
                    attempts++;
                }
                
                return result ?? new StatisticsDto 
                { 
                    BooksCount = 0, 
                    OffersCount = 0, 
                    UsersCount = 0 
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching statistics: {ex.Message}");
                return new StatisticsDto 
                { 
                    BooksCount = 0, 
                    OffersCount = 0, 
                    UsersCount = 0 
                };
            }
        }
    }
}
