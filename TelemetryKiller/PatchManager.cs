using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;
using Microsoft.Win32;

namespace TelemetryKiller {
    public class PatchManager {
        
        private Label log;
        private bool disableCortana;
        private bool disableWebsearch;
        private bool blockTelemetry;
        private bool disableSystemAds;
        private bool disableOneDrive;
        private bool disableWifiShare;
        private bool disableGeolocation;
        private bool disableAccountSync;

        private Dictionary<string, string> firewallRules = new Dictionary<string, string> {
            {"telemetry_vortex.data.microsoft.com", "191.232.139.254"},
            {"telemetry_telecommand.telemetry.microsoft.com", "65.55.252.92"},
            {"telemetry_oca.telemetry.microsoft.com", "65.55.252.63"},
            {"telemetry_sqm.telemetry.microsoft.com", "65.55.252.93"},
            {"telemetry_watson.telemetry.microsoft.com", "65.55.252.43,65.52.108.29"},
            {"telemetry_redir.metaservices.microsoft.com", "194.44.4.200,194.44.4.208"},
            {"telemetry_choice.microsoft.com", "157.56.91.77"},
            {"telemetry_df.telemetry.microsoft.com", "65.52.100.7"},
            {"telemetry_reports.wes.df.telemetry.microsoft.com", "65.52.100.91"},
            {"telemetry_wes.df.telemetry.microsoft.com", "65.52.100.93"},
            {"telemetry_services.wes.df.telemetry.microsoft.com", "65.52.100.92"},
            {"telemetry_sqm.df.telemetry.microsoft.com", "65.52.100.94"},
            {"telemetry_telemetry.microsoft.com", "65.52.100.9"},
            {"telemetry_watson.ppe.telemetry.microsoft.com", "65.52.100.11"},
            {"telemetry_telemetry.appex.bing.net", "168.63.108.233"},
            {"telemetry_telemetry.urs.microsoft.com", "157.56.74.250"},
            {"telemetry_settings-sandbox.data.microsoft.com", "111.221.29.177"},
            {"telemetry_vortex-sandbox.data.microsoft.com", "64.4.54.32"},
            {"telemetry_survey.watson.microsoft.com", "207.68.166.254"},
            {"telemetry_watson.live.com", "207.46.223.94"},
            {"telemetry_watson.microsoft.com", "65.55.252.71"},
            {"telemetry_statsfe2.ws.microsoft.com", "64.4.54.22"},
            {"telemetry_corpext.msitadfs.glbdns2.microsoft.com", "131.107.113.238"},
            {"telemetry_compatexchange.cloudapp.net", "23.99.10.11"},
            {"telemetry_cs1.wpc.v0cdn.net", "68.232.34.200"},
            {"telemetry_a-0001.a-msedge.net", "204.79.197.200"},
            {"telemetry_statsfe2.update.microsoft.com.akadns.net", "64.4.54.22"},
            {"telemetry_sls.update.microsoft.com.akadns.net", "157.56.77.139"}, 
            {"telemetry_fe2.update.microsoft.com.akadns.net", "134.170.58.121,134.170.58.123,134.170.53.29,66.119.144.190,134.170.58.189,134.170.58.118,134.170.53.30,134.170.51.190"},
            {"telemetry_diagnostics.support.microsoft.com", "157.56.121.89"},
            {"telemetry_corp.sts.microsoft.com", "131.107.113.238"},
            {"telemetry_statsfe1.ws.microsoft.com", "134.170.115.60"},
            {"telemetry_pre.footprintpredict.com", "204.79.197.200"},
            {"telemetry_i1.services.social.microsoft.com", "104.82.22.249"},
            {"telemetry_feedback.windows.com", "134.170.185.70"},
            {"telemetry_feedback.microsoft-hohm.com", "64.4.6.100,65.55.39.10"},
            {"telemetry_feedback.search.microsoft.com", "157.55.129.21"},
            {"telemetry_rad.msn.com", "207.46.194.25"},
            {"telemetry_preview.msn.com", "23.102.21.4"},
            {"telemetry_dart.l.doubleclick.net", "173.194.113.220,173.194.113.219,216.58.209.166"}, 
            {"telemetry_ads.msn.com", "157.56.91.82,157.56.23.91,104.82.14.146,207.123.56.252,185.13.160.61,8.254.209.254"},
            {"telemetry_a.ads1.msn.com", "198.78.208.254,185.13.160.61"},
            {"telemetry_global.msads.net.c.footprint.net", "185.13.160.61,8.254.209.254,207.123.56.252"},
            {"telemetry_az361816.vo.msecnd.net", "68.232.34.200"},
            {"telemetry_oca.telemetry.microsoft.com.nsatc.net", "65.55.252.63"},
            {"telemetry_ssw.live.com", "207.46.101.29"},
            {"telemetry_msnbot-65-55-108-23.search.msn.com", "65.55.108.23"},
            {"telemetry_a23-218-212-69.deploy.static.akamaitechnologies.com", "23.218.212.69"}
        };

        public PatchManager(Label log, bool disableCortana, bool disableWebsearch, bool blockTelemetry,
            bool disableSystemAds, bool disableOneDrive, bool disableWifiShare, bool disableGeolocation,
            bool disableAccountSync) {
            this.log = log;
            this.disableCortana = disableCortana;
            this.disableWebsearch = disableWebsearch;
            this.blockTelemetry = blockTelemetry;
            this.disableSystemAds = disableSystemAds;
            this.disableOneDrive = disableOneDrive;
            this.disableWifiShare = disableWifiShare;
            this.disableGeolocation = disableGeolocation;
            this.disableAccountSync = disableAccountSync;
        }

        public void StartPatch() {
            bool changes = false;
            log.Content = "Start Patching...";
            
            if (disableCortana) {
                DisableCortana();
                changes = true;
            }
            if (disableWebsearch) {
                DisableWebsearch();
                changes = true;
            }
            if (blockTelemetry) {
                DisableTelemetryKeys();
                DisableTelemetryFirewall();
                changes = true;
            }
            if (disableSystemAds) {
                DisableSystemAds();
                changes = true;
            }
            if (disableOneDrive) {
                DisableOneDrive();
                changes = true;
            }
            if (disableWifiShare) {
                DisableWifiShare();
                changes = true;
            }
            if (disableGeolocation) {
                DisableGeolocation();
                changes = true;
            }
            if (disableAccountSync) {
                DisableAccountSync();
                changes = true;
            }
            if (changes) {
                log.Foreground = Brushes.Lime;
                log.Content = "Patch Complete";
            } else {
                log.Content = "Nothing patched";
            }
        }

        public static bool CheckWin10() {
            return ((string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "ProductName", "")).StartsWith("Windows 10");
        }

        private void DisableCortana() {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search", "CortanaConsent", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PolicyManager\current\device\Experience", "AllowCortana", 0, RegistryValueKind.DWord);
        }

        private void DisableWebsearch() {
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search", "AllowSearchToUseLocation", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "ConnectedSearchUseWebOverMeteredConnections", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "DisableWebSearch", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "ConnectedSearchUseWeb", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "ConnectedSearchPrivacy", 3, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "ConnectedSearchSafeSearch", 3, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\SearchCompanion", "DisableContentFileUpdates", 1, RegistryValueKind.DWord);
        }

        private void DisableTelemetryKeys() {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\TabletPC", "PreventHandwritingDataSharing", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableUAR", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableInventory", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\AppCompat", "AITEnable", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync", "DisableSettingSync", 2, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\SettingSync", "DisableSettingSyncUserOverride", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\MediaPlayer\Preferences", "UsageTracking", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\MediaPlayer\Preferences", "MetadataRetrieval", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\MediaPlayer\Preferences", "SilentAcquisition", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\MediaPlayer\Preferences", "DisableLicenseRefresh", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\MediaPlayer\Preferences", "SendUserGUID", "00", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\MediaPlayer\PREFERENCES\HME\S-1-5-21-3279004032-3361476492-3816944365-1001", "UsageTracking", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\MediaPlayer\PREFERENCES\HME\S-1-5-21-3279004032-3361476492-3816944365-1001", "ForceUsageTracking", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization\TrainedDataStore", "HarvestContacts", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Input\TIPC", "Enabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization", "RestrictImplicitInkCollection", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Siuf\Rules", "NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Siuf\Rules", "PeriodInNanoSeconds", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Assistance\Client\1.0\Settings", "ImplicitFeedback", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\WMI\Autologger\AutoLogger-Diagtrack-Listener", "Start", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableWindowsConsumerFeatures", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Device Metadata", "PreventDeviceMetadataFromNetwork", 1, RegistryValueKind.DWord);
        }

        private void DisableSystemAds() {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSyncProviderNotifications", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "RotatingLockScreenEnabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "RotatingLockScreenOverlayEnabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SoftLandingEnabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SilentInstalledAppsEnabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\PenWorkspace", "PenWorkspaceAppSuggestionsEnabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "DisabledByGroupPolicy", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableWindowsSpotlightFeatures", 1, RegistryValueKind.DWord);
        }

        private void DisableAccountSync() {
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\SettingSync", "SyncPolicy", 5, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "NoConnectedUser", 3, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\wlidsvc", "Start", 4, RegistryValueKind.DWord);
        }

        private void DisableOneDrive() {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\OneDrive", "PreventNetworkTrafficPreUserSignIn", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\OneDrive", "DisablePersonalSync", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}", "System.IsPinnedToNameSpaceTree", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableFileSyncNGSC", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableLocation", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableLibrariesDefaultSaveToOneDrive", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableFileSync", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive", "PreventNetworkTrafficPreUserSignIn", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableMeteredNetworkFileSync", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Storage\EnabledDenyGP", "DenyAllGPState", 1, RegistryValueKind.DWord);
        }

        private void DisableWifiShare() {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager", "WiFiSenseCredShared", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager", "WiFiSenseOpen", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\config", "AutoConnectAllowedOEM", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\WcmSvc\wifinetworkmanager\config", "AutoConnectAllowedOEM", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\features\S-1-5-21-3279004032-3361476492-3816944365-1001\SocialNetworks\FACEBOOK", "OptInStatus", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\features\S-1-5-21-3279004032-3361476492-3816944365-1001\SocialNetworks\ABCH", "OptInStatus", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\features\S-1-5-21-3279004032-3361476492-3816944365-1001\SocialNetworks\ABCH-SKYPE", "OptInStatus", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\PolicyManager\default\WiFi\AllowWiFiHotSpotReporting", "value", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\PolicyManager\default\WiFi\AllowAutoConnectToWiFiSenseHotspots", "value", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\config", "AutoConnectAllowedOEM", 0, RegistryValueKind.DWord);
        }

        private void DisableGeolocation() {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocation", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableWindowsLocationProvider", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocationScripting", 1, RegistryValueKind.DWord);
        }

        private void DisableTelemetryFirewall() {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.Verb = "runas";
            cmd.Start();
            foreach (var rule in firewallRules) {
                cmd.StandardInput.WriteLine("netsh advfirewall firewall add rule name=" + rule.Key + " dir=out action=block remoteip=" + rule.Value + " enable=yes\r\n");
                cmd.StandardInput.Flush();
            }
            cmd.StandardInput.Close();
            cmd.Close();
        }
    }
}