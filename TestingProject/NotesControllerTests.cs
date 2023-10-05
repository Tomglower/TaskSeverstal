using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Task3.Controllers;
using Task3.Data;
using Task3.Models;

namespace TestingProject.Tests
{
    [TestClass]
    public class NotesControllerTests
    {
        [TestMethod]
        public void GetNotes()
        {
            var dbContextMock = new Mock<FakeDbContext>();
            var testData = new List<Note>
            {
                new Note { Id = 1, Title = "Note 1", Description = "Description 1" },
                new Note { Id = 2, Title = "Note 2", Description = "Description 2" },
            };
            var dbSetMock = new Mock<DbSet<Note>>();

            dbSetMock.As<IQueryable<Note>>().Setup(m => m.Provider).Returns(testData.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Note>>().Setup(m => m.Expression).Returns(testData.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Note>>().Setup(m => m.ElementType).Returns(testData.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Note>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            dbContextMock.Setup(db => db.Notes).Returns(dbSetMock.Object);

            var memoryCacheMock = new Mock<IMemoryCache>();

            var controller = new NotesController(dbContextMock.Object, memoryCacheMock.Object);

            var result = controller.GetNotes();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = (OkObjectResult)result;
            var notes = okResult.Value as IEnumerable<Note>;

            Assert.IsNotNull(notes);
            Assert.AreEqual(testData.Count, notes.Count());
        }
       

        [TestMethod]
        public void AddNote_InvalidNote()
        {
            var dbContextMock = new Mock<FakeDbContext>();
            var memoryCacheMock = new Mock<IMemoryCache>();
            var controller = new NotesController(dbContextMock.Object, memoryCacheMock.Object);
            Note invalidNote = null;

            var result = controller.AddNote(invalidNote);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var badRequestResult = (BadRequestObjectResult)result;
            Assert.AreEqual("Invalid data", badRequestResult.Value);

        }
        [TestMethod]
        public void AddNote_ValidNote()
        {
            var dbContextMock = new Mock<FakeDbContext>();
            var memoryCacheMock = new Mock<IMemoryCache>();
            var controller = new NotesController(dbContextMock.Object, memoryCacheMock.Object);
            var validNote = new Note { Id = 1, Title = "Valid Note", Description = "Valid Description" };

            dbContextMock.Setup(db => db.Notes.Add(validNote)).Verifiable();
            dbContextMock.Setup(db => db.SaveChanges()).Verifiable();

            var result = controller.AddNote(validNote);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));

            var createdAtRouteResult = (CreatedAtRouteResult)result;
            Assert.AreEqual("GetNote", createdAtRouteResult.RouteName);
            Assert.AreEqual(1, createdAtRouteResult.RouteValues["id"]);
            Assert.AreSame(validNote, createdAtRouteResult.Value);
        }
        [TestMethod]
        public void ChangeNote_ValidNote()
        {
            var dbContextMock = new Mock<FakeDbContext>();
            var memoryCacheMock = new Mock<IMemoryCache>();
            var controller = new NotesController(dbContextMock.Object, memoryCacheMock.Object);

            var existingNote = new Note { Id = 1, Title = "Existing Note", Description = "Existing Description",CreatedAt = "123123",UpdatedAt="123123" };
            dbContextMock.Setup(db => db.Notes.Find(1)).Returns(existingNote);

            var updatedNote = new Note { Id = 1, Title = "Updated Note", Description = "Updated Description", CreatedAt = "123123", UpdatedAt = "123123" };

            var result = controller.ChangeNote(1, updatedNote);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            Assert.AreEqual(updatedNote.Title, existingNote.Title);
            Assert.AreEqual(updatedNote.Description, existingNote.Description);
        }

        [TestMethod]
        public void ChangeNote_InvalidNote()
        {
            var dbContextMock = new Mock<FakeDbContext>();
            var memoryCacheMock = new Mock<IMemoryCache>();
            var controller = new NotesController(dbContextMock.Object, memoryCacheMock.Object);
            var existingNote = new Note { Id = 1, Title = "Existing Note", Description = "Existing Description" };

            dbContextMock.Setup(db => db.Notes.Find(1)).Returns(existingNote);

            var invalidNote = new Note { Id = 2, Title = "Invalid Note", Description = "Invalid Description" };

            var result = controller.ChangeNote(1, invalidNote);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        }
        [TestMethod]
        public void DeleteNote_ExistingNote()
        {
            var dbContextMock = new Mock<FakeDbContext>();
            var memoryCacheMock = new Mock<IMemoryCache>();
            var controller = new NotesController(dbContextMock.Object, memoryCacheMock.Object);
            var existingNote = new Note { Id = 1, Title = "Existing Note", Description = "Existing Description" };

            dbContextMock.Setup(db => db.Notes.Find(1)).Returns(existingNote);

            var result = controller.DeleteNote(1);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public void DeleteNote_NonExistingNote()
        {
            var dbContextMock = new Mock<FakeDbContext>();
            var memoryCacheMock = new Mock<IMemoryCache>();
            var controller = new NotesController(dbContextMock.Object, memoryCacheMock.Object);

            dbContextMock.Setup(db => db.Notes.Find(1)).Returns((Note)null);


            var result = controller.DeleteNote(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

        }
        


    }
}
