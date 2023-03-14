using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EasyIngressos.Entity;
using EasyIngressos.Managers;
using ProjetoCores_1._0;
using static EasyIngressos.Conection.EventsSynchronize;

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
                    var responseObject = JsonSerializer.Deserialize<LoginResponse>(responseContent);
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

        public static async Task PostEvent()
        {
            using var client = new HttpClient();
            client.BaseAddress = Opptions.BaseURL;
            EventsSynchronize synchronize = SqliteConn.SelectEventSynchronize();

            HttpContent lContent =  new StringContent(JsonSerializer.Serialize(synchronize), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Opptions.Token}");

            try
            {
                var response = await client.PostAsync(Synchronize.Route, lContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<EventsResponse>(responseContent);

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
                    var responseObject = JsonSerializer.Deserialize<EventsResponse>(responseContent);

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
                    var responseObject = JsonSerializer.Deserialize<EventResponse>(responseContent);

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

    public class EventsSynchronize
    {
        public class EventSynchronize
        {
            public int id { get; set; } = new int();
            public TicketResponse[] tickets { get; set; }

            public class TicketResponse
            {
                public int id { get; set; } = new int();
                public string status { get; set; }
            }
        }

        public EventSynchronize[] events { get; set; } = new EventSynchronize[1];

    }

    public class Synchronize
    { 
        public static string Route => "system/synchronize";
    }

    public class EventsRequest
    {
        public bool userValidated { get; set; }
        public static string Route => "system/events";
        public static string Query => "";

    }

    public class LoginResponse
    {
        public LoginData data { get; set; }
    }

    public class EventsResponse
    {
        public EventData[] data { get; set; }
    }

    public class EventResponse
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

