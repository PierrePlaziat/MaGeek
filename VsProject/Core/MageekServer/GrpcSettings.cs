public class GrpcSettings
{
    public string CertPath { get; set; }
    public string KeyPath { get; set; }

    // Constructor to initialize default values
    public GrpcSettings()
    {
        CertPath = "grpc_cert.pem"; // default path
        KeyPath = "grpc_key.pem";   // default path
    }
}
