using Riok.Mapperly.Abstractions;

using ShareLoc.Client.DAL.Entities;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Client.BL;

[Mapper]
public sealed partial class EntityMapper
{
	public partial PlaceRequest PlaceToRequest(PlaceEntity place);
}
