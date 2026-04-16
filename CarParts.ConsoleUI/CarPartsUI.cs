using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.Models;

namespace CarParts.ConsoleUI
{
    public class CarPartsUI
    {
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task RunMainMenuAsync()
        {
            Console.WriteLine("welcome! This app helps you manage you car's components");

            while(true)
            {
                PrintMainMenu();
                string input = Console.ReadLine();
                switch(input)
                {
                    case "get-all":
                        await PrintAllCarParts();
                        break;

                    case "get-id":
                        await PrintCarPart();
                        break;

                    case "add":
                        await AddCarPart();
                        break;

                    case "remove":
                        await RemoveCarPart();
                        break;

                    case "update":
                        await UpdateCarPart();
                        break;

                    case "login":
                        await Login();
                        break;

                    case "register":
                        await Register();
                        break;

                    case "renew":
                        await _apiClient.RenewAccessTokenAsync();
                        break;

                    case "logout":
                        await _apiClient.LogoutAsync();
                        break;

                    case "auth-state":
                        PrintAuthState();
                        break;


                    case "exit":
                        Environment.Exit(0);
                        return;
                }
            }
        }

        private async Task PrintAllCarParts()
        {
            var carParts = await _apiClient.GetAllAsync();
            if (carParts.Count == 0)
            {
                Console.WriteLine("list is currenlty empty");
            }

            foreach (var carPart in carParts)
            {
                Console.WriteLine($"id: {carPart.Id}, Name: {carPart.Name}");
                Console.WriteLine($"installed at km: {carPart.InstalledAtKm}KM, recommended: {carPart.MaxKmLifetime}KM");
                Console.WriteLine($"installed at date: {carPart.InstalledAtDate}, recommended: {FormatLifespan(carPart.MaxMonthsLifetime)}");
            }

            Pause();
        }

        private async Task PrintCarPart()
        {
            Guid id = ConsoleInputReader.InputGuid("give item id: ");
            var carPart = await _apiClient.GetByIdAsync(id);

            if (carPart == null)
            {
                Console.WriteLine("no component found");
                return;
            }

            Console.WriteLine("Found component");
            Console.WriteLine($"name: {carPart.Name}");
            Console.WriteLine($"installed at km: {carPart.InstalledAtKm}KM, recommended: {carPart.MaxKmLifetime}KM");
            Console.WriteLine($"installed at date: {carPart.InstalledAtDate}, recommended: {FormatLifespan(carPart.MaxMonthsLifetime)}");

            Pause();
        }

        private async Task AddCarPart()
        {
            Console.WriteLine("please complete the information below: ");
            string name = ConsoleInputReader.InputString("name: ");
            int installedAtKm = ConsoleInputReader.InputInt("installed at km: ");
            int maxKmLifetime = ConsoleInputReader.InputInt("max km lifetime: ");
            DateOnly installedAtDate = ConsoleInputReader.InputDate("give date as input (format is yyyy-mm=dd)");
            int maxMonthsLifetime = ConsoleInputReader.InputInt("max months lifetime: ");

            var carPart = new CarPartRequest(
                name,
                installedAtKm,
                maxKmLifetime,
                installedAtDate,
                maxMonthsLifetime
            );

            bool isSuccess = await _apiClient.AddAsync(carPart);

            Console.WriteLine(isSuccess ? "component added succesfully" : "couldnt add component");
            Pause();
        }

        private async Task UpdateCarPart()
        {
            Guid id = ConsoleInputReader.InputGuid("give id of item to be updated: ");

            Console.WriteLine("please complete the information below: ");
            string name = ConsoleInputReader.InputString("new name: ");
            int installedAtKm = ConsoleInputReader.InputInt("new installed at km: ");
            int maxKmLifetime = ConsoleInputReader.InputInt("new max km lifetime: ");
            DateOnly installedAtDate = ConsoleInputReader.InputDate("new give date as input (format is yyyy-mm=dd)");
            int maxMonthsLifetime = ConsoleInputReader.InputInt("new max months lifetime: ");

            var carPart = new CarPartRequest(
                name,
                installedAtKm,
                maxKmLifetime,
                installedAtDate,
                maxMonthsLifetime
            );

            bool isSuccess = await _apiClient.UpdateAsync(id, carPart);

            Console.WriteLine(isSuccess ? "component updated succesfully" : "couldnt update component");
            Pause();
        }

        private async Task RemoveCarPart()
        {
            Guid id = ConsoleInputReader.InputGuid("give id of item to be removed: ");

            bool isSuccess = await _apiClient.RemoveAsync(id);

            Console.WriteLine(isSuccess ? "component removed succesfully" : "couldnt remove component");
            Pause();
        }

        private string FormatLifespan(int totalMaxMonths)
        {
            int years = totalMaxMonths / 12;
            int months = totalMaxMonths % 12;

            if (years > 0 && months > 0)
            {
                return $"{years} years, {months} months";
            }
            else if (years > 0)
            {
                return $"{years} years";
            }
            return $"{months} months";
        }

        private async Task Login()
        {
            string username = ConsoleInputReader.InputString("username: ");
            string password = ConsoleInputReader.InputString("password: ");

            await _apiClient.LoginAsync(username, password);

            Pause();
        }

        private async Task Register()
        {
            string username = ConsoleInputReader.InputString("username: ");
            string email = ConsoleInputReader.InputString("email: ");
            string password = ConsoleInputReader.InputString("password: ");

            await _apiClient.RegisterAsync(username, email, password);

            Pause();
        }

        private void PrintAuthState()
        {
            var (accessToken, refreshToken) = _apiClient.GetAuthState();

            Console.WriteLine("AUTH STATE");

            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("empoty access token");
            }
            else
            {
                Console.WriteLine($"access token: {accessToken}");
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                Console.WriteLine("empoty refresh token");
            }
            else
            {
                Console.WriteLine($"refresh token: {refreshToken}");
            }

            Pause();
        }

        private void Pause()
        {
            Console.WriteLine("press any key to go on...");
            Console.ReadKey(true);
        }

        private void PrintMainMenu()
        {
            Console.WriteLine("AUTH=================================");
            Console.WriteLine("login: login user");
            Console.WriteLine("register: register user");
            Console.WriteLine("renew: renew access token");
            Console.WriteLine("logout: logout user");
            Console.WriteLine("auth-state: see current tokens");
            Console.WriteLine("API==================================");
            Console.WriteLine("get-all: get all car parts");
            Console.WriteLine("get-id: get car part by guid");
            Console.WriteLine("add: add car part");
            Console.WriteLine("update: update car part");
            Console.WriteLine("remove: remove car part");      
            Console.WriteLine("exit: close program");
            Console.WriteLine("pick your choice out of the ones above: ");
        }
    }
}
