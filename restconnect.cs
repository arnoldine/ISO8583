using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
namespace ISO8583_Test
{
  public  class restconnect
    {
 public void Lgauth()
    {
        var client = new RestClient("https://192.168.70.18:443/token");
        var request = new RestRequest(Method.POST);
        request.AddHeader("Postman-Token", "7448519c-2c25-4397-bf04-c8c360717624");
        request.AddHeader("cache-control", "no-cache");
        request.AddHeader("Connection", "keep-alive");
        request.AddHeader("content-length", "55");
        request.AddHeader("accept-encoding", "gzip, deflate");
        request.AddHeader("Host", "192.168.70.18:443");
        request.AddHeader("Accept", "*/*");
        request.AddHeader("User-Agent", "PostmanRuntime/7.11.0");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("undefined", "grant_type=password&username=gh96007&password=net%40ASL", ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);
    }
        public object getbalance()
        {
            var client = new RestClient("https://192.168.70.18:443/api/accounts/GetAccountBalance");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "1d4c72c5-0d2d-4922-9e48-0ea9213ad202");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-length", "486");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("Host", "192.168.70.18:443");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.11.0");
            request.AddHeader("Authorization", "Bearer AQAAANCMnd8BFdERjHoAwE_Cl-sBAAAAdOBC87sjgkqoVetntL1jWwAAAAACAAAAAAADZgAAwAAAABAAAADuYpNUrFzSUqZAH1wjoPyDAAAAAASAAACgAAAAEAAAADgZ7QedzIWDjd_BdOdw5e-wAAAAOnBaCsKQCvQCCQMDQZlsMb5czYzyUz4dKKLGAgt47GZll7fTCQudEfQjuU66vrzbEge1b18Q1IvDBznETJNHguaWERv8slbNadN9NqDHpCPJdLIFKyeVKaqtQZuHBM0GtFQgN3tYLjklfHTg3jwIC8uLHxjKJG2efqIZKFlNULv-RuQMzCkI03pde7xrQiMPY2xlxoENtCxSCgbUWVQ268wa_9te2n99CEAJhBiwC6AUAAAAjcgCOR_JBZd2rqrQ5-hfOisAzPU");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"ContractNumber\":\"2000100611001\",\n\"access_token\": \"AQAAANCMnd8BFdERjHoAwE_Cl-sBAAAAt_nTDavdu0WLqdjQsftTNAAAAAACAAAAAAADZgAAwAAAABAAAAC1PRfR0qVb1XeC3blrDgvKAAAAAASAAACgAAAAEAAAAJ3EmIRSRTn0qhC4ODNjOJm4AAAAOS65MmINf5wRtn-TflRKHnmpJJi-_QcFxnvq0u-9ld3v8lQbSjr6fpCSMH5MjAYmWlx2_t0dmVwYLKdZ9RUxYsLVzgdCt7o6Su2NNo8mPGyctKi1MIz9PtreruE_xSj99vdA2ZmlbpcoUAi0Lp7O16AgFp1YXl_tIgTGJnYYqYP3cpf2giIILhn0oI2vvbL3vOHgaKpI0cczLSTy1g6OuG_ZFMqDO9brH45bzILpZ6AoXgcVumDuoxQAAAAqZfwNuop11gUaoNqcq23M5gVreQ\",\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
    }
   
}
