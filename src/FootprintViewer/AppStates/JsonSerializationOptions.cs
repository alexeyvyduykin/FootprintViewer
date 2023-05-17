﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace FootprintViewer.AppStates;

public class JsonSerializationOptions
{
    private static readonly JsonSerializerSettings CurrentSettings = new()
    {
        Converters = new List<JsonConverter>() { }
    };

    public static readonly JsonSerializationOptions Default = new();

    private JsonSerializationOptions()
    {
    }

    public JsonSerializerSettings Settings => CurrentSettings;
}