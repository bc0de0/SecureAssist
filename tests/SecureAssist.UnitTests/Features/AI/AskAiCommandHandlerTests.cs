using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using SecureAssist.Application.Features.AI.Commands;
using SecureAssist.Application.Interfaces;
using SecureAssist.Infrastructure.Persistence;
using Xunit;
using FluentAssertions;

namespace SecureAssist.UnitTests.Features.AI;

public class AskAiCommandHandlerTests
{
    private readonly Mock<IAiService> _aiServiceMock;
    private readonly Mock<IAiResponseStorage> _storageMock;
    private readonly AppDbContext _context;
    private readonly AskAiCommandHandler _handler;

    public AskAiCommandHandlerTests()
    {
        _aiServiceMock = new Mock<IAiService>();
        _storageMock = new Mock<IAiResponseStorage>();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _handler = new AskAiCommandHandler(_context, _aiServiceMock.Object, _storageMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnWorkspaceId_WhenCommandIsValid()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var command = new AskAiCommand
        {
            Prompt = "Hello AI",
            WorkspaceId = workspaceId,
            TenantId = Guid.NewGuid(),
            Temperature = 0.7,
            Metadata = new Dictionary<string, object> { { "source", "test" } }
        };

        _aiServiceMock.Setup(x => x.ProcessPromptAsync(It.IsAny<string>()))
            .ReturnsAsync("Mocked AI Response");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(workspaceId);
        var interaction = await _context.AIInteractions.FirstOrDefaultAsync();
        interaction.Should().NotBeNull();
        interaction!.Prompt.Should().Be("Hello AI");
        
        _aiServiceMock.Verify(x => x.ProcessPromptAsync("Hello AI"), Times.Once);
    }
}
