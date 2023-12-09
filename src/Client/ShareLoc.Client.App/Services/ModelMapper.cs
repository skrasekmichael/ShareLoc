using Riok.Mapperly.Abstractions;

using ShareLoc.Client.App.Models;
using ShareLoc.Client.DAL.Entities;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Client.App.Services;

[Mapper]
public sealed partial class ModelMapper
{
	[MapProperty(nameof(PlaceEntity.CratedUTC), nameof(PlaceModel.CreatedUTC))]
	public partial PlaceModel Map(PlaceEntity placeEntity);

	public partial PlaceRequest Map(PlaceModel placeModel);
}
