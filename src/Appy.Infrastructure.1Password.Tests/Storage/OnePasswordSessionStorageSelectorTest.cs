using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.Infrastructure.OnePassword.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace Appy.Infrastructure.OnePassword.Tests.Storage;

public class OnePasswordSessionStorageSelectorTest
{
    public class GetCurrent
    {
        public class WhenRunningOnWindows
        {
            [Fact]
            public async Task ShouldGetSessionFromEnvSessionStorage()
            {
                var session = AppyOnePasswordSession.New(
                    organization: "appy",
                    environment: "DEV",
                    vault: "Development",
                    sessionToken: "FakeToken");

                var fixture = new Fixture()
                    .WithWindowsPlatform()
                    .WithEnvSessionStorageSession(session);

                var sut = fixture.CreateSubject();

                var result = await sut.GetCurrent();

                fixture.FileSessionStorage.VerifyGetNotCalled();
                result.Should().BeEquivalentTo(session);
            }
        }

        public class WhenNotRunningOnWindowsAndOrganizationIsEmpty
        {
            [Fact]
            public async Task ShouldTryGetSessionFromEnvSessionStorageAndThenFromFileSessionStorage()
            {
                var empty = AppyOnePasswordSession.Empty();
                var session = AppyOnePasswordSession.New(
                    organization: string.Empty,
                    environment: "DEV",
                    vault: "Development",
                    sessionToken: "FakeToken");

                var fixture = new Fixture()
                    .WithoutWindowsPlatform()
                    .WithEnvSessionStorageSession(empty)
                    .WithFileSessionStorageSession(session);

                var sut = fixture.CreateSubject();

                var result = await sut.GetCurrent();

                result.Should().BeEquivalentTo(session);
            }
        }
    }

    public class UpdateSession
    {
        public class WhenRunningOnWindows
        {
            [Fact]
            public async Task ShouldUpdateEnvSessionStorage()
            {
                var session = AppyOnePasswordSession.Empty();

                var fixture = new Fixture()
                    .WithWindowsPlatform();

                var sut = fixture.CreateSubject();

                await sut.Update(session);

                fixture.EnvSessionStorage.VerifyUpdateWith(session);
                fixture.FileSessionStorage.VerifyUpdateNotCalled();
            }
        }

        public class WhenNotRunningOnWindows
        {
            [Fact]
            public async Task ShouldUpdateFileSessionStorage()
            {
                var session = AppyOnePasswordSession.Empty();

                var fixture = new Fixture()
                    .WithoutWindowsPlatform();

                var sut = fixture.CreateSubject();

                await sut.Update(session);

                fixture.EnvSessionStorage.VerifyUpdateNotCalled();
                fixture.FileSessionStorage.VerifyUpdateWith(session);
            }
        }
    }

    class Fixture
    {
        public Fixture()
        {
            PlatformInfo = new PlatformInformationMock();
            FileSessionStorage = new OnePasswordSessionStorageMock();
            EnvSessionStorage = new OnePasswordSessionStorageMock();
        }

        public PlatformInformationMock PlatformInfo { get; }
        public OnePasswordSessionStorageMock EnvSessionStorage { get; }
        public OnePasswordSessionStorageMock FileSessionStorage { get; }

        public IOnePasswordSessionStorage CreateSubject()
        {
            return new OnePasswordSessionStorageSelector(PlatformInfo.Object, EnvSessionStorage.Object, FileSessionStorage.Object);
        }

        public Fixture WithWindowsPlatform()
        {
            PlatformInfo.SetupRunningOnWindows(true);
            return this;
        }

        public Fixture WithoutWindowsPlatform()
        {
            PlatformInfo.SetupRunningOnWindows(false);
            return this;
        }

        public Fixture WithEnvSessionStorageSession(AppyOnePasswordSession session)
        {
            EnvSessionStorage.SetupGetAndReturns(session);
            return this;
        }

        public Fixture WithFileSessionStorageSession(AppyOnePasswordSession session)
        {
            FileSessionStorage.SetupGetAndReturns(session);
            return this;
        }
    }
}