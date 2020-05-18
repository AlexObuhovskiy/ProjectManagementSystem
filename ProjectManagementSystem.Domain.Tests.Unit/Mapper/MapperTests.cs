using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;
using Xunit;

namespace ProjectManagementSystem.Domain.Tests.Unit.Mapper
{
    /// <summary>
    /// Test all profiles to correct mapping
    /// </summary>
    public class MapperTests
    {
        private readonly AutoMapper.Mapper _mapper;

        public MapperTests()
        {
            var configuration = new MapperConfigurationExpression();

            try
            {
                var assembly = Assembly.Load($"{nameof(ProjectManagementSystem)}.{nameof(Domain)}");

                var profiles = assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(Profile)) && t.GetConstructor(Type.EmptyTypes) != null)
                    .Select(p => (Profile) Activator.CreateInstance(p));

                foreach (var item in profiles)
                {
                    configuration.AddProfile(item);
                }

                configuration.ConstructServicesUsing(t => t);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var mapperConfig = new MapperConfiguration(configuration);
            _mapper = new AutoMapper.Mapper(mapperConfig);
        }

        [Fact]
        public void MapperConfigIsValid()
        {
            // Act/Assert
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}