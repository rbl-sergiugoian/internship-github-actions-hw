using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using CarParts.Common.Models;
using System.Net.Http.Headers;

namespace CarParts.ConsoleUI
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthState _authState;
        private const string BaseUrl = "https://localhost:7153/api/CarParts";
        private const string AuthenticationBaseUrl = "https://localhost:7153/api/Authentication";

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _authState = new AuthState();
        }

        public async Task<CarPartResponse?> GetByIdAsync(Guid id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/{id.ToString()}");

            await HandleStatusCode(response);

            var errorHandlerResponse = await HandleErrors(response);
            if (!errorHandlerResponse)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CarPartResponse>();
        }

        public async Task<List<CarPartResponse>> GetAllAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);

            await HandleStatusCode(response);
            
            var errorHandlerResponse = await HandleErrors(response);
            if (!errorHandlerResponse)
            {
                return new List<CarPartResponse>();
            }

            var carParts = await response.Content.ReadFromJsonAsync<List<CarPartResponse>>();
            return carParts ?? new List<CarPartResponse>();
        }

        public async Task<bool> AddAsync(CarPartRequest carPartRequest)
        {
            AddAuthorizationHeader();

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseUrl, carPartRequest);

            await HandleStatusCode(response);
            return await HandleErrors(response);
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            AddAuthorizationHeader();

            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id.ToString()}");

            await HandleStatusCode(response);

            return await HandleErrors(response);
        }

        public async Task<bool> UpdateAsync(Guid id, CarPartRequest newCarPart)
        {
            AddAuthorizationHeader();

            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id.ToString()}", newCarPart);

            await HandleStatusCode(response);

            return await HandleErrors(response);
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var request = new
            {
                Username = username,
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/register", request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("registration failed");
                return false;
            }

            Console.WriteLine("registraation successful");
            return true;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var request = new
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/login", request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("login failed");
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            _authState.AccessToken = result.AccessToken;
            _authState.RefreshToken = result.RefreshToken;

            Console.WriteLine("login successful");
            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            var request = new
            {
                RefreshToken = _authState.RefreshToken
            };

            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/logout", request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("logout failed");
                return false;
            }

            _authState.AccessToken = string.Empty;
            _authState.RefreshToken = string.Empty;

            Console.WriteLine("loggout successfull");
            return true;
        }

        public async Task<bool> RenewAccessTokenAsync()
        {
            var request = new
            {
                AccessToken = _authState.AccessToken
            };

            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/renew-access-token", request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("renew access token failed");
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            _authState.AccessToken = result.AccessToken;

            Console.WriteLine("token renewd succesfuly");
            return true;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var request = new
            {
                RefreshToken = _authState.RefreshToken
            };

            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/refresh-token", request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("refreshing access token failed");
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            _authState.AccessToken = result.AccessToken;
            _authState.RefreshToken = result.RefreshToken;

            return true;
        }

        public (string AccessToken, string RefreshToken) GetAuthState()
        {
            return (_authState.AccessToken, _authState.RefreshToken);
        }

        private async Task HandleStatusCode(HttpResponseMessage response)
        {
            HttpStatusCode statusCode = response.StatusCode;

            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    Console.WriteLine("200 - request succeeded");
                    break;

                case HttpStatusCode.Created:
                    Console.WriteLine("201 - response succeeded");
                    break;

                case HttpStatusCode.NoContent:
                    Console.WriteLine("204 - no content");
                    break;

                case HttpStatusCode.BadRequest:
                    Console.WriteLine("400 - bad request");
                    break;

                case HttpStatusCode.NotFound:
                    Console.WriteLine("404 - not found");
                    break;

                case HttpStatusCode.Unauthorized:
                    Console.WriteLine("401 - not authorized");
                    break;

                case HttpStatusCode.Forbidden:
                    Console.WriteLine("403 - forbidden");
                    break;

                default:
                    Console.WriteLine("no mapped status code");
                    break;
            }
        }

        private async Task<bool> HandleErrors(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            try
            {
                var validationErrors = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
                if (validationErrors?.Errors != null)
                {
                    foreach (var validationError in validationErrors.Errors)
                    {
                        foreach (var message in validationError.Value)
                        {
                            Console.WriteLine(message);
                        }
                    }

                    return false;
                }
            }
            catch
            {
                Console.WriteLine("failed to parse validation errors");
            }

            return false;
        }

        private void AddAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_authState.AccessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    _authState.AccessToken
                );
            }
        }
    }
}
