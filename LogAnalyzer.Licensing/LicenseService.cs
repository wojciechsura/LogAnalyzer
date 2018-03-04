using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using LogAnalyzer.Models.Licensing;
using LogAnalyzer.Wpf.Input;
using LogAnalyzer.Services.Interfaces;

namespace LogAnalyzer.Licensing
{
    public class LicenseService : ILicenseService
    {
        private sealed class InternalLicenseCondition : BaseCondition, IDisposable
        {
            private LicenseService licenseManager;

            public InternalLicenseCondition(LicenseService newLicenseManager)
            {
                if (newLicenseManager == null)
                    throw new ArgumentNullException(nameof(newLicenseManager));

                licenseManager = newLicenseManager;
                licenseManager.LicensedChanged += this.LicensedChanged;
            }

            void LicensedChanged(object sender, EventArgs e)
            {
                OnValueChanged(licenseManager.Licensed);
            }

            public override bool GetValue()
            {
                return licenseManager.Licensed;
            }

            public void Dispose()
            {
                licenseManager.LicensedChanged -= this.LicensedChanged;
            }
        }

        private AppInfo appInfo;
        private License license;
        RSAParameters parameters;

        public LicenseService(AppInfo newAppInfo)
        {
            appInfo = newAppInfo ?? throw new ArgumentNullException(nameof(newAppInfo));
            license = null;
            parameters = Cryptography.LoadRSAKeys(newAppInfo.PublicLicenseKey);

            LicenseCondition = new InternalLicenseCondition(this);
        }

        public bool Verify(License license)
        {
            if (license == null)
                return false;

            if (!license.VerifySignature(parameters))
                return false;

            if (license.Module != appInfo.LicenseModule && license.Module != "ALL")
                return false;

            if (license.Expires && license.ExpirationDate < DateTime.Now)
                return false;

            return true;
        }

        public bool Load(string filename)
        {
            try
            {
                License license = License.LoadFromFile(filename);

                return Install(license);
            }
            catch
            {
                return false;
            }
        }

        public void Remove()
        {
            this.license = null;

            if (LicensedChanged != null)
                LicensedChanged(this, new EventArgs());
        }

        public bool Install(License license)
        {
            if (license == null)
            {
                this.license = null;

                LicensedChanged?.Invoke(this, new EventArgs());

                return true;
            }
            else
            {
                if (Verify(license))
                {
                    this.license = license;

                    LicensedChanged?.Invoke(this, new EventArgs());

                    return true;
                }
                else
                    return false;
            }
        }

        public bool Licensed
        {
            get
            {
                return license != null;
            }
        }

        public string Username
        {
            get
            {
                if (license == null)
                    throw new InvalidOperationException("Internal error: not licensed, cannot retreive username!");

                return license.Username;
            }
        }

        public bool Expires
        {
            get
            {
                if (license == null)
                    throw new InvalidOperationException("Internal error: not licensed, cannot retreive expiration!");

                return license.Expires;
            }
        }

        public DateTime ExpirationDate
        {
            get
            {
                if (license == null)
                    throw new InvalidOperationException("Internal error: not licensed, cannot retreive expiration date!");

                return license.ExpirationDate;
            }
        }

        public BaseCondition LicenseCondition { get; }

        public event EventHandler LicensedChanged;
    }
}
