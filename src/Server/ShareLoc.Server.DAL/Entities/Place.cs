﻿using Amazon.DynamoDBv2.DataModel;

using ShareLoc.Server.DAL.Converters;

namespace ShareLoc.Server.DAL.Entities;

[DynamoDBTable("Places")]
public sealed record Place
{
	[DynamoDBHashKey]
	public required Guid Id { get; init; }

	public required double Latitude { get; init; }
	public required double Longitude { get; init; }
	public required string Message { get; init; }
	public required byte[] Image { get; init; }
	public required DateTime TimeStampUTC { get; init; }

	[DynamoDBProperty(typeof(GuessListConverter))]
	public required List<Guess> Guesses { get; init; }
}
