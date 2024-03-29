﻿using Riok.Mapperly.Abstractions;

using ShareLoc.Client.App.Models;
using ShareLoc.Client.DAL.Entities;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Client.App.Services;

[Mapper]
public sealed partial class ModelMapper
{
	public partial PlaceModel Map(PlaceEntity placeEntity);

	public partial PlaceRequest Map(PlaceModel placeModel);

	public partial GuessModel Map(GuessResponse guessResponse);
}
