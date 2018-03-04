using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using LogAnalyzer.Wpf.Input;
using LogAnalyzer.Services.Interfaces;

namespace LogAnalyzer.Licensing
{
    public class LicenseService : ILicenseService
    {
        // Private constants --------------------------------------------------

        private readonly string APPLICATION_MODULE_NAME = "LOGANALYZER";
      
        // Private types ------------------------------------------------------

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

        private IPathProviderService pathProviderService;

        // Private fields -----------------------------------------------------

        private License license;
        RSAParameters parameters;

        // Private methods ----------------------------------------------------

        private bool LoadLicense(string filename)
        {
            try
            {
                License license = License.LoadFromFile(filename);

                return InstallLicense(license);
            }
            catch
            {
                return false;
            }
        }

        private bool InstallLicense(License license)
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

        private bool Verify(License license)
        {
            if (license == null)
                return false;

            if (!license.VerifySignature(parameters))
                return false;

            if (license.Module != APPLICATION_MODULE_NAME && license.Module != "ALL")
                return false;

            if (license.Expires && license.ExpirationDate < DateTime.Now)
                return false;

            return true;
        }

        private void Clear()
        {
            this.license = null;

            if (LicensedChanged != null)
                LicensedChanged(this, new EventArgs());
        }

        // Public methods -----------------------------------------------------

        public LicenseService(IPathProviderService pathProviderService)
        {
            this.pathProviderService = pathProviderService;

            license = null;

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LogAnalyzer.Licensing.Resources.protools.public.key");
            parameters = Cryptography.LoadRSAKeys(stream);

            LicenseCondition = new InternalLicenseCondition(this);

            LoadLicense(pathProviderService.GetLicenseFilePath());
        }

        public bool Install(string filename)
        {
            if (LoadLicense(filename))
            {
                try
                {
                    File.Copy(filename, pathProviderService.GetLicenseFilePath());
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }

        public void Remove()
        {
            Clear();
            File.Delete(pathProviderService.GetLicenseFilePath());
        }

        // Public properties --------------------------------------------------

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
                return license?.Username;
            }
        }

        public bool? Expires
        {
            get
            {
                return license?.Expires;
            }
        }

        public DateTime? ExpirationDate
        {
            get
            {
                return license?.ExpirationDate;
            }
        }
       
        public BaseCondition LicenseCondition { get; }

        public event EventHandler LicensedChanged;
    }
}
