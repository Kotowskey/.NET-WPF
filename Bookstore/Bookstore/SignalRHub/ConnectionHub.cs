using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Bookstore.Models;

namespace Bookstore.SignalRHub
{
    //Tej klasy musi byc tylko 1 globalna instancja, bo jak utworzymy 2 to
    //Wtedy będą 2 połączenia do serwera, więc trzeba tą instancje a raczej referencje do
    //niej przekazywac do kolejnych widoków
    //Narazie stworzenie tej instancji jest w pliku App.xaml.cs
    //i przekazywana jest do SingInUp.xaml.cs na starcie
    //a potem SingInUp.xaml.cs przekazuje ją do MainWindow.xaml.cs
    public class ConnectionHub
    {
        private HubConnection _connection;

        public ConnectionHub()
        {
            InitSignalR();
        }

        private async void InitSignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7109/gamehub") //Uwaga port może się różnić u was
                .WithAutomaticReconnect()
                .Build();

            _connection.Reconnecting += (ex) =>
            {
                //Dispatcher.Invoke(() => ConnectionStatus.Text = "Połączenie: ponawianie...");
                return Task.CompletedTask;
            };

            _connection.Reconnected += (id) =>
            {
                //Dispatcher.Invoke(() => ConnectionStatus.Text = $"Połączono ponownie: {id}");
                return Task.CompletedTask;
            };

            _connection.Closed += async (ex) =>
            {
                //Dispatcher.Invoke(() => ConnectionStatus.Text = "Rozłączono.");
                await Task.Delay(2000);
                await _connection.StartAsync();
            };

            try
            {
                await _connection.StartAsync();
                //ConnectionStatus.Text = $"Połączenie: OK (ID: {_connection.ConnectionId})";
            }
            catch (Exception ex)
            {
                //ConnectionStatus.Text = $"Błąd połączenia: {ex.Message}";
            }
        }
        public async Task<bool> Login(string username, string password)
        {
            try
            {
                return await _connection.InvokeAsync<bool>("Login", username, password);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> Register(string email, string username, string firstName, string lastName, string password)
        {
            try
            {
                return await _connection.InvokeAsync<bool>("Register", email, username, firstName, lastName, password);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> IsAdmin()
        {
            try
            {
                return await _connection.InvokeAsync<bool>("IsAdmin");
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<StatisticsModel> GetStatistics()
        {
            try
            {
                return await _connection.InvokeAsync<StatisticsModel>("GetStatistics");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching statistics: {ex.Message}");
                return new StatisticsModel();
            }
        }
        public async Task<Guid> GetUserId()
        {
            try
            {
                return await _connection.InvokeAsync<Guid>("GetUserId");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd pobierania ID użytkownika: {ex.Message}");
                return Guid.Empty;
            }
        }
    }
}
