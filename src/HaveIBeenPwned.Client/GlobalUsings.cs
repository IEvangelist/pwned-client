// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Net.Http.Json;
global using System.Net.Mime;
global using System.Reflection;
global using System.Security.Cryptography;
global using System.Text;
global using System.Web;
global using HaveIBeenPwned.Client;
global using HaveIBeenPwned.Client.Abstractions;
global using HaveIBeenPwned.Client.Extensions;
global using HaveIBeenPwned.Client.Http;
global using HaveIBeenPwned.Client.Options;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using static HaveIBeenPwned.Client.Http.HttpClientNames;