using Kohlhaas.Application.DTO.Project;
using Kohlhaas.Application.Services;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Enums;
using Kohlhaas.Domain.Interfaces;
using Moq;
using NUnit.Framework;

namespace Kohlhaas.Application.Tests.Services;

/// <summary>
/// Unit tests for ProjectService
/// These tests use Moq to mock dependencies (repositories, UnitOfWork)
/// </summary>
[TestFixture]
public class ProjectServiceTests
{
    // Mocks - declared at class level for reuse across tests
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IRepository<Project>> _mockProjectRepo;
    private Mock<IRepository<User>> _mockUserRepo;
    private Mock<IRepository<ProjectMember>> _mockProjectMemberRepo;
    
    // Service under test
    private ProjectService _projectService;
    
    // Test data - reusable across tests
    private User _adminUser;
    private User _projectManagerUser;
    private User _regularUser;
    private Project _testProject;
    private ProjectMember _ownerMember;

    /// <summary>
    /// Runs before each test - sets up fresh mocks and test data
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Initialize mocks
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProjectRepo = new Mock<IRepository<Project>>();
        _mockUserRepo = new Mock<IRepository<User>>();
        _mockProjectMemberRepo = new Mock<IRepository<ProjectMember>>();
        
        _mockProjectRepo
            .Setup(repo => repo.Insert(It.IsAny<Project>()))
            .ReturnsAsync((Project p) =>
            {
                if (p.Id == Guid.Empty)
                {
                    p = new Project
                    {
                        Id = Guid.NewGuid(),
                        Name = p.Name,
                        Description = p.Description,
                        CurrentPhase = p.CurrentPhase,
                        OwnerId = p.OwnerId,
                        Code = p.Code,
                        CreatedAt = p.CreatedAt,
                    };
                }

                return p;
            });
        
        // Configure UnitOfWork to return repository mocks
        _mockUnitOfWork
            .Setup(uow => uow.GetRepository<Project>())
            .Returns(_mockProjectRepo.Object);
        
        _mockUnitOfWork
            .Setup(uow => uow.GetRepository<User>())
            .Returns(_mockUserRepo.Object);
        
        _mockUnitOfWork
            .Setup(uow => uow.GetRepository<ProjectMember>())
            .Returns(_mockProjectMemberRepo.Object);
        
        // Create service with mocked dependencies
        _projectService = new ProjectService(_mockUnitOfWork.Object);
        
        // Initialize test data
        SetupTestData();
    }

    /// <summary>
    /// Helper method to create consistent test data
    /// </summary>
    private void SetupTestData()
    {
        // Admin user
        _adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@test.com",
            FirstName = "Admin",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true,
            PasswordHash = "hashed_password",
            Department = "IT"
        };

        // Project Manager user
        _projectManagerUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "pm@test.com",
            FirstName = "Project",
            LastName = "Manager",
            Role = UserRole.ProjectManager,
            IsActive = true,
            PasswordHash = "hashed_password",
            Department = "Engineering"
        };

        // Regular user
        _regularUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            FirstName = "Regular",
            LastName = "User",
            Role = UserRole.User,
            IsActive = true,
            PasswordHash = "hashed_password",
            Department = "Engineering"
        };

        // Test project
        _testProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Test Project",
            Description = "Test Description",
            Code = "TEST-001",
            CurrentPhase = VModelPhase.UserRequirements,
            OwnerId = _projectManagerUser.Id,
            OwnerName = _projectManagerUser.FullName,
            IsArchived = false,
            DocumentsCount = 0,
            CreatedById = _adminUser.Id,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        // Owner membership
        _ownerMember = new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = _testProject.Id,
            UserId = _projectManagerUser.Id,
            Role = ProjectRole.Manager,
            Email = _projectManagerUser.Email,
            IsOwner = true,
            IsActive = true,
            JoinedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Runs after each test - cleanup if needed
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // Cleanup resources if needed
        _mockUnitOfWork = null;
        _mockProjectRepo = null;
        _mockUserRepo = null;
        _mockProjectMemberRepo = null;
        _projectService = null;
    }

    #region CreateProjectAsync Tests

    /// <summary>
    /// Test successful project creation by admin
    /// This demonstrates the full pattern for testing service methods
    /// </summary>
    [Test]
    public async Task CreateProjectAsync_WithAdminUser_ShouldCreateProjectSuccessfully()
    {
        // ========== ARRANGE ==========
        // Define what we're testing
        var creatorId = _adminUser.Id;
        var ownerId = _projectManagerUser.Id;
        
        var createDto = new CreateProjectDto
        {
            Name = "New Project",
            Description = "New Description",
            Code = "NEW-001",
            CurrentPhase = VModelPhase.UserRequirements,
            OwnerId = ownerId,
            StartDate = DateTime.UtcNow,
            TargetEndDate = DateTime.UtcNow.AddMonths(6)
        };

        // Mock: GetById returns users
        _mockUserRepo
            .Setup(repo => repo.GetById(creatorId))
            .ReturnsAsync(_adminUser);
        
        _mockUserRepo
            .Setup(repo => repo.GetById(ownerId))
            .ReturnsAsync(_projectManagerUser);

        // Mock: Project code doesn't exist (unique check)
        _mockProjectRepo
            .Setup(repo => repo.AsQueryable())
            .Returns(new List<Project>().AsQueryable());

        // Mock: Insert returns the tracked entity
        _mockProjectRepo
            .Setup(repo => repo.Insert(It.IsAny<Project>()))
            .ReturnsAsync((Project p) => p); // Return the same project passed in

        // Mock: ProjectMember insert
        _mockProjectMemberRepo
            .Setup(repo => repo.Insert(It.IsAny<ProjectMember>()))
            .ReturnsAsync((ProjectMember pm) => pm);

        // Mock: Commit succeeds
        _mockUnitOfWork
            .Setup(uow => uow.Commit())
            .ReturnsAsync(1);

        // ========== ACT ==========
        // Execute the method under test
        var result = await _projectService.CreateProjectAsync(creatorId, createDto);

        // ========== ASSERT ==========
        // Verify the result
        Assert.That(result.IsSuccess, Is.True, "Expected successful result");
        Assert.That(result.Value, Is.Not.Null, "Expected project DTO to be returned");
        Assert.That(result.Value.Name, Is.EqualTo(createDto.Name), "Project name should match");
        Assert.That(result.Value.Code, Is.EqualTo(createDto.Code), "Project code should match");

        // Verify method calls - ensures the service behaved correctly
        _mockUserRepo.Verify(
            repo => repo.GetById(creatorId), 
            Times.Once, 
            "Should load creator user exactly once");
        
        _mockUserRepo.Verify(
            repo => repo.GetById(ownerId), 
            Times.Once, 
            "Should load owner user exactly once");

        _mockProjectRepo.Verify(
            repo => repo.Insert(It.Is<Project>(p => 
                p.Name == createDto.Name && 
                p.Code == createDto.Code)), 
            Times.Once, 
            "Should insert project with correct properties");

        _mockProjectMemberRepo.Verify(
            repo => repo.Insert(It.Is<ProjectMember>(pm => 
                pm.UserId == ownerId && 
                pm.IsOwner == true && 
                pm.Role == ProjectRole.Manager)), 
            Times.Once, 
            "Should create owner membership");

        _mockUnitOfWork.Verify(
            uow => uow.Commit(), 
            Times.Once, 
            "Should commit transaction exactly once");
    }

    /// <summary>
    /// Test authorization failure - non-admin cannot create projects
    /// </summary>
    [Test]
    public async Task CreateProjectAsync_WithNonAdminUser_ShouldReturnForbidden()
    {
        // ========== ARRANGE ==========
        var creatorId = _regularUser.Id;
        var ownerId = _projectManagerUser.Id;
        
        var createDto = new CreateProjectDto
        {
            Name = "New Project",
            Description = "New Description",
            Code = "NEW-001",
            CurrentPhase = VModelPhase.UserRequirements,
            OwnerId = ownerId
        };

        // Mock: GetById returns users
        _mockUserRepo
            .Setup(repo => repo.GetById(creatorId))
            .ReturnsAsync(_regularUser);
        
        _mockUserRepo
            .Setup(repo => repo.GetById(ownerId))
            .ReturnsAsync(_projectManagerUser);

        // ========== ACT ==========
        var result = await _projectService.CreateProjectAsync(creatorId, createDto);

        // ========== ASSERT ==========
        Assert.That(result.IsSuccess, Is.False, "Expected failure for non-admin");
        Assert.That(result.Error.Code, Is.EqualTo("Error.Authorization.Forbidden"));

        // Verify no insert or commit was called
        _mockProjectRepo.Verify(
            repo => repo.Insert(It.IsAny<Project>()), 
            Times.Never, 
            "Should not insert project when forbidden");
        
        _mockUnitOfWork.Verify(
            uow => uow.Commit(), 
            Times.Never, 
            "Should not commit when forbidden");
    }

    /// <summary>
    /// Test validation failure - duplicate project code
    /// </summary>
    [Test]
    public async Task CreateProjectAsync_WithDuplicateCode_ShouldReturnError()
    {
        // ========== ARRANGE ==========
        var creatorId = _adminUser.Id;
        var ownerId = _projectManagerUser.Id;
        
        var createDto = new CreateProjectDto
        {
            Name = "New Project",
            Description = "New Description",
            Code = "TEST-001", // Duplicate code
            CurrentPhase = VModelPhase.UserRequirements,
            OwnerId = ownerId
        };

        // Mock: GetById returns users
        _mockUserRepo
            .Setup(repo => repo.GetById(creatorId))
            .ReturnsAsync(_adminUser);
        
        _mockUserRepo
            .Setup(repo => repo.GetById(ownerId))
            .ReturnsAsync(_projectManagerUser);

        // Mock: Project code already exists
        var existingProjects = new List<Project> { _testProject }.AsQueryable();
        _mockProjectRepo
            .Setup(repo => repo.AsQueryable())
            .Returns(existingProjects);

        // ========== ACT ==========
        var result = await _projectService.CreateProjectAsync(creatorId, createDto);

        // ========== ASSERT ==========
        Assert.That(result.IsSuccess, Is.False, "Expected failure for duplicate code");
        Assert.That(result.Error.Code, Is.EqualTo("Error.Project.ProjectCodeNotUnique"));

        // Verify no insert or commit
        _mockProjectRepo.Verify(
            repo => repo.Insert(It.IsAny<Project>()), 
            Times.Never);
        
        _mockUnitOfWork.Verify(
            uow => uow.Commit(), 
            Times.Never);
    }

    /// <summary>
    /// Test edge case - creator user not found
    /// </summary>
    [Test]
    public async Task CreateProjectAsync_WithInvalidCreatorId_ShouldReturnUserNotFound()
    {
        // ========== ARRANGE ==========
        var invalidCreatorId = Guid.NewGuid();
        var ownerId = _projectManagerUser.Id;
        
        var createDto = new CreateProjectDto
        {
            Name = "New Project",
            Code = "NEW-001",
            OwnerId = ownerId
        };

        // Mock: Creator not found, owner found
        _mockUserRepo
            .Setup(repo => repo.GetById(invalidCreatorId))
            .ReturnsAsync((User)null);
        
        _mockUserRepo
            .Setup(repo => repo.GetById(ownerId))
            .ReturnsAsync(_projectManagerUser);

        // ========== ACT ==========
        var result = await _projectService.CreateProjectAsync(invalidCreatorId, createDto);

        // ========== ASSERT ==========
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error.Code, Is.EqualTo("Error.User.NotFound"));
        
        _mockProjectRepo.Verify(
            repo => repo.Insert(It.IsAny<Project>()), 
            Times.Never);
    }

    #endregion

    #region GetProjectAsync Tests

    /// <summary>
    /// Test successful project retrieval by admin
    /// </summary>
    [Test]
    public async Task GetProjectAsync_ByAdmin_ShouldReturnProject()
    {
        // ========== ARRANGE ==========
        var userId = _adminUser.Id;
        var projectId = _testProject.Id;

        _mockUserRepo
            .Setup(repo => repo.GetById(userId))
            .ReturnsAsync(_adminUser);
        
        _mockProjectRepo
            .Setup(repo => repo.GetById(projectId))
            .ReturnsAsync(_testProject);

        // ========== ACT ==========
        var result = await _projectService.GetProjectAsync(userId, projectId);

        // ========== ASSERT ==========
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Id, Is.EqualTo(projectId));
        Assert.That(result.Value.Name, Is.EqualTo(_testProject.Name));

        // Admin should bypass membership check
        _mockProjectMemberRepo.Verify(
            repo => repo.Any(It.IsAny<System.Linq.Expressions.Expression<Func<ProjectMember, bool>>>()), 
            Times.Never, 
            "Admin should not require membership check");
    }

    /// <summary>
    /// Test project retrieval by project member
    /// </summary>
    [Test]
    public async Task GetProjectAsync_ByProjectMember_ShouldReturnProject()
    {
        // ========== ARRANGE ==========
        var userId = _regularUser.Id;
        var projectId = _testProject.Id;

        _mockUserRepo
            .Setup(repo => repo.GetById(userId))
            .ReturnsAsync(_regularUser);
        
        _mockProjectRepo
            .Setup(repo => repo.GetById(projectId))
            .ReturnsAsync(_testProject);

        // Mock: User is an active member
        _mockProjectMemberRepo
            .Setup(repo => repo.Any(It.IsAny<System.Linq.Expressions.Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync(true);

        // ========== ACT ==========
        var result = await _projectService.GetProjectAsync(userId, projectId);

        // ========== ASSERT ==========
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Id, Is.EqualTo(projectId));
        
        _mockProjectMemberRepo.Verify(
            repo => repo.Any(It.IsAny<System.Linq.Expressions.Expression<Func<ProjectMember, bool>>>()), 
            Times.Once, 
            "Should check membership for non-admin");
    }

    /// <summary>
    /// Test authorization failure - non-member cannot view project
    /// </summary>
    [Test]
    public async Task GetProjectAsync_ByNonMember_ShouldReturnForbidden()
    {
        // ========== ARRANGE ==========
        var userId = _regularUser.Id;
        var projectId = _testProject.Id;

        _mockUserRepo
            .Setup(repo => repo.GetById(userId))
            .ReturnsAsync(_regularUser);
        
        _mockProjectRepo
            .Setup(repo => repo.GetById(projectId))
            .ReturnsAsync(_testProject);

        // Mock: User is NOT a member
        _mockProjectMemberRepo
            .Setup(repo => repo.Any(It.IsAny<System.Linq.Expressions.Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync(false);

        // ========== ACT ==========
        var result = await _projectService.GetProjectAsync(userId, projectId);

        // ========== ASSERT ==========
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error.Code, Is.EqualTo("Error.Authorization.Forbidden"));
    }

    #endregion

    #region Helper Test Patterns

    /// <summary>
    /// Example: Testing with multiple test cases using TestCase attribute
    /// </summary>
    [TestCase(UserRole.Admin, true)]
    [TestCase(UserRole.ProjectManager, false)]
    [TestCase(UserRole.Reviewer, false)]
    [TestCase(UserRole.User, false)]
    public async Task CreateProjectAsync_WithDifferentRoles_ShouldRespectAuthorization(
        UserRole role, 
        bool expectedSuccess)
    {
        // ========== ARRANGE ==========
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Role = role,
            IsActive = true,
            PasswordHash = "hash",
            Department = "Test"
        };

        var createDto = new CreateProjectDto
        {
            Name = "Test",
            Code = "TEST",
            OwnerId = _projectManagerUser.Id
        };

        _mockUserRepo.Setup(r => r.GetById(user.Id)).ReturnsAsync(user);
        _mockUserRepo.Setup(r => r.GetById(_projectManagerUser.Id)).ReturnsAsync(_projectManagerUser);
        _mockProjectRepo.Setup(r => r.AsQueryable()).Returns(new List<Project>().AsQueryable());
        _mockProjectRepo.Setup(r => r.Insert(It.IsAny<Project>())).ReturnsAsync((Project p) => p);
        _mockProjectMemberRepo.Setup(r => r.Insert(It.IsAny<ProjectMember>())).ReturnsAsync((ProjectMember pm) => pm);
        _mockUnitOfWork.Setup(u => u.Commit()).ReturnsAsync(1);

        // ========== ACT ==========
        var result = await _projectService.CreateProjectAsync(user.Id, createDto);

        // ========== ASSERT ==========
        Assert.That(result.IsSuccess, Is.EqualTo(expectedSuccess));
    }

    #endregion
}
