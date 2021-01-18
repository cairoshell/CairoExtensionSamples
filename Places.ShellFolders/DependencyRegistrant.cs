﻿using CairoDesktop.Application.Interfaces;
using System.ComponentModel.Composition;

namespace Places.ShellFolders
{
    [Export(typeof(IDependencyRegistrant))]
    public class DependencyRegistrant : IDependencyRegistrant
    {
        public void Register(IDependencyRegistrar registrar)
        {
            registrar.AddScoped<IShellExtension, ShellFolderExtension>();
        }
    }
}