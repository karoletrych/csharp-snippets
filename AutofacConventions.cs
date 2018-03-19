using System;
using System.Linq;
using System.Net;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Xunit;

namespace Snippets
{
	public class AutofacConventions
	{
		[Fact]
		public void Test()
		{
			var builder = new ContainerBuilder();

			var assembly = Assembly.GetAssembly(typeof(AutofacConventions));
			builder.RegisterAssemblyTypes(assembly)
				.AsSelf()
				.AsImplementedInterfaces();

			builder.RegisterModule(new SettingsModule());

			var consumer = builder.Build().Resolve<Consumer>();

			consumer.Print();
		}


		class SettingsModule : Autofac.Module
		{
			protected override void AttachToComponentRegistration(
				IComponentRegistry componentRegistry,
				IComponentRegistration registration)
			{
				registration.Preparing += InjectSettings;
			}

			private void InjectSettings(object sender, PreparingEventArgs e)
			{
				e.Parameters = e.Parameters.Union(new[]
				{
				new ResolvedParameter(
					(info, context) => 
						info.ParameterType.IsGenericType && info.ParameterType.GetGenericTypeDefinition() == typeof(ISetting<>),
					(parameterInfo, context) =>
						context.ResolveNamed(parameterInfo.Name.ToLower(), parameterInfo.ParameterType))
			});
			}

			protected override void Load(ContainerBuilder builder)
			{
				bool IsISetting(Type type)
				{
					return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ISetting<>);
				}
				var settingTypes = Assembly.GetAssembly(typeof(AutofacConventions))
					.GetTypes()
					.Where(t => t.GetInterfaces()
						.Any(IsISetting));
				foreach (var type in settingTypes)
					builder.RegisterType(type)
						.Named(type.Name.ToLower(), type.GetInterfaces().Single(IsISetting));
			}
		}

		class NonGenericDependency
		{

		}
		class Consumer
		{
			private readonly ISetting<IPAddress> _ipAddressSetting;
			private readonly ISetting<string> _nameSetting;
			private readonly NonGenericDependency _dep;

			public Consumer(ISetting<IPAddress> ipAddressSetting, 
				ISetting<string> name2, NonGenericDependency dep)
			{
				_ipAddressSetting = ipAddressSetting;
				_nameSetting = name2;
				_dep = dep;
			}

			public void Print()
			{
				Console.WriteLine(_ipAddressSetting.Value);
				Console.WriteLine(_nameSetting.Value);
			}
		}

		interface ISetting<T>
		{
			T Value { get; set; }
		}

		class IpAddressSetting : ISetting<IPAddress>
		{
			public IPAddress Value { get; set; } = IPAddress.Loopback;
		}

		class Name3 : ISetting<string>
		{
			public string Value { get; set; } = "B";
		}

		class Name2 : ISetting<string>
		{
			public string Value { get; set; } = "A";
		}

		class NameSetting : ISetting<string>
		{
			public string Value { get; set; } = "Karol";
		}
	}
}
