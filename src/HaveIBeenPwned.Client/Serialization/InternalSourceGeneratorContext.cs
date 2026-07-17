// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Serialization;

[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(BreachedAccountHashRange))]
[JsonSerializable(typeof(DomainVerificationRequest))]
[JsonSerializable(typeof(DomainVerificationEmailRequest))]
internal sealed partial class InternalSourceGeneratorContext : JsonSerializerContext;
