using eTeacherProject.Interfaces;
using eTeacherProject.Models.Entities;
using eTeacherProject.Services;
using Moq;

namespace eTeacherProject.unit.test;

[TestClass]
public class CourseServiceTests
{
    private Mock<IGenericRepository<Course>> _repoMock = null!;
    private CourseService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repoMock = new Mock<IGenericRepository<Course>>();
        _service = new CourseService(_repoMock.Object);
    }

    [TestMethod]
    public async Task GetAllCoursesAsync_ForwardsCallAndReturnsResult()
    {
        var expected = new[] { new Course { Id = 1, Title = "A" }, new Course { Id = 2, Title = "B" } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await _service.GetAllCoursesAsync();

        CollectionAssert.AreEqual(expected.ToArray(), result.ToArray());
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public async Task GetCourseAsync_ForwardsCallAndReturnsValue()
    {
        var course = new Course { Id = 5, Title = "X" };
        _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(course);

        var result = await _service.GetCourseAsync(5);

        Assert.AreEqual(course, result);
        _repoMock.Verify(r => r.GetByIdAsync(5), Times.Once);
    }

    [TestMethod]
    public async Task AddCourseAsync_CallsAddAndReturnsEntityId()
    {
        var course = new Course { Id = 0, Title = "New" };
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Course>()))
            .Returns(Task.CompletedTask)
            .Callback<Course>(c => c.Id = 42);

        var id = await _service.AddCourseAsync(course);

        Assert.AreEqual(42, id);
        _repoMock.Verify(r => r.AddAsync(It.Is<Course>(c => c.Title == "New" && c.Id == 42)), Times.Once);
    }

    [TestMethod]
    public async Task UpdateCourseAsync_ReturnsFalseWhenIdMismatch()
    {
        var course = new Course { Id = 10, Title = "T" };

        var result = await _service.UpdateCourseAsync(11, course);

        Assert.IsFalse(result);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Course>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateCourseAsync_CallsUpdateAndReturnsTrueWhenIdsMatch()
    {
        var course = new Course { Id = 10, Title = "T" };

        var result = await _service.UpdateCourseAsync(10, course);

        Assert.IsTrue(result);
        _repoMock.Verify(r => r.UpdateAsync(It.Is<Course>(c => c == course)), Times.Once);
    }
}