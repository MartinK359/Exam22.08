using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using RevueCraftersSystem;
using RevueCraftersSystem.DTOs;
using System.Net;

[TestFixture]
public class RevueApiTests
{
    private RestClient client;
    private static string revueId;

    private const string BaseUrl = "https://d2925tksfvgq8c.cloudfront.net/api";

    [SetUp]
    public void Setup()
    {
        var email = Environment.GetEnvironmentVariable("TEST_EMAIL");
        var password = Environment.GetEnvironmentVariable("TEST_PASSWORD");

        if (string.IsNullOrEmpty(email))
        {
            email = "martinkirilov359@gmail.com"; 
        }
        if (string.IsNullOrEmpty(password))
        {
            password = "pass123"; 
        }

        var tempClient = new RestClient(BaseUrl);

        var authRequest = new RestRequest("/User/Authentication", Method.Post);
        authRequest.AddJsonBody(new
        {
            email = email,
            password = password
        });

        var authResponse = tempClient.Execute(authRequest);
        var authContent = JsonConvert.DeserializeObject<AuthResponse>(authResponse.Content);
        var accessToken = authContent.AccessToken;

        var options = new RestClientOptions(BaseUrl)
        {
            Authenticator = new JwtAuthenticator(accessToken)
        };

        this.client = new RestClient(options);
    }

    [TearDown]
    public void TearDown()
    {
        this.client?.Dispose();
    }

    private class AuthResponse
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }

    [Test, Order(1)]
    public void Test_CreateRevue_WithRequiredFields()
    {
        var request = new RestRequest("/Revue/Create", Method.Post);
        var revueData = new RevueDTO
        {
            Title = "My New Test Revue - " + System.DateTime.Now.Ticks,
            Url = "https://example.com/image.jpg",
            Description = "This is a test description for a new revue."
        };
        request.AddJsonBody(revueData);

        var response = this.client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseData = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);
        Assert.That(responseData.Msg, Is.EqualTo("Successfully created!"));
    }

    [Test, Order(2)]
    public void Test_GetAllRevues_And_StoreLastId()
    {
        var request = new RestRequest("/Revue/All", Method.Get);

        var response = this.client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var revues = JsonConvert.DeserializeObject<List<RevueListItemDTO>>(response.Content);

        Assert.IsNotNull(revues);
        Assert.That(revues.Count, Is.GreaterThan(0));

        revueId = revues.Last().RevueId;
        Assert.IsNotNull(revueId, "Revue ID should not be null after storing.");
    }

    [Test, Order(3)]
    public void Test_EditTheLastRevue()
    {
        var request = new RestRequest("/Revue/Edit", Method.Put);
        request.AddQueryParameter("revueId", revueId);

        var updatedData = new RevueDTO
        {
            Title = "Edited Revue Title",
            Url = "https://example.com/edited.jpg",
            Description = "This is the edited description."
        };
        request.AddJsonBody(updatedData);

        var response = this.client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseData = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);
        Assert.That(responseData.Msg, Is.EqualTo("Edited successfully"));
    }

    [Test, Order(4)]
    public void Test_DeleteTheRevue()
    {
        var request = new RestRequest("/Revue/Delete", Method.Delete);
        request.AddQueryParameter("revueId", revueId);

        var response = this.client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseData = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);
        Assert.That(responseData.Msg, Is.EqualTo("The revue is deleted!"));
    }

    [Test, Order(5)]
    public void Test_CreateRevue_WithMissingRequiredFields()
    {
        var request = new RestRequest("/Revue/Create", Method.Post);
        var revueData = new RevueDTO
        {
            Title = "",
            Description = ""
        };
        request.AddJsonBody(revueData);

        var response = this.client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(6)]
    public void Test_EditNonExistingRevue()
    {
        var request = new RestRequest("/Revue/Edit", Method.Put);
        request.AddQueryParameter("revueId", "nonexistingrevue123");

        var updatedData = new RevueDTO
        {
            Title = "Attempting to Edit",
            Description = "This should fail."
        };
        request.AddJsonBody(updatedData);

        var response = this.client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseData = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);
        Assert.That(responseData.Msg, Is.EqualTo("There is no such revue!"));
    }

    [Test, Order(7)]
    public void Test_DeleteNonExistingRevue()
    {
        var request = new RestRequest("/Revue/Delete", Method.Delete);
        request.AddQueryParameter("revueId", "nonexistingrevue123");

        var response = this.client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseData = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);
        Assert.That(responseData.Msg, Is.EqualTo("There is no such revue!"));
    }
}

