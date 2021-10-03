
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace NextDB
{
    public static class Extension
    {
        public static Mediator _mediator;
        public static unsafe delegate* unmanaged<string, string, string, int> Callback;

        [UnmanagedCallersOnly(EntryPoint = "RVExtensionRegisterCallback")]
        public static unsafe void RvExtensionRegisterCallback(delegate* unmanaged<string, string, string, int> callback)
        {
            Callback = callback;
            Console.WriteLine("Loaded Callback");
        }

        [UnmanagedCallersOnly(EntryPoint = "RVExtension")]
        public static unsafe void RVExtension(char* output, int outputSize, char* function)
        {
            var method = Marshal.PtrToStringAnsi((IntPtr) function) ?? "";

            var result = "";
            switch (method)
            {
                default:
                    result = "Method not implemented";
                    break;
            }

            byte[] byteFinalString = Encoding.ASCII.GetBytes(result);
            Marshal.Copy(byteFinalString, 0, (IntPtr) output, byteFinalString.Length);
        }

        [UnmanagedCallersOnly(EntryPoint = "RVExtensionArgs")]
        public static unsafe int RVExtensionArgs(char* output, int outputSize, char* function, char** argv, int argc)
        {
            var method = Marshal.PtrToStringAnsi((IntPtr) function) ?? "";

            var parameters = new List<string>();
            for (int i = 0; i < argc; i++)
            {
                var tmp = Marshal.PtrToStringAnsi((IntPtr) argv[i]) ?? "";
                tmp = tmp.Replace("\"", "");
                parameters.Add(tmp);
            }

            var result = string.Empty;

            switch (method)
            {

                default:
                    result = "Method not implemented";
                    break;
            }

            byte[] byteFinalString = Encoding.ASCII.GetBytes(result);
            Marshal.Copy(byteFinalString, 0, (IntPtr) output, byteFinalString.Length);
            return 100;
        }

        [UnmanagedCallersOnly(EntryPoint = "RVExtensionVersion")]
        public static unsafe void RVExtensionVersion(char* output, int outputSize)
        {
            Application.BuildApplication(Callback);
            Application.ReadConfiguration();

            var scope = Application.ServiceProvider.CreateScope();
            _mediator = scope.ServiceProvider.GetRequiredService<Mediator>();

            
            byte[] byteFinalString = Encoding.ASCII.GetBytes(Constants.Version);
            Marshal.Copy(byteFinalString, 0, (IntPtr) output, byteFinalString.Length);
        }
    }
}

