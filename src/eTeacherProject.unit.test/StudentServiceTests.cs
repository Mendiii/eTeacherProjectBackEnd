using eTeacherProject.Interfaces;
using eTeacherProject.Models.Entities;
using eTeacherProject.Services;
using Moq;

namespace eTeacherProject.unit.test;

[TestClass]
public class StudentServiceTests
{
    private Mock<IGenericRepository<Student>> _repoMock = null!;
    private StudentService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repoMock = new Mock<IGenericRepository<Student>>();
        _service = new StudentService(_repoMock.Object);
    }

    [TestMethod]
    public async Task GetAllStudentsAsync_ForwardsCallAndReturnsResult()
    {
        var expected = new[] { new Student { Id = 1, Name = "A" }, new Student { Id = 2, Name = "B" } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await _service.GetAllStudentsAsync();

        CollectionAssert.AreEqual(expected.ToArray(), result.ToArray());
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public async Task GetStudentAsync_ForwardsCallAndReturnsValue()
    {
        var student = new Student { Id = 3, Name = "S" };
        _repoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(student);

        var result = await _service.GetStudentAsync(3);

        Assert.AreEqual(student, result);
        _repoMock.Verify(r => r.GetByIdAsync(3), Times.Once);
    }

    [TestMethod]
    public async Task AddStudentAsync_CallsAddAndReturnsEntityId()
    {
        var student = new Student { Id = 0, Name = "New" };
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask)
            .Callback<Student>(s => s.Id = 7);

        var id = await _service.AddStudentAsync(student);

        Assert.AreEqual(7, id);
        _repoMock.Verify(r => r.AddAsync(It.Is<Student>(s => s.Name == "New" && s.Id == 7)), Times.Once);
    }

    [TestMethod]
    public async Task UpdateStudentAsync_ReturnsFalseWhenIdMismatch()
    {
        var student = new Student { Id = 4, Name = "X" };

        var result = await _service.UpdateStudentAsync(5, student);

        Assert.IsFalse(result);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateStudentAsync_CallsUpdateAndReturnsTrueWhenIdsMatch()
    {
        var student = new Student { Id = 4, Name = "X" };

        var result = await _service.UpdateStudentAsync(4, student);

        Assert.IsTrue(result);
        _repoMock.Verify(r => r.UpdateAsync(It.Is<Student>(s => s == student)), Times.Once);
    }
}