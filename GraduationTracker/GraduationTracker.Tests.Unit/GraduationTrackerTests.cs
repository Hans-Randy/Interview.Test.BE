using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using GraduationTracker.Models;
using GraduationTracker.Services;
using GraduationTracker.Repositories;
using GraduationTracker.Interfaces;

namespace GraduationTracker.Tests.Unit
{
    [TestClass]
    public class GraduationTrackerTests
    {
        IGraduationTracker _tracker;
        Diploma _diploma;
        IRepository<Student> _studentRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _studentRepository = new StudentRepository();
            IRepository<Diploma> diplomaRepository = new DiplomaRepository();
            IRepository<Requirement> requirementRepository = new RequirementRepository();

            _diploma = diplomaRepository.GetAll().First();
            _tracker = new Services.GraduationTracker(requirementRepository);
        }

        [DataTestMethod]
        [DataRow(1, true, Standing.SummaCumLaude)]
        [DataRow(2, true, Standing.Average)]
        [DataRow(3, true, Standing.Average)]
        [DataRow(4, false, Standing.Remedial)]
        public void SeededStudent_EvaluatedAgainstSeededDiploma_ReturnsExpectedGraduationAndStanding(int studentId, bool expectedGraduation, Standing expectedStanding)
        {
            var student = _studentRepository.GetById(studentId);
            var result = _tracker.Evaluate(_diploma, student);

            Assert.AreEqual(expectedGraduation, result.HasGraduated);
            Assert.AreEqual(expectedStanding, result.Standing);
        }

        /// <summary>
        /// A student with an average mark in the Average range who has not completed all required
        /// courses must NOT graduate, even though their standing alone would allow it.
        /// This test directly verifies that the credits check is part of the graduation decision.
        /// </summary>
        [TestMethod]
        public void StudentWithAverageStandingButInsufficientCredits_ShouldNotGraduate()
        {
            var diploma = new Diploma { Id = 1, CreditsRequired = 4, RequirementIds = [100, 102, 103, 104] };

            // Satisfies only 3 of 4 requirements (missing Physical Education, course id 4).
            // Average = (75+75+75) / 3 = 75 → Average standing, but credits earned = 3 < 4.
            var student = new Student
            {
                Id = 99,
                Courses =
                [
                    new Course { Id = 1, Name = "Math", Mark = 75 },
                    new Course { Id = 2, Name = "Science", Mark = 75 },
                    new Course { Id = 3, Name = "Literature", Mark = 75 }
                ]
            };

            var requirementRepo = StubRepository(
                new Requirement { Id = 100, Name = "Math",               MinimumMark = 50, CourseIds = [1], CreditsAwarded = 1 },
                new Requirement { Id = 102, Name = "Science",            MinimumMark = 50, CourseIds = [2], CreditsAwarded = 1 },
                new Requirement { Id = 103, Name = "Literature",         MinimumMark = 50, CourseIds = [3], CreditsAwarded = 1 },
                new Requirement { Id = 104, Name = "Physical Education", MinimumMark = 50, CourseIds = [4], CreditsAwarded = 1 }
            );

            var tracker = new Services.GraduationTracker(requirementRepo);

            var result = tracker.Evaluate(diploma, student);

            Assert.IsFalse(result.HasGraduated);
            Assert.AreEqual(Standing.Average, result.Standing);
        }

        /// <summary>
        /// A mark that exactly equals the minimum required mark must satisfy the requirement.
        /// This guards against an off-by-one where strict greater-than was used instead of
        /// greater-than-or-equal.
        /// </summary>
        [TestMethod]
        public void StudentWithMarkExactlyAtMinimum_ShouldEarnCredit()
        {
            var diploma = new Diploma { Id = 1, CreditsRequired = 1, RequirementIds = [100] };

            var student = new Student
            {
                Id = 99,
                Courses = [new Course { Id = 1, Name = "Math", Mark = 50 }]
            };

            var requirementRepo = StubRepository(
                new Requirement { Id = 100, Name = "Math", MinimumMark = 50, CourseIds = [1], CreditsAwarded = 1 }
            );

            var tracker = new Services.GraduationTracker(requirementRepo);

            var result = tracker.Evaluate(diploma, student);

            Assert.IsTrue(result.HasGraduated);
            Assert.AreEqual(Standing.Average, result.Standing);
        }

        /// <summary>
        /// Standing boundary: average of exactly 80 should produce MagnaCumLaude, not Average.
        /// </summary>
        [TestMethod]
        public void StudentWithAverageOf80_ShouldBeMagnaCumLaude()
        {
            var diploma = new Diploma { Id = 1, CreditsRequired = 1, RequirementIds = [100] };

            var student = new Student
            {
                Id = 99,
                Courses = [new Course { Id = 1, Name = "Math", Mark = 80 }]
            };

            var requirementRepo = StubRepository(
                new Requirement { Id = 100, Name = "Math", MinimumMark = 50, CourseIds = [1], CreditsAwarded = 1 }
            );

            var tracker = new Services.GraduationTracker(requirementRepo);

            var result = tracker.Evaluate(diploma, student);

            Assert.IsTrue(result.HasGraduated);
            Assert.AreEqual(Standing.MagnaCumLaude, result.Standing);
        }

        /// <summary>
        /// Standing boundary: average of exactly 95 should produce SummaCumLaude, not MagnaCumLaude.
        /// </summary>
        [TestMethod]
        public void StudentWithAverageOf95_ShouldBeSummaCumLaude()
        {
            var diploma = new Diploma { Id = 1, CreditsRequired = 1, RequirementIds = [100] };

            var student = new Student
            {
                Id = 99,
                Courses = [new Course { Id = 1, Name = "Math", Mark = 95 }]
            };

            var requirementRepo = StubRepository(
                new Requirement { Id = 100, Name = "Math", MinimumMark = 50, CourseIds = [1], CreditsAwarded = 1 }
            );

            var tracker = new Services.GraduationTracker(requirementRepo);

            var result = tracker.Evaluate(diploma, student);

            Assert.IsTrue(result.HasGraduated);
            Assert.AreEqual(Standing.SummaCumLaude, result.Standing);
        }

        // Lightweight stub so each test controls its own data without touching seed repositories.
        private static IRepository<T> StubRepository<T>(params T[] items) where T : IEntity
            => new InMemoryStubRepository<T>(items);

        private class InMemoryStubRepository<T> : IRepository<T> where T : IEntity
        {
            private readonly T[] _items;
            public InMemoryStubRepository(T[] items) => _items = items;
            public T GetById(int id) => _items.FirstOrDefault(x => x.Id == id);
            public IEnumerable<T> GetAll() => _items;
        }
    }
}
