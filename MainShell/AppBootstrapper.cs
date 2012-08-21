using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using Caliburn.Micro;
using MainShell.ViewModels;
using Simulator.ViewModels;

namespace MainShell
{
	internal class AppBootstrapper : Bootstrapper<IShell>
	{
		private IContainer container;

		protected override void Configure()
		{
			var builder = new ContainerBuilder();
			builder.Register(x => new WindowManager()).As<IWindowManager>();
			builder.Register(x => new EventAggregator()).As<IEventAggregator>();
			builder.Register(x => new ShellViewModel()).AsImplementedInterfaces();
			builder.Register(x => new InputGenerationViewModel()).AsSelf();
			builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

			container = builder.Build();
		}

		private Service GetServiceForKey(string key)
		{
			Contract.Requires<InvalidOperationException>(container.ComponentRegistry.Registrations != null, "Registrations collection in ComponentRegistry can't be null when fetching service for key");
			var registrations = container.ComponentRegistry.Registrations.ToList();

			foreach (var registration in registrations)
			{
				var result = registration.Services.OfType<KeyedService>()
					.FirstOrDefault(service => service.ServiceKey as string == key);
				if (result != null)
				{
					return result;
				}
			}

			return registrations
				.Select(registration => registration.Services.OfType<IServiceWithType>()
				.FirstOrDefault(service => service.ServiceType.FullName == key || service.ServiceType.Name == key))
				.Where(result => result != null)
				.Cast<Service>()
				.FirstOrDefault();
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			object instance;
			if (serviceType == null)
			{
				var service = GetServiceForKey(key);
				if (service != null && container.TryResolveService(service, out instance))
				{
					return instance;
				}
			}
			else
			{
				if (string.IsNullOrWhiteSpace(key))
				{
					if (container.TryResolve(serviceType, out instance))
					{
						return instance;
					}
				}
				else
				{
					if (container.TryResolveKeyed(key, serviceType, out instance))
					{
						return instance;
					}
				}
			}

			throw new Exception(string.Format("Could not locate any instances of contract {0}", key ?? (serviceType == null ? string.Empty : serviceType.Name)));
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return container.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType)) as IEnumerable<object>;
		}

		protected override void BuildUp(object instance)
		{
			container.InjectProperties(instance);
		}
	}
}
