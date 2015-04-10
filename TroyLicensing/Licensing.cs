using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Troy.Licensing.Client.Classes;

namespace Troy.TSPE3.Licensing
{
    public static class Licensing
    {
        /// <summary>
        /// The current licensed printer list
        /// </summary>
        private static readonly List<string> CurrentLicensedPrinterList = new List<string>();
        /// <summary>
        /// The printer lock
        /// </summary>
        private static readonly object PrinterLock = new object();

        /// <summary>
        /// Determines whether [is valid license] [the specified LQR].
        /// </summary>
        /// <param name="lqr">The LQR.</param>
        /// <returns></returns>
        public static bool IsValidLicense(out LicenseQueryResult lqr)
        {
            lqr = ValidateLicense();
            if (lqr.CurrentLicenseStatus == LicenseStatus.Trial && lqr.NumberOfDaysLeft <= 0)
            {
                lqr.CurrentLicenseStatus = LicenseStatus.Expired;
            }
            return lqr.ValidLicense;
        }

        /// <summary>
        /// Determines whether [is printer licensed] [the specified printer name].
        /// </summary>
        /// <param name="printerName">Name of the printer.</param>
        /// <param name="lqr">The LQR.</param>
        /// <returns></returns>
        public static bool IsPrinterLicensed(string printerName, out LicenseQueryResult lqr)
        {
            lqr = ValidateLicense();

            if (!lqr.ValidLicense) return false;
            lock (PrinterLock)
            {

                if (CurrentLicensedPrinterList.Contains(printerName))
                {
                    return true;
                }

                if (CurrentLicensedPrinterList.Count >= lqr.PrinterCount)
                {
                    return false;
                }

                CurrentLicensedPrinterList.Add(printerName);
                return true;
            }
        }

        /// <summary>
        /// Activates the license.
        /// </summary>
        /// <param name="licenseCode">The license code.</param>
        /// <returns></returns>
        public static bool ActivateLicense(string licenseCode)
        {
            try
            {
                var validator = new LicenseValidation();

                var codebase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codebase);
                var path = Uri.UnescapeDataString(uri.Path);
                var assemblyPath = Path.GetDirectoryName(path);

                var licenseFilePath = Path.Combine(assemblyPath, Strings.LicenseFileName);

                var lqr = validator.ValidateLicense(licenseCode, Strings.ApplicationId);

                if (lqr.ValidLicense)
                {
                    validator.CreateLicense(new FileInfo(licenseFilePath), licenseCode);
                }

                return lqr.ValidLicense;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the license.
        /// </summary>
        /// <returns></returns>
        private static LicenseQueryResult ValidateLicense()
        {
            try
            {
                var validator = new LicenseValidation();

                var codebase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codebase);
                var path = Uri.UnescapeDataString(uri.Path);
                var assemblyPath = Path.GetDirectoryName(path);

                var licenseFilePath = Path.Combine(assemblyPath, Strings.LicenseFileName);

                var lqr = validator.ValidateLicense(new FileInfo(licenseFilePath), Strings.ApplicationId);

                if (!lqr.ValidLicense)
                {
                    // if first use (no license file exists), will auto generate a trial license for the product
                    validator.CreateLicense(new FileInfo(licenseFilePath), Strings.TrialCode);
                    lqr = validator.ValidateLicense(new FileInfo(licenseFilePath), Strings.ApplicationId);
                }

                return lqr;
            }
            catch (Exception)
            {
                return new LicenseQueryResult() { CurrentLicenseStatus = LicenseStatus.InValid};
            }
        }
    }
}
