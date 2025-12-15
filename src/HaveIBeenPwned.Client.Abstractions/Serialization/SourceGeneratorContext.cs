// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using BreachDetailsModel = HaveIBeenPwned.Client.Abstractions.BreachDetails;
using BreachHeaderModel = HaveIBeenPwned.Client.Abstractions.BreachHeader;
using DomainBreachesModel = HaveIBeenPwned.Client.Abstractions.DomainBreaches;
using PastesModel = HaveIBeenPwned.Client.Abstractions.Pastes;
using PwnedPasswordModel = HaveIBeenPwned.Client.Abstractions.PwnedPassword;
using StealerLogsByEmailDomainModel = HaveIBeenPwned.Client.Abstractions.StealerLogsByEmailDomain;
using SubscribedDomainModel = HaveIBeenPwned.Client.Abstractions.SubscribedDomain;
using SubscriptionStatusModel = HaveIBeenPwned.Client.Abstractions.SubscriptionStatus;

namespace HaveIBeenPwned.Client.Abstractions.Serialization;

/// <summary>
/// Provides metadata for the <see cref="BreachDetailsModel"/>, <see cref="BreachHeaderModel"/>, <see cref="PastesModel"/>, <see cref="PwnedPasswordModel"/>, <see cref="DomainBreachesModel"/>, <see cref="SubscribedDomainModel"/>, and <see cref="StealerLogsByEmailDomainModel"/> types that is relevant to JSON serialization.
/// </summary>
[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(SubscriptionStatusModel))]
[JsonSerializable(typeof(BreachDetailsModel))]
[JsonSerializable(typeof(BreachDetailsModel[]))]
[JsonSerializable(typeof(BreachHeaderModel))]
[JsonSerializable(typeof(BreachHeaderModel[]))]
[JsonSerializable(typeof(PastesModel))]
[JsonSerializable(typeof(PastesModel[]))]
[JsonSerializable(typeof(PwnedPasswordModel))]
[JsonSerializable(typeof(DomainBreachesModel))]
[JsonSerializable(typeof(SubscribedDomainModel))]
[JsonSerializable(typeof(SubscribedDomainModel[]))]
[JsonSerializable(typeof(StealerLogsByEmailDomainModel))]
[JsonSerializable(typeof(string[]))]
public sealed partial class SourceGeneratorContext : JsonSerializerContext;
