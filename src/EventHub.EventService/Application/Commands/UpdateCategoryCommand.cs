using EventHub.EventService.Application.DTOs;

namespace EventHub.EventService.Application.Commands
{
    public class UpdateCategoryCommand
    {
        public Guid Id { get; set; }

        public UpdateCategoryRequest Request { get; set; } = new();
    }
}
