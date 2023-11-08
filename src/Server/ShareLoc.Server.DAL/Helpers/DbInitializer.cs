using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;

namespace ShareLoc.Server.DAL.Helpers;

public sealed class DbInitializer
{
	private readonly IAmazonDynamoDB _client;

	public DbInitializer(IAmazonDynamoDB client)
	{
		_client = client;
	}

	public Task CreateTablesAsync(CancellationToken ct)
	{
		return CreateTablePlacesAsync(ct);
	}

	private async Task CreateTablePlacesAsync(CancellationToken ct)
	{
		var tableName = "Places";
		if (await IsTableCreatedAsync(tableName, ct))
			return;

		var request = new CreateTableRequest
		{
			AttributeDefinitions = new List<AttributeDefinition>()
			{
				new AttributeDefinition
				{
					AttributeName = "Id",
					AttributeType = "S"
				}
			},
			KeySchema = new List<KeySchemaElement>
			{
				new KeySchemaElement
				{
					AttributeName = "Id",
					KeyType = "HASH" //Partition key
				},
				/*
				new KeySchemaElement
				{
					AttributeName = "Id",
					KeyType = "RANGE" //Sort key
				}
				*/
			},
			ProvisionedThroughput = new ProvisionedThroughput
			{
				ReadCapacityUnits = 5,
				WriteCapacityUnits = 6
			},
			TableName = tableName
		};

		await _client.CreateTableAsync(request, ct);
		await WaitUntilTableReadyAsync(request.TableName, ct);
	}

	private async Task WaitUntilTableReadyAsync(string tableName, CancellationToken ct)
	{
		var statusActive = false;
		// Wait until table is created.
		while (!statusActive)
		{
			await Task.Delay(2000, ct);
			statusActive = await IsTableCreatedAsync(tableName, ct);
		}
	}

	private async Task<bool> IsTableCreatedAsync(string tableName, CancellationToken ct)
	{
		try
		{
			var res = await _client.DescribeTableAsync(new DescribeTableRequest(tableName), ct);
			return res.Table.TableStatus == "ACTIVE";
		}
		catch (ResourceNotFoundException)
		{
			// DescribeTable is eventually consistent. So you might get resource not found.
			return false;
		}
	}

	private async Task ClearTableAsync(string tableName, CancellationToken ct)
	{
		var scanRequest = new ScanRequest
		{
			TableName = tableName,
			ProjectionExpression = "Id"
		};

		do
		{
			var scanResponse = await _client.ScanAsync(scanRequest, ct);

			foreach (var item in scanResponse.Items)
			{
				var deleteRequest = new DeleteItemRequest()
				{
					TableName = tableName,
					Key = item
				};

				await _client.DeleteItemAsync(deleteRequest, ct);
			}

			scanRequest.ExclusiveStartKey = scanResponse.LastEvaluatedKey;
		} while (scanRequest.ExclusiveStartKey is not null);
	}
}
