﻿using System.Security.Cryptography.X509Certificates;

namespace Bit.Core.Models.Business;

public interface ILicense
{
    string LicenseKey { get; set; }
    int Version { get; set; }
    DateTime Issued { get; set; }
    DateTime? Refresh { get; set; }
    DateTime? Expires { get; set; }
    bool Trial { get; set; }
    string Hash { get; set; }
    string Signature { get; set; }
    string Token { get; set; }
    byte[] SignatureBytes { get; }
    byte[] EncodedData { get; }
    byte[] EncodedHash { get; }
    bool ValidLicenseVersion { get; }
    string ToToken(X509Certificate2 certificate);
}
