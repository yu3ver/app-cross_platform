﻿using Autofac;
using Integreat.Shared.Services.Persistance;
using NUnit.Framework;

namespace Integreat.Shared.Test.Services.Loader
{
    [TestFixture]
    internal class PageLoaderTest
    {
        private IContainer _container;
        private PersistenceService _persistenceService;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            _container = Platform.Setup.CreateContainer();
            Assert.True(_container.TryResolve(out _persistenceService), "PersistenceService not found");
            _persistenceService.Init();
        }

        [SetUp]
        public void Setup()
        {
        }


        [TearDown]
        public void Tear()
        {
        }
    }
}