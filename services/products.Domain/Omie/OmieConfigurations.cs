namespace products.Domain.Omie;
public class OmieConfigurations
{
    public OmieConfigurations(string? oMIE_URL, string? aPP_KEY, string? aPP_SECRET)
    {
        OMIE_URL = oMIE_URL;
        APP_KEY = aPP_KEY;
        APP_SECRET = aPP_SECRET;
    }

    public string? OMIE_CALL { get; set; }
    public string? OMIE_URL { get; set; }
    public string? APP_KEY { get; set; }
    public string? APP_SECRET { get; set; }
}