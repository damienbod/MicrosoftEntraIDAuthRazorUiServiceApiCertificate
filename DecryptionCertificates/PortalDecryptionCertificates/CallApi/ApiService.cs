﻿using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace PortalDecryptionCertificates.CallApi;

public class ApiService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public ApiService(IHttpClientFactory clientFactory,
        ITokenAcquisition tokenAcquisition,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<JArray> GetApiDataAsync()
    {
        var client = _clientFactory.CreateClient();

        var scope = _configuration["CallApi:ScopeForAccessToken"];
        if (scope == null) throw new ArgumentNullException(nameof(scope));

        var uri = _configuration["CallApi:ApiBaseAddress"];
        if (uri == null) throw new ArgumentNullException(nameof(uri));

        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync([scope]);

        client.BaseAddress = new Uri(uri);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("weatherforecast");
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JArray.Parse(responseContent);

            return data;
        }

        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
    }
}