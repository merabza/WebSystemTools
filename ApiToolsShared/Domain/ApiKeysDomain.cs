﻿using System.Collections.Generic;
using System.Linq;
using ApiToolsShared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApiToolsShared.Domain;

public sealed class ApiKeysDomain
{
    private HashSet<ApiKeyAndRemoteIpAddressDomain> ApiKeys { get; } = new();

    public static ApiKeysDomain Create(IConfiguration configuration, ILogger logger)
    {
        var apiKeysSection = configuration.GetSection("ApiKeys");
        var apiKeys = apiKeysSection.Get<ApiKeys>();

        var apiKeysDomain = new ApiKeysDomain();
        var count = apiKeys?.AppSettingsByApiKey?.Count ?? 0;
        logger.LogInformation("ApiKeys count is {count}", count);
        if (apiKeys?.AppSettingsByApiKey is null)
            return apiKeysDomain;
        foreach (var apiKeyByRemoteIpAddressModel in apiKeys.AppSettingsByApiKey)
        {
            var apiKey = apiKeyByRemoteIpAddressModel.ApiKey;
            var remoteIpAddress = apiKeyByRemoteIpAddressModel.RemoteIpAddress;
            logger.LogInformation("ApiKey={apiKey} for {remoteIpAddress}", apiKey, remoteIpAddress);
            if (apiKeyByRemoteIpAddressModel.ApiKey is null || apiKeyByRemoteIpAddressModel.RemoteIpAddress is null)
            {
                logger.LogError("Invalid ApiKey or RemoteIpAddress ApiKey={apiKey} for {remoteIpAddress}", apiKey,
                    remoteIpAddress);
                continue;
            }

            apiKeysDomain.ApiKeys.Add(new ApiKeyAndRemoteIpAddressDomain(apiKeyByRemoteIpAddressModel.ApiKey,
                apiKeyByRemoteIpAddressModel.RemoteIpAddress));
        }

        return apiKeysDomain;
    }

    public ApiKeyAndRemoteIpAddressDomain? AppSettingsByApiKey(string apiKey, string remoteIpAddress)
    {
        return ApiKeys.SingleOrDefault(s => s.ApiKey == apiKey && s.RemoteIpAddress == remoteIpAddress);
    }
}