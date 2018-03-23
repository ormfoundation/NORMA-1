/*
	Natural Object-Role Modeling Architect for Visual Studio

	Copyright © Neumont University. All rights reserved.
	Copyright © ORM Solutions, LLC. All rights reserved.

	The use and distribution terms for this software are covered by the
	Common Public License 1.0 (http://opensource.org/licenses/cpl) which
	can be found in the file CPL.txt at the root of this distribution.
	By using this software in any fashion, you are agreeing to be bound by
	the terms of this license.

	You must not remove this notice, or any other, from this software.
*/

using System;
using System.Diagnostics;
using Microsoft.Deployment.WindowsInstaller;

namespace Setup.CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult DetermineVisualStudioInstalls(Session session)
        {
            try
            {
                string installPath = null;

                installPath = CustomActions.GetInstallPath("Microsoft.VisualStudio.Product.Community", session);
                if (string.IsNullOrWhiteSpace(installPath))
                {
                    session.Log("Visual Studio Community not found.");
                    session["VSPRODUCTDIR_Community"] = null;
                }
                else
                {
                    session.Log($"Found Visual Studio Community at '{installPath}.'");
                    session["VSPRODUCTDIR_Community"] = installPath;
                }

                installPath = CustomActions.GetInstallPath("Microsoft.VisualStudio.Product.Professional", session);
                if (string.IsNullOrWhiteSpace(installPath))
                {
                    session.Log("Visual Studio Professional not found.");
                    session["VSPRODUCTDIR_Professional"] = null;
                }
                else
                {
                    session.Log($"Found Visual Studio Professional at '{installPath}.'");
                    session["VSPRODUCTDIR_Professional"] = installPath;
                }

                installPath = CustomActions.GetInstallPath("Microsoft.VisualStudio.Product.Enterprise", session);
                if (string.IsNullOrWhiteSpace(installPath))
                {
                    session.Log("Visual Studio Enterprise not found.");
                    session["VSPRODUCTDIR_Enterprise"] = null;
                }
                else
                {
                    session.Log($"Found Visual Studio Enterprise at '{installPath}.'");
                    session["VSPRODUCTDIR_Enterprise"] = installPath;
                }
            }
            catch (Exception ex)
            {
                session.Log($"Error determining the Visual Studio install locations: {ex.Message}");
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// Get's the install path for the VS product.
        /// </summary>
        /// <param name="product">Product to get the install path for.  See https://docs.microsoft.com/en-us/visualstudio/install/workload-and-component-ids</param>
        /// <returns></returns>
        private static string GetInstallPath(string product, Session session)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"), $"-products {product} -property installationPath")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            session.Log($"Calling '{startInfo.FileName}' with arguments '{startInfo.Arguments}'.");
            using (Process p = Process.Start(startInfo))
            {
                string paths = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                string[] pathArray = paths.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (pathArray.Length > 0)
                {
                    return pathArray[pathArray.Length - 1].Trim();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
