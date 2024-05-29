using CairoDesktop.Application.Interfaces;
using System;
using System.Diagnostics;

namespace Places.ShellFolders
{
    public class ShellLocationMenuItem : IMenuItem
    {
        private readonly string _command;

        public ShellLocationMenuItem(string header, string command)
        {
            Header = header;
            _command = command;
        }

        public string Header { get; }

        public void MenuItem_Click<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            Process.Start(new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = _command
            });
        }
    }
}