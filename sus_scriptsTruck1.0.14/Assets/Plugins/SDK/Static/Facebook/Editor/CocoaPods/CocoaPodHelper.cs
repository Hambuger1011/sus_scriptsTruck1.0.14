/*
 * 1。复制podfile到工程根目录
 * 2.执行命令:
 * pod install
*/

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace GoogleMobileAds
{
    public class CocoaPodHelper
    {

        [MenuItem("Tools/CocoaPods")]
        static void Test(){
            CocoaPodHelper.Update(@"/Users/onyxgames/Desktop/Secrets/StoryGameBranch/StoryGame/APP_OUT/30-Secrets1.0.51(51)-debug/");
        }

        static string podPath = "/Users/xinglely/.gem/ruby/2.3.0/bin/pod";
        public static string Update(string projDir)
        {
            if (!Directory.Exists(projDir))
            {
                throw new Exception("project not found: " + projDir);
            }

            //podPath = IOSResolver.FindPodTool();
            //if (!File.Exists(podPath))
            //{
            //    throw new Exception("pod not found: " + podPath);
            //}
            string podPath = ExecuteCommand("which", "pod", null).Trim();

            UnityEngine.Debug.Log(@"pod executable in:\n" + podPath);
            if (string.IsNullOrEmpty(podPath))
            {
                throw new Exception("pod executable not found");
            }
            return ExecuteCommand(podPath, "install", projDir);
        }

        private static string ExecuteCommand(string command, string argument, string workingDir)
        {
            using (var process = new Process())
            {
                if (!process.StartInfo.EnvironmentVariables.ContainsKey("LANG"))
                {
                    process.StartInfo.EnvironmentVariables.Add("LANG", "en_US.UTF-8");
                }

                string path = process.StartInfo.EnvironmentVariables["PATH"];
                if(!path.Contains("/usr/local/bin"))
                {
                    path = path + ":/usr/local/bin";
                    process.StartInfo.EnvironmentVariables.Remove("PATH");
                    process.StartInfo.EnvironmentVariables.Add("PATH", path);
                }


                if (!path.Contains(podPath))
                {

					path = path + ":"+podPath;
					process.StartInfo.EnvironmentVariables.Remove("PATH");
					process.StartInfo.EnvironmentVariables.Add("PATH", path);
                }

                if (workingDir != null)
                {
                    process.StartInfo.WorkingDirectory = workingDir;
                }
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = argument;
                UnityEngine.Debug.Log("Executing " + command + " " + process.StartInfo.Arguments);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                try
                {
                    process.Start();
                    process.StandardError.ReadToEnd();
                    var stdOutput = process.StandardOutput.ReadToEnd();
                    var stdError = process.StandardError.ReadToEnd();

                    UnityEngine.Debug.Log("command stdout: " + stdOutput);

                    if (stdError != null && stdError.Length > 0)
                    {
                        UnityEngine.Debug.LogError("command stderr: " + stdError);
                    }

                    if (!process.WaitForExit(10 * 1000))
                    {
                        throw new Exception("command did not exit in a timely fashion");
                    }

                    return stdOutput;

                }
                catch (Exception e)
                {
                    throw new Exception("Encountered unexpected error while running pod", e);
                }
            }
        }
    }
}
