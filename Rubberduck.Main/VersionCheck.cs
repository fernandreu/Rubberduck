﻿using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.VersionCheck
{
    public class VersionCheck : IVersionCheck
    {
        private readonly Version _currentVersion;
        /// <param name="version">That would be the version of the assembly for the <c>_Extension</c> class.</param>
        public VersionCheck(Version version)
        {
            _currentVersion = version;
        }

        private Version _latestVersion;
        public async Task<Version> GetLatestVersionAsync(CancellationToken token = default)
        {
            if (_latestVersion != default) { return _latestVersion; }

            try
            {
                using (var client = new HttpClient())
                {
                    var url = new Uri("http://rubberduckvba.com/Build/Version/Stable");
                    var response = await client.GetAsync(url, token);
                    var content = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(content);
                    var version = doc.DocumentNode.Descendants("body").Single().InnerText.Trim();
                    return _latestVersion = new Version(version);
                }
            }
            catch
            {
                return _latestVersion;
            }
        }

        public Version CurrentVersion => _currentVersion;
    }
}