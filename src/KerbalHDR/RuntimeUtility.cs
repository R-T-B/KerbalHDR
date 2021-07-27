/**
 * KerbalHDR Planetary System Modifier
 * -------------------------------------------------------------
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 *
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright of TakeTwo Interactive. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 *
 * https://kerbalspaceprogram.com
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Contracts;
using Expansions;
using KerbalHDR.Configuration;
using KSP.UI;
using KSP.UI.Screens;
using KSP.UI.Screens.Mapview;
using KSP.UI.Screens.Mapview.MapContextMenuOptions;
using KSP.UI.Screens.Settings.Controls;
using ModularFI;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;
using Object = UnityEngine.Object;

namespace KerbalHDR.RuntimeUtility
{
    // Mod runtime utilities
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class HDR_RuntimeUtility : MonoBehaviour
    {
        //Plugin Path finding logic
        private static string pluginPath;
        public static string PluginPath
        {
            get
            {
                if (ReferenceEquals(null, pluginPath))
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    pluginPath = Uri.UnescapeDataString(uri.Path);
                    pluginPath = Path.GetDirectoryName(pluginPath);
                }
                return pluginPath;
            }
        }
        public static ConfigReader KerbalHDRConfig = new KerbalHDR.Configuration.ConfigReader();
        // Awake() - flag this class as don't destroy on load and register delegates
        [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
        private void Awake()
        {

            // Make sure the runtime utility isn't killed
            DontDestroyOnLoad(this);
            //Load our settings
            KerbalHDRConfig.loadMainSettings();
            // Init the runtime logging
            new Logger("KerbalHDR.Runtime", true).SetAsActive();
            // Add handlers
            GameEvents.onLevelWasLoaded.Add(s => OnLevelWasLoaded(s));

            // Log
            Logger.Default.Log("[KerbalHDR] HDR_RuntimeUtility Started");
            Logger.Default.Flush();
        }

        // Execute MainMenu functions
        private void Start()
        {
            Camera.current.allowHDR = KerbalHDR.RuntimeUtility.HDR_RuntimeUtility.KerbalHDRConfig.UseHDR;
            HDROutputSettings.SetPaperWhiteInNits(KerbalHDR.RuntimeUtility.HDR_RuntimeUtility.KerbalHDRConfig.PaperWhiteNits);
            FixHDRAndShadows();
        }

        private void FixHDRAndShadows()
        {
            Camera.current.allowHDR = KerbalHDR.RuntimeUtility.HDR_RuntimeUtility.KerbalHDRConfig.UseHDR;
            HDROutputSettings.SetPaperWhiteInNits(KerbalHDR.RuntimeUtility.HDR_RuntimeUtility.KerbalHDRConfig.PaperWhiteNits);
            QualitySettings.shadowCascade4Split = new Vector3(0.003f, 0.034f, 0.101f);
            QualitySettings.shadowProjection = ShadowProjection.StableFit;
        }

        // Run patches every time a new scene was loaded
        [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
        private void OnLevelWasLoaded(GameScenes scene)
        {
            FixHDRAndShadows();
        }
    }
}
