using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

using Newtonsoft.Json;

using ShareLoc.Server.DAL.Entities;

namespace ShareLoc.Server.DAL.Converters;

public class GuessListConverter : IPropertyConverter
{
	public object FromEntry(DynamoDBEntry entry)
	{
		Primitive? primitive = entry as Primitive;
		if (primitive is null || string.IsNullOrEmpty(primitive.Value as string)) throw new ArgumentNullException();

		var guesses = JsonConvert.DeserializeObject<List<Guess>>((primitive.Value as string)!);
		if (guesses is null) throw new ArgumentException();

		return guesses;
	}

	public DynamoDBEntry ToEntry(object value)
	{
		List<Guess>? guesses = value as List<Guess>;
		if (guesses == null) throw new ArgumentNullException();

		string guessesJson = JsonConvert.SerializeObject(guesses);

		DynamoDBEntry entry = new Primitive
		{
			Value = guessesJson
		};

		return entry;
	}
}
