using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Fishies.Domain
{
    public static class FishExtensions
    {
        public static IEnumerable<FishDTO> ToDTOs(this IEnumerable<Fish> fish) =>
            fish.Select(f => new FishDTO(f));

        public static IEnumerable<Fish> ToDomain(this IEnumerable<FishDTO> fishDTOs, FishSpeciesDatabase db) =>
            fishDTOs.Select(dto => new Fish(dto, db));
    }
}