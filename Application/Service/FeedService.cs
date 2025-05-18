using System.Collections.Generic;
using System.Linq;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Ports;

namespace CampusLove.Domain.Services
{
    public class FeedService
    {
        private readonly IAppUserRepository _userRepo;

        public FeedService(IAppUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Construye el feed para el usuario indicado:
        ///   1) Trae candidatos filtrados por repositorio (edad, orientación, ciudad, intereses).
        ///   2) Ordena: primero verificados, luego por # de intereses en común desc.
        /// </summary>
        public List<DtoAppUser> BuildFeed(int currentUserId)
        {
            // 1) Obtener los candidatos ya filtrados
            var candidates = _userRepo.GetFeedCandidates(currentUserId);

            // 2) Ordenar según reglas de negocio
            var ordered = candidates
                // Verified = true primero
                .OrderByDescending(u => u.UserProfile.Verified)
                // Luego los que más intereses en común tengan
                .ThenByDescending(u => u.UserProfile.CommonInterestCount)
                // (Opcional) podrías encadenar más .ThenBy para otros criterios
                .ToList();

            return ordered;
        }
    }
}
