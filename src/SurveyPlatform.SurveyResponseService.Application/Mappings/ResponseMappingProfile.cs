using AutoMapper;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;

namespace SurveyPlatform.SurveyResponseService.Application.Mappings;

public class ResponseMappingProfile : Profile
{
    public ResponseMappingProfile()
    {
        CreateMap<SurveyResponse, SurveyResponseDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.AnswerCount, o => o.MapFrom(s => s.Answers.Count));

        CreateMap<SurveyResponse, SurveyResponseDetailDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<Answer, AnswerDto>();
    }
}
