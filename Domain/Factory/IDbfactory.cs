using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.domain.Ports;

namespace SGCI_app.domain.Factory;

public interface IDbfactory
{
    IAppUserRepository CreateAppUserRepository();
    IUserCareerRepository CreateUserCareerRepository();
    ICareerRepository CreateCareerRepository();
    IGenderRepository CreateGenderRepository();
    IUserTypeRepository CreateUserTypeRepository();
    ICityRepository CreateCityRepository();
    ISexualOrientationRepository CreateSexualOrientationRepository();
    IInterestRepository CreateInterestRepository();
    IUserInterestRepository CreateUserInterestRepository();
    IInteractionRepository CreateInteractionRepository();
    IMatchRepository CreateMatchRepository();
}
