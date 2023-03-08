using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EasyIngressos.Entity;
using EasyIngressos.Managers;

namespace EasyIngressos.Conection
{
    public class ConectionServer
    {
        public static async Task AuthenticateBackend()
        {
            using var client = new HttpClient();
            client.BaseAddress = Opptions.BaseURL;

            try
            {
                var response = await client.PostAsync(LoginRequest.Route, Opptions.Content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<LoginRespose>(responseContent);
                    Opptions.Token = responseObject.data.token;

                    await GetEvent();

                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                client.Dispose();
            }

        }

        public static async Task GetEvent()
        {
            using var client = new HttpClient();
            client.BaseAddress = Opptions.BaseURL;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Opptions.Token}");

            try
            {
                var response = await client.GetAsync(EventsRequest.Route);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<EventsRespose>(responseContent);

                    AppManager.EventsData = responseObject.data;

                    for (int i = 0; i < responseObject.data.Length; i++)
                    {
                        AppManager.EventsData[i].address.event_id = responseObject.data[i].id;
                    }


                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                client.Dispose();
            }


        }

        public static async Task GetEvent(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = Opptions.BaseURL;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Opptions.Token}");

            try
            {
                var response = await client.GetAsync($"{EventsRequest.Route}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<EventRespose>(responseContent);

                    AppManager.SelectedEventData = responseObject.data;
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                client.Dispose();
            }


        }
    }
    public class Opptions
    {
        public static Uri BaseURL { get; set; } = new Uri("https://api.easy.risestudio.com.br/");
        public static string Token;
        public static HttpContent Content = new StringContent(JsonSerializer.Serialize(new Login()), Encoding.UTF8, "application/json");
    }

    #region Resquests and Responses

    public class Login
    {
        public string email { get; set; } = "master@risestudio.com.br";
        public string password { get; set; } = "master*123@";
    }
    public class LoginRequest
    {
        public bool userValidated { get; set; }
        public static string Route => "admin/login";
        public static string Query => "";
    }

    public class EventsRequest
    {
        public string token { get; set; }
        public bool userValidated { get; set; }
        public static string Route => "system/events";
        public static string Query => "";

    }

    public class LoginRespose
    {
        public LoginData data { get; set; }
    }

    public class EventsRespose
    {
        public EventData[] data { get; set; }
    }

    public class EventRespose
    {
        public EventData data { get; set; }
    }

    public class LoginData
    {
        public string token { get; set; }
        public string type { get; set; }
        public string refreshToken { get; set; }
    }
    #endregion
}

