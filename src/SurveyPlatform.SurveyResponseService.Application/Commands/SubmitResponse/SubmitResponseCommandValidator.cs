using FluentValidation;

namespace SurveyPlatform.SurveyResponseService.Application.Commands.SubmitResponse;

public class SubmitResponseCommandValidator : AbstractValidator<SubmitResponseCommand>
{
    public SubmitResponseCommandValidator()
    {
        RuleFor(x => x.SurveyId).NotEmpty();
        RuleFor(x => x.Answers).NotEmpty().WithMessage("At least one answer is required");
        RuleForEach(x => x.Answers).ChildRules(answer =>
        {
            answer.RuleFor(a => a.QuestionId).NotEmpty();
        });
    }
}
