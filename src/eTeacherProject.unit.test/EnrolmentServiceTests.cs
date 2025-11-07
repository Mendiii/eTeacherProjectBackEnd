using eTeacherProject.Interfaces;
using eTeacherProject.Models.Entities;
using eTeacherProject.Models.Exceptions;
using eTeacherProject.Services;
using Moq;

namespace eTeacherProject.unit.test;

[TestClass]
public class EnrolmentServiceTests
{
    private Mock<IGenericRepository<Enrolment>> _enrolmentsRepo = null!;
    private Mock<IGenericRepository<Student>> _studentRepo = null!;
    private EnrolmentService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _enrolmentsRepo = new Mock<IGenericRepository<Enrolment>>();
        _studentRepo = new Mock<IGenericRepository<Student>>();
        _service = new EnrolmentService(_enrolmentsRepo.Object, _studentRepo.Object);
    }

    [TestMethod]
    public async Task GetAllEnrolmentsAsync_ForwardsCallAndReturnsResult()
    {
        var expected = new[] { new Enrolment { Id = 1, Title = "A" }, new Enrolment { Id = 2, Title = "B" } };
        _enrolmentsRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await _service.GetAllEnrolmentsAsync();

        CollectionAssert.AreEqual(new List<Enrolment>(expected), new List<Enrolment>(result));
        _enrolmentsRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public async Task GetEnrolmentAsync_ForwardsCallAndReturnsValue()
    {
        var enrol = new Enrolment { Id = 5, Title = "E" };
        _enrolmentsRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(enrol);

        var result = await _service.GetEnrolmentAsync(5);

        Assert.AreEqual(enrol, result);
        _enrolmentsRepo.Verify(r => r.GetByIdAsync(5), Times.Once);
    }

    [TestMethod]
    public async Task AddEnrolmentAsync_SetsRandomIdAndCallsAdd()
    {
        Enrolment? captured = null;
        _enrolmentsRepo
            .Setup(r => r.AddAsync(It.IsAny<Enrolment>()))
            .Returns(Task.CompletedTask)
            .Callback<Enrolment>(e => captured = e);

        var enrol = new Enrolment { Id = 0, Title = "New", StudentsId = new List<int>() };
        var returnedId = await _service.AddEnrolmentAsync(enrol);

        Assert.IsTrue(returnedId >= 1 && returnedId <= 100000);
        Assert.IsNotNull(captured);
        Assert.AreEqual(returnedId, captured!.Id);
        _enrolmentsRepo.Verify(r => r.AddAsync(It.Is<Enrolment>(e => e == captured)), Times.Once);
    }

    [TestMethod]
    public async Task AssignStudentAsync_ThrowsWhenEnrolmentNotFound()
    {
        _enrolmentsRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Enrolment?)null);

        try
        {
            await _service.AssignStudentAsync(1, 10);
            Assert.Fail("Expected GeneralErrorException was not thrown.");
        }
        catch (GeneralErrorException ex)
        {
            Assert.IsNotNull(ex);
            StringAssert.Contains(ex.Message, "Enrolment with id 1 was not found.");
        }
    }

    [TestMethod]
    public async Task AssignStudentAsync_ThrowsWhenStudentNotFound()
    {
        var enrol = new Enrolment { Id = 1, StudentsId = new List<int>() };
        _enrolmentsRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(enrol);
        _studentRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Student?)null);

        try
        {
            await _service.AssignStudentAsync(1, 10);
            Assert.Fail("Expected GeneralErrorException was not thrown.");
        }
        catch (GeneralErrorException ex)
        {
            Assert.IsNotNull(ex);
            StringAssert.Contains(ex.Message, "Student with id 10 was not found.");
        }
    }

    [TestMethod]
    public async Task AssignStudentAsync_ThrowsWhenAlreadyAssigned()
    {
        var enrol = new Enrolment { Id = 1, StudentsId = new List<int> { 10 } };
        _enrolmentsRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(enrol);
        _studentRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Student { Id = 10 });

        try
        {
            await _service.AssignStudentAsync(1, 10);
            Assert.Fail("Expected GeneralErrorException was not thrown.");
        }
        catch (GeneralErrorException ex)
        {
            Assert.IsNotNull(ex);
            StringAssert.Contains(ex.Message, "Student with id 10 is already assigned to enrolment 1.");
        }
    }

    [TestMethod]
    public async Task AssignStudentAsync_AddsStudentAndUpdatesEnrolment()
    {
        var enrol = new Enrolment { Id = 1, StudentsId = new List<int>() };
        _enrolmentsRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(enrol);
        _studentRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Student { Id = 10 });

        _enrolmentsRepo.Setup(r => r.UpdateAsync(It.IsAny<Enrolment>())).Returns(Task.CompletedTask);

        await _service.AssignStudentAsync(1, 10);

        CollectionAssert.Contains(enrol.StudentsId, 10);
        _enrolmentsRepo.Verify(r => r.UpdateAsync(It.Is<Enrolment>(e => e == enrol && e.StudentsId.Contains(10))), Times.Once);
    }

    [TestMethod]
    public async Task UpdateEnrolmentAsync_ReturnsFalseWhenIdMismatch()
    {
        var enrol = new Enrolment { Id = 2, StudentsId = new List<int>() };

        var result = await _service.UpdateEnrolmentAsync(3, enrol);

        Assert.IsFalse(result);
        _enrolmentsRepo.Verify(r => r.UpdateAsync(It.IsAny<Enrolment>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateEnrolmentAsync_CallsUpdateAndReturnsTrueWhenIdsMatch()
    {
        var enrol = new Enrolment { Id = 2, StudentsId = new List<int>() };

        var result = await _service.UpdateEnrolmentAsync(2, enrol);

        Assert.IsTrue(result);
        _enrolmentsRepo.Verify(r => r.UpdateAsync(It.Is<Enrolment>(e => e == enrol)), Times.Once);
    }
}