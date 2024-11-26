using System.Text.Json;
using AchievementsPlatform.Dtos;
using AchievementsPlatform.Entities;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeamento de AccountGame para AccountGameDto
        CreateMap<AccountGame, AccountGameDto>()
            .ForMember(dest => dest.AchievementsCount, opt => opt.MapFrom(src => src.UserAchievements ?? 0))
            .ForMember(dest => dest.TotalAchievements, opt => opt.MapFrom(src => src.TotalAchievements ?? 0));

        CreateMap<AccountGameDto, AccountGame>()
            .ForMember(dest => dest.UserAchievements, opt => opt.MapFrom(src => src.AchievementsCount))
            .ForMember(dest => dest.TotalAchievements, opt => opt.MapFrom(src => src.TotalAchievements));

        // Mapeamento de GameFeedbackDto para GameFeedback
        CreateMap<GameFeedbackDto, GameFeedback>();
        CreateMap<GameFeedback, GameFeedbackDto>();
    }
}