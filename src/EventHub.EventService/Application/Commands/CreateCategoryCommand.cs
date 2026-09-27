using EventHub.EventService.Application.DTOs;

namespace EventHub.EventService.Application.Commands
{
    public class CreateCategoryCommand
    {
        public CreateCategoryRequest Request { get; set; } = new();
    }
}
